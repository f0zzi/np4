using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
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

namespace filedownloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public class FileInfo : INotifyPropertyChanged
    {
        public string URL { get; set; }
        public string SavePath { get; set; }
        public string FileName
        {
            get
            {
                return System.IO.Path.GetFileName(SavePath);
            }
        }
        public enum DownloadStatus { FINISHED, STOPED, PAUSED, LOADING, ERROR };

        private DownloadStatus status;

        public DownloadStatus Status
        {
            get => status; set
            {
                status = value;
                OnChange();
            }
        }
        private double progress;
        public double Progress
        {
            get => progress;
            set
            {
                progress = value;
                OnChange();
            }
        }
        public WebClient Client { get; set; }
        public FileInfo()
        {
            Status = DownloadStatus.LOADING;
        }
        public FileInfo(string url, string path)
        {
            URL = url;
            SavePath = path;
            Status = DownloadStatus.LOADING;
        }
        public void Download()
        {
            Client = new WebClient();

            Client.DownloadProgressChanged += UpdateProgress;
            Client.DownloadFileCompleted += SetFinishedAction;

            Client.DownloadFileAsync(new Uri(URL), SavePath);

        }
        private void UpdateProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            Progress = e.ProgressPercentage;
            OnChange("Progress");
        }
        private void SetFinishedAction(object sender, AsyncCompletedEventArgs e)
        {
            Status = DownloadStatus.FINISHED;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnChange([CallerMemberName]string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public override bool Equals(object obj)
        {
            FileInfo other = obj as FileInfo;
            return this.SavePath == other.SavePath || this.URL == other.URL;
        }

        public override int GetHashCode()
        {
            return URL.GetHashCode() + SavePath.GetHashCode();
        }
    }
    public partial class MainWindow : Window
    {
        static string path = "";
        public static string DownloadFolder = "Downloads\\";
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(tbURI.Text))
            {
                if (String.IsNullOrWhiteSpace(path))
                {
                    if (!Directory.Exists(DownloadFolder))
                        Directory.CreateDirectory(DownloadFolder);
                    path = DownloadFolder + System.IO.Path.GetFileName(tbURI.Text);
                }
                FileInfo tmp = new FileInfo(tbURI.Text, path);
                if (!lbFiles.Items.Contains(tmp))
                {
                    tmp.Download();
                    lbFiles.Items.Add(tmp);
                }
                else
                    MessageBox.Show("File is already in download list.");
            }
            tbURI.Text = path = "";
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                FileName = System.IO.Path.GetFileName(tbURI.Text)
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                path = saveFileDialog.FileName;
            }
        }
    }
}
