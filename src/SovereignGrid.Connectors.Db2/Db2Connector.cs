using IBM.Data.Db2;
using SovereignGrid.Connectors.Abstractions;
using SovereignGrid.Core.Workbook;

namespace SovereignGrid.Connectors.Db2;

public sealed class Db2Connector : IConnector
{
    public ConnectorCapabilities Capabilities => new(
        Id: "db.db2",
        DisplayName: "IBM Db2",
        Kind: ConnectorKind.Database,
        RequiresNetwork: true,
        SupportsRead: true,
        SupportsWrite: false);

    public async Task<ConnectorResult> ImportAsync(Worksheet target,
        IDictionary<string,string> options, CancellationToken ct = default)
    {
        var cs  = options["connectionString"];
        var sql = options["query"];

        await using var con = new DB2Connection(cs);
        await con.OpenAsync(ct);
        await using var cmd = new DB2Command(sql, con);
        await using var reader = await cmd.ExecuteReaderAsync(ct);

        for (int c = 0; c < reader.FieldCount; c++)
            target.SetValue(1, c + 1, reader.GetName(c));

        int row = 2;
        while (await reader.ReadAsync(ct))
        {
            for (int c = 0; c < reader.FieldCount; c++)
                target.SetValue(row, c + 1, reader[c]?.ToString() ?? "");
            row++;
        }
        return new ConnectorResult(true, "Imported from Db2", row - 2);
    }

    public Task<ConnectorResult> ExportAsync(Worksheet source,
        IDictionary<string,string> options, CancellationToken ct = default)
        => Task.FromResult(new ConnectorResult(false, "Db2 export not supported", 0));
}
