using System;
using System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace CryptoSoft;

public class BackupJob
{
    public int ID { get; set; }
    public string Name { get; set; } = null!;
    public string SourcePath { get; set; } = null!;
    public string DestinationPath { get; set; } = null!;
    public string Type { get; set; } = null!;
    public float SizeInOctets { get; set; }
    public TimeSpan LastExecDuration { get; set; }
    public DateTime LastExecTime { get; set; }
    public double TotalFileToCopy { get; set; }
    public double NbFileLeft { get; set; }
    public long DowloadingFileSize { get; set; }
    public List<string> NoPriorityFileList { get; set; }
    public List<string> PriorityUserExtList { get; set; }
    public TimeSpan EncryptionDuration { get; set; }
}