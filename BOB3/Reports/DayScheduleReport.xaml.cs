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
    /// Interaction logic for DayScheduleReport.xaml
    /// </summary>
    public partial class DayScheduleReport : Page
    {
        private ObservableCollection<Appointment> Appointments;
        public DayScheduleReport(DateTime selectedDay)
        {
            InitializeComponent();

            Appointments = new ObservableCollection<Appointment>(getAppointments(selectedDay));             

            ReportGrid.ItemsSource = Appointments;
        }

        private List<Appointment> getAppointments(DateTime dateTime)
        {
            Connection connection = new Connection();

            List<Appointment> appointments = new List<Appointment>(connection.GetAppointments());

            foreach (Appointment a in appointments.ToList())
            {
                if (a.Start.Date != dateTime.Date)
                    appointments.Remove(appointments.FirstOrDefault(x => x.ID == a.ID));
            }

            return appointments;
        }
    }
}
