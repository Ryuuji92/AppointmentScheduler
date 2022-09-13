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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BOB3
{
    /// <summary>
    /// Interaction logic for ViewCustomer.xaml
    /// </summary>
    public partial class ViewCustomers : Window
    {
        //create a collection of customers
        private ObservableCollection<Customer> Customers;
        public ViewCustomers()
        {
            InitializeComponent();

            //initialize the customer collection
            Customers = new ObservableCollection<Customer>();
            getCustomers();

            DataContext = Customers;
        }

        //get the customers from the database
        private void getCustomers()
        {
            //clear the customer collection if any exist
            if (Customers.Any())
                Customers.Clear();
            Connection connection = new Connection();
            //add all the customers from the database
            foreach (Customer customer in connection.GetCustomers())
            {
                Customers.Add(customer);
            }
        }

        //edit the selected customer's data
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            EditCustomer editCustomer = new EditCustomer((Customer)CustomersGrid.SelectedValue);
            editCustomer.ShowDialog();
            getCustomers();
        }

        //add a new customer
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            EditCustomer editCustomer = new EditCustomer();
            editCustomer.ShowDialog();
            getCustomers();
        }

        //delete the selected customer
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            Customer customer = CustomersGrid.SelectedValue as Customer;
            Connection connection = new Connection();
            connection.DeleteCustomer(customer.CustomerID);
            getCustomers();
        }
    }
}
