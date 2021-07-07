using Microsoft.Win32;
using SewoTranslator.Model;
using SewoTranslator.Properties;
using SewoTranslator.View;
using SewoTranslator.ViewModel;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Forms;

namespace SewoTranslator
{
    public sealed partial class App : System.Windows.Application, IDisposable
    {
        private NotifyIcon icon;
        private SavedWordsHandler savedWordsHandler = new SavedWordsHandler("saved_words.txt");

        public App()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CurrentCulture;
        }

        private void StartupMethod(object sender, StartupEventArgs e)
        {
            bool result;
            Mutex mutex = new Mutex(true, "SewoTranslatorID", out result);

            if (!result)
            {
                Dispose();
                Shutdown();
                return;
            }

            if (IsFirstOpening())
            {
                IntroductionWindow iWindow = new IntroductionWindow();
                iWindow.DataContext = new IntroductionVM();
                iWindow.Show();

                Settings.Default.IsFirstOpening = false;
                Settings.Default.Save();
            }

            savedWordsHandler.LoadSavedWordInfosFromFile();
            this.MainWindow = new MainWindow(new MainVM(savedWordsHandler));
            MainWindow.Show();
            
            GC.KeepAlive(mutex);

            CreateNotifyIcon();
        }
        
        private void CreateNotifyIcon()
        {
            MenuItem item1 = new MenuItem(SewoTranslator.Properties.Resources.ShowMI, OnShowClick);
            MenuItem item2 = new MenuItem(SewoTranslator.Properties.Resources.ManualMI, OnManualClick);
            MenuItem item3 = new MenuItem(SewoTranslator.Properties.Resources.SettingsMI, OnSettingsClick);
            MenuItem item4 = new MenuItem(SewoTranslator.Properties.Resources.AboutMI, OnAboutClick);
            MenuItem item5 = new MenuItem(SewoTranslator.Properties.Resources.ExitMI, OnExitClick);
            MenuItem item6 = new MenuItem(SewoTranslator.Properties.Resources.SavedWordsMI, OnSavedWordsClick);
            MenuItem[] items = { item1, item6, item2, item3, item4, item5 };

            icon = new NotifyIcon();
            icon.Icon = new Icon(App.GetResourceStream(new Uri("pack://application:,,,/icon.ico")).Stream);
            icon.ContextMenu = new ContextMenu(items);
            icon.Visible = true;
            icon.MouseClick += NI_MouseClick;
        }

        private bool IsFirstOpening()
        {
            return Settings.Default.IsFirstOpening;
        }

        private void NI_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                MainWindow.Visibility = Visibility.Visible;
                MainWindow.WindowState = WindowState.Normal;
                MainWindow.Activate();
            }
        }

        private void OnShowClick(object sender, EventArgs e)
        {
            MainWindow.Visibility = Visibility.Visible;
            MainWindow.Activate();
        }

        private void OnManualClick(object sender, EventArgs e)
        {
            IntroductionWindow iw = new IntroductionWindow();
            iw.DataContext = new IntroductionVM();
            iw.Show();
        }

        private void OnSettingsClick(object sender, EventArgs e)
        {
            SettingsWindow settingsWin = new SettingsWindow();
            settingsWin.DataContext = new SettingsVM();
            settingsWin.Show();
        }

        private void OnAboutClick(object sender, EventArgs e)
        {
            AboutWindow aboutWin = new AboutWindow();
            aboutWin.DataContext = new AboutVM();
            aboutWin.Show();
        }

        private void OnExitClick(object sender, EventArgs e)
        {
            Current.Shutdown();
        }

        private void OnSavedWordsClick(object sender, EventArgs e)
        {
            SavedWordsVM savedWordsVM = new SavedWordsVM(savedWordsHandler);
            SavedWordsWindow window = new SavedWordsWindow();
            window.DataContext = savedWordsVM;
            window.Show();
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (icon != null)
                {
                    icon.Dispose();
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~App()
        {
            Dispose(false);
        }
    }
}
