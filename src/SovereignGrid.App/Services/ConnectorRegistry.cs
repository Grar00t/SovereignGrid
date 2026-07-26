using SovereignGrid.Connectors.Abstractions;
using SovereignGrid.Connectors.Excel;
using SovereignGrid.Connectors.Oracle;
using SovereignGrid.Connectors.Db2;

namespace SovereignGrid.App.Services;

public static class ConnectorRegistry
{
    public static IReadOnlyList<IConnector> All { get; } = new IConnector[]
    {
        new ExcelConnector(),
        new OracleConnector(),
        new Db2Connector()
    };
}
