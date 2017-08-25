using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageVerification
{
    /// <summary>
    /// Utility class holding important program wide parameters
    /// </summary>
    public static class Utilities
    {
        public static string currentID = "";
        public static string serverName = "gulczy.ayz.pl";
        public static string databaseName = "gulczy_listaKlientow";
        public static string user = "gulczy_root";
        public static string password = "gulki1";
        public static string connectionString = "SERVER=" + serverName + "; " + " DATABASE=" + databaseName + ";" + "UID=" + user + "; " + "PASSWORD=" + password + "; ";
        ///Size of bounding hit box 
        public static int    grabPixelOffset = 10;
        //Size of the feature point on image
        public static int markPointSize = 8;
        public static bool resizeImage = false;
        public static double resizeFactor = 0.0;
        public static bool useHoughTransoformPupilDetection = false;

        // Incorrect index of point to move
        public enum Index
        {
            NotValidIndex = 99
        }
  


    }
}
