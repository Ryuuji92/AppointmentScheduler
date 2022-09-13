using System;
using System.Collections.Generic;
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
    /// Interaction logic for IndividualCustomer.xaml
    /// </summary>
    public partial class IndividualCustomer : Window
    {
        private Customer customer;
        public IndividualCustomer(int customerID)
        {
            InitializeComponent();
            Connection connection = new Connection();
            //get the customer from the database by provided customer ID number
            customer = connection.GetCustomer(customerID);

            //set the datacontext for the view to customer
            DataContext = customer;
        }

        //open a customer edit form for the current customer if requested by user
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            EditCustomer editCustomer = new EditCustomer(customer);
            editCustomer.ShowDialog();
        }

        //close the window
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
