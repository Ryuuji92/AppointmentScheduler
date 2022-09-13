using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BOB3
{
    /// <summary>
    /// Interaction logic for EditCustomer.xaml
    /// </summary>
    public partial class EditCustomer : Window
    {
        private Customer customer;
        private List<City> Cities;
        private bool isNewRecord;
        public EditCustomer()
        {
            InitializeComponent();
            Header.Text = "New Customer";

            Connection connection = new Connection();
            Cities = connection.GetCities();
            SelectCity.ItemsSource = Cities;

            customer = new Customer();
            DataContext = customer;

            isNewRecord = true;
        }
        public EditCustomer(Customer sentCustomer)
        {
            InitializeComponent();
            Header.Text = "Edit Customer";
            sentCustomer.PhoneNumber = new String(sentCustomer.PhoneNumber.Where(Char.IsDigit).ToArray());
            customer = sentCustomer;
            DataContext = customer;

            Connection connection = new Connection();
            Cities = connection.GetCities();
            SelectCity.ItemsSource = Cities;
            SelectCity.SelectedIndex = Cities.IndexOf(Cities.FirstOrDefault(x => x.CityID == sentCustomer.CityID));

            isNewRecord = false;
        }

        //prevent anything other than numbers being entered into the textbox
        private void Number_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        //prevent anything other than normal address related characters from being entered into the textbox
        private void Address_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[^\.0-9a-zA-Z ]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        //save the customer data to the database
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            //validate if the form has all required information
            if (validate())
            {
                //format the phonenumber for a 7 digit phone number
                if (customer.PhoneNumber.Length == 7)
                {
                    char[] phone = customer.PhoneNumber.ToCharArray();
                    customer.PhoneNumber = phone[0] + phone[1] + phone[2] + "-" + phone[3] + phone[4] + phone[5] + phone[6];
                }
                //format the phone number for a 10 digit phone number
                else if (customer.PhoneNumber.Length == 10)
                {
                    char[] phone = customer.PhoneNumber.ToCharArray();
                    customer.PhoneNumber = "(" + phone[0] + phone[1] + phone[2] + ")" + phone[3] + phone[4] + phone[5] + "-" + phone[6] + phone[7] + phone[8] + phone[9];
                }

                //set the customer city information
                City selectedcity = (City)SelectCity.SelectedItem;
                customer.CityID = selectedcity.CityID;
                customer.CityName = selectedcity.Name;

                //if there is no address 2 information than set the customer address 2 to an empty string
                if (customer.AddressTwo == null)
                    customer.AddressTwo = "";
                
                Connection connection = new Connection();

                //create a new database customer record if new
                if(isNewRecord)
                {
                    connection.InsertCustomer(customer);
                }

                //update existing customer record in database if editing
                else
                {
                    connection.UpdateCustomer(customer);
                }

                //close the window after operations have completed successfully
                this.Close();
            }
        }

        //close the window without saving
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        //validate form information
        private bool validate()
        {
            bool isValid = true;
            string message = "Please correct the following:\n\n";

            if (customer.CustomerName == null || customer.CustomerName.Length <= 1)
            {
                message += "Please enter a customer name\n";
                isValid = false;
            }
            if (customer.Address == null || customer.Address.Length <= 0)
            {
                message += "Please enter a valid addredss";
                isValid = false;
            }
            if (customer.PhoneNumber.Length != 7)
            {
                if (customer.PhoneNumber.Length != 10)
                {
                    message += "Please enter a valid 7 or 10 digit phone number\n";
                    isValid = false;
                }
            }
            if (SelectCity.SelectedItem == null)
            {
                message += "Please selecte a city\n";
                isValid = false;
            }
            if (PostalCodeField.Text.Length != 5)
            {
                message += "Please enter a 5 digit postal code\n";
                isValid = false;
            }
            if(!isValid)
            {
                MessageBox.Show(message, "Attention");
            }
            return isValid;
        }

    }
}
