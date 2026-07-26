using SovereignGrid.Core.Workbook;

namespace SovereignGrid.Connectors.Abstractions;

public enum ConnectorKind { FileImportExport, Database, OnlineService }

public sealed record ConnectorCapabilities(
    string Id,
    string DisplayName,
    ConnectorKind Kind,
    bool RequiresNetwork,
    bool SupportsRead,
    bool SupportsWrite);

public sealed record ConnectorResult(bool Success, string Message, int RowsAffected);

public interface IConnector
{
    ConnectorCapabilities Capabilities { get; }

    Task<ConnectorResult> ImportAsync(
        Worksheet target,
        IDictionary<string,string> options,
        CancellationToken ct = default);

    Task<ConnectorResult> ExportAsync(
        Worksheet source,
        IDictionary<string,string> options,
        CancellationToken ct = default);
}
