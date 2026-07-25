namespace SovereignGrid.Storage.LocalStorage;

public sealed class BackupService
{
    public void CreateBackup(
        string sourceFile,
        string backupFile)
    {
        File.Copy(
            sourceFile,
            backupFile,
            true);
    }
}
