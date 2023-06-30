using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using ProjetG2AdminDev.ViewModels;

namespace ProjetG2AdminDev.Views
{
    /// <summary>
    /// Logique d'interaction pour TabBackupJobs.xaml
    /// </summary>
    public partial class TabBackupJobs : UserControl
    {
        public TabBackupJobs()
        {
            InitializeComponent();
        }

        private bool _isCommittingEdit = false;

        private void BackupJobDataGrid_OnRowEditEnding(object? sender, DataGridRowEditEndingEventArgs e)
        {
            var dataGrid = sender as DataGrid;

            if (e.EditAction != DataGridEditAction.Commit || _isCommittingEdit) return;
            _isCommittingEdit = true;
            dataGrid?.CommitEdit(DataGridEditingUnit.Row, true);
            _isCommittingEdit = false;
            MainMenuViewModel viewModel = DataContext as MainMenuViewModel;
            viewModel?.RowEditEndingExecute(e);
        }
    }
}
