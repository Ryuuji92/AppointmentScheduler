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
    /// Interaction logic for ViewAppointment.xaml
    /// </summary>
    public partial class ViewAppointment : Window
    {
        private Appointment currentAppointment;
        public ViewAppointment(Appointment appointment)
        {
            InitializeComponent();
            currentAppointment = new Appointment();
            currentAppointment = appointment;
            DataContext = currentAppointment;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            Connection connection = new Connection();
            connection.DeleteAppointment(currentAppointment.ID);
            this.Close();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            EditAppointment editAppointment = new EditAppointment(currentAppointment);
            editAppointment.ShowDialog();
            this.Close();
        }

        //open a new customer window for the appointment's customer
        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            IndividualCustomer individualCustomer = new IndividualCustomer(currentAppointment.CustomerID);
            individualCustomer.ShowDialog();
        }
    }
}
