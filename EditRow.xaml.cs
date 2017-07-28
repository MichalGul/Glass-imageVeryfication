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
    /// Interaction logic for EditRow.xaml
    /// </summary>
    public partial class EditRow : Window
    {
        // tabela danych klienta
      


        public EditRow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            // Ustanowienie połaczenia z baza
            MySqlConnection connection = new MySqlConnection(Utilities.connectionString);
            MySqlCommand cmdPoints = new MySqlCommand("SELECT * from Klienci where id = " + Utilities.currentID, connection);
            if (cmdPoints == null)
            {
                MessageBox.Show("Połączenie nieudane. Sprawdź ustawienia połączenia");
                return;
            }

            //Wykonanie zapytania
            try
            {
                connection.Open();
                
                //Wczytanie wyniku do DataTable ktora sie dostosuje do typu danych
                MySqlDataReader dataFromDatabase = cmdPoints.ExecuteReader();
                DataTable dtKlientsData = new DataTable();
                dtKlientsData.Load(dataFromDatabase);               
                connection.Close();

                //Wczytanie danych z bazy do dataGrid
                dgridKlients.DataContext = dtKlientsData;
                //dgridProfilePoints.DataContext = dtPointsProfile;

                foreach (DataGridColumn column in dgridKlients.Columns)
                {                   
                    //if you want to size ur column as per the cell content
                    //column.Width = new DataGridLength(1.0, DataGridLengthUnitType.SizeToCells);
                    //if you want to size ur column as per the column header
                    //  column.Width = new DataGridLength(1.0, DataGridLengthUnitType.SizeToHeader);
                    //if you want to size ur column as per both header and cell content
                    column.Width = new DataGridLength(2.0, DataGridLengthUnitType.Auto);
                    if((string)column.Header == "Nazwisko")
                    {
                        column.Width = 80;
                    }
                    if((string)column.Header == "Email")
                    {
                        column.Width = 60;
                    }
                   
                }


            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Połączenie nieudane. Sprawdź ustawienia połączenia");
                return;
            }

        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {          
            // Przesładnie zmian do bazy danych
            // Ustanowienie połaczenia z baza
        
                MySqlConnection connection = new MySqlConnection(Utilities.connectionString);
                string updateQuerry = "Update Klienci SET id=@param1, imie=@param2, nazwisko=@param3, email=@param4, Rozstaw_Zrenic=@param5, Szerokosc_Twarzy=@param6, Szerokosc_Skroni=@param7, PraweOko_Nos=@param8, LeweOko_Nos=@param9, Ucho_Nos=@param10, zdjecie=@param11, zdjecie_profil = @param12, zatwierdzone=@param13 where id = " + Utilities.currentID;
                connection.Open();
                MySqlCommand prpCommand = new MySqlCommand(updateQuerry, connection);
                if (prpCommand == null)
                {
                    MessageBox.Show("Połączenie nieudane. Sprawdź ustawienia połączenia");
                    return;
                }

                
              
                //Datagrid ma tylko jeden rząd najprostszy sposob
                DataRowView data = (DataRowView)dgridKlients.Items[0];


                prpCommand.Prepare();
                prpCommand.Parameters.AddWithValue("@param1", data["id"]);
                prpCommand.Parameters.AddWithValue("@param2", data["imie"]);
                prpCommand.Parameters.AddWithValue("@param3", data["nazwisko"]);
                prpCommand.Parameters.AddWithValue("@param4", data["email"]);
                prpCommand.Parameters.AddWithValue("@param5", data["Rozstaw_Zrenic"]);
                prpCommand.Parameters.AddWithValue("@param6", data["Szerokosc_Twarzy"]);
                prpCommand.Parameters.AddWithValue("@param7", data["Szerokosc_Skroni"]);
                prpCommand.Parameters.AddWithValue("@param8", data["PraweOko_Nos"]);
                prpCommand.Parameters.AddWithValue("@param9", data["LeweOko_Nos"]);
                prpCommand.Parameters.AddWithValue("@param10", data["Ucho_Nos"]);
                prpCommand.Parameters.AddWithValue("@param11", data["zdjecie"]);
                prpCommand.Parameters.AddWithValue("@param12", data["zdjecie_profil"]);
                prpCommand.Parameters.AddWithValue("@param13", data["zatwierdzone"]);



            int result = prpCommand.ExecuteNonQuery();
           

            connection.Close();

            MessageBox.Show("Zatwierdzono wprowadzone zmiany", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
