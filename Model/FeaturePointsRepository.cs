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
        private string _pointName;
        public string PointName
        {
            get { return _pointName; }
            set
            {
                 _pointName = value;
                RaisePropertyChanged();
            }
        }
        private Point _pointCoordinates;
        public Point PointCoordinates
        {
            get
            {
                return _pointCoordinates;
            }

            set
            {
                _pointCoordinates = value;
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

        public static ObservableCollection<FeaturePointsRepository> GetPointsFormDatabase()
        {
            var faceFeaturePoints = new ObservableCollection<FeaturePointsRepository>();

            //Pobieranie danych z bazy
            string querry = "Select * from Punkty where id_klienta = " + Utilities.currentID;

            MySqlConnection connection = new MySqlConnection(Utilities.connectionString);
            //Odczytanie wynikow zapytania
            MySqlDataReader results = null;

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
                faceFeaturePoints.Add(new FeaturePointsRepository() { PointName = "Nos", PointCoordinates = read });
                
                read.X = (int)results["LSkron_X"];
                read.Y = (int)results["LSkron_Y"];               
                faceFeaturePoints.Add(new FeaturePointsRepository() { PointName = "LSkron", PointCoordinates = read });

                read.X = (int)results["PSkron_X"];
                read.Y = (int)results["PSkron_Y"];                
                faceFeaturePoints.Add(new FeaturePointsRepository() { PointName = "PSkron", PointCoordinates = read });

                read.X = (int)results["LOko_X"];
                read.Y = (int)results["LOko_Y"];               
                faceFeaturePoints.Add(new FeaturePointsRepository() { PointName = "LOko", PointCoordinates = read });

                read.X = (int)results["POko_X"];
                read.Y = (int)results["POko_Y"];
                faceFeaturePoints.Add(new FeaturePointsRepository() { PointName = "POko", PointCoordinates = read });


                read.X = (int)results["LPol_X"];
                read.Y = (int)results["LPol_Y"];               
                faceFeaturePoints.Add(new FeaturePointsRepository() { PointName = "LPol", PointCoordinates = read });

                read.X = (int)results["PPol_X"];
                read.Y = (int)results["PPol_Y"];
                faceFeaturePoints.Add(new FeaturePointsRepository() { PointName = "PPol", PointCoordinates = read });



            }         
            //Zamkniecie polaczenia
            connection.Close();

            return faceFeaturePoints;

        }

        public static ObservableCollection<FeaturePointsRepository> GetProfilePointsFormDatabase()
        {
            var profileFeaturePoints = new ObservableCollection<FeaturePointsRepository>();

            //Pobieranie danych z bazy
            string querry = "Select * from Punkty_Profil where id_klienta = " + Utilities.currentID;

            MySqlConnection connection = new MySqlConnection(Utilities.connectionString);
            //Odczytanie wynikow zapytania
            MySqlDataReader results = null;

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
                profileFeaturePoints.Add(new FeaturePointsRepository() { PointName = "Ucho", PointCoordinates = read });

                read.X = (int)results["Nos_X"];
                read.Y = (int)results["Nos_Y"];
                profileFeaturePoints.Add(new FeaturePointsRepository() { PointName = "Nos", PointCoordinates = read });

                read.X = (int)results["Oko_X"];
                read.Y = (int)results["Oko_Y"];
                profileFeaturePoints.Add(new FeaturePointsRepository() { PointName = "Oko", PointCoordinates = read });

             
     
            }
            //Zamkniecie polaczenia
            connection.Close();

            return profileFeaturePoints;
        }



    }
}
