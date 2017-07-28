using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace ImageVerification
{
    /// <summary>
    /// Interaction logic for ProfileImageDisplay.xaml
    /// </summary>
    public partial class ProfileImageDisplay : Window
    {
        DataSet ds;
        //Odniesienia do wyswietlanego obrazu na canvasie(do skalowania pixeli)
        ImageSource imageSource;
        BitmapImage bitmapImage;

        //Struktura słownikowa do zapamiętywania początkowych współrzędnych punktów      
        ObservableCollection<FeaturePointsRepository> profileoriginalPoints;
        //Stworzenie listy aktualnych punktow
        ObservableCollection<FeaturePointsRepository> profilefeaturePoints;

       

        #region elementy do przemieszczania narysowanych punktow na obrazie
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

        #endregion

        public ProfileImageDisplay()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                string querry = "Select zdjecie_profil from Klienci where id = " + Utilities.currentID;
                MySqlConnection connection = new MySqlConnection(Utilities.connectionString);
                //  MySqlCommand cmd = new MySqlCommand(querry, connection);

                //Wczytywanie obrazka
                connection.Open();
                ds = new DataSet(); // inicjalizacja danych
                                    //Pobranie obrazka z bazy
                MySqlDataAdapter sqa = new MySqlDataAdapter(querry, connection);
                sqa.Fill(ds);
                connection.Close();

                if(sqa == null)
                {
                    MessageBox.Show("Wczytywanie zakończone niepowodzeniem. Brak zdjęcia", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();
                    return;

                }
                //Konwersja zdjecia z bazy do tablicy byte
                byte[] data = (byte[])ds.Tables[0].Rows[0][0];

                // Stworzenie strumienia i przepisanie tablicy do niego
                MemoryStream strm = new MemoryStream();
                strm.Write(data, 0, data.Length);
                strm.Position = 0;

                //Stworzenie obrazga z tego strumienia
                System.Drawing.Image img = System.Drawing.Image.FromStream(strm);
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                MemoryStream ms = new MemoryStream();
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                ms.Seek(0, SeekOrigin.Begin);
                bi.StreamSource = ms;
                bi.EndInit();
                imageDisplay.Source = bi;

                //Skalowanie wspolrzednych na pixele
                imageSource = imageDisplay.Source;
                //Pobieranie bitmapy z kontrolki image
                bitmapImage = (BitmapImage)imageSource;
            }
            catch(Exception ex)
            {
                MessageBox.Show("Wczytywanie zakończone niepowodzeniem. Brak zdjęcia", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
              
               
            }




        }

        private void imageDisplay_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Rysowanie na obrazku

            //DrawCircle(e.GetPosition(imageDisplay).X, e.GetPosition(imageDisplay).Y,false);

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

        private void btnShowMark_Click(object sender, RoutedEventArgs e)
        {

            profilefeaturePoints = FeaturePointsRepository.GetProfilePointsFormDatabase();
            //Zapamietanie punktów przed przesuwaniem
            profileoriginalPoints = profilefeaturePoints;

            ///Dziala -> w słowniku sa wspolrzedne punktow z bazy

            // Krok 2 -> Namalowanie punktow z bazy na obrazie
            // Dziala rysowanie po obrazku
            DrawCircleUniform(GetPointByName(profilefeaturePoints, "Nos").X, GetPointByName(profilefeaturePoints, "Nos").Y, true);
            DrawCircleUniform(GetPointByName(profilefeaturePoints, "Ucho").X, GetPointByName(profilefeaturePoints, "Ucho").Y, true);
            DrawCircleUniform(GetPointByName(profilefeaturePoints, "Oko").X, GetPointByName(profilefeaturePoints, "Oko").Y, true);



            // Wyswietlenie punktow w kontrolce data Grid krok 3
            pointsDataGrid.ItemsSource = profilefeaturePoints;

            //Dodanie mozliwości ruchu dla kół na obrazie dla kazdego elementu na canvasie dodajemy 3 event handlery(zwiazane z myszka) ktore pozwola na przemieszczanie tych obiektow
            for (int i = 1; i <= profilefeaturePoints.Count(); i++)
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

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            if (profilefeaturePoints == null)
            {
                MessageBox.Show("Proszę wyświetlic punkty na obazie", "Błąd");
            }
            else
            {
                // Zerowym elementem canvasu jest kontrolka image
                Cnv.Children.RemoveRange(1, profilefeaturePoints.Count());
                pointsDataGrid.ItemsSource = null;
                pointsDataGrid.Items.Refresh();
                profilefeaturePoints.Clear();
            }
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {

            //  TODO: Moze nie dawać mozliwosci rysowania punktow tylko po prostu przesuwanie tych co juz sa
            // Po przesunieciu i maniu współczynnika skalowania będzie trzeba przeliczyć odległości ale do tego już sa wszystkie dane w bazie wiec wystarczy je brać
            //WSP skalowania jest i wspolrzedne -> jakas metode recalculate ktora tu sie wywola po nacisnieciu guzika 
            if (profilefeaturePoints == null)
            {
                MessageBox.Show("Proszę wyświetlić punkty na obrazie", "Błąd");
                return;

            }
            try
            {
                //Akceptowanie punktów na obrazie - update bazy -> dziala
                MySqlConnection connection = new MySqlConnection(Utilities.connectionString);         //ZAJRZYJ TAM --------------------------->
                string updateQuerry = "Update Punkty_Profil SET Ucho_X=@val1, Ucho_Y=@val2, Nos_X=@val3, Nos_Y=@val4, Oko_X=@val5, Oko_Y=@val6 where id_klienta = " + Utilities.currentID;
                connection.Open();
                MySqlCommand prpCommand = new MySqlCommand(updateQuerry, connection);

                prpCommand.Prepare();
                prpCommand.Parameters.AddWithValue("@val1", GetPointByName(profilefeaturePoints, "Ucho").X);
                prpCommand.Parameters.AddWithValue("@val2", GetPointByName(profilefeaturePoints, "Ucho").Y);

                prpCommand.Parameters.AddWithValue("@val3", GetPointByName(profilefeaturePoints, "Nos").X);
                prpCommand.Parameters.AddWithValue("@val4", GetPointByName(profilefeaturePoints, "Nos").Y);

                prpCommand.Parameters.AddWithValue("@val5", GetPointByName(profilefeaturePoints, "Oko").X);
                prpCommand.Parameters.AddWithValue("@val6", GetPointByName(profilefeaturePoints, "Oko").Y);

                int result = prpCommand.ExecuteNonQuery();

                connection.Close();

                // Update pola zatwierdzony (na true) w tabeli klienci i metoda Recaclucate
                Recalculate();



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }






        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        



        #region Moving Circles events

        private void Circle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.SizeAll;
            //Określenie kliknietego elementu
            lastClickedUIElement = sender as UIElement;
            Mouse.Capture(lastClickedUIElement);
            capture = true;
            //zapamietanie startowej pozycji
            startingPosition = e.GetPosition(lastClickedUIElement);
            var pointPixelClicked_X = Math.Round(e.GetPosition(imageDisplay).X * (bitmapImage.PixelWidth / imageDisplay.ActualWidth));
            var pointPixelClicked_Y = Math.Round(e.GetPosition(imageDisplay).Y * (bitmapImage.PixelHeight / imageDisplay.ActualHeight));

            foreach (var element in profilefeaturePoints)
            {
                //pixel offset z powodu błedow WPF (trafienie w okrag myszka)
                if ((pointPixelClicked_X >= element.PointCoordinates.X - Utilities.grabPixelOffset && pointPixelClicked_X <= element.PointCoordinates.X + Utilities.grabPixelOffset)
                    && (pointPixelClicked_Y >= element.PointCoordinates.Y - Utilities.grabPixelOffset && pointPixelClicked_Y <= element.PointCoordinates.Y + Utilities.grabPixelOffset))
                {
                    pointKey = element.PointName;
                    indexOfPoint = profilefeaturePoints.IndexOf(element);
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
                return;
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
            //aktualizacja wspolrzednych
            profilefeaturePoints[indexOfPoint].PointCoordinates = new Point(Math.Round(e.GetPosition(imageDisplay).X * (bitmapImage.PixelWidth / imageDisplay.ActualWidth)), Math.Round(e.GetPosition(imageDisplay).Y * (bitmapImage.PixelHeight / imageDisplay.ActualHeight)));
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

        private void Recalculate()
        {
            double scaleFactor = GetScaleFactorFromDatabase();

            Point ear = GetPointByName(profilefeaturePoints, "Ucho");
            Point nose = GetPointByName(profilefeaturePoints, "Nos");
            Point eye = GetPointByName(profilefeaturePoints, "Oko");

            //Odległość jest liczona poprawnie
            double earToNose = Math.Round((nose - ear).Length * scaleFactor,2);
            double eyeToNose = Math.Round((nose - eye).Length * scaleFactor, 2);
            tboxTest123.Text = earToNose.ToString();
            try
            {
                //Update odległości
                MySqlConnection connection = new MySqlConnection(Utilities.connectionString);
                string updateQuerry = "Update Punkty_Profil SET Ucho_Nos = @earToNose, Oko_Nos=@eyeToNose where Id_klienta = " + Utilities.currentID;
                string updateMainTable = "Update Klienci SET Ucho_Nos = @earToNose, Oko_Nos = @eyeToNose where id = " + Utilities.currentID;
                connection.Open();
                MySqlCommand prpCommand = new MySqlCommand(updateQuerry, connection);

                prpCommand.Prepare();
                prpCommand.Parameters.AddWithValue("@earToNose", earToNose);
                prpCommand.Parameters.AddWithValue("@eyeToNose", eyeToNose);
                int result = prpCommand.ExecuteNonQuery();

                MySqlCommand prpCommandMainTable = new MySqlCommand(updateMainTable, connection);
                prpCommandMainTable.Prepare();
                prpCommandMainTable.Parameters.AddWithValue("@earToNose", earToNose);
                prpCommandMainTable.Parameters.AddWithValue("@eyeToNose", eyeToNose);
                int maintableReult = prpCommandMainTable.ExecuteNonQuery();
                connection.Close();

                MessageBox.Show("Pomyślnie zaaktualizowano bazę", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch(MySqlException ex)
            {
                MessageBox.Show("Aktualizacja bazy nieudana. Sprawdź ustawienia połączenia", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }




        // Util -> metoda do rysowania elips na ekranie -> narazie testowa poniewaz te punkty nie sa takie jak potrzeba
        private void DrawCircle(double X, double Y, bool scaleToCanvas)
        {
            // Tworzenie elipsy na obrazku
            Ellipse ellipse = new Ellipse();
            ellipse.Fill = Brushes.Orange;
            ellipse.Width = Utilities.markPointSize;
            ellipse.Height = Utilities.markPointSize;
            ellipse.StrokeThickness = 1;
            //Dodanie elipsy jako dziecka canvas -> dlatego potem wiadomo metoda Canvas.Set -> wie co i na co wstawiać
            Cnv.Children.Add(ellipse);
            //Skalowanie wspolrzednych na pixele
            imageSource = imageDisplay.Source;
            //Pobieranie bitmapy z kontrolki image
            bitmapImage = (BitmapImage)imageSource;


            if (scaleToCanvas == false)
            {
                Canvas.SetLeft(ellipse, X - ellipse.Width/2);
                Canvas.SetTop(ellipse, Y - ellipse.Height/2);
            }
            else
            {
                X = X * imageDisplay.ActualWidth / bitmapImage.PixelWidth;
                Y = Y * imageDisplay.ActualHeight / bitmapImage.PixelHeight;

                Canvas.SetLeft(ellipse, X - ellipse.Width / 2);
                Canvas.SetTop(ellipse, Y - ellipse.Height / 2);
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
                Y = Y * imageDisplay.ActualHeight / bitmapImage.PixelHeight + uniformOffsetY;

                Canvas.SetLeft(ellipse, X - ellipse.Width / 2);
                Canvas.SetTop(ellipse, Y - ellipse.Width / 2);
            }

        }


        private double GetScaleFactorFromDatabase()
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

        private Point GetPointByName(ObservableCollection<FeaturePointsRepository> collection, string pointName)
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





        #endregion

    }
}
