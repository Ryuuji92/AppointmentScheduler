using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MySql.Data.MySqlClient;
using System.Threading;
using System.IO;
using System.Timers;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string errorMessage;
        private string messageTitle;

        public MainWindow()
        {
            InitializeComponent();

            //get the current culture for user
            var neutralCulture = Thread.CurrentThread.CurrentCulture.Parent.Name;

            //set the error message and login text to English if the user's language is set to English
            if (neutralCulture.Equals("en", StringComparison.OrdinalIgnoreCase))
            {
                errorMessage = "Username and/or password do not match.";
                messageTitle = "Attention";
                UsernameTitle.Text = "Username";
                PasswordTitle.Text = "Password";
            }

            //set the error message and login text to Japanese if the user's language is set to Japanese
            else if (neutralCulture.Equals("ja", StringComparison.OrdinalIgnoreCase))
            {
                errorMessage = "ユーザー名かパスワードが違う。情報を確認ください。";
                messageTitle = "警告！";
                UsernameTitle.Text = "ユーザー名";
                PasswordTitle.Text = "パスワード";
                MessageBox.Show("日本語です");
            }
        }


        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            Connection conn = new Connection();   

            //try to match the login information with the users in the database
            if (conn.IsValidLogin(UsernameField.Text, PasswordField.Password))
            {
                User currentUser = conn.Login(UsernameField.Text);
                string logFileName = @"C:\Users\LabUser\Documents\Userlog.Txt";

                try
                {
                    //if the log file doesn't already exist, create one and write to it.
                    if (!File.Exists(logFileName))
                    {
                        //create a filestream, create a new file, write new data to the new file
                        using (FileStream fileStream = File.Create(logFileName))
                        {
                            Byte[] title = new UTF8Encoding(true).GetBytes("User Activity\n\n");
                            fileStream.Write(title, 0, title.Length);
                            byte[] user = new UTF8Encoding(true).GetBytes(currentUser.UserName);
                            fileStream.Write(user, 0, user.Length);
                            byte[] timeStamp = new UTF8Encoding(true).GetBytes(DateTime.Now.ToString());
                            fileStream.Write(timeStamp, 0, timeStamp.Length);
                        }
                    }
                    //write to end of the file if one exists.
                    else
                    {
                        File.AppendAllText(logFileName, currentUser.UserName + DateTime.Now.ToString() + "\n\n");
                    }
                }

                //if there is an issue writing the file inform the user
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                //create a new calendar view page for the current user and display it in this (main) window
                CalendarView calendarView = new CalendarView(currentUser);
                this.Content = calendarView;

            }
            //if the login information doesn't exist in the database then let the user know that the login credentials are invalid and clear the password field
            else
            {
                MessageBox.Show(errorMessage, messageTitle);
                PasswordField.Password = "";

            }
        }
    }
}

