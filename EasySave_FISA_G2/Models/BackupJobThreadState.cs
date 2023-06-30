using System.Threading;

namespace ProjetG2AdminDev.Models;

public class BackupJobThreadState
{
    public BackupJob Job { get; }
    public CancellationToken CancellationToken { get; }
    public ManualResetEvent ManualResetEvent { get; }
    public BackupJobThreadState(BackupJob job, CancellationToken cancellationToken, ManualResetEvent manualResetEvent)
    {
        Job = job;
        CancellationToken = cancellationToken;
        ManualResetEvent = manualResetEvent;
    }
}