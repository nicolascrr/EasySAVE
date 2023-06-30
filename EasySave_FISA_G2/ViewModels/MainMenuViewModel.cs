using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using ProjetG2AdminDev.Command;
using ProjetG2AdminDev.Models;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System;

namespace ProjetG2AdminDev.ViewModels;

internal class MainMenuViewModel : AbstractViewModel
{
    private Model _model;
    public Model Model
    {
        get { return _model; }
        set { _model = value; OnPropertyChanged(nameof(Model)); }
    }
    public ActionCommand AddJobCommand { get; set; }
    public ActionCommand DeleteAllJobCommand { get; set; }
    public ActionCommand StartAllCommand { get; set; }
    public ActionCommand DeleteJobCommand { get; set; }
    public ActionCommand StartJobCommand { get; set; }
    public ActionCommand PauseJobCommand { get; set; }
    public ActionCommand StopJobCommand { get; set; }
    public ActionCommand BrowseSourcePathCommand { get; set; }
    public ActionCommand BrowseDestinationPathCommand { get; set; }
    
    private string? _selectedFolderPath;
    public string? SelectedFolderPath { get; set; }
    
    private string _format;
    public string Format
    {
        get { return _format; }
        set
        {
            _format = value;
            OnPropertyChanged();
            FormatChange(_format);
        }
    }

    private BackupJob _selectedJob;
    public BackupJob SelectedJob
    {
        get { return _selectedJob; }
        set { _selectedJob = value; OnPropertyChanged(nameof(SelectedJob)); }
    }

    public List<string> Formats { get; set; }

    public List<string> AppNameBanList { get; set; }

    protected bool isAddButtonEnabled;

    public bool IsAddButtonEnabled
    {
        get { return isAddButtonEnabled; }
        set
        {
            isAddButtonEnabled = value;
            OnPropertyChanged();
        }
    }

    protected static ObservableCollection<BackupJob>? BackupJobList;

    public ObservableCollection<BackupJob>? BackupJobs
    {
        get { return BackupJobList; }
        set
        {
            BackupJobList = value;
            OnPropertyChanged();
        }
    }
    
    private string _searchText;

    public string SearchText
    {
        get { return _searchText; }
        set
        {
            if (_searchText != value)
            {
                _searchText = value;
                RaisePropertyChanged(nameof(SearchText));
                FilterData();
            }
        }
    }

    public List<string> Types { get; set; }

    public MainMenuViewModel()
    {
        _model =(Model)Application.Current.Resources["AppModel"] ;
        BackupJobList = new ObservableCollection<BackupJob>(_model.BackupJobList);
        IsAddButtonEnabled = true;
        foreach (var job in BackupJobList)
        {
            RefreshPriority(job);
        }
        

        AppNameBanList = new List<string>();
        AddJobCommand = new ActionCommand(AddBackupJob);

        DeleteAllJobCommand = new ActionCommand(DeleteAllBackupJob);
        StartAllCommand = new ActionCommand(StartAllBackupJob);
        DeleteJobCommand = new ActionCommand(DeleteBackupJob);
        StartJobCommand = new ActionCommand(LaunchBackupJob);
        PauseJobCommand = new ActionCommand(PauseBackupJob);
        StopJobCommand = new ActionCommand(StopBackupJob);

        BrowseSourcePathCommand = new ActionCommand(BrowseSourcePath);
        BrowseDestinationPathCommand = new ActionCommand(BrowseDestinationPath);

        Types = new List<string> { "Differential", "Complete" };

        Format = "JSON";
        Formats = new List<string> { "JSON", "XML" };
        AppNameBanList.Add("notepad");
        Thread businessApp = new Thread(CheckingForBusinessSoftware);
        businessApp.Start();

        _model.UpdateJobState += UpdateJobState;
    }

    private void UpdateJobState(object? sender, ProgressEventArgs e)
    {
        GetJob(e.ID).State = e.State;
        GetJob(e.ID).IsRunning = e.IsRunning;
    }


    private void FormatChange(string format)
    {
        _model.SetFormat(format.ToLower());
    }
    

    private void AddBackupJob(object sender)
    {
        if (BackupJobList.Count == 0)
        {
            BackupJobList?.Add(new BackupJob(0, "Click to edit", "", "", "Complete"));
        }
        else
        {
            BackupJobList?.Add(new BackupJob(BackupJobList[^1].ID + 1, "Click to edit", "", "", "Complete"));
        }
        IsAddButtonEnabled = false;
        OnPropertyChanged(nameof(IsAddButtonEnabled));
        //MessageBox.Show(_model.ExtensionsList[^1].Extension);
        //MessageBox.Show(_model.CryptFileExtList[^1].Extension2);

        //MessageBox.Show(_model.BusinessAppList[^1].Extension3);


    }

