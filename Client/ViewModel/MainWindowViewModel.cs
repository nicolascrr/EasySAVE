using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks.Dataflow;
using System.Windows;
using Client.Model;
using ProjetG2AdminDev.Command;


namespace Client;
internal class MainWindowViewModel : ViewModel.AbstractViewModel
{
    private Socket _socket;
    private IPEndPoint _endPoint;
    private List<BackupJob>? _backupJobList;
    
    public ActionCommand StartJobCommand { get; set; }
    public ActionCommand PauseJobCommand { get; set; }
    public ActionCommand StopJobCommand { get; set; }
    public List<BackupJob> BackupJobList
    {
        get { return _backupJobList; }
        set
        {
            _backupJobList = value;
            OnPropertyChanged(nameof(_backupJobList));
        }
    }
    public MainWindowViewModel()
    {
        StartJobCommand = new ActionCommand(StartBackupJob);
        PauseJobCommand = new ActionCommand(PauseBackupJob);
        StopJobCommand = new ActionCommand(StopBackupJob);
        
        IPAddress address = IPAddress.Parse("127.0.0.1");
        _endPoint = new IPEndPoint(address, 12345);
        _socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        Start();
        //Close();
    }
    
    public void Start()
    {
        Thread clientSocketThread = new Thread(() =>
        {
            _socket.Connect(_endPoint);
            while (true)
            {
                byte[] buffer = new byte[_socket.ReceiveBufferSize];
                int bytesRecus = _socket.Receive(buffer);
                string data = Encoding.UTF8.GetString(buffer, 0, bytesRecus);
                string[] dataReceived = data.Split("|end|");
                foreach (string dataSplitted in dataReceived)
                {
                    data = dataSplitted;
                    if(data == "") break;
                    if (data.Substring(0, 4) == "json")
                    {
                        data = data.Substring(4);
                        _backupJobList = JsonSerializer.Deserialize<List<BackupJob>>(data);
                    }
                    else
                    {
                        string[] s = data.Split('|');
                        BackupJob job = _backupJobList.FirstOrDefault(j => j.ID == int.Parse(s[1]));
                        switch (s[0])
                        {
                            case "start":
                                job.State = "Active";
                                job.IsRunning = true;
                                OnPropertyChanged(nameof(BackupJobList));
                                break;
                            case "progress":
                                //MessageBox.Show(s[2]);
                                job.Name = s[2];
                                //MessageBox.Show(job.Progress.ToString());
                                OnPropertyChanged("");
                                break;
                        }
                    }
                    OnPropertyChanged(nameof(BackupJobList));
                }
            }
        });
        clientSocketThread.Start();
    }
    
    public void Close()
    {
        _socket.Shutdown(SocketShutdown.Both);
        _socket.Close();
    }
    
    private void StartBackupJob(object obj)
    {
        BackupJob? job = obj as BackupJob;
        _socket.Send(Encoding.UTF8.GetBytes("start," + job.ID + "|end|"));
    }
    
    private void PauseBackupJob(object obj)
    {
        BackupJob? job = obj as BackupJob;
        _socket.Send(Encoding.UTF8.GetBytes("pause," + job.ID + "|end|"));
    }

    private void StopBackupJob(object obj)
    {
        BackupJob? job = obj as BackupJob;
        _socket.Send(Encoding.UTF8.GetBytes("stop," + job.ID + "|end|"));
    }
}