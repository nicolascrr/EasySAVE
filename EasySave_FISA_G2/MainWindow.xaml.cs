using System.ComponentModel;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Windows;
using ProjetG2AdminDev.Views;
using System.Text;
using System;
using ProjetG2AdminDev.ViewModels;

namespace ProjetG2AdminDev
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
       
        private MainMenu mainMenu;
        private SettingsMenu settingsMenu;
        private Mutex mutex;

        public MainWindow()
        {
            mutex = new Mutex(true, "EasySave");
            if (!mutex.WaitOne(TimeSpan.Zero, true)) Environment.Exit(0); 
            
            InitializeComponent();
            Closing += Window_Closing;
            
            mainMenu = new MainMenu();
            settingsMenu = new SettingsMenu();
            contentControl.Content = mainMenu;
            mainMenu.SettingsButton.Click += Button_Click_Settings;
            settingsMenu.RetourButton.Click += Button_Click_Retour;
        }

        
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Voulez-vous vraiment quitter ?", "Confirmation", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                Environment.Exit(0);
                mutex.ReleaseMutex();
            }
        }

        private void Button_Click_Settings(object sender, RoutedEventArgs e)
        {
            contentControl.Content = settingsMenu;
        }

        private void Button_Click_Retour(object sender, RoutedEventArgs e)
        {
            contentControl.Content = mainMenu;
        }
    }
}
