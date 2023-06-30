using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;

namespace ProjetG2AdminDev.Models;

public class BackupJob : INotifyPropertyChanged
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
    public List<string> NoPriorityFileListSourceFile { get; set; }
    public List<string> NoPriorityFileListDestPath { get; set; }
    public List<string> PriorityUserExtList { get; set; }
    public TimeSpan EncryptionDuration { get; set; }
    private float _progress;
    public float Progress
    {
        get => _progress;
        set
        {
            if (_progress == value) return;
            _progress = value;
            OnPropertyChanged(nameof(Progress));
        }
    }
    private List<string> _priorityList = null!;
    public List<string> PriorityList
    {
        get => _priorityList;
        set
        {
            if (_priorityList == value) return;
            _priorityList = value;
            OnPropertyChanged(nameof(PriorityList));
        }
    }
    private bool _hasPriority;
    public bool HasPriority
    {
        get => _hasPriority;
        set
        {
            if (_hasPriority == value) return;
            _hasPriority = value;
            OnPropertyChanged(nameof(HasPriority));
        }
    }
    private bool _isRunning;
    public bool IsRunning
    {
        get => _isRunning;
        set
        {
            if (_isRunning == value) return;
            _isRunning = value;
            OnPropertyChanged(nameof(IsRunning));
        }
    }
    private string _state = null!;
    public string State
    {
        get => _state;
        set
        {
            if (_state == value) return;
            _state = value;
            OnPropertyChanged(nameof(State));
        }
    }

    public event EventHandler<ProgressEventArgs> RaiseEvent = null!;
    public event EventHandler<ProgressEventArgs> RaiseEventSizeLimit = null!;
    public event PropertyChangedEventHandler? PropertyChanged;

    public BackupJob()
    {
        PriorityUserExtList = new List<string>();
        NoPriorityFileListSourceFile = new List<string>();
        NoPriorityFileListDestPath = new List<string>();
    }

    public BackupJob(int id, string name, string sourcePath, string destinationPath, string type)
    {
        ID = id;
        Name = name;
        SourcePath = @sourcePath;
        DestinationPath = @destinationPath;
        Type = type;
        State = "Inactive";
        IsRunning = false;
        HasPriority = false;
        NoPriorityFileListSourceFile = new List<string>();
        NoPriorityFileListDestPath = new List<string>();
        PriorityUserExtList = new List<string>();
        try
        {
            DirectoryInfo directorySource = new DirectoryInfo(SourcePath);
            PriorityList = GetExtensions(directorySource);
            if (PriorityList.Contains(".mp4"))
            {
                HasPriority = true;
            }
        }
        catch (ArgumentException)
        {
        }
    }

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChangedEventHandler? handler = PropertyChanged;
        handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public string Start(CancellationToken cancellationToken, ManualResetEvent manualResetEvent)
    {
        // Start the backup using the source path and the destination path of the object
        if (Directory.Exists(SourcePath))
        {
            LastExecTime = DateTime.Now;

            DirectoryInfo directorySource = new DirectoryInfo(SourcePath);
            SizeInOctets = DirSize(directorySource);
            TotalFileToCopy = DirNbFile(directorySource);
            PriorityList = GetExtensions(directorySource);
            if (SizeInOctets == 0)
            {
                return "NoAccess";
            }

            Stopwatch stopwatch = new Stopwatch();
            NbFileLeft = TotalFileToCopy;
            State = "Active";
            IsRunning = true;
            stopwatch.Start();
            string result = SaveDirectory(SourcePath, DestinationPath, Type, cancellationToken, manualResetEvent);
            if (result != "ok") return result;
            stopwatch.Stop();
            Chiffrement();
            State = "Inactive";
            IsRunning = false;
            LastExecDuration = stopwatch.Elapsed;
            return "ok";
        }
        else
        {
            return "NonExistingPath";
        }
    }

    private string SaveDirectory(string sourcePath, string destinationPath, string type,
        CancellationToken cancellationToken, ManualResetEvent manualResetEvent)
    {
        // Create the target path if it doesn't exist, then copy every file in the target path. If it find repositories, call himself to be recursive and copy every sub-directories and their files
        if (!Directory.Exists(destinationPath))
        {
            Directory.CreateDirectory(destinationPath);
        }
        PriorityUserExtList.Add(".mp4");

        bool saveResult = type == "Complete"
            ? CompleteBackup(sourcePath, destinationPath, type, cancellationToken, manualResetEvent)
            : DifferentialBackup(sourcePath, destinationPath, type, cancellationToken, manualResetEvent);
        return saveResult ? "ok" : "BackupProblem";
    }

    private bool CompleteBackup(string sourcePath, string destinationPath, string type,
        CancellationToken cancellationToken, ManualResetEvent manualResetEvent)
    {
        try
        {
            foreach (var file in Directory.GetFiles(sourcePath))
            {
                if (cancellationToken.IsCancellationRequested) return false;
                manualResetEvent.WaitOne();
                string targetFile = Path.Combine(destinationPath, Path.GetFileName(file));
                CallEvent(file, targetFile);
            }

            foreach (var directory in Directory.GetDirectories(sourcePath))
            {
                destinationPath = Path.Combine(destinationPath, Path.GetFileName(directory));
                SaveDirectory(directory, destinationPath, type, cancellationToken, manualResetEvent);
            }

            if (NoPriorityFileListSourceFile != null)
            {
                var zippedLists = NoPriorityFileListSourceFile.Zip(NoPriorityFileListDestPath, (a, b) => new { First = a, Second = b });
                foreach (var zippedItem in zippedLists)
                {
                    var sourceFile = zippedItem.First;
                    var destPath = zippedItem.Second;
                    if (cancellationToken.IsCancellationRequested) return false;
                    manualResetEvent.WaitOne();
                    string targetFile = Path.Combine(destPath, Path.GetFileName(sourceFile));
                    PriorityUserExtList.Clear();
                    CallEvent(sourceFile, targetFile);
                }
                NoPriorityFileListSourceFile.Clear();
                NoPriorityFileListDestPath.Clear();
            }

            return true;
        }
        catch (UnauthorizedAccessException)
        {
            return true;
        }
    }

    private bool DifferentialBackup(string sourcePath, string destinationPath, string type,
        CancellationToken cancellationToken, ManualResetEvent manualResetEvent)
    {
        foreach (var file in Directory.GetFiles(sourcePath))
        {
            string targetFile = Path.Combine(destinationPath, Path.GetFileName(file));
            if (File.Exists(targetFile) &&
                File.GetLastWriteTime(file) <= File.GetLastWriteTime(targetFile)) continue;
            if (cancellationToken.IsCancellationRequested) return false;
            manualResetEvent.WaitOne();
            CallEvent(file, targetFile);
        }

        string[] subDirectories = Directory.GetDirectories(sourcePath);
        foreach (string subDirectory in subDirectories)
        {
            string targetSubDirectory = Path.Combine(destinationPath, Path.GetFileName(subDirectory));
            if (Directory.GetLastWriteTime(subDirectory) > Directory.GetLastWriteTime(targetSubDirectory))
            {
                SaveDirectory(subDirectory, targetSubDirectory, type, cancellationToken, manualResetEvent);
            }
        }

        if (NoPriorityFileListSourceFile != null)
        {
            var zippedLists = NoPriorityFileListSourceFile.Zip(NoPriorityFileListDestPath, (a, b) => new { First = a, Second = b });
            foreach (var zippedItem in zippedLists)
            {
                var sourceFile = zippedItem.First;
                var destPath = zippedItem.Second;
                if (cancellationToken.IsCancellationRequested) return false;
                manualResetEvent.WaitOne();
                string targetFile = Path.Combine(destPath, Path.GetFileName(sourceFile));
                PriorityUserExtList.Clear();
                CallEvent(sourceFile, targetFile);
            }
            NoPriorityFileListSourceFile.Clear();
            NoPriorityFileListDestPath.Clear();
        }

        return true;
    }

    private void CallEvent(string file, string targetFile)
    {
        try
        {
            string extension = Path.GetExtension(file);
            if (PriorityUserExtList.Count != 0 && !PriorityUserExtList.Contains(extension.ToLower()))
            {
                NoPriorityFileListSourceFile.Add(file);
                NoPriorityFileListDestPath.Add(Path.GetDirectoryName(targetFile));
                return;
            }
            FileInfo fileInfo = new FileInfo(file);
            DowloadingFileSize = fileInfo.Length;
            RaiseEventSizeLimit?.Invoke(this, new ProgressEventArgs
            {
                DowloadingFileSize = DowloadingFileSize,
            });
            File.Copy(file, targetFile, true);
            NbFileLeft--;
            Progress = 100 - (float)(NbFileLeft / TotalFileToCopy * 100);
            RaiseEvent?.Invoke(this, new ProgressEventArgs
            {
                ID = ID,
                Name = Name,
                SourcePath = SourcePath,
                DestinationPath = DestinationPath,
                Type = Type,
                SizeInOctets = SizeInOctets,
                LastExecDuration = LastExecDuration,
                LastExecTime = LastExecTime,
                State = State,
                IsRunning = IsRunning,
                TotalFileToCopy = TotalFileToCopy,
                NbFileLeft = NbFileLeft,
                Progress = Progress,
                PriorityList = PriorityList,
                HasPriority = HasPriority,
                DowloadingFileSize = DowloadingFileSize,
            });
        }
        catch (PathTooLongException)
        {
            return;
        }
    }

    private static long DirSize(DirectoryInfo directory)
    {
        try
        {
            // Parcourt les fichiers du dossier
            FileInfo[] files = directory.GetFiles();
            long size = files.Sum(file => file.Length);
            // Parcourt les sous-dossiers du dossier
            DirectoryInfo[] directories = directory.GetDirectories();
            size += directories.Sum(dir => DirSize(dir));
            return size;
        }
        catch (UnauthorizedAccessException)
        {
            return 0;
        }
    }

    private static long DirNbFile(DirectoryInfo directory)
    {
        try
        {
            long nbFile = 0;
            // Parcourt les sous-dossiers du dossier
            nbFile += directory.GetFiles().Length;
            DirectoryInfo[] directories = directory.GetDirectories();
            nbFile += directories.Sum(dir => DirNbFile(dir));
            return nbFile;
        }
        catch (UnauthorizedAccessException)
        {
            return 0;
        }
    }

    private static List<string> GetExtensions(DirectoryInfo directory)
    {
        List<string> extensionsPriorityList = new List<string>();

        FileInfo[] files = directory.GetFiles("*.*", SearchOption.AllDirectories);
        foreach (FileInfo file in files)
        {
            string extension = Path.GetExtension(file.FullName);
            if (!extensionsPriorityList.Contains(extension.ToLower()))
            {
                extensionsPriorityList.Add(extension.ToLower());
            }
        }

        return extensionsPriorityList;
    }
    private static void Chiffrement()
    {
        Process process = new Process();

        process.StartInfo.FileName = @"..\..\..\..\CryptoSoft\bin\Debug\net6.0\CryptoSoft.exe";
        string configFile1 = @"..\..\..\..\CryptoSoft\bin\Debug\net6.0\CryptoSoft.deps.json";
        string configFile2 = @"..\..\..\..\CryptoSoft\bin\Debug\net6.0\CryptoSoft.pdb";
        string configFile3 = @"..\..\..\..\CryptoSoft\bin\Debug\net6.0\CryptoSoft.runtimeconfig.json";
        process.StartInfo.Arguments = $"{configFile1} {configFile2} {configFile3}";
        process.StartInfo.CreateNoWindow = true;

        // Lancer l'application
        process.Start();
    }
}