    private void LaunchBackupJob(object obj)
    {
        foreach (Process process in Process.GetProcesses())
        {
            if (!AppNameBanList.Contains(process.ProcessName)) continue;
            MessageBox.Show(string.Format(
                "Application métier détecté, vous ne pouvez pas lancer de sauvegarde : {0}",
                process.ProcessName));
            return;
        }

        BackupJob? job = obj as BackupJob;
        if (job is { IsRunning: true })
        {
            _model.ResumeBackupJob(job);
            BackupJob backupJob = BackupJobList.FirstOrDefault(bJob => bJob.ID == job.ID);
            backupJob.State = "Active";
        }
        else { _model.StartBackupJob(job); StartBackupJob(job);}
    }

    private void StartBackupJob(BackupJob job)
    {
        job.RaiseEvent += HandleEvent;
        job.RaiseEventSizeLimit += HandleEventSizeLimit;
    }

    private void PauseAll()
    {
        BackupJob job = new BackupJob();
        foreach (KeyValuePair<int, List<object>> threadInfo in _model.BackupJobThreads)
        {
            foreach (BackupJob backupJob in BackupJobList)
            {
                if (backupJob.ID == threadInfo.Key)
                {
                    job = backupJob;
                }
            }
            lock (_model.JSONStatusLock) 
            { 
                PauseBackupJob(job);
            }
        }
    }

    protected void RefreshPriority(BackupJob job)
    {
        if (job.PriorityList.Contains(".mp4"))
        {
            job.HasPriority = true;
            OnPropertyChanged(nameof(job.HasPriority));
        }
        else
        {
            job.HasPriority = false;
            OnPropertyChanged(nameof(job.HasPriority));
        }

        job = _model.EditBackupJob(job);
        int index = BackupJobList.IndexOf(BackupJobList.FirstOrDefault(backupJob => backupJob.ID == job.ID));
        BackupJobList[index].HasPriority = job.HasPriority;
    }

    private void CheckingForBusinessSoftware()
    {
        while (true)
        {
            foreach (Process process in Process.GetProcesses())
            {
                if (AppNameBanList.Contains(process.ProcessName))
                {
                    PauseAll();
                }
            }
        }
    }

    private void DeleteAllBackupJob(object sender)
    {
        if (ConfirmAction("supprimer"))
        {
            BackupJobList.Clear();
            _model.BackupJobList.Clear();
            lock (_model.JSONStatusLock)
            {
                Model.SaveCreationOfBackup();
                MessageBox.Show("Tous les travaux de sauvegarde ont été supprimés.");
            }

            IsAddButtonEnabled = true;
            OnPropertyChanged(nameof(IsAddButtonEnabled));
        }
        else
        {
            return;
        }
    }

    private void StartAllBackupJob(object sender)
    {
        if (ConfirmAction("exécuter"))
        {
            List<Task> backupTasks = new List<Task>();
            foreach (var backupJob in Model.BackupJobList)
            {
                backupTasks.Add(Task.Run(() => LaunchBackupJob(backupJob)));
            }
        }
        else
        {
            return;
        }
    }

    private void FilterData()
    {
        CollectionViewSource.GetDefaultView(BackupJobList).Filter = item =>
        {
            var data = item as BackupJob;
            return data.Name.Contains(SearchText);
        };
    }

    public new event PropertyChangedEventHandler? PropertyChanged;

    private void RaisePropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void HandleEvent(object sender, ProgressEventArgs BackupJobEvent)
    {
        BackupJob backupJob = new BackupJob(BackupJobEvent.ID, BackupJobEvent.Name, BackupJobEvent.SourcePath,
            BackupJobEvent.DestinationPath, BackupJobEvent.Type)
        {
            SizeInOctets = BackupJobEvent.SizeInOctets,
            LastExecDuration = BackupJobEvent.LastExecDuration,
            LastExecTime = BackupJobEvent.LastExecTime,
            State = BackupJobEvent.State,
            IsRunning = BackupJobEvent.IsRunning,
            TotalFileToCopy = BackupJobEvent.TotalFileToCopy,
            NbFileLeft = BackupJobEvent.NbFileLeft,
            Progress = BackupJobEvent.Progress,
            PriorityList = BackupJobEvent.PriorityList,
            HasPriority = BackupJobEvent.HasPriority,
            DowloadingFileSize = BackupJobEvent.DowloadingFileSize,
        };
        /*SettingsMenuViewModel settingsMenuViewModel = new SettingsMenuViewModel();
        if (backupJob.DowloadingFileSize > settingsMenuViewModel.MaxFileSizeConvert)
        {
            SizeLimitSemaphore.Release();
        }*/

        if (backupJob.Progress == 100)
        {
            backupJob.RaiseEvent -= HandleEvent;
            backupJob.RaiseEventSizeLimit -= HandleEventSizeLimit;
        }

        lock (_model.JSONStatusLock)
        {
            _model.SaveInRealTime(backupJob);
            //Model.SendDataToClient("progress", BackupJobEvent.ID, BackupJobEvent.Progress);
            _model.SendJsonToClient();
        }
    }

