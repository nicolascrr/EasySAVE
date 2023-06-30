using System.Collections.Generic;
using System.Text.Json;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using System.ComponentModel;
using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Windows;
using static ProjetG2AdminDev.ViewModels.SettingsMenuViewModel;

namespace ProjetG2AdminDev.Models;

internal class Model
{
    public List<BackupJob> BackupJobList;
    private List<BackupJob> _backupJobListHistory = new();
    private Socket _serverSocket;
    protected Socket ClientSocket;
    
    private const string PathBackupJobInfoStatus = @"C:\EasySave\backupJobInfoStatus.json";
    private const string PathBackupJobInfoHistory = @"C:\EasySave\backupJobInfoHistory.";
    private const string PathBackupJobRepo = @"C:\EasySave\";
    private readonly XmlSerializer _serializer = new(typeof(List<BackupJob>));

    string _format = "json";

    private Semaphore SizeLimitSemaphore = new Semaphore(1, 1);
    private object JSONHistoryLock = new object();
    public object JSONStatusLock = new object();

    public Dictionary<int, List<object>>? BackupJobThreads;

    public event EventHandler<ProgressEventArgs> UpdateJobState;

    //public event PropertyChangedEventHandler? PropertyChanged;
    //public event EventHandler<ProgressEventArgs> RaiseEventSocketClient = null!;

    //private int _maxFileSize = 10000;
    //TODO faire get set
    public List<ExtensionItem> ExtensionsList;
    public List<ExtensionItem> CryptFileExtList;
    public List<ExtensionItem> BusinessAppList;

    public string GetFormat()
        {
            return _format;
        }

    public void SetFormat(string value)
    {
        _format = value;
    }

    public Model()
    {
        BackupJobList = new List<BackupJob>();
        BackupJobThreads = new Dictionary<int, List<object>>();
        Directory.CreateDirectory(PathBackupJobRepo);
        if (File.Exists(PathBackupJobInfoStatus))
        {
            BackupJobList = JsonSerializer.Deserialize<List<BackupJob>>(File.ReadAllText(PathBackupJobInfoStatus));
        }

        ResetJsonAtStart();
        if (!File.Exists(PathBackupJobInfoHistory + _format)) return;
        if (_format == "json")
        {
            _backupJobListHistory = JsonSerializer.Deserialize<List<BackupJob>>(File.ReadAllText(PathBackupJobInfoHistory + _format));
        }
        else
        {
            _backupJobListHistory = (List<BackupJob>)_serializer.Deserialize(new FileStream(PathBackupJobInfoHistory + _format, FileMode.Open));
        }
        ConnectSocket();

        ExtensionsList = new List<ExtensionItem>();
        CryptFileExtList = new List<ExtensionItem>();
        BusinessAppList = new List<ExtensionItem>();


    }

    public void ConnectSocket()
    {
        Thread threadSocket = new Thread(() =>
        {
            IPAddress address = IPAddress.Parse("127.0.0.1");
            IPEndPoint endPoint = new IPEndPoint(address, 12345);
            _serverSocket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _serverSocket.Bind(endPoint);
            _serverSocket.Listen(10);
            ClientSocket = _serverSocket.Accept();
            SendJsonToClient();
            ProcessResponse();
        });
        threadSocket.Start();
    }

    private void ProcessResponse()
    {
        while (true)
        {
            byte[] buffer = new byte[1024];
            int bytesReceived = ClientSocket.Receive(buffer);
            string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
            string[] data = dataReceived.Split("|end|");
            foreach (string dataSplitted in data)
            {
                if(dataSplitted == "") break;
                string[] d = dataSplitted.Split(",");
                switch (d[0])
                {
                    case "start":
                        BackupJob job = BackupJobList.FirstOrDefault(j => j.ID == int.Parse(d[1]));
                        if (job.State == "Inactive") { StartBackupJob(job); } 
                        else if(job.State == "Paused") {ResumeBackupJob(job);}
                        break;
                    case "pause":
                        PauseBackupJob(int.Parse(d[1]));
                        break;
                    case "stop":
                        StopBackupJob(int.Parse(d[1]));
                        break;
                }
            }
        }
    }

    public void CloseSocket() {
        ClientSocket.Shutdown(SocketShutdown.Both);
        ClientSocket.Close();
    }

    public void SendJsonToClient()
    {
        try
        {
            if (_serverSocket == null || ClientSocket == null) return;
            string json = "json" + File.ReadAllText("C:\\EasySave\\backupJobInfoStatus.json") + "|end|";
            byte[] data = Encoding.UTF8.GetBytes(json);
            ClientSocket.Send(data);
        }
        catch (SocketException)
        {

        }
    }

    public void SendDataToClient(string action, int id,  float obj)
    {
        try
        {
            if (_serverSocket == null || ClientSocket == null) return;
            string data = action + "|" + id + "|" + obj;
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            ClientSocket.Send(bytes);
        }
        catch (SocketException)
        {

        }
    }


    private void ResetJsonAtStart()
    {
        foreach (BackupJob backupJob in BackupJobList)
        {
            backupJob.State = "Inactive";
            backupJob.Progress = 0;
            backupJob.IsRunning = false;
        }
    }

    public void SaveBackupJobToJsonHistory(BackupJob backupJob)
    {
        if (backupJob.Progress == 100) { backupJob.Progress = 0; }
        backupJob.DowloadingFileSize = 0;
        _backupJobListHistory.Add(backupJob);
        if (_format == "json")
        {
            var options = new JsonSerializerOptions
                { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };
            string json = JsonSerializer.Serialize(_backupJobListHistory, options);
            File.WriteAllText(PathBackupJobInfoHistory + _format, json);
        }
        else { _serializer.Serialize(new StreamWriter(PathBackupJobInfoHistory + _format), _backupJobListHistory); }
    }

