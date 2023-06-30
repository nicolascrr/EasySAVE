using System;
using System.Collections.Generic;

namespace Client.Model;

public class BackupJob
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string SourcePath { get; set; }
    public string DestinationPath { get; set; }
    public string Type { get; set; }
    public float SizeInOctets { get; set; }
    public TimeSpan LastExecDuration { get; set; }
    public DateTime LastExecTime { get; set; }
    public double TotalFileToCopy { get; set; }
    public double NbFileLeft { get; set; }
    public long DowloadingFileSize { get; set; }
    public float Progress { get; set; }
    public List<string> PriorityList { get; set; }
    public bool HasPriority { get; set; }
    public bool IsRunning { get; set; }
    public string State { get; set; }
}

