using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace ImageVerification
{
    /// <summary>
    /// Manage conversion from various data to bitmapImage and forth
    /// </summary>
    class ImageConverter
    {
        /// <summary>
        /// Convert image data set from database into  bitmapImage format
        /// </summary>
        /// <param name="incomingData"> Data set from MySQL querry</param>
        /// <returns></returns>
        public BitmapImage ConvertDataSetToBitmap(DataSet incomingData)
        {
            BitmapImage image = new BitmapImage();
                  
            try
            { 
            //Converting Data Set from database to byte arrat
            byte[] data = (byte[])incomingData.Tables[0].Rows[0][0];

            // Straming byta table to memory stream
            MemoryStream strm = new MemoryStream();
            strm.Write(data, 0, data.Length);
            strm.Position = 0;

            //Creating image stream
            System.Drawing.Image img = System.Drawing.Image.FromStream(strm);

            //inicializing bitmapImage
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
                MessageBox.Show("Błąd konversji obrazu. " + ex.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }
        /// <summary>
        /// Convert steam of byte into bitmapImage
        /// </summary>
        /// <param name="incomingData"> incoming data fomr database</param>
        /// <returns></returns>
        public BitmapImage ConvertByteArrayToBitmap(byte[] incomingData)
        {
            BitmapImage image = new BitmapImage();

            try
            {             
                if(incomingData == null) 
                {
                    return null;
                }
               
                MemoryStream strm = new MemoryStream();
                strm.Write(incomingData, 0, incomingData.Length);
                strm.Position = 0;

              
                System.Drawing.Image img = System.Drawing.Image.FromStream(strm);

                image.BeginInit();
                MemoryStream ms = new MemoryStream();
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                ms.Seek(0, SeekOrigin.Begin);
                image.StreamSource = ms;
                image.EndInit();           
                return image;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd konversji obrazu. " + ex.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }



    }
}
