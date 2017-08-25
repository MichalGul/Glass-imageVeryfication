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
    /// <summary>
    /// Manages feature points data display 
    /// </summary>
    class PointsTableRepository
    {
        public static DataTable frontPointsDataTable;
        public static DataTable profilePointsDataTable;

        /// <summary>
        /// Loads front feature table points from datatable
        /// </summary>
        /// <returns>front points data table</returns>
        public DataTable GetFrontPointsDataTable()
        {
            LoadFrontPointsDataFromDatabase();
            return frontPointsDataTable;
        }
        /// <summary>
        /// Load profile feature table points from database
        /// </summary>
        /// <returns>front points data table</returns>
        public DataTable GetProfilePointsDataTable()
        {
            LoadProfilePointsFromDatabase();
            return profilePointsDataTable;
        }
        /// <summary>
        /// Connect to MySQL database and load data from front points table
        /// </summary>
        public void LoadFrontPointsDataFromDatabase()
        {

            MySqlConnection connection = new MySqlConnection(Utilities.connectionString);
            MySqlCommand cmdPoints = new MySqlCommand("SELECT * from Punkty", connection);

            if (cmdPoints == null)
            {
                MessageBox.Show("Połączenie nieudane. Sprawdź ustawienia połączenia","Błąd",MessageBoxButton.OK,MessageBoxImage.Error);
                return;
            }

         
            try
            {
                connection.Open();
                DataTable dtPoints = new DataTable();
                //Wczytanie wyniku do DataTable ktora sie dostosuje do typu danych
                MySqlDataReader dataPoints = cmdPoints.ExecuteReader();
                dtPoints.Load(dataPoints);               
                connection.Close();
                frontPointsDataTable = dtPoints;

            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Połączenie nieudane. Sprawdź ustawienia połączenia " + ex.Message.ToString(), "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        /// <summary>
        /// Connect to MySQL data table and load data from profile points table
        /// </summary>
        public void LoadProfilePointsFromDatabase()
        {      
            MySqlConnection connectionProfile = new MySqlConnection(Utilities.connectionString);
            MySqlCommand cmdProfile = new MySqlCommand("select * from Punkty_Profil", connectionProfile);
            if (cmdProfile == null)
            {
                MessageBox.Show("Połączenie nieudane. Sprawdź ustawienia połączenia");
                return;
            }
            try
            {             
                connectionProfile.Open();
                DataTable dtPointsProfile = new DataTable();
                MySqlDataReader dataProfile = cmdProfile.ExecuteReader();
                dtPointsProfile.Load(dataProfile);
                connectionProfile.Close();


                profilePointsDataTable = dtPointsProfile;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Połączenie nieudane. Sprawdź ustawienia połączenia "+ ex.Message.ToString(), "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


        }

    }
}
