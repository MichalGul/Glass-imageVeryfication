using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageVerification
{
    public static class Utilities
    {
        public static string currentID = "";
        public static string serverName = "localhost";
        public static string databaseName = "lista_klientow";
        public static string user = "root";
        public static string password = "gulki1";
        public static string connectionString = "SERVER=" + serverName + "; " + " DATABASE=" + databaseName + ";" + "UID=" + user + "; " + "PASSWORD=" + password + "; ";
        //Rozmiar bounding box w ktory trzeba trafic aby przeniesc punkt
        public static int    grabPixelOffset = 10;
        //Rozmiar punktu charakterystycznego na obrazie
        public static int markPointSize = 8;
        public static bool resizeImage = false;
        public static double resizeFactor = 0.0;
        // Błędny indeks punktu do przeniesienia
        public enum Index
        {
            NotValidIndex = 99
        }
  


    }
}
