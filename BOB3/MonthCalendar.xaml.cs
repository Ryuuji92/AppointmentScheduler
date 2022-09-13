using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Timers;
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
    /// Interaction logic for MonthCalendar.xaml
    /// </summary>
    public partial class CalendarView : Page
    {
        public static List<Appointment> Appointments;
        public static List<Customer> Customers = new List<Customer>();
        public static User CurrentUser;

        private const double fiveMinuteInterval = 5 * 1000;
        private static System.Timers.Timer checkForTime = new System.Timers.Timer(fiveMinuteInterval);

        public CalendarView(User currentuser)
        {
            InitializeComponent();

            Connection connection = new Connection();
            CurrentUser = currentuser;
            SelectedMonth.SelectedDate = DateTime.Now;

            Customers = connection.GetCustomers();  
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Connection connection = new Connection();
            Appointments = connection.GetAppointments(/*CurrentUser.ID*/);

            DatePeriod datePeriod = new DatePeriod(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.AddMonths(1).AddDays(-1).Day));
            //foreach(Appointment appointment in appointments)
            //{
            //    if(!datePeriod.Contains(appointment.Start))
            //    {
            //        appointments.Remove(appointment);
            //    }
            //}
            //appointments.OrderBy(x => x.Start);


            checkForTime.Elapsed += new ElapsedEventHandler(checkForTime_Elapsed);
            checkForTime.Enabled = true;
        }

        private static void checkForTime_Elapsed(object sender, ElapsedEventArgs e)
        {
            Reminder();
        }

        private static void Reminder()
        {
            TimeSpan timeSpan = new TimeSpan(0, 15, 0);
            foreach (Appointment appointment in Appointments)
            {
                if (DateTime.Now + timeSpan <= appointment.Start)
                {
                    MessageBox.Show("Upcoming appointment:\n" + appointment.Title + "\nat: " + appointment.Start.ToShortTimeString(), "Reminder");
                }
            }
        }

        private void SelectedMonth_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void CalendarViewBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CustomerBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ReportsBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ViewAppointmentBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddAppointmentBtn_Click(object sender, RoutedEventArgs e)
        {
            EditAppointment editAppointment = new EditAppointment(Customers);
            AppointmentWindow appointmentWindow = new AppointmentWindow();
            appointmentWindow.Content = editAppointment;
            appointmentWindow.ShowDialog();
        }

        private void DeleteAppoitnmentBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
