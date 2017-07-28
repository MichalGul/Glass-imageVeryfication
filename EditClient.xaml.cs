using MySql.Data.MySqlClient;
using System.Data;
using System.Windows;
using MySql.Data.MySqlClient;
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
using System.Windows.Documents;
using System.Windows.Markup;
using Microsoft.Win32;
using ImageVerification.Model;

namespace ImageVerification
{
    /// <summary>
    /// Interaction logic for EditClient.xaml
    /// </summary>
    public partial class EditClient : Window
    {

        private Customer SelectedCustomer { get; set; }


        BitmapImage image = new BitmapImage();
        FileStream imageToDatabaseFileStream;
        byte[] frontImageData;
        byte[] profileImageData;
        bool updateDatabaseWithPhoto = false;
        bool updateDatabaseWithProfilePhoto = false;
        public EditClient()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataSet frontImageDataSet = new DataSet();
            DataSet profileImageDataSet = new DataSet();
            //Pobieranie danych z bazy
            string querry = "Select * from Klienci where id = " + Utilities.currentID;
            string querryImage = "Select zdjecie from Klienci where id = " + Utilities.currentID;
            string querryProfileImage = "Select zdjecie_profil from Klienci where id = " + Utilities.currentID;
            MySqlConnection connection = new MySqlConnection(Utilities.connectionString);
            //Odczytanie wynikow zapytania
            MySqlDataReader results = null;
            connection.Open();

            MySqlDataAdapter frontImageDataAdapter = new MySqlDataAdapter(querryImage, connection);
            MySqlDataAdapter profileImageDataAdapter = new MySqlDataAdapter(querryProfileImage, connection);

            //Pobranie obrazka z bazy         
            frontImageDataAdapter.Fill(frontImageDataSet);
            profileImageDataAdapter.Fill(profileImageDataSet);

            //Stworzenie polecenia do bazy danych
            MySqlCommand command = new MySqlCommand(querry, connection);
            results = command.ExecuteReader();

            while (results.Read())
            {
                Idtbox.Text = results.GetInt32(0).ToString();
                imietbox.Text = results[1].ToString();
                nazwiskotbox.Text = results["nazwisko"].ToString();
                emailtbox.Text = results["email"].ToString();
                Zrenicetbox.Text = results["Rozstaw_Zrenic"].ToString();
                Twarztbox.Text = results["Szerokosc_Twarzy"].ToString();
                Skrontbox.Text = results["Szerokosc_Skroni"].ToString();
                PraweOkoNostbox.Text = results["PraweOko_Nos"].ToString();
                LeweOkoNostbox.Text = results["LeweOko_Nos"].ToString();
                UchoNostbox.Text = results["Ucho_Nos"].ToString();
                OkoNostbox.Text = results["Oko_Nos"].ToString();
                Zatwierdztbox.IsChecked = (bool)results["zatwierdzone"];

            }
            connection.Close();

            try
            {
                ImageConverter convertImage = new ImageConverter();             
                imageCtr.Source = convertImage.Convert(frontImageDataSet);// przypisanie kontrolce obrazka
                profileImageCtr.Source = convertImage.Convert(profileImageDataSet);
            }
            catch(Exception ex)
            {
                return;

            }

        }
       
