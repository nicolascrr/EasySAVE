using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using ProjetG2AdminDev.Command;
using ProjetG2AdminDev.Models;
using System.Globalization;
using System.Threading;

namespace ProjetG2AdminDev.ViewModels
{
    internal class SettingsMenuViewModel : AbstractViewModel
    {
        private Model _model;
        public Model Model
        {
            get { return _model; }
            set { _model = value; OnPropertyChanged(nameof(Model)); }
        }

        public List<string> Languages { get; set; }

        public string MaxFileSize { get; set; }
        public int MaxFileSizeConvert { get; set; }

        public ActionCommand ButtonAjouter_ClickCommand { get; set; }
        public ActionCommand ButtonAjouter_Click2Command { get; set; }
        public ActionCommand ButtonAjouter_Click3Command { get; set; }
        public ActionCommand ButtonDelete_ClickCommand { get; set; }
        public ActionCommand DeleteButton_Click2Command { get; set; }
        public ActionCommand DeleteButton_Click3Command { get; set; }
        
        public SettingsMenuViewModel()
        {
            Model= (Model) Application.Current.Resources["AppModel"] ;
            //MessageBox.Show(Model.BackupJobList[0].Name);
            //ExtensionsList = new ObservableCollection<ExtensionItem> (_model.ExtensionsList );
            //CryptFileExtList = new ObservableCollection<ExtensionItem> (_model.CryptFileExtList);
            //BusinessAppList = new ObservableCollection<ExtensionItem> (_model.BusinessAppList);
            ExtensionsList = new ObservableCollection<ExtensionItem> { new ExtensionItem() };
            CryptFileExtList = new ObservableCollection<ExtensionItem> { new ExtensionItem() };
            BusinessAppList = new ObservableCollection<ExtensionItem> { new ExtensionItem() };


            ButtonAjouter_ClickCommand = new ActionCommand(addExtension);
            ButtonAjouter_Click2Command = new ActionCommand(ButtonAjouter_Click2);
            ButtonAjouter_Click3Command = new ActionCommand(ButtonAjouter_Click3);
            ButtonDelete_ClickCommand = new ActionCommand(DeleteButton_Click);
            DeleteButton_Click2Command = new ActionCommand(DeleteButton_Click2);
            DeleteButton_Click3Command = new ActionCommand(DeleteButton_Click3);
            
            _language = "FR";
            Languages = new List<string> { "FR", "EN-US" };
            MaxFileSize = "1000";
            MaxFileSizeConvert = int.Parse(MaxFileSize) * 100;
            // Ajouter des éléments à la liste
            ExtensionsList.Add(new ExtensionItem { Extension = ".exe", IsSelected = true });
            ExtensionsList.Add(new ExtensionItem { Extension = ".pdf", IsSelected = false });
            ExtensionsList.Add(new ExtensionItem { Extension = ".jpg", IsSelected = false });


            // Ajouter des éléments à la liste
            CryptFileExtList.Add(new ExtensionItem { Extension2 = ".exe", IsSelected = true });
            CryptFileExtList.Add(new ExtensionItem { Extension2 = ".pdf", IsSelected = false });
            CryptFileExtList.Add(new ExtensionItem { Extension2 = ".jpg", IsSelected = false });

            // Ajouter des éléments à la liste
            BusinessAppList.Add(new ExtensionItem { Extension3 = "notepad", IsSelected = true });
            BusinessAppList.Add(new ExtensionItem { Extension3 = "calculator", IsSelected = false });
            BusinessAppList.Add(new ExtensionItem { Extension3 = "explorer", IsSelected = false });

            _model.ExtensionsList.Add(new ExtensionItem { Extension = ".exe", IsSelected = true });
            _model.ExtensionsList.Add(new ExtensionItem { Extension = ".pdf", IsSelected = false });
            _model.ExtensionsList.Add(new ExtensionItem { Extension = ".jpg", IsSelected = false });

            // Ajouter des éléments à la liste
            _model.CryptFileExtList.Add(new ExtensionItem { Extension2 = ".exe", IsSelected = true });
            _model.CryptFileExtList.Add(new ExtensionItem { Extension2 = ".pdf", IsSelected = false });
            _model.CryptFileExtList.Add(new ExtensionItem { Extension2 = ".jpg", IsSelected = false });

            // Ajouter des éléments à la liste
            _model.BusinessAppList.Add(new ExtensionItem { Extension3 = "notepad", IsSelected = true });
            _model.BusinessAppList.Add(new ExtensionItem { Extension3 = "calculator", IsSelected = false });
            _model.BusinessAppList.Add(new ExtensionItem { Extension3 = "explorer", IsSelected = false });
        }

