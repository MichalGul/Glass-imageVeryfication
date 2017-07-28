﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace ImageVerification
{
    /// <summary>
    /// Interaction logic for ImageDisplay.xaml
    /// </summary>
    /// 
    public partial class ImageDisplay : Window
    {     
        
        DataSet imageData;
        //Struktura słownikowa do zapamiętywania początkowych współrzędnych punktów      
        ObservableCollection<FeaturePointsRepository> originalPoints;
        //Stworzenie listy aktualnych punktow
        ObservableCollection<FeaturePointsRepository> featurePoints; 
        
        //Odniesienia do wyswietlanego obrazu na canvasie(do skalowania pixeli)
        ImageSource imageSource;
        BitmapImage bitmapImage;
        //Poczatkowa pozycja elementu przed przemieszczeniem
        private Point startingPosition;
        //Odwołanie do kliknietego elementu
        private UIElement lastClickedUIElement = null;
        // Klucz przesuwanego punktu
        string pointKey = "";
        //indeks przesuwanego punktu
        int indexOfPoint = (int)Utilities.Index.NotValidIndex;
        // Czy trzymano lewy klawisz myszy
        bool capture = false;
        //Przesuniecie wymagane przy rysowaniu i zaznaczaniu punktow jezeli chcemy zachowac proporcje obrazu
        double uniformOffsetX = 0;
        double uniformOffsetY = 0;

        public ImageDisplay()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Loading selected image from database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                
                string querry = "Select zdjecie from Klienci where id = " + Utilities.currentID;
                MySqlConnection connection = new MySqlConnection(Utilities.connectionString);
                //  MySqlCommand cmd = new MySqlCommand(querry, connection);
            
                //Wczytywanie obrazka
                connection.Open();
                imageData = new DataSet(); // inicjalizacja danych
                                    //Pobranie obrazka z bazy
                MySqlDataAdapter sqa = new MySqlDataAdapter(querry, connection);
                sqa.Fill(imageData);
                connection.Close();
        
                ImageConverter dataSetToImage = new ImageConverter();
                imageDisplay.Source = dataSetToImage.Convert(imageData);
                //Skalowanie wspolrzednych na pixele
                imageSource = imageDisplay.Source;
                //Pobieranie bitmapy z kontrolki image
                bitmapImage = (BitmapImage)imageSource;

               
            }
            catch(MySqlException ex)
            {
                MessageBox.Show("Wyświetlenie nieudane. Kod błędu: "+ ex.ErrorCode,"Błąd",MessageBoxButton.OK,MessageBoxImage.Error );
                
            }

        }

        
       
        /// <summary>
        /// Update changes to database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnApply_Click(object sender, RoutedEventArgs e)
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
                prpCommand.Parameters.AddWithValue("@val1", GetPointByName(featurePoints, "LSkron").X);
                prpCommand.Parameters.AddWithValue("@val2", GetPointByName(featurePoints, "LSkron").Y);

                prpCommand.Parameters.AddWithValue("@val3", GetPointByName(featurePoints, "PSkron").X);
                prpCommand.Parameters.AddWithValue("@val4", GetPointByName(featurePoints, "PSkron").Y);

                prpCommand.Parameters.AddWithValue("@val5", GetPointByName(featurePoints, "Nos").X);
                prpCommand.Parameters.AddWithValue("@val6", GetPointByName(featurePoints, "Nos").Y);

                prpCommand.Parameters.AddWithValue("@val7", GetPointByName(featurePoints, "LOko").X);
                prpCommand.Parameters.AddWithValue("@val8", GetPointByName(featurePoints, "LOko").Y);

                prpCommand.Parameters.AddWithValue("@val9", GetPointByName(featurePoints, "POko").X);
                prpCommand.Parameters.AddWithValue("@val10", GetPointByName(featurePoints, "POko").Y);

                prpCommand.Parameters.AddWithValue("@val11", GetPointByName(featurePoints, "LPol").X);
                prpCommand.Parameters.AddWithValue("@val12", GetPointByName(featurePoints, "LPol").Y);

                prpCommand.Parameters.AddWithValue("@val13", GetPointByName(featurePoints, "PPol").X);
                prpCommand.Parameters.AddWithValue("@val14", GetPointByName(featurePoints, "PPol").Y);

                int result = prpCommand.ExecuteNonQuery();

                connection.Close();
               
                Recalculate();


               
            }
            catch(Exception ex)
            {
                MessageBox.Show("Proszę wyświetlic punkty na obrazie " + ex.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }



        }

        //TODO: Moze??-> jesli bede chciał rysować nowe punkty no wystarczy napisac funkcje ktora sie wywoła w funkcji ktora sie dzieje po nacisnieciu myszki na obrazie lub canvas tam gdzie klikne myszka pobierze te wspolrzedne
        // doda do struktury słownikowej i na nowo przepisze i zrefreshuje datagrid ale nowe punkty nie będą kompatybilne z bazą, która stoi???
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //  Resetowanie położenia punktow/ czyszczenie pol
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            if (featurePoints == null)
            {
                MessageBox.Show("Proszę wyświetlic punkty na obrazie","Błąd",MessageBoxButton.OK,MessageBoxImage.Error);
            }
            else
            {
                // Zerowym elementem canvasu jest kontrolka image
                Cnv.Children.RemoveRange(1, featurePoints.Count());
                pointsDataGrid.ItemsSource = null;
                pointsDataGrid.Items.Refresh();
                featurePoints.Clear();
            }
        }

        /// <summary>
        /// Display all points from database on image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnShowMark_Click(object sender, RoutedEventArgs e)
        {

           featurePoints = FeaturePointsRepository.GetPointsFormDatabase();

            //Zapamietanie punktów przed przesuwaniem
            originalPoints = featurePoints;
            
            // Namalowanie punktow z bazy na obrazie          
            DrawCircleUniform(GetPointByName(featurePoints, "Nos").X, GetPointByName(featurePoints, "Nos").Y, true);
            DrawCircleUniform(GetPointByName(featurePoints, "LSkron").X, GetPointByName(featurePoints, "LSkron").Y, true);
            DrawCircleUniform(GetPointByName(featurePoints, "PSkron").X, GetPointByName(featurePoints, "PSkron").Y, true);
            DrawCircleUniform(GetPointByName(featurePoints, "LOko").X, GetPointByName(featurePoints, "LOko").Y, true);
            DrawCircleUniform(GetPointByName(featurePoints, "LPol").X, GetPointByName(featurePoints, "LPol").Y, true);
            DrawCircleUniform(GetPointByName(featurePoints, "PPol").X, GetPointByName(featurePoints, "PPol").Y, true);
            DrawCircleUniform(GetPointByName(featurePoints, "POko").X, GetPointByName(featurePoints, "POko").Y, true);

            // Wyswietlenie punktow w kontrolce data Grid krok 3
            pointsDataGrid.ItemsSource = featurePoints;

            //Dodanie mozliwości ruchu dla kół na obrazie
            for (int i = 1; i<= featurePoints.Count(); i++)
            {
                //Dodanie do każdej elipsy mouse UP i mouse Down i mouse move (ten sam event dla kazdego punktu od 1 bo 0 dzieckiem jest obrazek)
                Cnv.Children[i].MouseLeftButtonDown += Circle_MouseLeftButtonDown;
                Cnv.Children[i].MouseMove += Circle_MouseMove;
                Cnv.Children[i].MouseLeftButtonUp += Circle_MouseLeftButtonUp;
                Cnv.Children[i].MouseEnter += Circle_MouseEnter;
                Cnv.Children[i].MouseLeave += Circle_MouseLeave;
            }
            



        }

        private void Circle_MouseLeave(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = null;
        }

        private void Circle_MouseEnter(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Hand;
        }


        /// <summary>
        /// Recalculate all distances after coordinating points
        /// </summary> 
        private void Recalculate()
        {
            double scaleFactor = GetFullScaleFactorFromDatabase();

            Point leftEye = GetPointByName(featurePoints, "POko");
            Point rightEye = GetPointByName(featurePoints, "LOko");

            Point leftTemple = GetPointByName(featurePoints, "LSkron");
            Point rightTemple = GetPointByName(featurePoints, "PSkron");

            Point leftCheek = GetPointByName(featurePoints, "LPol");
            Point rightCheek = GetPointByName(featurePoints, "PPol");

            Point nose = GetPointByName(featurePoints, "Nos");

            //Odległość 
            double eyeToEye =  Math.Round((rightEye - leftEye).Length * scaleFactor,2);
            double templeToTemple = Math.Round((rightTemple - leftTemple).Length * scaleFactor,2);
            double CheektoCheek = Math.Round((rightCheek - leftCheek).Length * scaleFactor,2);
            double leftEyeNose = Math.Round((nose - leftEye).Length * scaleFactor,2);
            double rightEyeNose = Math.Round((nose - rightEye).Length * scaleFactor,2);

            try
            {
                //Update odległości
                MySqlConnection connection = new MySqlConnection(Utilities.connectionString);         //ZAJRZYJ TAM --------------------------->
                string updateQuerry = "Update Klienci SET Rozstaw_Zrenic = @eyeToeye, Szerokosc_Twarzy = @val2, Szerokosc_Skroni=@val3, PraweOko_Nos = @val4, LeweOko_Nos = @val5 where id = " + Utilities.currentID;
                connection.Open();
                MySqlCommand prpCommand = new MySqlCommand(updateQuerry, connection);

                prpCommand.Prepare();
                prpCommand.Parameters.AddWithValue("@eyeToeye", eyeToEye);
                prpCommand.Parameters.AddWithValue("@val2", CheektoCheek);
                prpCommand.Parameters.AddWithValue("@val3", templeToTemple);
                prpCommand.Parameters.AddWithValue("@val4", rightEyeNose);
                prpCommand.Parameters.AddWithValue("@val5", leftEyeNose);
                
                int result = prpCommand.ExecuteNonQuery();

                connection.Close();
                MessageBox.Show("Pomyślnie zaaktualizowano bazę","Sukces",MessageBoxButton.OK,MessageBoxImage.Information);
            }
            catch(MySqlException ex)
            {
                MessageBox.Show("Aktualizacja bazy nieudana. Sprawdź ustawienia połączenia","Błąd",MessageBoxButton.OK,MessageBoxImage.Error);
            }
            

        }
        

       
        #region Moving Circles events
        private void Circle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            lastClickedUIElement = null;
            Mouse.OverrideCursor = Cursors.SizeAll;
            //Określenie kliknietego elementu
            lastClickedUIElement = sender as UIElement;
            Mouse.Capture(lastClickedUIElement);
            capture = true;
            //zapamietanie startowej pozycji
            startingPosition = e.GetPosition(lastClickedUIElement);

            var pointPixelClicked_X = Math.Round(e.GetPosition(imageDisplay).X * (bitmapImage.PixelWidth / imageDisplay.ActualWidth));
            var pointPixelClicked_Y = Math.Round(e.GetPosition(imageDisplay).Y * (bitmapImage.PixelHeight / imageDisplay.ActualHeight));

            foreach (var element in featurePoints)
                    {
                //+-5 z powodu błedow WPF (trafienie w okrag myszka) // JEZELI PRZY KLIKNIENICU nie wejdzie sie do tego to punkt i tak zostaje przeniesiony ale referencja do
                //indeksu zostaje 0 -> czyli punkt na nosie.Wtedy zaaktualizowane zostana punkty nosa - trzeba dac wiekszy margines trafienia.
                        if ((pointPixelClicked_X >= element.PointCoordinates.X - Utilities.grabPixelOffset && pointPixelClicked_X <= element.PointCoordinates.X + Utilities.grabPixelOffset)
                            && (pointPixelClicked_Y>= element.PointCoordinates.Y - Utilities.grabPixelOffset &&  pointPixelClicked_Y <= element.PointCoordinates.Y + Utilities.grabPixelOffset))
                                 {
                                      pointKey = element.PointName;
                                      indexOfPoint = featurePoints.IndexOf(element);
                                      break;                    
                                 }
                                   
                    }
                        //Jezeli nie udało sie trafić w zakres poczatkowy punktu 
                        if (indexOfPoint == (int)Utilities.Index.NotValidIndex)
                        {
                            lastClickedUIElement = null;
                            Mouse.Capture(null);
                            capture = false;
                            Mouse.OverrideCursor = null;
                            return;
                        }
        }
       
        private void Circle_MouseMove(object sender, MouseEventArgs e)
        {

            if (lastClickedUIElement == null || lastClickedUIElement == imageDisplay)
            {
                lastClickedUIElement = null;
                return;
            }
            if (capture == true)
            {
                //Zmiana pozycji kliknietego obiektu
                lastClickedUIElement.SetValue(Canvas.LeftProperty, (e.GetPosition(imageDisplay).X - startingPosition.X) + uniformOffsetX);
                lastClickedUIElement.SetValue(Canvas.TopProperty, (e.GetPosition(imageDisplay).Y - startingPosition.Y) + uniformOffsetY);
            }


        }
        private void Circle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //TBD pobranie wspolrzednych i zapisanie ich do kolekcji feature points
            // identyfikacja ktory punkt klikamy?? - po połozeniu początkowych

            //jezeli w obrebie punktu wykona sie button up a nei trafilo sie w zakres
            if (indexOfPoint == (int)Utilities.Index.NotValidIndex)
            {
                return;
            }

             featurePoints[indexOfPoint].PointCoordinates = new Point(Math.Round(e.GetPosition(imageDisplay).X * (bitmapImage.PixelWidth / imageDisplay.ActualWidth)), Math.Round (e.GetPosition(imageDisplay).Y * (bitmapImage.PixelHeight / imageDisplay.ActualHeight)));
             pointsDataGrid.Items.Refresh();
            //Reset referencji do obiektu interfejsu
            lastClickedUIElement = null;
            Mouse.Capture(null);
            capture = false;
            Mouse.OverrideCursor = null;
            indexOfPoint = (int)Utilities.Index.NotValidIndex;

        }


        #endregion


        #region Utils
        // Util -> metoda do rysowania punktów na ekranie 
        private void DrawCircle(double X, double Y, bool scaleToCanvas)
        {
            // Tworzenie elipsy na obrazku
            Ellipse ellipse = new Ellipse();
            ellipse.Fill = Brushes.Orange;
            ellipse.Width = Utilities.markPointSize;
            ellipse.Height = Utilities.markPointSize;
            ellipse.StrokeThickness = 0;          
            //Dodanie elipsy jako dziecka canvas -> dlatego potem wiadomo metoda Canvas.Set -> wie co i na co wstawiać
            Cnv.Children.Add(ellipse);
            //Skalowanie wspolrzednych na pixele
             imageSource = imageDisplay.Source;
            //Pobieranie bitmapy z kontrolki image
             bitmapImage = (BitmapImage)imageSource;

            
            if (scaleToCanvas == false)
            {
                Canvas.SetLeft(ellipse, X - ellipse.Width / 2);
                Canvas.SetTop(ellipse, Y - ellipse.Height / 2);
            }
            else
            {              

                X = X * imageDisplay.ActualWidth / bitmapImage.PixelWidth;
                Y = Y * imageDisplay.ActualHeight / bitmapImage.PixelHeight;

                Canvas.SetLeft(ellipse, X - ellipse.Width / 2);
                Canvas.SetTop(ellipse, Y - ellipse.Width / 2);
            }

        }

        private void DrawCircleUniform(double X, double Y, bool scaleToCanvas)
        {
            // Tworzenie elipsy na obrazku
            Ellipse ellipse = new Ellipse();
            ellipse.Fill = Brushes.Orange;
            ellipse.Width = Utilities.markPointSize;
            ellipse.Height = Utilities.markPointSize;
            ellipse.StrokeThickness = 0;
            //Dodanie elipsy jako dziecka canvas -> dlatego potem wiadomo metoda Canvas.Set -> wie co i na co wstawiać
            Cnv.Children.Add(ellipse);
            //Skalowanie wspolrzednych na pixele
            imageSource = imageDisplay.Source;
            //Pobieranie bitmapy z kontrolki image
            bitmapImage = (BitmapImage)imageSource;
            //Wyliczenie przesuniecia wywolanego zachowaniem proporcji zdjecia
            uniformOffsetX = (Imageborder.ActualWidth - imageDisplay.ActualWidth) / 2;
            uniformOffsetY = (Imageborder.ActualHeight - imageDisplay.ActualHeight) / 2;

            if (scaleToCanvas == false)
            {
                Canvas.SetLeft(ellipse, X - ellipse.Width / 2);
                Canvas.SetTop(ellipse, Y - ellipse.Height / 2);
            }
            else
            {
                
                X = X * imageDisplay.ActualWidth / bitmapImage.PixelWidth + uniformOffsetX;
                Y = Y * imageDisplay.ActualHeight / bitmapImage.PixelHeight+ uniformOffsetY;

                Canvas.SetLeft(ellipse, X - ellipse.Width / 2);
                Canvas.SetTop(ellipse, Y - ellipse.Width / 2);
            }

        }

        private double GetFullScaleFactorFromDatabase()
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

        private Point GetPointByName(ObservableCollection<FeaturePointsRepository> collection, string pointName)
        {
            Point selectedPoint = new Point(0, 0);

            foreach (var element in collection)
            {
                if(element.PointName == pointName)
                {
                    selectedPoint = element.PointCoordinates;
                    break;
                }

            }

            return selectedPoint;


        }

        private void imageDisplay_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            //Rysowanie na obrazku
            //TODO: !!!Wazne -> gdy punkty rysuje wzgledem canvasu a nie kontrolki image to wspolrzedne sa prawidłowe
            // DrawCircle(e.GetPosition(Cnv).X, e.GetPosition(Cnv).Y,false);

            //Skalowanie wspolrzednych na pixele
            ImageSource imageSource = imageDisplay.Source;
            //Pobieranie bitmapy z kontrolki image
            BitmapImage bitmapImage = (BitmapImage)imageSource;
            //Przeskalowywanie rozmiarow kontrolni image na pixele
            double pixelMousePositionX = e.GetPosition(imageDisplay).X * bitmapImage.PixelWidth / imageDisplay.ActualWidth;
            double pixelMousePositionY = e.GetPosition(imageDisplay).Y * bitmapImage.PixelHeight / imageDisplay.ActualHeight;
            // Wypisanie wspolrzednych postawienie elipsy
            tboxTest.Text = pixelMousePositionX.ToString();
            tboxTest2.Text = pixelMousePositionY.ToString();

        }



        #endregion






    }
}
