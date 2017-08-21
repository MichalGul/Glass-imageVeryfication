using ImageVerification.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ImageVerification
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
       
        private Customer selectedCustomer;
        private ObservableCollection<Customer> Customers;
        ICustomerRepository customersRepository;

       //Id zaznaczonego rekordu
        string selectedID = "";
       
        //Import analizy obrazu z biblioteki C++       
        [DllImport("E:\\_PROJEKTY\\C++DLLtoC#\\FaceLandmarkDLL\\x64\\Debug\\FaceLandmarkDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool CalculateFrontFeaturePoints(int ID, bool ResizaImage, double resizeFactor, bool useHoughTransformDetection);

        [DllImport("E:\\_PROJEKTY\\C++DLLtoC#\\FaceLandmarkDLL\\x64\\Debug\\FaceLandmarkDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool CalculateProfileFeaturePoints(int ID);

        [DllImport("E:\\_PROJEKTY\\C++DLLtoC#\\FaceLandmarkDLL\\x64\\Debug\\FaceLandmarkDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void CloseAllImageWindows();


        //TODO: Udokumentowac jakoś kod
        //TODO: Napisać jakaś instrukcje obsługi
        //TODO: Ułożenie interfejsu


        public MainWindow()
        {
            InitializeComponent();
          
        }
       
        private void LoadData()
        {
            customersRepository = new CustomerRepository();
            Customers = customersRepository.GetCustomers();
            
        }


        /// <summary>
        /// Display window of database connection settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExecute_Click(object sender, RoutedEventArgs e)
        {

            DatabaseConnectionSettings settings = new DatabaseConnectionSettings();
            settings.ShowDialog();
           

        }
        /// <summary>
        /// Load image of selected record
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLoadImage_Click(object sender, RoutedEventArgs e)
        {

            if (dataGrid.Items.Count == 0)
            {
                MessageBox.Show("Należy załadować listę klientów","Błąd", MessageBoxButton.OK,MessageBoxImage.Error);

                return; }

            else
            {
                if (chbDisplayProfileImage.IsChecked == true)
                {
                    //Wyświetl okno do zdjęcia z profilu
                    ProfileImageDisplay showProfileImage = new ProfileImageDisplay();
                    showProfileImage.selectedCustomerToRecalculate = selectedCustomer;
                    showProfileImage.ShowDialog();

                }
                else
                {
                    //Otworzenie nowego do zdjecia z frontu
                    ImageDisplay showimage = new ImageDisplay();
                    showimage.selectedCustomerToRecalculate = selectedCustomer;
                    showimage.ShowDialog();
                }
            }
        }
        /// <summary>
        /// Display all data from database on datagrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnShowAllData_Click(object sender, RoutedEventArgs e)
        {
            
                LoadData();                              
                dataGrid.ItemsSource = Customers;

                //Ustawienie nazw kolumn
                dataGrid.Columns[0].Header = "Id";
                dataGrid.Columns[1].Header = "Imię";
                dataGrid.Columns[2].Header = "Nazwisko";
                dataGrid.Columns[3].Header = "Email";
                dataGrid.Columns[4].Header = "Rozstaw źrenic [mm]";
                dataGrid.Columns[5].Header = "Szerokość twarzy [mm]";
                dataGrid.Columns[6].Header = "Szerokość skroni [mm]";
                dataGrid.Columns[7].Header = "Nos - prawe oko [mm]";
                dataGrid.Columns[8].Header = "Nos - lewe oko [mm]";
                dataGrid.Columns[9].Header = "Nos - ucho [mm]";
                dataGrid.Columns[10].Header = "Głębokość oczodołu [mm]";
                dataGrid.Columns[11].Header = "Twarz z przodu";
                dataGrid.Columns[12].Header = "Twarz z profilu";
                dataGrid.Columns[13].Header = "Zatwierdzone";

                  dataGrid.Items.Refresh();

                foreach (DataGridColumn column in dataGrid.Columns)
                {                   
                    //if you want to size ur column as per the cell content
                    //column.Width = new DataGridLength(1.0, DataGridLengthUnitType.SizeToCells);
                    //if you want to size ur column as per the column header
                  //  column.Width = new DataGridLength(1.0, DataGridLengthUnitType.SizeToHeader);
                    //if you want to size ur column as per both header and cell content
                    column.Width = new DataGridLength(1.0, DataGridLengthUnitType.Auto);
                }
            
          
        }


        private void dataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            //zaznaczenie wybranego klienta
            selectedCustomer = dataGrid.SelectedItem as Customer;

            // Pobieranie ID zaznaczonego elementu         
            if (selectedCustomer != null)
            {
                var result = selectedCustomer.CustomerId.ToString();                             
                Utilities.currentID = result;
                selectedID = result;
            }
        }
   

        private void InformationMenuItem_Click(object sender, RoutedEventArgs e)
        {
            About aboutWindow = new About();
            aboutWindow.ShowDialog();

        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            
            Close();
        }

       

        private void btnInfo_Click(object sender, RoutedEventArgs e)
        {

            //CustomerRepository customersService = new CustomerRepository();
           // ObservableCollection<Customer> customers = customersService.GetCustomers();


             About info = new About();
            info.ShowDialog();

        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        // Wykorzystanie kodu C++ w celu obliczenia cech i odległości danego klienta
        private void btnCalculatePoints_Click(object sender, RoutedEventArgs e)
        {

            PleaseWait wait = new PleaseWait();
            wait.ShowDialog();  

        }
        /// <summary>
        /// Perform calculation on profile image forma database. Call extern c++ library
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCalculateProfilePoints_Click(object sender, RoutedEventArgs e)
        {
            // jezeli currentID nie jest wybrane - jest pustym Stringiem (nie wczytano danych z bazy) to nic nie robić
            if (Utilities.currentID == "")
            {
                return;
            }
            else if (Utilities.currentID.Length == 1)
            {
                if (CalculateProfileFeaturePoints(Int32.Parse(Utilities.currentID)) == true)
                {
                    MessageBox.Show("Pomyślnie przetworzono zdjęcie.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                    CloseAllImageWindows();
                }
                else
                {
                    MessageBox.Show("Analiza zdjęcia zakończyła się niepowodzeniem. Spróbuj jeszcze raz z innym zdjeciem.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnTableInfo_Click(object sender, RoutedEventArgs e)
        {
            TablesInfo tableInf = new TablesInfo();
            tableInf.ShowDialog();
        }
       
            
        private void Cell_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            
            DataGridCell cell  = sender as DataGridCell;       
                
            if (cell.Column == frontImageCell)
            {              
                ImageDisplay showimage = new ImageDisplay();
                showimage.selectedCustomerToRecalculate = selectedCustomer;
                showimage.ShowDialog();

            }
            else if(cell.Column == profileImageCell)
            {              
                ProfileImageDisplay showProfileImage = new ProfileImageDisplay();
                showProfileImage.selectedCustomerToRecalculate = selectedCustomer;
                showProfileImage.ShowDialog();
            }
            else
            {                               
                //EditRow editCurrentRow = new EditRow();
                //editCurrentRow.ShowDialog();

                EditClient editCurrentClient = new EditClient();
                editCurrentClient.selectedCustomerToEdit = selectedCustomer;
                editCurrentClient.ShowDialog();
            }
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (Utilities.currentID == "")
            {
                return;
            }
            MessageBoxResult result = MessageBox.Show("Czy napewno chcesz skasować zaznaczony rekord?", "Usuwanie rekordu", MessageBoxButton.YesNo,MessageBoxImage.Question);

            switch(result)
            {
                case MessageBoxResult.Yes:
                    customersRepository.DeleteCustomer(selectedCustomer);
                    
                    break;
                case MessageBoxResult.No:
                    break;
            }

        }

         
        private void resizaCmb_DropDownClosed(object sender, EventArgs e)
        {
           
            if (resizaCmb.SelectedItem == null) return;
            switch((int)resizaCmb.SelectedItem)
            {
                case 1:
                    Utilities.resizeImage = false;
                    Utilities.resizeFactor = 1;
                    break;
                case 2:
                    Utilities.resizeImage = true;
                    Utilities.resizeFactor = 0.5;
                    break;
                case 4:
                    Utilities.resizeImage = true;
                    Utilities.resizeFactor = 0.25;
                    break;
                case 8:
                    Utilities.resizeImage = true;
                    Utilities.resizeFactor = 0.125;
                    break;

            }
           // MessageBox.Show(Utilities.resizeImage.ToString() + "   " + Utilities.resizeFactor.ToString());
         
           


        }

        private void btnEditCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.Items.Count < 1)
                return;

            EditClient editCurrentClient = new EditClient();
            editCurrentClient.selectedCustomerToEdit = selectedCustomer;
            editCurrentClient.ShowDialog();
        }

        private void chbUseHoughTransform_Checked(object sender, RoutedEventArgs e)
        {
            Utilities.useHoughTransoformPupilDetection = true;
            
        }

        private void chbUseHoughTransform_Unchecked(object sender, RoutedEventArgs e)
        {
            Utilities.useHoughTransoformPupilDetection = false;
           
        }
    }
}
