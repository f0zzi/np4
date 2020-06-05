using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
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

namespace mail
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(tbEmail.Text) ||
               String.IsNullOrWhiteSpace(tbPass.Password) ||
               String.IsNullOrWhiteSpace(tbTo.Text))
                MessageBox.Show("Email, Password, To fields are requaired");
            else
            {
                MailMessage message = new MailMessage(tbEmail.Text, tbTo.Text, tbSubject.Text, tbText.Text);
                if (cbImportant.IsChecked == true)
                    message.Priority = MailPriority.High;
                SmtpClient client = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    EnableSsl = true,
                    Credentials = new NetworkCredential(tbEmail.Text, tbPass.Password)
                };
                foreach (string item in lbAttach.Items)
                {
                    message.Attachments.Add(new Attachment(item));
                }
                client.SendAsync(message, null);
                tbEmail.Text = tbPass.Password = tbTo.Text = tbSubject.Text = tbText.Text = "";
                lbAttach.Items.Clear();
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                if (lbAttach.Items.Contains(openFileDialog.FileName))
                    MessageBox.Show("File already added.");
                else
                    lbAttach.Items.Add(openFileDialog.FileName);
            }
        }

        private void Click_Delete(object sender, RoutedEventArgs e)
        {
            if (lbAttach.SelectedItem != null)
                lbAttach.Items.Remove(lbAttach.SelectedItem);
        }
    }
}
