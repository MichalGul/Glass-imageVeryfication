using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;

namespace ImageVerification
{
    /// <summary>
    /// Interaction logic for PleaseWait.xaml window
    /// </summary>
    public partial class PleaseWait : Window
    {
      
        //Importing extern C++ library
        [DllImport("E:\\_PROJEKTY\\C++DLLtoC#\\FaceLandmarkDLL\\x64\\Debug\\FaceLandmarkDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool CalculateFrontFeaturePoints(int ID, bool ResizaImage, double resizeFactor, bool useHoughTransformDetection);

        [DllImport("E:\\_PROJEKTY\\C++DLLtoC#\\FaceLandmarkDLL\\x64\\Debug\\FaceLandmarkDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool CalculateProfileFeaturePoints(int ID);

        [DllImport("E:\\_PROJEKTY\\C++DLLtoC#\\FaceLandmarkDLL\\x64\\Debug\\FaceLandmarkDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void CloseAllImageWindows();

        public PleaseWait()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Creating background worker on loading window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
          
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //At the end of calculation      
            CloseWindow();
            
        }

        /// <summary>
        /// Perform calculations of front feature on background
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (Utilities.currentID == "")
            {
                return;
            }
            else if (Utilities.currentID.Length > 0)
            {
                //PleaseWait wait = new PleaseWait();
                // wait.ShowDialog();    
                if (Utilities.resizeImage == false)
                {
                    if (CalculateFrontFeaturePoints(Int32.Parse(Utilities.currentID), Utilities.resizeImage, 1.0,Utilities.useHoughTransoformPupilDetection) == true)
                    {
                        MessageBox.Show("Przetwarzanie zakończone.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                        CloseAllImageWindows();
                    }
                    else
                    {
                        MessageBox.Show("Analiza zdjęcia zakończyła się niepowodzeniem. Spróbuj jeszcze raz z innym zdjeciem.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                        CloseAllImageWindows();
                       
                    }
                }
                else if (Utilities.resizeImage == true)
                 {
                    if (CalculateFrontFeaturePoints(Int32.Parse(Utilities.currentID), Utilities.resizeImage, Utilities.resizeFactor, Utilities.useHoughTransoformPupilDetection) == true)
                    {
                        MessageBox.Show("Przetwarzanie zakończone.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                        CloseAllImageWindows();
                    }
                    else
                    {
                        MessageBox.Show("Analiza zdjęcia zakończyła się niepowodzeniem. Spróbuj jeszcze raz z innym zdjeciem.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                        
                    }

                }
        
            }
        }

        private void CloseWindow()
        {
            this.Close();
        }
    }
}
