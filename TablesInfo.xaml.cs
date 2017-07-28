using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for TablesInfo.xaml
    /// </summary>
    public partial class TablesInfo : Window
    {
        public TablesInfo()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Ustanowienie połaczenia z baza
            
                MySqlConnection connection = new MySqlConnection(Utilities.connectionString);
                MySqlCommand cmdPoints = new MySqlCommand("SELECT * from Punkty", connection);

                if (cmdPoints == null)
                {
                    MessageBox.Show("Połączenie nieudane. Sprawdź ustawienia połączenia");
                    return;
                }

                //Wykonanie zapytania
                try
                {
                    connection.Open();
                    DataTable dtPoints = new DataTable();

                    //Wczytanie wyniku do DataTable ktora sie dostosuje do typu danych
                    MySqlDataReader dataPoints = cmdPoints.ExecuteReader();
                              
                    dtPoints.Load(dataPoints);
                    //dtPointsProfile.Load(dataProfile);
                    connection.Close();

                    //Wczytanie danych z bazy do dataGrid
                    dgridPoints.DataContext = dtPoints;
                    //dgridProfilePoints.DataContext = dtPointsProfile;

                 
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Połączenie nieudane. Sprawdź ustawienia połączenia");
                    return;
                }
            
            //Tabela punktów z profilu
            MySqlConnection connectionProfile = new MySqlConnection(Utilities.connectionString);
            MySqlCommand cmdProfile = new MySqlCommand("select * from Punkty_profil", connectionProfile);
                 if (cmdProfile == null)
                   {
                    MessageBox.Show("Połączenie nieudane. Sprawdź ustawienia połączenia");
                    return;
                 }
            try
            {
                //Pobieranie danych z bazy
                connectionProfile.Open();
                DataTable dtPointsProfile = new DataTable();
                MySqlDataReader dataProfile = cmdProfile.ExecuteReader();  
                dtPointsProfile.Load(dataProfile);               
                connectionProfile.Close();
                dgridProfilePoints.DataContext = dtPointsProfile;
            }
            catch(MySqlException ex)
            {
                MessageBox.Show("Połączenie nieudane. Sprawdź ustawienia połączenia");
                return;
            }


            foreach (DataGridColumn column in dgridPoints.Columns)
                {                        
               // if you want to size ur column as per both header and cell content
                column.Width = 60;               
                }            
            }
           

        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           
            this.Close();
        }

        private void dgridProfilePoints_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if(e.PropertyName=="WspSkalowania")
            {
                e.Column.Width = 115;
            }
            if (e.PropertyName == "Ucho_Nos")
            {
                e.Column.Width = new DataGridLength(1.0, DataGridLengthUnitType.Auto);
            }
           
        }

        private void dgridPoints_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if(e.PropertyName == "WspSkalowania")
            {
                e.Column.Width = new DataGridLength(1.0, DataGridLengthUnitType.Auto);
                
            }
        }
    }
}
