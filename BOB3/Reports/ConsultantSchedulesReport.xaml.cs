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
    /// Interaction logic for ConsultantSchedulesReport.xaml
    /// </summary>
    public partial class ConsultantSchedulesReport : Page
    {
        ObservableCollection<User> Users;
        public ConsultantSchedulesReport(DateTime selectedDate)
        {
            InitializeComponent();

            Users = new ObservableCollection<User>();

            DatePeriod datePeriod = new DatePeriod(new DateTime(selectedDate.Year, selectedDate.Month, 1), new DateTime(selectedDate.Year, selectedDate.Month, DateTime.DaysInMonth(selectedDate.Year, selectedDate.Month)));
            getAppointmentsIntoUsers(datePeriod);

            foreach (User user in Users)
            {
                StackPanel stackPanel = new StackPanel();
                stackPanel.Height = 450;
                stackPanel.Width = 200;
                stackPanel.Orientation = Orientation.Vertical;
                TextBlock header = new TextBlock();
                header.FontSize = 15;
                header.FontWeight = FontWeights.Bold;
                header.Text = user.UserName;
                stackPanel.Children.Add(header);
                ListBox listBox = new ListBox();
                foreach (Appointment a in user.Appointments)
                {
                    TextBlock textBlock = new TextBlock();
                    textBlock.Text = a.Title + "\n" + a.Start.ToString("g") + " - " + a.End.ToString("t");
                    listBox.Items.Add(textBlock);
                }
                stackPanel.Children.Add(listBox);
                ReportView.Children.Add(stackPanel);
            }
        }

        private void getAppointmentsIntoUsers(DatePeriod selectedDatePeriod)
        {
            Connection connection = new Connection();
            List<Appointment> appointments = connection.GetAppointments();

            foreach (Appointment a in appointments)
            {
                if (Users.Contains(Users.FirstOrDefault(x => x.ID == a.UserID)))
                {
                    Users.First(x => x.ID == a.UserID).Appointments.Add(a);
                }
                else
                {
                    User user = new User();
                    user.ID = a.UserID;
                    user.UserName = a.UserName;
                    user.Appointments = new List<Appointment>();
                    user.Appointments.Add(a);
                    Users.Add(user);
                }
            }
        }
    }
}
