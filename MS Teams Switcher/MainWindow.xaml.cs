using Microsoft.VisualBasic;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading;
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

namespace MS_Teams_Switcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        readonly string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString();
        readonly string localappdata = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).ToString();
        Properties.Settings settings;
        public MainWindow()
        {
            settings = Properties.Settings.Default;
            InitializeComponent();
            CheckFirstLaunch();
            RefreshMenu();
        }

        public void RefreshMenu()
        {
            // Button tempalte = defaultButton;

            for (int i = 0; i < ButtonsHolder.Children.Count; i++)
            {

                ButtonsHolder.Children[i].Visibility = Visibility.Hidden;
            }


            for (int i = 0; i < settings.Accounts.Count; i++)
            {
                Button button = ButtonsHolder.Children[i] as Button;
                button.Content = settings.Accounts[i];
                button.Click += (sender, EventArgs) => { button_Click(sender, EventArgs); };

                button.Visibility = Visibility.Visible;

            }

        }

        protected void button_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            int index = ButtonsHolder.Children.IndexOf(button);
            chooseAccount(index);
        }



        public void chooseAccount(int index)
        {

            if (index != settings.LastId)
            {
                foreach (var process in Process.GetProcessesByName("Teams"))
                {
                    process.Kill();
                }


                while (Process.GetProcessesByName("Teams").Length > 0)
                {
                    Thread.Sleep(1000);
                }



                Directory.Move(appdata + "/Microsoft/Teams", appdata + "/Microsoft/Teams" + settings.LastId.ToString());
                Directory.Move(localappdata + "/Microsoft/Teams", localappdata + "/Microsoft/Teams" + settings.LastId.ToString());

                Directory.Move(appdata + "/Microsoft/Teams" + index.ToString(), appdata + "/Microsoft/Teams");
                Directory.Move(localappdata + "/Microsoft/Teams" + index.ToString(), localappdata + "/Microsoft/Teams");

                settings.LastId = index;


                Process.Start(appdata + "/Microsoft/Windows/Start Menu/Programs/Microsoft Teams.lnk");
                this.Close();
            }
            else
            {
                Process.Start(appdata + "/Microsoft/Windows/Start Menu/Programs/Microsoft Teams.lnk");
                this.Close();

            }



        }



        public void CheckFirstLaunch()
        {
            if (settings.Accounts == null)
            {
                settings.Accounts = new System.Collections.Specialized.StringCollection();
            }


            //Create Fresh Backup
            if (!settings.IsPrepared)
            {
                Copy(appdata + "/Microsoft/Teams", appdata + "/Microsoft/TeamsFresh");
                Copy(localappdata + "/Microsoft/Teams", localappdata + "/Microsoft/TeamsFresh");

            }

            if (settings.Accounts.Count == 0)
            {
                settings.Accounts.Add(Interaction.InputBox("Podaj opierwszy adres e-mail", "Dodawanie Adres E-mail", "user@domain.eq"));
            }

            settings.IsPrepared = true;
        }



        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ///Dodaj Konto
            String newAccount = Interaction.InputBox("Podaj następny adres e-mail", "Dodawanie Adres E-mail", "user@domain.eq");
            int newId = settings.Accounts.Add(newAccount);
            CreateDirectory(newId);
        }


        public void CreateDirectory(int id)
        {
            Copy(appdata + "/Microsoft/TeamsFresh", appdata + "/Microsoft/Teams" + id.ToString());
            Copy(localappdata + "/Microsoft/TeamsFresh", localappdata + "/Microsoft/Teams" + id.ToString());
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            settings.Save();
        }


        public void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                //Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                fi.CopyTo(System.IO.Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }

        public void Copy(string sourceDirectory, string targetDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

            CopyAll(diSource, diTarget);
        }

    }
}
