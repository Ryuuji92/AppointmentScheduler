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
        private ObservableCollection<Customer> Customers;
        public ViewCustomers()
        {
            InitializeComponent();
            Connection connection = new Connection();

            Customers = new ObservableCollection<Customer>();
            foreach (Customer customer in connection.GetCustomers())
            {
                Customers.Add(customer);
            }

            DataContext = Customers;
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            EditCustomer editCustomer = new EditCustomer((Customer)CustomersGrid.SelectedValue);
            editCustomer.ShowDialog();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            Customer customer = CustomersGrid.SelectedValue as Customer;
            Connection connection = new Connection();
            connection.DeleteCustomer(customer.CustomerID);
            Customers.Remove(customer);
        }
    }
}
