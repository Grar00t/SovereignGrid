namespace SovereignGrid.Storage.LocalStorage;

public sealed class StorageProfile
{
    public string WorkspacePath { get; set; }
        = "workspace";

    public bool CreateBackupOnSave { get; set; }
        = true;
}