        private ObservableCollection<ExtensionItem> _extensionsList;
        public ObservableCollection<ExtensionItem> ExtensionsList
        {
            get { return _extensionsList; }
            set
            {
                _extensionsList = value;
                OnPropertyChanged();
            }
        }


        private ObservableCollection<ExtensionItem> _cryptFileExtList;
        public ObservableCollection<ExtensionItem> CryptFileExtList
        {
            get { return _cryptFileExtList; }
            set
            {
                _cryptFileExtList = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<ExtensionItem> _businessAppList;
        public ObservableCollection<ExtensionItem> BusinessAppList
        {
            get { return _businessAppList; }
            set
            {
                _businessAppList = value;
                OnPropertyChanged();
            }
        }

        private string _language;
        public string Language
        {
            get { return _language; }
            set
            {
                _language = value;
                OnPropertyChanged();
                ChangeLanguage(_language);
            }
        }

        public static List<CultureInfo> AvailableCultures;
        
        public string GetLanguage()
        {
            return _language;
        }

        public void SetLanguage(string value)
        {
            _language = value;
        }
        private static List<CultureInfo>? GetAvailableCultures()
        {
            /*ResourceManager rm = new ResourceManager("ProjetG2AdminDev.Internationalization.View", typeof(ProjetG2AdminDev.Internationalization.View).Assembly);
            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
            return (from culture in cultures
                    let resourceSet = rm.GetResourceSet(culture, true, false)
                    where resourceSet != null
                    select culture).ToList();*/
            return null;
        }

        private void ChangeLanguage(string lang)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(lang);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);
        }

        public class ExtensionItem
        {
            public string Extension { get; set; }
            public string Extension2 { get; set; }
            public string Extension3 { get; set; }
            public bool IsSelected { get; set; }
        }

        public string TextBoxExtensions { get; set; }
        public string TextBoxExtensions2 { get; set; }
        public string TextBoxExtensions3 { get; set; }

        private void addExtension(object sender)
        {
            // Ajouter une nouvelle ligne à la DataGrid avec la valeur de la TextBox
            ExtensionsList.Add(new ExtensionItem { Extension = TextBoxExtensions, IsSelected = false });
            _model.ExtensionsList.Add(new ExtensionItem { Extension = TextBoxExtensions, IsSelected = false });

            TextBoxExtensions = "";
            OnPropertyChanged(nameof(ExtensionsList));
            OnPropertyChanged(nameof(TextBoxExtensions));
        }
        private void ButtonAjouter_Click2(object sender)
        {
            // Ajouter une nouvelle ligne à la DataGrid avec la valeur de la TextBox
            CryptFileExtList.Add(new ExtensionItem { Extension2 = TextBoxExtensions2, IsSelected = false });
            _model.CryptFileExtList.Add(new ExtensionItem { Extension2 = TextBoxExtensions2, IsSelected = false });

            TextBoxExtensions2 = "";
            OnPropertyChanged(nameof(CryptFileExtList));
            OnPropertyChanged(nameof(TextBoxExtensions2));
        }
        private void ButtonAjouter_Click3(object sender)
        {
            // Ajouter une nouvelle ligne à la DataGrid avec la valeur de la TextBox
            BusinessAppList.Add(new ExtensionItem { Extension3 = TextBoxExtensions3, IsSelected = false });
            _model.BusinessAppList.Add(new ExtensionItem { Extension3 = TextBoxExtensions3, IsSelected = false });

            TextBoxExtensions3 = "";
            OnPropertyChanged(nameof(BusinessAppList));
            OnPropertyChanged(nameof(TextBoxExtensions3));
        }
        private void DeleteButton_Click(object sender)
        {
            var selectedItem = ((FrameworkElement)sender).DataContext as ExtensionItem;
            if (selectedItem != null)
            {
                ExtensionsList.Remove(selectedItem);
                OnPropertyChanged(nameof(ExtensionsList));

            }
        }
        private void DeleteButton_Click2(object sender)
        {
            var selectedItem = ((FrameworkElement)sender).DataContext as ExtensionItem;
            if (selectedItem != null)
            {
                CryptFileExtList.Remove(selectedItem);
                OnPropertyChanged(nameof(CryptFileExtList));

            }
        }
        private void DeleteButton_Click3(object sender)
        {
            var selectedItem = ((FrameworkElement)sender).DataContext as ExtensionItem;
            if (selectedItem != null)
            {
                BusinessAppList.Remove(selectedItem);
                OnPropertyChanged(nameof(BusinessAppList));

            }
        }

    }
}
