using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace ImageVerification
{
    class ImageConverter
    {
        //Convert data set from single querry into bitmap
        public BitmapImage Convert(DataSet incomingData)
        {
            BitmapImage image = new BitmapImage();

            try
            { 
            //Konwersja zdjecia z bazy do tablicy byte
            byte[] data = (byte[])incomingData.Tables[0].Rows[0][0];

            // Stworzenie strumienia i przepisanie tablicy do niego
            MemoryStream strm = new MemoryStream();
            strm.Write(data, 0, data.Length);
            strm.Position = 0;

            //Stworzenie obrazu z strumienia danych
            System.Drawing.Image img = System.Drawing.Image.FromStream(strm);

            image.BeginInit();
            MemoryStream ms = new MemoryStream();
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();


            return image;
             }
            catch(Exception ex)
            {
                return null;
            }
        }

       
    }
}