    private void HandleEventSizeLimit(object sender, ProgressEventArgs BackupJobEvent)
    {
        /*SettingsMenuViewModel settingsMenuViewModel = new SettingsMenuViewModel();
        if (BackupJobEvent.DowloadingFileSize > settingsMenuViewModel.MaxFileSizeConvert)
        {
            SizeLimitSemaphore.WaitOne();
        }*/
    }

    private bool ConfirmAction(string word)
    {
        MessageBoxResult result =
            MessageBox.Show(string.Format("Voulez-vous vraiment {0} tous les travaux de sauvegarde ?", word),
                "Confirmation", MessageBoxButton.YesNo);
        return result != MessageBoxResult.No;
    }

    public static BackupJob GetJob(object job)
    {
        if (job is int)
        {
            return BackupJobList.FirstOrDefault(j => j.ID == (int)job);
        }
        BackupJob bJob = job as BackupJob;
        return BackupJobList.FirstOrDefault(j => j.ID == bJob.ID);
    }

    private void PauseBackupJob(object obj)
    {
        _model.PauseBackupJob(GetJob(obj).ID);
        BackupJob backupJob = BackupJobList.FirstOrDefault(bJob => bJob.ID == GetJob(obj).ID);
        backupJob.State = "Paused";
    }

    private void StopBackupJob(object obj)
    {
        PauseBackupJob(obj);
        if (obj is not BackupJob job) return;
        GetJob(obj).Progress = 0;
        GetJob(obj).State = "Inactive";
        GetJob(obj).IsRunning = false;
        _model.StopBackupJob(job.ID);

    }

    private void DeleteBackupJob(object obj)
    {
        BackupJob job = (BackupJob)obj;
        int index = BackupJobList.IndexOf(job);
        BackupJobList.RemoveAt(BackupJobList.IndexOf(job));
        if (!_model.BackupJobList.Any(o => o.ID == job.ID))
        {
            IsAddButtonEnabled = true;
            OnPropertyChanged(nameof(IsAddButtonEnabled));
            return;
        }

        lock (_model.JSONStatusLock)
        {
            _model.DeleteBackupJob(index);
            MessageBox.Show("Le travail de sauvegarde a bien été supprimé.");
        }
    }

    public void RowEditEndingExecute(DataGridRowEditEndingEventArgs parameter)
    {
        DataGridRow? row = parameter?.Row;
        BackupJob job = (BackupJob)row?.Item!;
        if (job.Name != "" && job.Name != "Click to edit" && job.SourcePath != "" && job.DestinationPath != "" &&
            Types.Contains(job.Type))
        {
            if (BackupJobList.Count > _model.BackupJobList.Count)
            {
                job.ID++; // Starting to one to be able store it in JSON
                job = _model.CreateBackupJob(job.ID, job.Name, job.SourcePath, job.DestinationPath, job.Type);
                OnPropertyChanged(nameof(IsAddButtonEnabled));
                MessageBox.Show("Le travail de sauvegarde a été enregistré avec succès.");
            }
            else
            {
                lock (_model.JSONStatusLock)
                {
                    job = _model.EditBackupJob(job);
                    MessageBox.Show("Le travail de sauvegarde a été modifiée avec succès.");
                }
            }
            RefreshPriority(job);
        }
    }

    private string BrowsePath()
    {
        var dialog = new Microsoft.Win32.OpenFileDialog();
        dialog.ValidateNames = false;
        dialog.CheckFileExists = false;
        dialog.CheckPathExists = true;
        dialog.FileName = "Select folder";
        dialog.Filter = "Folders|no files";
        dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
        dialog.Title = "Select Folder";
        dialog.Multiselect = false;

        return dialog.ShowDialog() == true ? System.IO.Path.GetDirectoryName(dialog.FileName) : "error";
    }

    private void BrowseDestinationPath(object obj)
    {
        BackupJobList[BackupJobList.IndexOf(GetJob(obj))].DestinationPath = BrowsePath();
    }

    private void BrowseSourcePath(object obj)
    {
        BackupJobList[BackupJobList.IndexOf(GetJob(obj))].SourcePath = BrowsePath(); ;
    }
}