using Microsoft.Win32;
using SewoTranslator.Command;
using SewoTranslator.Model;
using SewoTranslator.Properties;
using SewoTranslator.View;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace SewoTranslator.ViewModel
{
    public sealed class SettingsVM : ViewModelBase
    {
        private int access = 6;
        private bool _isUnfoundWordsButtonVisible;
        private bool _runApplicationOnComputerStartup;
        private ICommand unfoundWords;
        private ICommand close;
        private ICommand mouseLeftButtonDown;
        private ICommand mouseRightButtonDown;

        private readonly string applicationName = "SewoTranslator";
        private readonly string currentVersionRun = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

        public SettingsVM()
        {
            InitRunApplicationOnComputerStartup();
            InitUnfoundWordsButtonVisibility();
        }

        public bool IsUnfoundWordsButtonVisible
        {
            get
            {
                return _isUnfoundWordsButtonVisible;
            }
            set
            {
                _isUnfoundWordsButtonVisible = value;

                OnPropertyChanged("IsUnfoundWordsButtonVisible");
            }
        }

        public bool RunApplicationOnComputerStartup
        {
            get
            {
                return _runApplicationOnComputerStartup;
            }
            set
            {
                _runApplicationOnComputerStartup = value;

                OnPropertyChanged("RunApplicationOnComputerStartup");
            }
        }

        //public string SavedWordsDirectoryPath
        //{
        //    get => _savedWordsDirectoryPath;

        //}

        public ICommand Close
        {
            get
            {
                if (close == null)
                {
                    close = new RelayCommand<IClosable>(CloseWindow);
                }
                return close;
            }
        }

        public ICommand UnfoundWords
        {
            get
            {
                if (unfoundWords == null)
                {
                    unfoundWords = new RelayCommand(UnfoundWordsAction);
                }
                return unfoundWords;
            }
        }

        public ICommand MouseLeftButtonDown
        {
            get
            {
                if (mouseLeftButtonDown == null)
                {
                    mouseLeftButtonDown = new RelayCommand(OnMouseLeftButtonDown);
                }
                return mouseLeftButtonDown;
            }
        }

        public ICommand MouseRightButtonDown
        {
            get
            {
                if (mouseRightButtonDown == null)
                {
                    mouseRightButtonDown = new RelayCommand(OnMouseRightButtonDown);
                }
                return mouseRightButtonDown;
            }
        }
        
        private void InitRunApplicationOnComputerStartup()
        {
            try
            {
                object value = Registry.CurrentUser.OpenSubKey(currentVersionRun).GetValue(applicationName);

                if (value != null)
                {
                    RunApplicationOnComputerStartup = true;
                }
            }
            catch (UnauthorizedAccessException e)
            {
                MessageBox.Show("Access denied: " + e.Message);
            }
        }

        private void InitUnfoundWordsButtonVisibility()
        {
            IsUnfoundWordsButtonVisible = Settings.Default.HandleUnfoundWords ? true : false;
        }

        private void UnfoundWordsAction()  //  !!!!!!!!!!!
        {
            UnfoundWordsManager uwm = new UnfoundWordsManager();

            UnfoundWordsWindow uww = new UnfoundWordsWindow();
            uww.DataContext = new UnfoundWordsVM(uwm.UnfoundWords());
            uww.Show();
        }

        public void OnMouseLeftButtonDown()
        {
            access = 0;
        }

        public void OnMouseRightButtonDown()
        {
            if (++access == 5)
            {
                if (IsUnfoundWordsButtonVisible == false)
                {
                    IsUnfoundWordsButtonVisible = true;

                    Settings.Default.HandleUnfoundWords = true;
                }
                else
                {
                    IsUnfoundWordsButtonVisible = false;

                    Settings.Default.HandleUnfoundWords = false;
                }
            }
        }

        private void CloseWindow(IClosable window)
        {
            SaveSettings();
            Settings.Default.Save();

            window.Close();
        }

        private void SaveSettings()
        {
            RegistryKey rkey = Registry.CurrentUser.OpenSubKey(currentVersionRun, true);

            try
            {
                if (RunApplicationOnComputerStartup)
                {
                    string path = GetPathToExeFile();
                    rkey.SetValue(applicationName, path);
                }
                else
                {
                    if (rkey.GetValueNames().Contains(applicationName))
                    {
                        rkey.DeleteValue(applicationName);
                    }
                }
            }
            catch (UnauthorizedAccessException e)
            {
                MessageBox.Show("Could not save settings. Access denied.");
            }
            finally
            {
                if (rkey != null)
                    rkey.Close();
            }
        }

        private string GetPathToExeFile()
        {
            return System.Reflection.Assembly.GetEntryAssembly().Location;
        }

    }
}
