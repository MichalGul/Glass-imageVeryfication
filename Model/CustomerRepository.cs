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
    class CustomerRepository
    {
        public static ObservableCollection<Customer> customersBase;


        public ObservableCollection<Customer> GetCustomers()
        {
            if (customersBase == null)
                LoadCustomersFromDatabase();
            return customersBase;

        }

      //TODO: w okienku edycji klienta trzeba bedzie dodac kod ktory selected Customera zmienia
        public void UpdateCustomer(Customer selectedCustomer)
        {
            Customer customerToUpdate = customersBase.Where(C => C.CustomerId == selectedCustomer.CustomerId).FirstOrDefault();
            customerToUpdate = selectedCustomer;

            try
            {

                MySqlConnection connection = new MySqlConnection(Utilities.connectionString);
                string updateQuerry = "Update Klienci SET imie=@param2, nazwisko=@param3, email=@param4, Rozstaw_Zrenic=@param5, Szerokosc_Twarzy=@param6, Szerokosc_Skroni=@param7, PraweOko_Nos=@param8, LeweOko_Nos=@param9, Ucho_Nos=@param10, Oko_Nos =@param11,  zatwierdzone=@param12, zdjecie=@param13, zdjecie_profil=@param14 where id = " + Utilities.currentID;
                connection.Open();
                MySqlCommand prpCommand = new MySqlCommand(updateQuerry, connection);
                if (prpCommand == null)
                {
                    MessageBox.Show("Połączenie nieudane. Sprawdź ustawienia połączenia");
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

                MessageBox.Show("Błąd wprowadzania wartośc lub błąd połączenia. Dane liczbowe prosze wprowadzać przy pomocy przecinka.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;

            }






        }



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
                //Stworzenie polecenia do bazy danych
                MySqlCommand command = new MySqlCommand(querry, connection);
                results = command.ExecuteReader();

                while(results.Read())
                {
                    Customer customer = new Customer();
                    customer.CustomerId = (int)results["id"];
                    customer.CustomerName = results["imie"].ToString();
                    customer.CustomerSurname = results["nazwisko"].ToString();
                    customer.PupilDistance = (double)results["Rozstaw_Zrenic"];
                    customer.FaceWidth = (double)results["Szerokosc_Twarzy"];
                    customer.TempleWidth = (double)results["Szerokosc_Skroni"];
                    customer.RightEyeNoseDistance = (double)results["PraweOko_Nos"];
                    customer.LeftEyeNoseDistance = (double)results["LeweOko_Nos"];
                    customer.ProfileNoseEarDistance = (double)results["Ucho_Nos"];
                    customer.ProfileNoseEyeDistance = (double)results["Oko_Nos"];
                    customer.Approved = (bool)results["zatwierdzone"];
                    customer.FrontImage = (byte[]) results["zdjecie"];                    
                    customer.ProfileImage = results["zdjecie_profil"] as byte[];
                    Customers.Add(customer);
                }


            }
            catch(MySqlException ex)
            {
                MessageBox.Show("Błąd odczytu z bazy danych. Proszę sprawdzić ustawienia połączenia. " + ex.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                throw ex;
            }
            finally
            {
                connection.Close();
                customersBase = Customers;
            }
           
       
        }

    }
}
