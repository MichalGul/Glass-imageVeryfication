using System.Windows;
using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Documents;
using System.Windows.Markup;
using Microsoft.Win32;
using ImageVerification.Model;

namespace ImageVerification
{
    /// <summary>
    /// Interaction logic for Edit Customer Window
    /// </summary>
    public partial class EditClient : Window
    {

        public Customer selectedCustomerToEdit;
        public CustomerRepository customerRepository;

        BitmapImage image = new BitmapImage();
        FileStream imageToDatabaseFileStream;
        byte[] frontImageData;
        byte[] profileImageData;
      //  bool updateDatabaseWithPhoto = false;
       // bool updateDatabaseWithProfilePhoto = false;
        public EditClient()
        {
            InitializeComponent();
            this.Loaded += EditClient_Loaded;
        }

        private void EditClient_Loaded(object sender, RoutedEventArgs e)
        {
            //Using databinding tree every control on window will find its own property based on name
            this.DataContext = selectedCustomerToEdit;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ImageConverter convertImage = new ImageConverter();             
                imageCtr.Source = convertImage.ConvertByteArrayToBitmap(selectedCustomerToEdit.FrontImage);
                profileImageCtr.Source = convertImage.ConvertByteArrayToBitmap(selectedCustomerToEdit.ProfileImage);

                //In case of not chaning picture we left oryginal pictures
                frontImageData = selectedCustomerToEdit.FrontImage;
                profileImageData = selectedCustomerToEdit.ProfileImage;
            }
            catch(Exception ex)
            {
                MessageBox.Show("Bład wczytywania rekordu. " + ex.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;

            }
        }
       
        private void Okbtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {              
                UpdateCustomer();
            
            }
            catch (Exception ex)
            {

                MessageBox.Show("Błąd wprowadzania wartośc lub błąd połączenia. Dane liczbowe prosze wprowadzać przy pomocy kropki.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;

            }


        }

        private void Cancelbtn_Click(object sender, RoutedEventArgs e)
        {

            this.Close();
        }

        /// <summary>
        /// Setting printing details of the document
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Printbtn_Click(object sender, RoutedEventArgs e)
        {

            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() != true) return;

            //Creating fixed document
            FixedDocument document = new FixedDocument();
            document.DocumentPaginator.PageSize = new Size(printDialog.PrintableAreaWidth, printDialog.PrintableAreaHeight);
            //Creating page
            FixedPage pageOne = new FixedPage();
            pageOne.Width = document.DocumentPaginator.PageSize.Width;
            pageOne.Height = document.DocumentPaginator.PageSize.Height;
   
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
            
            //Adding page to document
            PageContent pageOneContent = new PageContent();
            ((IAddChild)pageOneContent).AddChild(pageOne);
            document.Pages.Add(pageOneContent);

            //printing
            printDialog.PrintDocument(document.DocumentPaginator, "Dane Klienta: " + imietbox.Text + " " + nazwiskotbox.Text);

        }
        /// <summary>
        /// Opening search dialog for new photo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                    
                    return;
                }
            }

            if (op.FileName != "")
            {
                imageToDatabaseFileStream = new FileStream(op.FileName, FileMode.Open, FileAccess.Read);
                frontImageData = new byte[imageToDatabaseFileStream.Length];
                imageToDatabaseFileStream.Read(frontImageData, 0, System.Convert.ToInt32(imageToDatabaseFileStream.Length));
                imageToDatabaseFileStream.Close();
               
            }else
            {
               
                return;
            }

        }

        /// <summary>
        /// Updating edited customer
        /// </summary>
        private void UpdateCustomer()
        {
            customerRepository = new CustomerRepository();
            //Binding pictures rest of data is binded by two way binding in XAML
            selectedCustomerToEdit.FrontImage = frontImageData;
            selectedCustomerToEdit.ProfileImage = profileImageData;
            customerRepository.UpdateCustomer(selectedCustomerToEdit);
        }



        private void SearchProfileDialogbtn_Click(object sender, RoutedEventArgs e)
        {        
                OpenFileDialog op = new OpenFileDialog();
                op.Title = "Select a picture";
                op.Filter = "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg";

                if (op.ShowDialog() == true)
                {
                    profileImageCtr.Source = new BitmapImage(new Uri(op.FileName));
                    if (op.FileName == null)
                    {
                       
                        return;
                    }
                }

                if (op.FileName != "")
                {
                    FileStream profileImageToDatabaseFileStream = new FileStream(op.FileName, FileMode.Open, FileAccess.Read);
                    profileImageData = new byte[profileImageToDatabaseFileStream.Length];
                    profileImageToDatabaseFileStream.Read(profileImageData, 0, System.Convert.ToInt32(profileImageToDatabaseFileStream.Length));
                    profileImageToDatabaseFileStream.Close();
                    
                }
                else
                {                  
                    return;
                }        
        }
    }
}