    public void SaveCreationOfBackup()
    {
        var options = new JsonSerializerOptions
            { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };
        string newJson = JsonSerializer.Serialize(BackupJobList, options);
        File.WriteAllText(PathBackupJobInfoStatus, newJson);
        if(_serverSocket != null && ClientSocket != null) SendJsonToClient();

    }

    public void SaveInRealTime(BackupJob backupJob)
    {
        var options = new JsonSerializerOptions
            { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };
        int count = 0;
        foreach (BackupJob backup in BackupJobList)
        {
            if (backup.ID == backupJob.ID)
            {
                BackupJobList.Remove(backup);
                break;
            }
            count++;
        }

        if (backupJob.Progress == 100)
        {
            backupJob.Progress = 0;
            backupJob.State = "Inactive";
            backupJob.IsRunning = false;
        }

        BackupJobList.Insert(count, backupJob);
        string newJson = JsonSerializer.Serialize(BackupJobList, options);
        File.WriteAllText(PathBackupJobInfoStatus, newJson);
    }

    internal BackupJob CreateBackupJob(int id, string name, string pathSource, string pathDestination, string type)
    {
        BackupJob job = new BackupJob(id, name, pathSource, pathDestination, type);
        BackupJobList.Add(job);
        SaveCreationOfBackup();
        return job;
    }

    public void StartBackupJob(BackupJob job)
    {
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        ManualResetEvent manualResetEvent = new ManualResetEvent(true);
        BackupJobThreadState threadState = new BackupJobThreadState(job, cancellationTokenSource.Token, manualResetEvent);

        List<object> list = new List<object>();
        list.Add(manualResetEvent);
        list.Add(cancellationTokenSource);
        BackupJobThreads?.Add(job.ID, list);

        Thread thread = new Thread(DoBackupJob);
        thread.Start(threadState);
        SendJsonToClient();
        SendDataToClient("start", job.ID, 0);
    }
    
    private void DoBackupJob(object state)
    {
        BackupJobThreadState threadState = (BackupJobThreadState)state;
        BackupJob job = threadState.Job;
        CancellationToken cancellationToken = threadState.CancellationToken;
        ManualResetEvent manualResetEvent = threadState.ManualResetEvent;
        string res = job.Start(cancellationToken, manualResetEvent);
        
        switch (res)
        {
            //TODO move to viewmodel
            case "NoAccess":
                MessageBox.Show("L'accès aux dossiers et fichiers n'est pas possible");
                break;
            case "NonExistingPath":
                MessageBox.Show("Le travail n'a pas pu être exécuté parce que le chemin source n'est pas valide");
                break;
            case "ok":
            {
                //BackupJob backupJob = BackupJobList.FirstOrDefault(bJob => bJob.ID == job.ID);
                //backupJob.IsRunning = true;
                //OnPropertyChanged(nameof(backupJob.IsRunning));
                UpdateJobState?.Invoke(this, new ProgressEventArgs{ID = job.ID, IsRunning = true, State = "Active"});
                lock (JSONHistoryLock)
                {
                    //backupJob.IsRunning = false;
                    //OnPropertyChanged(nameof(backupJob.IsRunning));
                    SaveBackupJobToJsonHistory(job);
                    UpdateJobState?.Invoke(this, new ProgressEventArgs{ID = job.ID, IsRunning = false, State = "Inactive"});
                    BackupJobThreads.Remove(job.ID);
                    MessageBox.Show(string.Format("Le travail {0} a été exécuté avec succès", job.Name));
                }
                break;
            }
        } 
    }

    public void ResumeBackupJob(BackupJob job)
    {
        BackupJob backupJob = BackupJobList.FirstOrDefault(bJob => bJob.ID == job.ID);
        backupJob.State = "Active";
        ManualResetEvent mre = (ManualResetEvent)BackupJobThreads[job.ID][0];
        mre.Set();
        SendJsonToClient();
        SendDataToClient("start", job.ID, 0);
    }

    public void PauseBackupJob(int id)
    {
        try
        {
            //if(BackupJobThreads[id] == null) return;
            ManualResetEvent mre = BackupJobThreads[id][0] as ManualResetEvent;
            mre.Reset();
            BackupJob backupJob = BackupJobList.FirstOrDefault(bJob => bJob.ID == id);
            backupJob.State = "Paused";
            SaveCreationOfBackup();
        }
        catch (NullReferenceException)
        {
            return;
        }
    }

    public void StopBackupJob(int jobID)
    {
        CancellationTokenSource cts = (CancellationTokenSource)BackupJobThreads[jobID][1];
        cts.Cancel();
        cts.Dispose();
        BackupJob backupJob = BackupJobList.Find(j => j.ID == jobID);
        backupJob.Progress = 0;
        backupJob.State = "Inactive";
        backupJob.IsRunning = false;
        BackupJobThreads.Remove(jobID);
        SaveCreationOfBackup();
    }
    
    public void DeleteBackupJob(int index)
    {
        BackupJobList.RemoveAt(index);
        SaveCreationOfBackup();
    }

    public BackupJob EditBackupJob(BackupJob job)
    {
        BackupJob backupJob = BackupJobList.Find(j => j.ID == job.ID);
        backupJob.Name = job.Name;
        backupJob.SourcePath = job.SourcePath;
        backupJob.DestinationPath = job.DestinationPath;
        backupJob.Type = job.Type;
        SaveCreationOfBackup();
        return backupJob;
    }
}