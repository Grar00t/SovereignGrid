using Oracle.ManagedDataAccess.Client;
using SovereignGrid.Connectors.Abstractions;
using SovereignGrid.Core.Workbook;

namespace SovereignGrid.Connectors.Oracle;

public sealed class OracleConnector : IConnector
{
    public ConnectorCapabilities Capabilities => new(
        Id: "db.oracle",
        DisplayName: "Oracle Database",
        Kind: ConnectorKind.Database,
        RequiresNetwork: true,
        SupportsRead: true,
        SupportsWrite: false);

    public async Task<ConnectorResult> ImportAsync(Worksheet target,
        IDictionary<string,string> options, CancellationToken ct = default)
    {
        var cs  = options["connectionString"];
        var sql = options["query"];

        await using var con = new OracleConnection(cs);
        await con.OpenAsync(ct);
        await using var cmd = new OracleCommand(sql, con);
        await using var reader = await cmd.ExecuteReaderAsync(ct);

        for (int c = 0; c < reader.FieldCount; c++)
            target.SetValue(1, c + 1, reader.GetName(c));   // صف العناوين

        int row = 2;
        while (await reader.ReadAsync(ct))
        {
            for (int c = 0; c < reader.FieldCount; c++)
                target.SetValue(row, c + 1, reader[c]?.ToString() ?? "");
            row++;
        }
        return new ConnectorResult(true, $"Imported from Oracle", row - 2);
    }

    public Task<ConnectorResult> ExportAsync(Worksheet source,
        IDictionary<string,string> options, CancellationToken ct = default)
        => Task.FromResult(new ConnectorResult(false, "Oracle export not supported", 0));
}
