using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ImageVerification.Model
{
    /// <summary>
    /// Manages whole customers carried in database
    /// </summary>
   public class CustomerRepository :ICustomerRepository
    {
        public static ObservableCollection<Customer> customersBase;

        /// <summary>
        /// Pass customer base 
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<Customer> GetCustomers()
        {       
             LoadCustomersFromDatabase();
             return customersBase;
        }

     /// <summary>
     /// Update selected customer in collection and send that data to database
     /// </summary>
     /// <param name="selectedCustomer"> Selected customer to modify</param>
        public void UpdateCustomer(Customer selectedCustomer)
        {
            //Switching customer
            Customer customerToUpdate = customersBase.Where(C => C.CustomerId == selectedCustomer.CustomerId).FirstOrDefault();
            customerToUpdate = selectedCustomer; //switching places            
            try
            {
                //Update customer in database
                MySqlConnection connection = new MySqlConnection(Utilities.connectionString);
                string updateQuerry = "Update Klienci SET imie=@param2, nazwisko=@param3, email=@param4, Rozstaw_Zrenic=@param5, Szerokosc_Twarzy=@param6, Szerokosc_Skroni=@param7, PraweOko_Nos=@param8, LeweOko_Nos=@param9, Ucho_Nos=@param10, Oko_Nos =@param11,  zatwierdzone=@param12, zdjecie=@param13, zdjecie_profil=@param14 where id = " + Utilities.currentID;
                connection.Open();
                MySqlCommand prpCommand = new MySqlCommand(updateQuerry, connection);
                if (prpCommand == null)
                {
                    MessageBox.Show("Połączenie nieudane. Sprawdź ustawienia połączenia","Błąd",MessageBoxButton.OK,MessageBoxImage.Error);
                    return;
                }
                prpCommand.Prepare();
                prpCommand.Parameters.AddWithValue("@param2", customerToUpdate.CustomerName);
                prpCommand.Parameters.AddWithValue("@param3", customerToUpdate.CustomerSurname);
                prpCommand.Parameters.AddWithValue("@param4", customerToUpdate.CustomerEmail);
                prpCommand.Parameters.AddWithValue("@param5", Convert.ToDouble(customerToUpdate.PupilDistance));
                prpCommand.Parameters.AddWithValue("@param6", Convert.ToDouble(customerToUpdate.FaceWidth));
                prpCommand.Parameters.AddWithValue("@param7", Convert.ToDouble(customerToUpdate.TempleWidth));
                prpCommand.Parameters.AddWithValue("@param8", Convert.ToDouble(customerToUpdate.RightEyeNoseDistance));
                prpCommand.Parameters.AddWithValue("@param9", Convert.ToDouble(customerToUpdate.LeftEyeNoseDistance));
                prpCommand.Parameters.AddWithValue("@param10", Convert.ToDouble(customerToUpdate.ProfileNoseEarDistance));
                prpCommand.Parameters.AddWithValue("@param11", Convert.ToDouble(customerToUpdate.ProfileNoseEyeDistance));
                prpCommand.Parameters.AddWithValue("@param12", Convert.ToInt32(customerToUpdate.Approved));
                prpCommand.Parameters.AddWithValue("@param13", customerToUpdate.FrontImage);
                prpCommand.Parameters.AddWithValue("@param14", customerToUpdate.ProfileImage);
                int result = prpCommand.ExecuteNonQuery();
                connection.Close();

                MessageBox.Show("Zatwierdzono wprowadzone zmiany", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {

                MessageBox.Show("Błąd wprowadzania wartośc lub błąd połączenia. Dane liczbowe prosze wprowadzać przy pomocy kropki.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;

            }         
        }
   
/// <summary>
/// Manage deleting Customer from collections and all tables in database in proper order 
/// </summary>
/// <param name="selectedCustomerToDelete"></param>
        public void DeleteCustomer(Customer selectedCustomerToDelete)
        {                            
            if (CheckIfRowExists(selectedCustomerToDelete.CustomerId.ToString(), "Punkty") == true)
            {
                DeleteRowInTable("Punkty", selectedCustomerToDelete.CustomerId.ToString(), "Id_klienta");

            }
            if (CheckIfRowExists(selectedCustomerToDelete.CustomerId.ToString(), "Punkty_Profil") == true)
            {
                DeleteRowInTable("Punkty_Profil", selectedCustomerToDelete.CustomerId.ToString(), "Id_klienta");
            }
            //Usuniecie klienta z głównej tabeli
            DeleteRowInTable("Klienci", selectedCustomerToDelete.CustomerId.ToString(), "id");
            customersBase.Remove(selectedCustomerToDelete);
          
        }

        /// <summary>
        /// Manages deleting row in a table in database specified by parameters
        /// </summary>
        /// <param name="tableName"> Name of the table which row will be removed</param>
        /// <param name="id">Uniqe Id of row to delete</param>
        /// <param name="column">Specified colum on which where statement will distinguish row to delete (usually ID)</param>
        private void DeleteRowInTable(string tableName, string id, string column)
        {
            try
            {
                MySqlConnection connection = new MySqlConnection(Utilities.connectionString);
                string querry = "DELETE FROM " + tableName + " WHERE " + column + " = " + id;
                MySqlCommand command = new MySqlCommand(querry, connection);

                connection.Open();
                command.ExecuteReader();
                connection.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Bład usuwania rekordu w tabeli: " + tableName + " ." + ex.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        /// <summary>
        /// Checking if specified row at specified id in specified table exists
        /// </summary>
        /// <param name="klientId"> Id of the customer in table</param>
        /// <param name="tableName">Name of the table in which presence of specified record is checked</param>
        /// <returns></returns>
        private bool CheckIfRowExists(string klientId, string tableName)
        {
            MySqlConnection connection = new MySqlConnection(Utilities.connectionString);
            string querry = "Select count(Id) FROM " + tableName + " where Id_klienta = " + klientId;
            MySqlCommand command = new MySqlCommand(querry, connection);
            int result = 0;

            connection.Open();

            MySqlDataReader data = command.ExecuteReader();
            while (data.Read())
            {
                result = data.GetInt32(0);
            }
            connection.Close();

            return (result >= 1);

        }
        /// <summary>
        /// Extracting front photo of customer by its record ID
        /// </summary>
        /// <param name="id">Id of the customer</param>
        /// <returns>Picture of the selected customer</returns>
        public byte[] GetCustomerFrontImageById(int id)
        {
            if (customersBase == null)
                LoadCustomersFromDatabase();
            return customersBase.Where(c => c.CustomerId == id).FirstOrDefault().FrontImage;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public byte[] GetCustomerProfileImageById(int id)
        {
            if (customersBase == null)
                LoadCustomersFromDatabase();
            return customersBase.Where(c => c.CustomerId == id).FirstOrDefault().ProfileImage;

        }


        /// <summary>
        /// Loading customerst data from main Customer table in database
        /// </summary>
        public void LoadCustomersFromDatabase()
        {

            ObservableCollection<Customer> Customers = new ObservableCollection<Customer>();
            //Pobieranie danych z bazy
            string querry = "Select * from Klienci";
            MySqlConnection connection = new MySqlConnection(Utilities.connectionString);
            //Odczytanie wynikow zapytania
            MySqlDataReader results = null;

            try
            {
                connection.Open();
                //Creating connection to database
                MySqlCommand command = new MySqlCommand(querry, connection);
                results = command.ExecuteReader();

                while(results.Read())
                {
                    Customer customer = new Customer();
                    customer.CustomerId = (int)results["id"];
                    customer.CustomerName = results["imie"].ToString();
                    customer.CustomerSurname = results["nazwisko"].ToString();
                    customer.CustomerEmail = results["email"].ToString();
                    customer.PupilDistance = (double)results["Rozstaw_Zrenic"];
                    customer.FaceWidth = (double)results["Szerokosc_Twarzy"];
                    customer.TempleWidth = (double)results["Szerokosc_Skroni"];
                    customer.RightEyeNoseDistance = (double)results["PraweOko_Nos"];
                    customer.LeftEyeNoseDistance = (double)results["LeweOko_Nos"];
                    customer.ProfileNoseEarDistance = (double)results["Ucho_Nos"];
                    customer.ProfileNoseEyeDistance = (double)results["Oko_Nos"];               
                    customer.Approved = (bool)results["zatwierdzone"];                  
                    customer.FrontImage =  results["zdjecie"] as byte[];                    
                    customer.ProfileImage = results["zdjecie_profil"] as byte[];
                    Customers.Add(customer);
                }


            }
            catch(MySqlException ex)
            {
                MessageBox.Show("Błąd odczytu z bazy danych. Proszę sprawdzić ustawienia połączenia. " + ex.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            finally
            {
                connection.Close();
                customersBase = Customers;
            }
           
       
        }

    }
}
