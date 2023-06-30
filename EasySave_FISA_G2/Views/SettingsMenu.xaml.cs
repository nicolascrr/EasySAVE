using ProjetG2AdminDev.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProjetG2AdminDev.Views
{
    /// <summary>
    /// Logique d'interaction pour SettingsMenu.xaml
    /// </summary>
    public partial class SettingsMenu : UserControl
    {

        public SettingsMenu()
        {
            InitializeComponent();
            //DataContext = new SettingsMenuViewModel();
        }
            
        private void Button_Click_Retour(object sender, RoutedEventArgs e)
        {
            //this.Visibility = System.Windows.Visibility.Collapsed;
        }
        private void SwitchMode(object sender, RoutedEventArgs e)
        {
        }


    }

}
