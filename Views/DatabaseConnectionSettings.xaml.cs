using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace ImageVerification
{
    /// <summary>
    /// Interaction logic for DatabaseConnectionSettings.xaml
    /// </summary>
    public partial class DatabaseConnectionSettings : Window
    {
        string connectionString = "";

        public DatabaseConnectionSettings()
        {
            InitializeComponent();
        }

        private void Okbtn_Click(object sender, RoutedEventArgs e)
        {
            //connectionString = "SERVER=localhost; DATABASE=lista_klientow;UID=root;PASSWORD=gulki1;"

            connectionString = "SERVER=" + Servertbox.Text + "; " + "DATABASE=" + Basetbox.Text + "; " + "UID=" + Usertbox.Text + ";" +"PASSWORD="+ Passwordtbox.Password + ";";
            Utilities.connectionString = connectionString;
            MessageBox.Show("Zapisano ustawienia","Sukces",MessageBoxButton.OK,MessageBoxImage.Information);
            this.Close();
        }

        private void Cancelbtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Servertbox.Text = Utilities.serverName;
            Basetbox.Text = Utilities.databaseName;
            Usertbox.Text = Utilities.user;
            Passwordtbox.Password = Utilities.password;

        }
    }
}
