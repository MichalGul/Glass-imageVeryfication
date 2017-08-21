using ImageVerification.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace ImageVerification
{
    public class FeaturePointsRepository : INotifyPropertyChanged
    {
        public static ObservableCollection<FeaturePoint> frontFeaturePointsCollection;
        public static ObservableCollection<FeaturePoint> profileFeaturePointsCollection;

        private int customerId;
        private int _id;             
        private double _scaleFactor;
        private double _maxScaleFactor;
        private double _earNoseDistance;
        private double _eyeNoseDistance;

        public int Id
        {
            get
            {
                return _id;
            }

            set
            {
                _id = value;
                RaisePropertyChanged();
            }
        }

        public int CustomerId
        {
            get
            {
                return customerId;
            }

            set
            {
                customerId = value;
                RaisePropertyChanged();
            }
        }

        public double ScaleFactor
        {
            get
            {
                return _scaleFactor;
            }

            set
            {
                _scaleFactor = value;
                RaisePropertyChanged();
            }
        }

        public double MaxScaleFactor
        {
            get
            {
                return _maxScaleFactor;

            }

            set
            {
                _maxScaleFactor = value;
                RaisePropertyChanged();
            }
        }

        public double EarNoseDistance
        {
            get
            {
                return _earNoseDistance;
            }

            set
            {
                _earNoseDistance = value;
                RaisePropertyChanged();
            }
        }

        public double EyeNoseDistance
        {
            get
            {
                return _eyeNoseDistance;
            }

            set
            {
                _eyeNoseDistance = value;
                RaisePropertyChanged();
            }
        }




        // Obsługa zmiany wartości
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(
            [CallerMemberName] string caller =" ")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(caller));
            }

        }

        public ObservableCollection<FeaturePoint> GetFrontPoints()
        {
            LoadFrontPointsFormDatabaseForSelectedCustomer();
            return frontFeaturePointsCollection;

        }
        public ObservableCollection<FeaturePoint> GetProfilePoints()
        {
            LoadProfilePointsFormDatabaseForSelectedCustomer();
            return profileFeaturePointsCollection;

        }

        public void UpdateFrontPointsData(ObservableCollection<FeaturePoint> selectedCustomerPoints)
        {


            // Po przesunieciu i maniu współczynnika skalowania będzie trzeba przeliczyć odległości ale do tego już sa wszystkie dane w bazie wiec wystarczy je brać         
            try
            {
                //Akceptowanie punktów na obrazie - update bazy -> dziala
                MySqlConnection connection = new MySqlConnection(Utilities.connectionString);         //--------------------------->
                string updateQuerry = "Update Punkty SET LSkron_X=@val1, LSkron_Y=@val2, PSkron_X=@val3, PSkron_Y=@val4, Nos_X=@val5, Nos_Y=@val6, LOko_X=@val7, LOko_Y=@val8, POko_X=@val9, POko_Y=@val10, LPol_X=@val11, LPol_Y=@val12, PPol_X=@val13, PPol_Y=@val14 where id_klienta = " + Utilities.currentID;
                connection.Open();
                MySqlCommand prpCommand = new MySqlCommand(updateQuerry, connection);

                prpCommand.Prepare();
                prpCommand.Parameters.AddWithValue("@val1", GetPointByName(selectedCustomerPoints, "LSkron").X);
                prpCommand.Parameters.AddWithValue("@val2", GetPointByName(selectedCustomerPoints, "LSkron").Y);

                prpCommand.Parameters.AddWithValue("@val3", GetPointByName(selectedCustomerPoints, "PSkron").X);
                prpCommand.Parameters.AddWithValue("@val4", GetPointByName(selectedCustomerPoints, "PSkron").Y);

                prpCommand.Parameters.AddWithValue("@val5", GetPointByName(selectedCustomerPoints, "Nos").X);
                prpCommand.Parameters.AddWithValue("@val6", GetPointByName(selectedCustomerPoints, "Nos").Y);

                prpCommand.Parameters.AddWithValue("@val7", GetPointByName(selectedCustomerPoints, "LOko").X);
                prpCommand.Parameters.AddWithValue("@val8", GetPointByName(selectedCustomerPoints, "LOko").Y);

                prpCommand.Parameters.AddWithValue("@val9", GetPointByName(selectedCustomerPoints, "POko").X);
                prpCommand.Parameters.AddWithValue("@val10", GetPointByName(selectedCustomerPoints, "POko").Y);

                prpCommand.Parameters.AddWithValue("@val11", GetPointByName(selectedCustomerPoints, "LPol").X);
                prpCommand.Parameters.AddWithValue("@val12", GetPointByName(selectedCustomerPoints, "LPol").Y);

                prpCommand.Parameters.AddWithValue("@val13", GetPointByName(selectedCustomerPoints, "PPol").X);
                prpCommand.Parameters.AddWithValue("@val14", GetPointByName(selectedCustomerPoints, "PPol").Y);

                int result = prpCommand.ExecuteNonQuery();
                connection.Close();


            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd aktualizowania bazy punktów. " + ex.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
    

        }

        public void UpdateProfilePoints(ObservableCollection<FeaturePoint> selectedCustomerPoints)
        {

            try
            {


                //Akceptowanie punktów na obrazie - update bazy -> dziala
                MySqlConnection connection = new MySqlConnection(Utilities.connectionString);         //ZAJRZYJ TAM --------------------------->
                string updateQuerry = "Update Punkty_Profil SET Ucho_X=@val1, Ucho_Y=@val2, Nos_X=@val3, Nos_Y=@val4, Oko_X=@val5, Oko_Y=@val6 where id_klienta = " + Utilities.currentID;
                connection.Open();
                MySqlCommand prpCommand = new MySqlCommand(updateQuerry, connection);

                prpCommand.Prepare();
                prpCommand.Parameters.AddWithValue("@val1", GetPointByName(selectedCustomerPoints, "Ucho").X);
                prpCommand.Parameters.AddWithValue("@val2", GetPointByName(selectedCustomerPoints, "Ucho").Y);

                prpCommand.Parameters.AddWithValue("@val3", GetPointByName(selectedCustomerPoints, "Nos").X);
                prpCommand.Parameters.AddWithValue("@val4", GetPointByName(selectedCustomerPoints, "Nos").Y);

                prpCommand.Parameters.AddWithValue("@val5", GetPointByName(selectedCustomerPoints, "Oko").X);
                prpCommand.Parameters.AddWithValue("@val6", GetPointByName(selectedCustomerPoints, "Oko").Y);

                int result = prpCommand.ExecuteNonQuery();

                connection.Close();

 
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd aktualizowania bazy punktów. " + ex.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

      
        public void UpdateProfileDistances(double earNose, double eyeNose)
        {
            try
            {
                //Akceptowanie punktów na obrazie - update bazy -> dziala
                MySqlConnection connection = new MySqlConnection(Utilities.connectionString);         
            string updateQuerry = "Update Punkty_Profil SET Ucho_Nos=@val1, Oko_Nos=@val2 where id_klienta = " + Utilities.currentID;
            connection.Open();
            MySqlCommand prpCommand = new MySqlCommand(updateQuerry, connection);

            prpCommand.Prepare();
            prpCommand.Parameters.AddWithValue("@val1", earNose);
            prpCommand.Parameters.AddWithValue("@val2", eyeNose);

            int result = prpCommand.ExecuteNonQuery();

            connection.Close();


        }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd aktualizowania bazy punktów. " + ex.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }



}


        private Point GetPointByName(ObservableCollection<FeaturePoint> collection, string pointName)
        {
            Point selectedPoint = new Point(0, 0);

            foreach (var element in collection)
            {
                if (element.PointName == pointName)
                {
                    selectedPoint = element.PointCoordinates;
                    break;
                }

            }
            return selectedPoint;
        }



        public void LoadFrontPointsFormDatabaseForSelectedCustomer()
        {
            var faceFeaturePoints = new ObservableCollection<FeaturePoint>();

            //Pobieranie danych z bazy
            string querry = "Select * from Punkty where id_klienta = " + Utilities.currentID;

            MySqlConnection connection = new MySqlConnection(Utilities.connectionString);
            //Odczytanie wynikow zapytania
            MySqlDataReader results = null;
            try
            {
                connection.Open();
                //Stworzenie polecenia do bazy danych
                MySqlCommand command = new MySqlCommand(querry, connection);
                results = command.ExecuteReader();

                //Odczytywanie danych
                Point read = new Point(0, 0);
                while (results.Read())
                {
                    read.X = (int)results["Nos_X"];
                    read.Y = (int)results["Nos_Y"];
                    faceFeaturePoints.Add(new FeaturePoint() { PointName = "Nos", PointCoordinates = read });

                    read.X = (int)results["LSkron_X"];
                    read.Y = (int)results["LSkron_Y"];
                    faceFeaturePoints.Add(new FeaturePoint() { PointName = "LSkron", PointCoordinates = read });

                    read.X = (int)results["PSkron_X"];
                    read.Y = (int)results["PSkron_Y"];
                    faceFeaturePoints.Add(new FeaturePoint() { PointName = "PSkron", PointCoordinates = read });

                    read.X = (int)results["LOko_X"];
                    read.Y = (int)results["LOko_Y"];
                    faceFeaturePoints.Add(new FeaturePoint() { PointName = "LOko", PointCoordinates = read });

                    read.X = (int)results["POko_X"];
                    read.Y = (int)results["POko_Y"];
                    faceFeaturePoints.Add(new FeaturePoint() { PointName = "POko", PointCoordinates = read });


                    read.X = (int)results["LPol_X"];
                    read.Y = (int)results["LPol_Y"];
                    faceFeaturePoints.Add(new FeaturePoint() { PointName = "LPol", PointCoordinates = read });

                    read.X = (int)results["PPol_X"];
                    read.Y = (int)results["PPol_Y"];
                    faceFeaturePoints.Add(new FeaturePoint() { PointName = "PPol", PointCoordinates = read });

                    Id = (int)results["Id"];
                    ScaleFactor = (double)results["WspSkalowania"];
                    MaxScaleFactor = (double)results["WspSkalowaniaMax"];
                    CustomerId = (int)results["Id_klienta"];
                    EarNoseDistance = 0;
                    EyeNoseDistance = 0;


                }
                //Zamkniecie polaczenia
                connection.Close();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Błąd odczytu z bazy danych. Proszę sprawdzić ustawienia połączenia. " + ex.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                throw ex;
            }
            finally
            {


                connection.Close();
                frontFeaturePointsCollection = faceFeaturePoints;
            }


        }




        public static double GetFullScaleFactorFromPointsDatabase()
        {


            double scaleFactor = 0;
            //Pobieranie danych z bazy
            string querry = "Select WspSkalowaniaMax from Punkty where id_klienta = " + Utilities.currentID;

            MySqlConnection connection = new MySqlConnection(Utilities.connectionString);
            //Odczytanie wynikow zapytania
            MySqlDataReader results = null;

            connection.Open();
            //Stworzenie polecenia do bazy danych
            MySqlCommand command = new MySqlCommand(querry, connection);
            results = command.ExecuteReader();

            while (results.Read())
            {
                scaleFactor = (double)results["WspSkalowaniaMax"];
            }

            connection.Close();

            return scaleFactor;

        }

        public static double GetProfileScaleFactor()
        {
            double scaleFactor = 0;
            //Pobieranie danych z bazy
            string querry = "Select WspSkalowania from Punkty_Profil where id_klienta = " + Utilities.currentID;

            MySqlConnection connection = new MySqlConnection(Utilities.connectionString);
            //Odczytanie wynikow zapytania
            MySqlDataReader results = null;

            connection.Open();
            //Stworzenie polecenia do bazy danych
            MySqlCommand command = new MySqlCommand(querry, connection);
            results = command.ExecuteReader();

            while (results.Read())
            {
                scaleFactor = (double)results["WspSkalowania"];
            }

            return scaleFactor;

        }



        public void LoadProfilePointsFormDatabaseForSelectedCustomer()
        {
            var profileFeaturePoints = new ObservableCollection<FeaturePoint>();

            //Pobieranie danych z bazy
            string querry = "Select * from Punkty_Profil where id_klienta = " + Utilities.currentID;

            MySqlConnection connection = new MySqlConnection(Utilities.connectionString);
            //Odczytanie wynikow zapytania
            MySqlDataReader results = null;
            try
            {
                connection.Open();
                //Stworzenie polecenia do bazy danych
                MySqlCommand command = new MySqlCommand(querry, connection);
                results = command.ExecuteReader();

                //Odczytywanie danych
                Point read = new Point(0, 0);
                while (results.Read())
                {
                    read.X = (int)results["Ucho_X"];
                    read.Y = (int)results["Ucho_Y"];
                    profileFeaturePoints.Add(new FeaturePoint() { PointName = "Ucho", PointCoordinates = read });

                    read.X = (int)results["Nos_X"];
                    read.Y = (int)results["Nos_Y"];
                    profileFeaturePoints.Add(new FeaturePoint() { PointName = "Nos", PointCoordinates = read });

                    read.X = (int)results["Oko_X"];
                    read.Y = (int)results["Oko_Y"];
                    profileFeaturePoints.Add(new FeaturePoint() { PointName = "Oko", PointCoordinates = read });

                    Id = (int)results["Id"];
                    EarNoseDistance = (double)results["Ucho_Nos"];
                    EyeNoseDistance = (double)results["Oko_Nos"];
                    ScaleFactor = (double)results["WspSkalowania"];
                    MaxScaleFactor = 0;
                    CustomerId = (int)results["Id_klienta"];



                }

            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Błąd odczytu z bazy danych. Proszę sprawdzić ustawienia połączenia. " + ex.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                throw ex;
            }

            finally
            {
                connection.Close();
                profileFeaturePointsCollection = profileFeaturePoints;
            }
        }



    }
}
