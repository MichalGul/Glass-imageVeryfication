using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ImageVerification.Model
{
    class PointsTableRepository
    {
        public static DataTable frontPointsDataTable;
        public static DataTable profilePointsDataTable;


        public DataTable GetFrontPointsDataTable()
        {
            LoadFrontPointsDataFromDatabase();
            return frontPointsDataTable;
        }
        public DataTable GetProfilePointsDataTable()
        {
            LoadProfilePointsFromDatabase();
            return profilePointsDataTable;
        }

        public void LoadFrontPointsDataFromDatabase()
        {

            MySqlConnection connection = new MySqlConnection(Utilities.connectionString);
            MySqlCommand cmdPoints = new MySqlCommand("SELECT * from Punkty", connection);

            if (cmdPoints == null)
            {
                MessageBox.Show("Połączenie nieudane. Sprawdź ustawienia połączenia","Błąd",MessageBoxButton.OK,MessageBoxImage.Error);
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
                frontPointsDataTable = dtPoints;

            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Połączenie nieudane. Sprawdź ustawienia połączenia");
                return;
            }


        }

        public void LoadProfilePointsFromDatabase()
        {
           
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


                profilePointsDataTable = dtPointsProfile;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Połączenie nieudane. Sprawdź ustawienia połączenia");
                return;
            }


        }




    }
}
