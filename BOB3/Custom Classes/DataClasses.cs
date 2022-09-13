using System;
using System.Collections.Generic;

public class Appointment
{
    public int ID { get; set; }
    public int UserID { get; set; }
    public string UserName { get; set; }
    public int CustomerID { get; set; }
    public string CustomerName { get; set; }
    public string Title { get; set; }
    public string AppointmentType { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string Description { get; set; }
    public string Location { get; set; }
    public string Contact { get; set; }
    public string AppointmentURL { get; set; }
}

public class User
{
    public int ID { get; set; }
    public string UserName { get; set; }
    public List<Appointment> Appointments { get; set; }
}

public class Customer
{
    public int CustomerID { get; set; }
    public string CustomerName { get; set; }
    public int AddressID { get; set; }
    public string Address { get; set; }
    public string AddressTwo { get; set; }
    public int CityID { get; set; }
    public string CityName { get; set; }
    public int PostalCode { get; set; }
    public string PhoneNumber { get; set; }
}

public class City
{
    public int CityID { get; set; }
    public string Name { get; set; }
}

public static class DateTimeExtensions
{
    public static DateTime StartOfWeek(this DateTime dt)
    {
        var startofweek = dt.Date.AddDays(-(int)dt.DayOfWeek);
        return startofweek;
    }

    public static DateTime EndOfWeek(this DateTime dt)
    {
        var startofweek = dt.Date.AddDays(-(int)dt.DayOfWeek);
        var endofweek = startofweek.AddDays(7);
        return endofweek;
    }
}

public class DatePeriod
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }

    public DatePeriod(DateTime start, DateTime end)
    {
        Start = start;
        End = end;
    }

    public bool Contains(DateTime time)
    {
        return Start.CompareTo(time) <= 0 && End.CompareTo(time) >= 0;
    }

    public bool IsAfter(DateTime time)
    {
        return End.CompareTo(time) <= 0;
    }

    public bool IsBefore(DateTime time)
    {
        return Start.CompareTo(time) >= 0;
    }
}