        private void Okbtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (updateDatabaseWithPhoto == false && updateDatabaseWithProfilePhoto == false)
                {
                    UpdateDatabaseWithoutPhoto();

                }
                else if (updateDatabaseWithPhoto == true && updateDatabaseWithProfilePhoto == false)
                {
                    UpdateDatabaseWithPhoto();
                    UpdateDatabaseWithoutPhoto();

                }
                else if(updateDatabaseWithPhoto == false && updateDatabaseWithProfilePhoto == true)
                {
                    UpdateDatabaseWithProfilePhoto();
                    UpdateDatabaseWithoutPhoto();

                }else if(updateDatabaseWithPhoto == true && updateDatabaseWithProfilePhoto == true)
                {
                    UpdateDatabaseWithPhoto();
                    UpdateDatabaseWithProfilePhoto();
                    UpdateDatabaseWithoutPhoto();

                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Błąd wprowadzania wartośc lub błąd połączenia. Dane liczbowe prosze wprowadzać przy pomocy przecinka.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;

            }


        }

        private void Cancelbtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Printbtn_Click(object sender, RoutedEventArgs e)
        {

            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() != true) return;

            //Creating fixed document
            FixedDocument document = new FixedDocument();
            document.DocumentPaginator.PageSize = new Size(printDialog.PrintableAreaWidth, printDialog.PrintableAreaHeight);
            //Stworzenie strony
            FixedPage pageOne = new FixedPage();
            pageOne.Width = document.DocumentPaginator.PageSize.Width;
            pageOne.Height = document.DocumentPaginator.PageSize.Height;

            //Dodawanie elementow na strone
            //Image printImage = new Image();
            //printImage.Source = image;
            //pageOne.Children.Add((UIElement)imageCtr);

            TextBlock tbTitle = new TextBlock();
            tbTitle.Text = "Dane Klienta";
            tbTitle.FontSize = 28;
            tbTitle.FontFamily = new FontFamily("Segoe UI");
            tbTitle.FontWeight = FontWeights.Bold;
            FixedPage.SetLeft(tbTitle,350); // left margin
            FixedPage.SetTop(tbTitle, 20); // top margin
            pageOne.Children.Add((UIElement)tbTitle);

            Image printControl = new Image();
            printControl.Source = imageCtr.Source;
            printControl.Stretch = Stretch.Fill;
            printControl.Width = imageCtr.Width;
            printControl.Height = imageCtr.Height;
            FixedPage.SetLeft(printControl, 120);
            FixedPage.SetTop(printControl, 70);
            pageOne.Children.Add((UIElement)printControl);

            Image printProfileControl = new Image();
            printProfileControl.Source = profileImageCtr.Source;
            printProfileControl.Stretch = Stretch.Fill;
            printProfileControl.Width = imageCtr.Width;
            printProfileControl.Height = imageCtr.Height;
            FixedPage.SetLeft(printProfileControl, 420);
            FixedPage.SetTop(printProfileControl, 70);
            pageOne.Children.Add((UIElement)printProfileControl);



            TextBlock Id = new TextBlock();
            Id.Text = "ID: " + Idtbox.Text;
            Id.FontSize = 16;
            Id.FontFamily = new FontFamily("Segoe UI");
            FixedPage.SetLeft(Id, 120); // left margin
            FixedPage.SetTop(Id, 370); // top margin
            pageOne.Children.Add((UIElement)Id);

            TextBlock imie = new TextBlock();
            imie.Text = "Imię: " +  (imietbox.Text);
            imie.FontSize = 16;
            imie.FontFamily = new FontFamily("Segoe UI");
            FixedPage.SetLeft(imie, 120); // left margin
            FixedPage.SetTop(imie, 400); // top margin
            pageOne.Children.Add((UIElement)imie);

            TextBlock nazw = new TextBlock();
            nazw.Text = "Nazwisko: " + nazwiskotbox.Text;
            nazw.FontSize = 16;
            nazw.FontFamily = new FontFamily("Segoe UI");
            FixedPage.SetLeft(nazw, 120); // left margin
            FixedPage.SetTop(nazw, 430); // top margin
            pageOne.Children.Add((UIElement)nazw);

            TextBlock email = new TextBlock();
            email.Text = "Email: " + nazwiskotbox.Text;
            email.FontSize = 16;
            email.FontFamily = new FontFamily("Segoe UI");
            FixedPage.SetLeft(email, 120); // left margin
            FixedPage.SetTop(email, 460); // top margin
            pageOne.Children.Add((UIElement)email);

            TextBlock zrenice = new TextBlock();
            zrenice.Text = "Rozstaw źrenic: " + Zrenicetbox.Text;
            zrenice.FontSize = 16;
            zrenice.FontFamily = new FontFamily("Segoe UI");
            FixedPage.SetLeft(zrenice, 120); // left margin
            FixedPage.SetTop(zrenice, 490); // top margin
            pageOne.Children.Add((UIElement)zrenice);

            TextBlock twarz = new TextBlock();
            twarz.Text = "Szerokość twarzy: " + Twarztbox.Text;
            twarz.FontSize = 16;
            twarz.FontFamily = new FontFamily("Segoe UI");
            FixedPage.SetLeft(twarz, 120); // left margin
            FixedPage.SetTop(twarz, 520); // top margin
            pageOne.Children.Add((UIElement)twarz);

            TextBlock skron = new TextBlock();
            skron.Text = "Szerokość skroni: " + Skrontbox.Text;
            skron.FontSize = 16;
            skron.FontFamily = new FontFamily("Segoe UI");
            FixedPage.SetLeft(skron, 120); // left margin
            FixedPage.SetTop(skron, 550); // top margin
            pageOne.Children.Add((UIElement)skron);

            TextBlock rightEyeNose = new TextBlock();
            rightEyeNose.Text = "Odległość prawego oka do nosa: " + PraweOkoNostbox.Text;
            rightEyeNose.FontSize = 16;
            rightEyeNose.FontFamily = new FontFamily("Segoe UI");
            FixedPage.SetLeft(rightEyeNose, 120); // left margin
            FixedPage.SetTop(rightEyeNose, 580); // top margin
            pageOne.Children.Add((UIElement)rightEyeNose);

            TextBlock leftEyeNose = new TextBlock();
            leftEyeNose.Text = "Odległość lewego oka do nosa: " + LeweOkoNostbox.Text;
            leftEyeNose.FontSize = 16;
            leftEyeNose.FontFamily = new FontFamily("Segoe UI");
            FixedPage.SetLeft(leftEyeNose, 120); // left margin
            FixedPage.SetTop(leftEyeNose, 610); // top margin
            pageOne.Children.Add((UIElement)leftEyeNose);

            TextBlock earNose = new TextBlock();
            earNose.Text = "Odległość od ucha do nosa: " + UchoNostbox.Text;
            earNose.FontSize = 16;
            earNose.FontFamily = new FontFamily("Segoe UI");
            FixedPage.SetLeft(earNose, 120); // left margin
            FixedPage.SetTop(earNose, 640); // top margin
            pageOne.Children.Add((UIElement)earNose);

            TextBlock eyeNose = new TextBlock();
            eyeNose.Text = "Odległość od ucha do nosa: " + OkoNostbox.Text;
            eyeNose.FontSize = 16;
            eyeNose.FontFamily = new FontFamily("Segoe UI");
            FixedPage.SetLeft(eyeNose, 120); // left margin
            FixedPage.SetTop(eyeNose, 670); // top margin
            pageOne.Children.Add((UIElement)eyeNose);
        
            pageOne.Measure(new Size(printDialog.PrintableAreaWidth, printDialog.PrintableAreaHeight));
            pageOne.Arrange(new Rect(new Point(), (new Size (printDialog.PrintableAreaWidth, printDialog.PrintableAreaHeight))));
            pageOne.UpdateLayout();
            
            //Dodawanie strony do dokumentu
            PageContent pageOneContent = new PageContent();
            ((IAddChild)pageOneContent).AddChild(pageOne);
            document.Pages.Add(pageOneContent);

            //drukowanie
            printDialog.PrintDocument(document.DocumentPaginator, "Dane Klienta: " + imietbox.Text + " " + nazwiskotbox.Text);

        }

        private void SearchDialogbtn_Click(object sender, RoutedEventArgs e)
        {
            
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg";
                
            if (op.ShowDialog() == true)
            {
                imageCtr.Source = new BitmapImage(new Uri(op.FileName));
                if (op.FileName == null)
                {
                    updateDatabaseWithPhoto = false;
                    return;
                }
            }

            if (op.FileName != "")
            {
                imageToDatabaseFileStream = new FileStream(op.FileName, FileMode.Open, FileAccess.Read);
                frontImageData = new byte[imageToDatabaseFileStream.Length];
                imageToDatabaseFileStream.Read(frontImageData, 0, System.Convert.ToInt32(imageToDatabaseFileStream.Length));
                imageToDatabaseFileStream.Close();
                updateDatabaseWithPhoto = true;
            }else
            {
                updateDatabaseWithPhoto = false;
                return;
            }

        }

        private void UpdateDatabaseWithPhoto()
        {
            try
            {


                MySqlConnection connection = new MySqlConnection(Utilities.connectionString);
                string updateQuerry = "Update Klienci SET zdjecie=@param1 where id = " + Utilities.currentID;
                connection.Open();
                MySqlCommand prpCommand = new MySqlCommand(updateQuerry, connection);
                if (prpCommand == null)
                {
                    MessageBox.Show("Połączenie nieudane. Sprawdź ustawienia połączenia");
                    return;
                }


                prpCommand.Prepare();              
                prpCommand.Parameters.AddWithValue("@param1", frontImageData);        
                int result = prpCommand.ExecuteNonQuery();
                connection.Close();
                
            }
            catch (Exception ex)
            {

                MessageBox.Show("Błąd przy przesyłaniu zdjęcia do bazy. Proszę sprawdzić połączenie.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;

            }


        }

        private void UpdateDatabaseWithoutPhoto()
        {
            try
            {


                MySqlConnection connection = new MySqlConnection(Utilities.connectionString);
                string updateQuerry = "Update Klienci SET imie=@param2, nazwisko=@param3, email=@param4, Rozstaw_Zrenic=@param5, Szerokosc_Twarzy=@param6, Szerokosc_Skroni=@param7, PraweOko_Nos=@param8, LeweOko_Nos=@param9, Ucho_Nos=@param10, Oko_Nos =@param11,  zatwierdzone=@param12 where id = " + Utilities.currentID;
                connection.Open();
                MySqlCommand prpCommand = new MySqlCommand(updateQuerry, connection);
                if (prpCommand == null)
                {
                    MessageBox.Show("Połączenie nieudane. Sprawdź ustawienia połączenia");
                    return;
                }


                prpCommand.Prepare();
                prpCommand.Parameters.AddWithValue("@param2", imietbox.Text);
                prpCommand.Parameters.AddWithValue("@param3", nazwiskotbox.Text);
                prpCommand.Parameters.AddWithValue("@param4", emailtbox.Text);
                prpCommand.Parameters.AddWithValue("@param5", Convert.ToDouble(Zrenicetbox.Text));
                prpCommand.Parameters.AddWithValue("@param6", Convert.ToDouble(Twarztbox.Text));
                prpCommand.Parameters.AddWithValue("@param7", Convert.ToDouble(Skrontbox.Text));
                prpCommand.Parameters.AddWithValue("@param8", Convert.ToDouble(PraweOkoNostbox.Text));
                prpCommand.Parameters.AddWithValue("@param9", Convert.ToDouble(LeweOkoNostbox.Text));
                prpCommand.Parameters.AddWithValue("@param10", Convert.ToDouble(UchoNostbox.Text));
                prpCommand.Parameters.AddWithValue("@param11", Convert.ToDouble(OkoNostbox.Text));
                prpCommand.Parameters.AddWithValue("@param12", Convert.ToInt32(Zatwierdztbox.IsChecked));
                int result = prpCommand.ExecuteNonQuery();


                connection.Close();

                MessageBox.Show("Zatwierdzono wprowadzone zmiany", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {

                MessageBox.Show("Błąd wprowadzania wartośc lub błąd połączenia. Dane liczbowe prosze wprowadzać przy pomocy przecinka.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;

            }

        }

        private void UpdateDatabaseWithProfilePhoto()
        {

            try
            {
            
                MySqlConnection connection = new MySqlConnection(Utilities.connectionString);
                string updateQuerry = "Update Klienci SET zdjecie_profil = @param1 where id = " + Utilities.currentID;
                connection.Open();
                MySqlCommand prpCommand = new MySqlCommand(updateQuerry, connection);
                if (prpCommand == null)
                {
                    MessageBox.Show("Połączenie nieudane. Sprawdź ustawienia połączenia");
                    return;
                }

                prpCommand.Prepare();
                prpCommand.Parameters.AddWithValue("@param1", profileImageData);
                int result = prpCommand.ExecuteNonQuery();
                connection.Close();

            }
            catch (Exception ex)
            {

                MessageBox.Show("Błąd przy przesyłaniu zdjęcia z profilu do bazy. Proszę sprawdzić połączenie.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;

            }


        }

   

        private void SearchProfileDialogbtn_Click(object sender, RoutedEventArgs e)
        {
            if(profileImageCtr.Source != null)
            {

                OpenFileDialog op = new OpenFileDialog();
                op.Title = "Select a picture";
                op.Filter = "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg";

                if (op.ShowDialog() == true)
                {
                    profileImageCtr.Source = new BitmapImage(new Uri(op.FileName));
                    if (op.FileName == null)
                    {
                        updateDatabaseWithProfilePhoto = false;
                        return;
                    }
                }

                if (op.FileName != "")
                {
                    FileStream profileImageToDatabaseFileStream = new FileStream(op.FileName, FileMode.Open, FileAccess.Read);
                    profileImageData = new byte[profileImageToDatabaseFileStream.Length];
                    profileImageToDatabaseFileStream.Read(profileImageData, 0, System.Convert.ToInt32(profileImageToDatabaseFileStream.Length));
                    profileImageToDatabaseFileStream.Close();
                    updateDatabaseWithProfilePhoto = true;
                }
                else
                {
                    updateDatabaseWithProfilePhoto = false;
                    return;
                }

            } 
        }






    }
}
