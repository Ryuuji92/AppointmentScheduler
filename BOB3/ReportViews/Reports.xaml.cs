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
using System.Windows.Shapes;

namespace BOB3
{
    /// <summary>
    /// Interaction logic for Reports.xaml
    /// </summary>
    public partial class Reports : Window
    {
        public Reports()
        {
            InitializeComponent();

            DateSelector.SelectedDate = DateTime.Now;
        }

        //create the appointments by type in a month report page and display it in the current window
        private void AppointmentTypesBtn_Click(object sender, RoutedEventArgs e)
        {
            AppTypesMonthReport appTypesMonthReport = new AppTypesMonthReport(DateSelector.SelectedDate.Value);
            this.Title = "Number of Appointment Types by Month";
            this.Content = appTypesMonthReport;
        }

        //create the consultant shcedule report page and display it in the current window
        private void ConsultantSchedulesBtn_Click(object sender, RoutedEventArgs e)
        {
            ConsultantSchedulesReport consultantSchedulesReport = new ConsultantSchedulesReport(DateSelector.SelectedDate.Value);
            this.Title = "Consultant Schedule";
            this.Content = consultantSchedulesReport;
        }

        //create the appointments by day report page and display it in the current window
        private void AppointmentsByDayBtn_Click(object sender, RoutedEventArgs e)
        {
            DayScheduleReport dayScheduleReport = new DayScheduleReport(DateSelector.SelectedDate.Value);
            this.Title = "Appointments for " + DateSelector.SelectedDate.Value.ToString("M");
            this.Content = dayScheduleReport;
        }
    }
}
