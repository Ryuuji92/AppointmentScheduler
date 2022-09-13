using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.ObjectModel;
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
    /// Interaction logic for EditAppointment.xaml
    /// </summary>
    public partial class EditAppointment : Window
    {
        //track if this is a new appointment or an existing one
        private bool isNewAppointment;
        //class for the datacontext of this window
        private Appointment appointment;

        //create a collection for selectable customers from existing customers in db
        private ObservableCollection<Customer> Customers;
        //create collections for available start times and available end times
        private ObservableCollection<DateTime> availableStart;
        private ObservableCollection<DateTime> availableEnd;


        //constructor for creating new appointments
        public EditAppointment()
        {
            InitializeComponent();
            isNewAppointment = true;
            pageTitle.Text = "New Appointment";

            //create a new appointment 
            appointment = new Appointment();
            //set defualt date to today
            appointment.Start = DateTime.Now;

            //set the datacontext to the new appointment
            DataContext = appointment;

            //set default start date selection to today
            StartSelect.SelectedDate = DateTime.Now;

            //populate collections
            getAppointments();
            getAvailableTimes();
            getCustomers();
        }

        //constructor for editing existing appointments
        public EditAppointment(Appointment inputAppointment)
        {
            InitializeComponent();
            isNewAppointment = false;

            //set the appoinment for this winow to the provided appointment and set the datacontext
            appointment = inputAppointment;
            DataContext = appointment;

            //get the customers from the database and set the selection to the customer currenty associated with the appointment
            getCustomers();
            CustomerSelector.SelectedIndex = Customers.IndexOf(Customers.FirstOrDefault(x => x.CustomerID == inputAppointment.CustomerID));

            //set the start date selection to the one currently associated with the appointment
            StartSelect.SelectedDate = inputAppointment.Start;

            //get the available start times and set the selected time to the one currently associated with the appointment
            getAvailableStartTimes();
            StartTime.SelectedIndex = availableStart.IndexOf(availableStart.FirstOrDefault(x => x.ToString("t") == appointment.Start.ToString("t")));

            //get the available end times and set the selected time to the one currently associated with the appointment
            getAvailableEndTimes();
            EndTime.SelectedIndex = availableEnd.IndexOf(availableEnd.FirstOrDefault(x => x.ToString("t") == appointment.End.ToString("t")));
        }

        //get the customers from the database
        private void getCustomers()
        {
            Connection conn = new Connection();
            Customers = new ObservableCollection<Customer>(conn.GetCustomers());
            CustomerSelector.ItemsSource = Customers;
        }

        //get the current appointments from the database to prevent scheduling overlapping appointments
        private List<Appointment> getAppointments()
        {
            Connection connection = new Connection();
            List<Appointment> appointments = connection.GetAppointments(CalendarView.CurrentUser.ID);

            return appointments;
        }

        //get the available start and end times
        private void getAvailableTimes()
        {
            getAvailableStartTimes();
            getAvailableEndTimes();
        }

        //get the available start times
        private void getAvailableStartTimes()
        {
            //get the used times for existing appointments
            List<DatePeriod> usedTimes = getUsedTimes();
            //initialize the availableStart collection
            availableStart = new ObservableCollection<DateTime>();

            //set open time to 8am
            DateTime bussinessOpenHours = new DateTime(StartSelect.SelectedDate.Value.Year, StartSelect.SelectedDate.Value.Month, StartSelect.SelectedDate.Value.Day, 8, 0, 0);

            //create bussiness hours availability for appointment start times every half hour.
            for (int i = 0; i < 18; i++)
            {
                DateTime timeSlot = new DateTime();
                TimeSpan timeIntv = new TimeSpan(0, 30 * i, 0);
                timeSlot = bussinessOpenHours.Add(timeIntv);

                //using a lambda here reduces the amount of code needed to sort through the bussinessOpenHours list. Only add available times where appointments don't exist
                if (!usedTimes.Where(x => x.Contains(timeSlot)).Any())
                {
                    availableStart.Add(timeSlot);
                }
            }

            //set the items source for the start time combobox to the available start times.
            StartTime.ItemsSource = availableStart;
        }

        //get the available end times
        private void getAvailableEndTimes()
        { 
            List<DatePeriod> usedTimes = getUsedTimes();

            //initialize the availableEnd collection
            availableEnd = new ObservableCollection<DateTime>();

            //set the window for the current appointment if editing an appointment
            DatePeriod currentAppointment = new DatePeriod(appointment.Start, appointment.End);

            //set business open hours to 8am. Appointments can't end during the first block of the day so create a half hour delay to the first available end time.
            DateTime bussinessOpenHours = new DateTime(StartSelect.SelectedDate.Value.Year, StartSelect.SelectedDate.Value.Month, StartSelect.SelectedDate.Value.Day, 8, 30, 0);

            //create bussiness hours availability for appointment end times every half hour.
            for (int i = 0; i < 18; i++)
            {
                DateTime timeSlot = new DateTime();
                TimeSpan timeIntv = new TimeSpan(0, 30 * i, 0);
                timeSlot = bussinessOpenHours.Add(timeIntv);

                //using a lambda here reduces the amount of code needed to sort through the bussinessOpenHours list. Prevent any available end times to occur before the time of creation.
                if (!usedTimes.Where(x => x.Contains(timeSlot)).Any() || currentAppointment.Contains(timeSlot) && timeSlot > DateTime.Now)
                {
                    availableEnd.Add(timeSlot);
                }
            }

            //if there is a selected start time then only allow end times that occur after the start time and before the next appointment starts
            if (StartTime.SelectedItem != null)
            {
                DateTime starttime = (DateTime)StartTime.SelectedItem;

                foreach (DateTime dt in availableEnd.ToList())
                {
                    //using a lambda here helps to sort through information more easily than writing nested statements
                    if (usedTimes.Where(x => x.IsBefore(starttime)).Any() && !usedTimes.Where(x => x.IsBefore(dt)).Any() || usedTimes.Where(x => x.IsAfter(starttime)).Any() && !usedTimes.Where(x => x.IsAfter(dt)).Any() || dt <= starttime)
                    {
                        //remove the matching end time object from availableEnd times collection
                        availableEnd.Remove(availableEnd.First(x => x == dt));
                    }
                }
            }

            //set the end time selector source to the end times collection
            EndTime.ItemsSource = availableEnd;
        }

        //get the time periods for existing appointments
        private List<DatePeriod> getUsedTimes()
        {
            List<DatePeriod> usedTimes = new List<DatePeriod>();
            List<Appointment> currentAppointments = getAppointments();

            //Using a lambda expression here reduces the amount of code needed to compair the current appointment (a) with the StartDate.SelectedDate property
            foreach (Appointment a in currentAppointments.Where(x => x.Start.Date == appointment.Start.Date))
            {
                if (a.ID != appointment.ID)
                {
                    DatePeriod timePeriod = new DatePeriod(a.Start, a.End);
                    usedTimes.Add(timePeriod);
                }
            }

            return usedTimes;
        }

        /*Selection Change Logic*/
        //get the new start and end times after appointment date has been selected.
        private void StartSelect_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            getAvailableTimes();
        }

        //get the new end times after a start time has been selected.
        private void StartTime_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            getAvailableEndTimes();
        }


        /*Button logic*/
        //save button logic
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            //only save if form meets validation requirements
            if (validate())
            {
                //get the current customer selection and assign it to the appointment
                Customer customer = CustomerSelector.SelectedValue as Customer;
                appointment.CustomerID = customer.CustomerID;
                appointment.CustomerName = customer.CustomerName;

                //get the combined start date and time and add it to the appointment
                DateTime startTime = (DateTime)StartTime.SelectedValue;
                DateTime newStart = new DateTime(appointment.Start.Year, appointment.Start.Month, appointment.Start.Day, startTime.Hour, startTime.Minute, 0);
                //get the selected end time and add it to the appointment
                DateTime endTime = (DateTime)EndTime.SelectedValue;
                DateTime newEnd = new DateTime(appointment.Start.Year, appointment.Start.Month, appointment.Start.Day, endTime.Hour, endTime.Minute, 0);

                appointment.Start = newStart;
                appointment.End = newEnd;

                Connection connection = new Connection();
                //insert the new appointment into the database table if new
                if (isNewAppointment)
                {
                    connection.InsertAppointment(CalendarView.CurrentUser.UserName, CalendarView.CurrentUser.ID, appointment);
                    MessageBox.Show("Appoinment added!", "Success");
                }
                //update the existing appointment in the database table if editing an existing appointment
                else
                {
                    connection.UpdateAppointment(CalendarView.CurrentUser.UserName, appointment); 
                    MessageBox.Show("Appoinment updated!", "Success");
                }

                //close the window after operations have successfully completed.
                this.Close();
            }
        }

        //close the window without saving appointment data
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        //form validation
        private bool validate()
        {
            //check to make sure that all required information is filled out and alert user on missing information if any
            bool isValid = true;
            string message = "Please correct the following before saving:\n\n";
            if (appointment.Title.Length <= 0)
            {
                message += "Please input a title\n";
                isValid = false;
            }
            if (appointment.Description.Length <= 0)
            {
                message += "Please enter a description\n";
                isValid = false;
            }
            if (StartTime.SelectedItem == null)
            {
                message += "Please select a valid start time\n";
                isValid = false;
            }
            if (EndTime.SelectedItem == null)
            {
                message += "Please select a valid end time\n";
                isValid = false;
            }
            if (CustomerSelector.SelectedItem == null)
            {
                message += "Please select a customer\n";
                isValid = false;
            }
            if ((DateTime)StartTime.SelectedValue > (DateTime)EndTime.SelectedValue)
            {
                message += "Appointment end time must be greater than start time\n";
                isValid = false;
            }
            if (appointment.AppointmentType.Length <= 0 || appointment.AppointmentType == null)
            {
                message += "Please enter an appointment type\n";
                isValid = false;
            }
            if (appointment.AppointmentURL == null)
            {
                appointment.AppointmentURL = "";
            }
            if (isValid == false)
            {
                MessageBox.Show(message, "Attention!");
            }

            return isValid;
        }

    }
}
