using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        //create a static observablecollection of appointments to both populate the appointment view, but also check for which appointments need reminders.
        private static ObservableCollection<Appointment> Appointments;
        //current user will be referenced several times throughout the application by different classes.  Resource needs to remain static during program opperation.
        public static User CurrentUser;

        //logic for reminder timer
        private static ObservableCollection<Appointment> reminderSent;
        private const double fiveMinuteInterval = 5 * 1000;
        private static System.Timers.Timer checkForTime = new System.Timers.Timer(fiveMinuteInterval);
        
        //keep track of the calendar viewing pattern (by week or month)
        private bool viewingByMonth;

        public CalendarView(User currentuser)
        {
            InitializeComponent();

            //set current user
            CurrentUser = currentuser;
            //set the view to today by default on log in
            SelectedMonth.SelectedDate = DateTime.Now;

            //get the appointments for the current month
            Appointments = new ObservableCollection<Appointment>(getAppointmentsForMonth());
            //populate the calendarview with data from the appointments list
            Calendar.ItemsSource = Appointments;

            //keep track of which appointments have had a reminder sent so only one reminder is sent per appointment
            reminderSent = new ObservableCollection<Appointment>();
            //initialize the events for the reminder timer
            checkForTime.Elapsed += new ElapsedEventHandler(checkForTime_Elapsed);
            checkForTime.Enabled = true;
        }


        /*Calendar view logic*/
        //update the appointments collection, called when a view change is requested by user
        private void updateAppointments()
        {
            //do not perform any opperations on an empty collection
            if (Appointments != null)
            {
                //if the current view is by week then set the view to month and retrieve all the appointments for the selected month
                if (!viewingByMonth)
                {
                    Appointments.Clear();
                    foreach (Appointment a in getAppointmentsForMonth())
                    {
                        Appointments.Add(a);
                    }
                }
                //if the current view is by month then set the view to week and retrieve all the appointments for the selected week
                else
                {
                    Appointments.Clear();
                    foreach (Appointment a in getAppointmentsForWeek())
                    {
                        Appointments.Add(a);
                    }
                }
            }
        }

        //get the appointments for the selected month
        private List<Appointment> getAppointmentsForMonth()
        {
            List<Appointment> appointments = new List<Appointment>();
            Connection connection = new Connection();

            appointments = connection.GetAppointments(CurrentUser.ID);

            //create a date period representing the current month
            DatePeriod datePeriod = new DatePeriod(new DateTime(SelectedMonth.SelectedDate.Value.Year, SelectedMonth.SelectedDate.Value.Month, 1), new DateTime(SelectedMonth.SelectedDate.Value.Year, SelectedMonth.SelectedDate.Value.Month, DateTime.DaysInMonth(SelectedMonth.SelectedDate.Value.Year, SelectedMonth.SelectedDate.Value.Month)));
            //if the appointment is not inside the current month then remove it from the appointment collection.  List cannot be modified at this point so create a copy list to itterate through.
            foreach (Appointment appointment in appointments.ToList())
            {
                if (!datePeriod.Contains(appointment.Start.Date))
                {
                    //using a lamba here enables selecting the object in appointments to be removed which matches the data in the current object copy
                    appointments.Remove(appointments.FirstOrDefault(x => x.ID == appointment.ID));
                }
            }
            //order appointments by the time that they start. The lambda here provides an exceletnt short hand to save on the amount of code needed and to make code more readable.
            appointments.OrderBy(x => x.Start);

            //track that the view is by month
            viewingByMonth = true;
            //set the view change button to display view by week
            CalendarViewBtn.Content = "View by Week";
            //set the heading for the calendar view to display the month and year being viewed
            PageHeading.Text = SelectedMonth.SelectedDate.Value.ToString("Y");

            return appointments;
        }

        //get the appointments for the selected week
        private List<Appointment> getAppointmentsForWeek()
        {
            List<Appointment> appointments = new List<Appointment>();

            Connection connection = new Connection();

            appointments = connection.GetAppointments(CurrentUser.ID);

            //create a date period representing the selected week
            DatePeriod datePeriod = new DatePeriod(SelectedMonth.SelectedDate.Value.StartOfWeek().Date, SelectedMonth.SelectedDate.Value.EndOfWeek().Date);
            //if an appointment is not within the selected week remove it.  Cannot itterate through the appointment list so create a copy of it to itterate through
            foreach (Appointment appointment in appointments.ToList())
            {
                if(!datePeriod.Contains(appointment.Start))
                {
                    //using a lamba here enables selecting the object in appointments to be removed which matches the data in the current object copy
                    appointments.Remove(appointments.FirstOrDefault(x => x.ID == appointment.ID));
                }
            }
            //order appointments by the time that they start. The lambda here provides an exceletnt short hand to save on the amount of code needed and to make code more readable.
            appointments.OrderBy(x => x.Start);

            //track that the view is by week
            viewingByMonth = false;
            // set the view change button to view by month
            CalendarViewBtn.Content = "View by Month";
            //set the calendar header to week of selected week
            PageHeading.Text = "Week of " + SelectedMonth.SelectedDate.Value.ToString("M");

            return appointments;
        }

        //change the calendar view to reflect the selected date if user wants to view a different time period
        private void SelectedMonth_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!viewingByMonth)
                viewingByMonth = true;
            else
                viewingByMonth = false;
            updateAppointments();
        }

        //call the updateAppointments method.  The updateAppointments method handles view change
        private void CalendarViewBtn_Click(object sender, RoutedEventArgs e)
        {
            updateAppointments();
        }


        /*Reminder logic*/
        //Check to see if there are any appointments within the next 15 minutes and remind the user if so.
        private static void checkForTime_Elapsed(object sender, ElapsedEventArgs e)
        {
            TimeSpan timeSpan = new TimeSpan(0, 15, 0);
            //create a time window of 15 minutes
            DatePeriod datePeriod = new DatePeriod(DateTime.Now, DateTime.Now.Add(timeSpan));
            //check to see if any appointments are within the 15 minute window
            foreach (Appointment appointment in Appointments)
            {
                if (datePeriod.Contains(appointment.Start) && !reminderSent.Contains(appointment))
                {
                    //add the appointment to the reminded list so that a duplicate reminder isn't sent
                    reminderSent.Add(appointment);
                    //display a messagebox with the appointment info
                    MessageBox.Show("Upcoming appointment:\n" + appointment.Title + "\nat: " + appointment.Start.ToShortTimeString(), "Reminder");

                }
            }
        }


        /*Form buttons logic*/
        //create a customer window and display
        private void CustomerBtn_Click(object sender, RoutedEventArgs e)
        {
            ViewCustomers viewCustomers = new ViewCustomers();
            viewCustomers.ShowDialog();
            //prevent the selected view from changing
            if (!viewingByMonth)
                viewingByMonth = true;
            else
                viewingByMonth = false;
            updateAppointments();
        }

        //create a reports window and display
        private void ReportsBtn_Click(object sender, RoutedEventArgs e)
        {
            Reports reprots = new Reports();
            reprots.ShowDialog();
        }

        //create an appointment view window for the selected appointment and display
        private void ViewAppointmentBtn_Click(object sender, RoutedEventArgs e)
        {
            ViewAppointment viewAppointment = new ViewAppointment(Calendar.SelectedValue as Appointment);
            viewAppointment.ShowDialog();
            //prevent changing the selected view mode
            if (!viewingByMonth)
                viewingByMonth = true;
            else
                viewingByMonth = false;
            updateAppointments();
        }

        //create an edit appointment window for a new appointment and display
        private void AddAppointmentBtn_Click(object sender, RoutedEventArgs e)
        {
            EditAppointment editAppointment = new EditAppointment();
            editAppointment.ShowDialog();
            //prevent changing the selected view mode
            if (!viewingByMonth)
                viewingByMonth = true;
            else
                viewingByMonth = false;
            updateAppointments();
        }

        //delete the selected appointment from the database and update the appointments collection
        private void DeleteAppoitnmentBtn_Click(object sender, RoutedEventArgs e)
        {
            Appointment appointment = (Appointment)Calendar.SelectedValue;
            Connection connection = new Connection();
            connection.DeleteAppointment(appointment.ID);
            //prevent changing the selected view mode
            if (!viewingByMonth)
                viewingByMonth = true;
            else
                viewingByMonth = false;
            updateAppointments();
        }
    }
}
