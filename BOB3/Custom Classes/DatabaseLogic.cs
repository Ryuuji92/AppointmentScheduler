using System;
using System.Collections.Generic;
using System.Windows;
using MySql.Data.MySqlClient;


namespace BOB3
{
    class Connection
    {
        private MySqlConnection connection;
        private string connectionString;

        public Connection()
        {
            Initialize();
        }

        private void Initialize()
        {
            connectionString = "server=127.0.0.1;user=sqlUser;database=client_schedule;port=3306;password=Passw0rd!";
            connection = new MySqlConnection(connectionString);
        }

        private bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }

            catch (MySqlException ex)
            {
                switch (ex.Number)
                {
                    case 0:
                        MessageBox.Show("Cannot connect to server.", "Error");
                        break;
                    case 1045:
                        MessageBox.Show("Invalid username/password.", "Attention");
                        break;
                }
                return false;
            }
        }

        private bool CloseConenction()
        {
            try
            {
                connection.Close();
                return true;
            }

            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public bool IsValidLogin(string username, string password)
        {
            string query = "SELECT password, active FROM user WHERE userName=@userName";
            try
            {
                if (this.OpenConnection() == true)
                {
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@userName", username);
                    MySqlDataReader reader = command.ExecuteReader();
                    bool isValid = false;

                    while (reader.Read())
                    {
                        string valid = reader.GetString(0);
                        if (password.Equals(valid))
                            isValid = true;                        
                    }
                    this.CloseConenction();
                    return isValid;
                }
                else
                    return true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }

        }

        public User Login(string username)
        {
            User user = new User();

            string query = "SELECT userId FROM user WHERE userName=@userName";
            try
            {
                if (this.OpenConnection() == true)
                {
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@userName", username);
                    MySqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        user.ID = reader.GetInt32(0);
                        user.UserName = username;
                    }
                    this.CloseConenction(); 
                }
            }
            catch
            {
                user = null;
            }

            return user;
        }
        public void InsertCustomer(Customer customer)
        {
            string query1 = "INSERT INTO address (address, address2, cityId, postalCode, phone, createDate, createdBy, lastUpdate, lastUpdateBy) VALUES (@address, @address2, @cityId, @postalCode, @phone, @address.createDate, @address.createdBy, @address.lastUpdate, @address.lastUpdateBy)";
            string query2 = "INSERT INTO customer (customerName, addressId, active, createDate, createdBy, lastUpdate, lastUpdateBy) VALUES (@customerName, (SELECT addressId FROM address WHERE address.address=@address AND address.postalCode=@postalCode AND address.phone=@phone LIMIT 1), '1', @customer.createDate, @customer.CreatedBy, @customer.lastUpdate, @customer.lastUpdateBy)";

            if (this.OpenConnection() == true)
            {
                MySqlCommand command = new MySqlCommand(query1, connection);
                command.Parameters.AddWithValue("@address", customer.Address);
                command.Parameters.AddWithValue("@address2", customer.AddressTwo);
                command.Parameters.AddWithValue("@cityId", customer.CityID.ToString());
                command.Parameters.AddWithValue("@postalCode", customer.PostalCode.ToString());
                command.Parameters.AddWithValue("@phone", customer.PhoneNumber);
                command.Parameters.AddWithValue("@address.createDate", DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss"));
                command.Parameters.AddWithValue("@address.createdBy", CalendarView.CurrentUser.UserName);
                command.Parameters.AddWithValue("@address.lastUpdate", DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss"));
                command.Parameters.AddWithValue("@address.lastUpdateBy", CalendarView.CurrentUser.UserName);
                command.ExecuteNonQuery();

                command.CommandText = query2;
                command.Parameters.AddWithValue("@customerName", customer.CustomerName);
                command.Parameters.AddWithValue("@customer.createDate", DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss"));
                command.Parameters.AddWithValue("@customer.createdBy", CalendarView.CurrentUser.UserName);
                command.Parameters.AddWithValue("@customer.lastUpdate", DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss"));
                command.Parameters.AddWithValue("@customer.lastUpdateBy", CalendarView.CurrentUser.UserName);
                command.ExecuteNonQuery();
                this.CloseConenction();
            }
        }

        public void UpdateCustomer(Customer customer)
        {
            string query1 = "UPDATE address  SET address=@address, address2=@address2, cityId=@cityId, postalCode=@postalCode, phone=@phone, lastUpdate=@address.lastUpdate, lastUpdateBy=@address.lastUpdateBy WHERE addressId=@address.addressId";
            string query2 = "UPDATE customer SET customerName=@customerName, addressId=@customer.addressId, lastUpdate=@customer.lastUpdate, lastUpdateBy=@customer.lastUpdateBy WHERE customerId=@customerId";

            if (this.OpenConnection() == true)
            {
                MySqlCommand command = new MySqlCommand(query1, connection);
                command.Parameters.AddWithValue("@address", customer.Address);
                command.Parameters.AddWithValue("@address2", customer.AddressTwo);
                command.Parameters.AddWithValue("@cityId", customer.CityID.ToString());
                command.Parameters.AddWithValue("@postalCode", customer.PostalCode.ToString());
                command.Parameters.AddWithValue("@phone", customer.PhoneNumber);
                command.Parameters.AddWithValue("@address.lastUpdate", DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss"));
                command.Parameters.AddWithValue("@address.lastUpdateBy", CalendarView.CurrentUser.UserName);
                command.Parameters.AddWithValue("@address.addressId", customer.AddressID.ToString());
                command.ExecuteNonQuery();

                command.CommandText = query2;
                command.Parameters.AddWithValue("@customerName", customer.CustomerName);
                command.Parameters.AddWithValue("@customer.addressId", customer.AddressID.ToString());
                command.Parameters.AddWithValue("@customer.lastUpdate", DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss"));
                command.Parameters.AddWithValue("@customer.lastUpdateBy", CalendarView.CurrentUser.UserName);
                command.Parameters.AddWithValue("@customerId", customer.CustomerID.ToString());
                command.ExecuteNonQuery();
                this.CloseConenction();
            }
        }

        public void DeleteCustomer(int customerID)
        {
            string query1 = "DELETE FROM appointment WHERE appointment.customerId=@appointment.customerId";
            string query2 = "DELETE FROM customer WHERE customer.customerId=@customer.customerId";

            if (this.OpenConnection() == true)
            {
                MySqlCommand command = new MySqlCommand(query1, connection);
                command.Parameters.AddWithValue("@appointment.customerId", customerID.ToString());
                command.ExecuteNonQuery();

                command.CommandText = query2;
                command.Parameters.AddWithValue("@customer.customerId", customerID.ToString());
                command.ExecuteNonQuery();

                this.CloseConenction();
            }
        }

        public List<Customer> GetCustomers()
        {
            List<Customer> customers = new List<Customer>();
            string query = "SELECT customer.customerID, customer.customerName, customer.addressId, address.address, address.address2, address.postalCode, address.phone, address.cityId, city.city FROM customer LEFT JOIN address ON customer.addressId=address.addressId LEFT JOIN city ON address.cityId=city.cityId";
            if (this.OpenConnection() == true)
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Customer customer = new Customer();
                    customer.CustomerID = reader.GetInt32(0);
                    customer.CustomerName = reader.GetString(1);
                    customer.AddressID = reader.GetInt32(2);
                    customer.Address = reader.GetString(3);
                    customer.AddressTwo = reader.GetString(4);
                    customer.PostalCode = reader.GetInt32(5);
                    customer.PhoneNumber = reader.GetString(6);
                    customer.CityID = reader.GetInt32(7);
                    customer.CityName = reader.GetString(8);
                    customers.Add(customer);
                }
            }
            return customers;
        }

        public Customer GetCustomer(int customerID)
        {
            Customer customer = new Customer();
            string query = "SELECT customer.customerID, customer.customerName, customer.addressId, address.address, address.address2, address.postalCode, address.phone, address.cityId, city.city FROM customer LEFT JOIN address ON customer.addressId=address.addressId LEFT JOIN city ON address.cityId=city.cityId WHERE customer.customerId=@customerId";
            if (this.OpenConnection() == true)
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@customerId", customerID.ToString());
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    customer.CustomerID = reader.GetInt32(0);
                    customer.CustomerName = reader.GetString(1);
                    customer.AddressID = reader.GetInt32(2);
                    customer.Address = reader.GetString(3);
                    customer.AddressTwo = reader.GetString(4);
                    customer.PostalCode = reader.GetInt32(5);
                    customer.PhoneNumber = reader.GetString(6);
                    customer.CityID = reader.GetInt32(7);
                    customer.CityName = reader.GetString(8);
                }
            }
            return customer;
        }

        public void InsertAppointment(string userName, int userID, Appointment appointment)
        {
            string query = "INSERT INTO appointment (customerId, userId, title, description, location, contact, type, url, start, end, createDate, createdBy, lastUpdate, lastUpdateBy) VALUES (@customerId, @userId, @title, @description, @location, @contact, @type, @url, @start, @end, @createDate, @createdBy, @lastUpdate, @lastUpdateBy)";

            if (this.OpenConnection() == true)
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@customerId", appointment.CustomerID.ToString());
                command.Parameters.AddWithValue("@userId", CalendarView.CurrentUser.ID.ToString());
                command.Parameters.AddWithValue("@title", appointment.Title);
                command.Parameters.AddWithValue("@description", appointment.Description);
                command.Parameters.AddWithValue("@location", appointment.Location);
                command.Parameters.AddWithValue("@contact", appointment.Contact);
                command.Parameters.AddWithValue("@type", appointment.AppointmentType);
                command.Parameters.AddWithValue("@url", appointment.AppointmentURL);
                command.Parameters.AddWithValue("@start", appointment.Start.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss"));
                command.Parameters.AddWithValue("@end", appointment.End.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss"));
                command.Parameters.AddWithValue("@createDate", DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss"));
                command.Parameters.AddWithValue("@createdBy", userName);
                command.Parameters.AddWithValue("@lastUpdate", DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss"));
                command.Parameters.AddWithValue("@lastUpdateBy", userName);
                command.ExecuteNonQuery();
                this.CloseConenction();
            }
        }

        public void UpdateAppointment(string userName, Appointment appointment)
        {
            string query = "UPDATE appointment SET appointment.customerId=@customerId, appointment.title=@title, appointment.description=@description, appointment.location=@location, appointment.contact=@contact, appointment.type=@type, appointment.url=@url, appointment.start=@start, appointment.end=@end, appointment.lastUpdate='" + DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss") + "', appointment.lastUpdateBy='" + userName + "' WHERE appointmentId='" + appointment.ID.ToString() + "'";

            if (this.OpenConnection() == true)
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@customerId", appointment.CustomerID);
                command.Parameters.AddWithValue("@title", appointment.Title);
                command.Parameters.AddWithValue("@description", appointment.Description);
                command.Parameters.AddWithValue("@location", appointment.Location);
                command.Parameters.AddWithValue("@contact", appointment.Contact);
                command.Parameters.AddWithValue("@type", appointment.AppointmentType);
                command.Parameters.AddWithValue("@url", appointment.AppointmentURL);
                command.Parameters.AddWithValue("@start", appointment.Start.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss"));
                command.Parameters.AddWithValue("@end", appointment.End.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss"));
                command.ExecuteNonQuery();
                this.CloseConenction();
            }
        }

        public void DeleteAppointment(int appointmentID)
        {
            string query = "DELETE FROM appointment WHERE appointmentId='" + appointmentID.ToString() + "'";

            if (this.OpenConnection() == true)
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                command.ExecuteNonQuery();
                this.CloseConenction();
            }
        }

        public Appointment GetAppointment(int appointmentID)
        {
            Appointment appointment = new Appointment();
            string query = "SELECT appointment.appointmentId, appointment.customerId, customer.customerName, appointment.userId, appointment.title, appointment.type, appointment.start, appointment.end, appointment.description, appointment.location, appointment.contact, appointment.url FROM appointment LEFT JOIN customer ON appointment.customerId=customer.customerId WHERE appointment.appointmentId='" + appointmentID.ToString() + "'";

            if (this.OpenConnection() == true)
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    appointment.ID = reader.GetInt32(0);
                    appointment.CustomerID = reader.GetInt32(1);
                    appointment.CustomerName = reader.GetString(2);
                    appointment.UserID = reader.GetInt32(3);
                    appointment.Title = reader.GetString(4);
                    appointment.AppointmentType = reader.GetString(5);
                    appointment.Start = reader.GetDateTime(6).ToLocalTime();
                    appointment.End = reader.GetDateTime(7).ToLocalTime();
                    appointment.Description = reader.GetString(8);
                    appointment.Location = reader.GetString(9);
                    appointment.Contact = reader.GetString(10);
                    appointment.AppointmentURL = reader.GetString(11);
                }
                this.CloseConenction();
            }
            return appointment;
        }

        public List<Appointment> GetAppointments()
        {
            List<Appointment> appointments = new List<Appointment>();
            string query = "SELECT appointment.appointmentId, appointment.customerId, customer.customerName, appointment.userId, user.userName, appointment.title, appointment.type, appointment.start, appointment.end, appointment.description, appointment.location, appointment.contact, appointment.url FROM appointment LEFT JOIN customer ON appointment.customerId=customer.customerId LEFT JOIN user ON appointment.userId=user.userId";

            if (this.OpenConnection() == true)
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader reader = command.ExecuteReader();
                while(reader.Read())
                {
                    Appointment appointment = new Appointment();
                    appointment.ID = reader.GetInt32(0);
                    appointment.CustomerID = reader.GetInt32(1);
                    appointment.CustomerName = reader.GetString(2);
                    appointment.UserID = reader.GetInt32(3);
                    appointment.UserName = reader.GetString(4);
                    appointment.Title = reader.GetString(5);
                    appointment.AppointmentType = reader.GetString(6);
                    appointment.Start = reader.GetDateTime(7).ToLocalTime();
                    appointment.End = reader.GetDateTime(8).ToLocalTime();
                    appointment.Description = reader.GetString(9);
                    appointment.Location = reader.GetString(10);
                    appointment.Contact = reader.GetString(11);
                    appointment.AppointmentURL = reader.GetString(12);
                    appointments.Add(appointment);
                }
                this.CloseConenction();
            }
            return appointments;
        }

        public List<Appointment> GetAppointments(int userID)
        {
            List<Appointment> appointments = new List<Appointment>();
            string query = "SELECT appointment.appointmentId, appointment.customerId, customer.customerName, appointment.userId, appointment.title, appointment.type, appointment.start, appointment.end, appointment.description, appointment.location, appointment.contact, appointment.url FROM appointment LEFT JOIN customer ON appointment.customerId=customer.customerId WHERE userId='" + userID.ToString() + "'";

            if (this.OpenConnection() == true)
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Appointment appointment = new Appointment();
                    appointment.ID = reader.GetInt32(0);
                    appointment.CustomerID = reader.GetInt32(1);
                    appointment.CustomerName = reader.GetString(2);
                    appointment.UserID = reader.GetInt32(3);
                    appointment.Title = reader.GetString(4);
                    appointment.AppointmentType = reader.GetString(5);
                    appointment.Start = reader.GetDateTime(6).ToLocalTime();
                    appointment.End = reader.GetDateTime(7).ToLocalTime();
                    appointment.Description = reader.GetString(8);
                    appointment.Location = reader.GetString(9);
                    appointment.Contact = reader.GetString(10);
                    appointment.AppointmentURL = reader.GetString(11);
                    appointments.Add(appointment);
                }
                this.CloseConenction();
            }
            return appointments;
        }

        public List<City> GetCities()
        {
            List<City> Cities = new List<City>();
            string query = "SELECT city.city, city.cityId FROM city";

            if(this.OpenConnection() == true)
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader reader = command.ExecuteReader();
                while(reader.Read())
                {
                    City city = new City();
                    city.Name = reader.GetString(0);
                    city.CityID = reader.GetInt32(1);
                    Cities.Add(city);
                }
            }
            return Cities;
        }
    }
}
