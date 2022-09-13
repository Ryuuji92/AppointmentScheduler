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
    /// Interaction logic for AppTypesMonthReport.xaml
    /// </summary>
    public partial class AppTypesMonthReport : Page
    {
        List<AppointmentTypeCount> appointmentTypes;
        public AppTypesMonthReport(DateTime selectedDate)
        {
            InitializeComponent();

            ReportHeader.Text = selectedDate.ToString("Y");

            appointmentTypes = new List<AppointmentTypeCount>();
            DatePeriod datePeriod = new DatePeriod(new DateTime(selectedDate.Year, selectedDate.Month, 1), new DateTime(selectedDate.Year, selectedDate.Month, DateTime.DaysInMonth(selectedDate.Year, selectedDate.Month)));
            getAppointmentTypes(datePeriod);

            ReprotView.ItemsSource = appointmentTypes;
        }


        private void getAppointmentTypes(DatePeriod selectedDatePeriod)
        {
            Connection connection = new Connection();
            List<Appointment> appointments = connection.GetAppointments();

            foreach (Appointment a in appointments)
            {
                //if the appointment type doesn't exist in the current appointemntTypes list then add it
                if (!appointmentTypes.Contains(appointmentTypes.FirstOrDefault(x => x.TypeName == a.AppointmentType)))
                {
                    AppointmentTypeCount appointmentTypeCount = new AppointmentTypeCount();
                    appointmentTypeCount.TypeName = a.AppointmentType;
                    appointmentTypeCount.TypeCount = 1;
                    appointmentTypes.Add(appointmentTypeCount);
                }
                //if the appointment type already exists in the current appointmentTypes list then incriment it's count
                else
                {
                    AppointmentTypeCount appType = appointmentTypes.Select(x => x.TypeName == a.AppointmentType) as AppointmentTypeCount;
                    appType.TypeCount++;
                }
            }
        }
    }

    //create a custom class to hold appointment types and their count
    class AppointmentTypeCount
    {
        public string TypeName { get; set; }
        public int TypeCount { get; set; }
    }
}
