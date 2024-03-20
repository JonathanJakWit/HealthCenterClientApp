using HealthCenterClientApp.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCenterClientApp.Data
{
    public class BookingRepository
    {
        private string connectStr;

        // Constructor
        public BookingRepository(string connectionString)
        {
            connectStr = connectionString;
        }

        // Get all bookings from patient medical number

        // Get all bookings from doctor employee number
        public List<Booking> GetDoctorBookings(int doctorEmployeeNumber)
        {
            List<Booking> bookings = new List<Booking>();

            using (SqlConnection connection = new SqlConnection(connectStr))
            {
                string query = "SELECT BookingId, DateTime, MedicalNotes, PatientMedicalNumber FROM Bookings WHERE DoctorEmployeeNumber = @DoctorEmployeeNumber";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DoctorEmployeeNumber", doctorEmployeeNumber);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Booking booking = new Booking();
                            booking.BookingId = (int)reader["BookingId"];
                            booking.AppointmentDate = (DateTime)reader["DateTime"];
                            booking.MedicalNotes = reader["MedicalNotes"] == DBNull.Value ? null : (string)reader["MedicalNotes"];
                            booking.PatientMedicalNumber = (int)reader["PatientMedicalNumber"];
                            bookings.Add(booking);
                        }
                    }
                }
            }

            return bookings;
        }

        public void AddMedicalNote(int bookingId, string medicalNote)
        {
            using (SqlConnection connection = new SqlConnection(connectStr))
            {
                connection.Open();

                // Update the appointment's medical notes
                string updateQuery = "UPDATE Bookings SET MedicalNotes = @MedicalNotes WHERE BookingId = @BookingId";

                using (var updateCommand = new SqlCommand(updateQuery, connection))
                {
                    updateCommand.Parameters.AddWithValue("@MedicalNotes", medicalNote);
                    updateCommand.Parameters.AddWithValue("@BookingId", bookingId);
                    updateCommand.ExecuteNonQuery();
                }
            }
        }

        public int GetNextBookingId()
        {
            int numberOfBookings = 0;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectStr))
                {
                    string query = "SELECT COUNT(*) FROM Bookings";

                    SqlCommand command = new SqlCommand(query, connection);

                    connection.Open();

                    numberOfBookings = (int)command.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return numberOfBookings + 1;
        }

        // Add booking function
        public void AddNewBooking(int doctorEmployeeId, int patientMedicalNumber, AvailabilitySlot timeSlot)
        {
            int bookingId = GetNextBookingId();

            DateTime today = DateTime.Today;
            DateTime nextMonday = today.AddDays((DayOfWeek.Monday + 7 - today.DayOfWeek) % 7);
            DateTime nextTuesday = today.AddDays((DayOfWeek.Tuesday + 7 - today.DayOfWeek) % 7);
            DateTime nextWednesday = today.AddDays((DayOfWeek.Wednesday + 7 - today.DayOfWeek) % 7);
            DateTime nextThursday = today.AddDays((DayOfWeek.Thursday + 7 - today.DayOfWeek) % 7);
            DateTime nextFriday = today.AddDays((DayOfWeek.Friday + 7 - today.DayOfWeek) % 7);

            DateTime bookingDate = DateTime.Today;
            switch (timeSlot.WeekDay)
            {
                case WeekDays.Monday:
                    bookingDate = nextMonday;
                    break;
                case WeekDays.Tuesday:
                    bookingDate = nextTuesday;
                    break;
                case WeekDays.Wednesday:
                    bookingDate = nextWednesday;
                    break;
                case WeekDays.Thursday:
                    bookingDate = nextThursday;
                    break;
                case WeekDays.Friday:
                    bookingDate = nextFriday;
                    break;
                default:
                    break;
            }

            switch (timeSlot.TimeSlot)
            {
                case TimeSlots.Nine:
                    bookingDate = new DateTime(bookingDate.Year, bookingDate.Month, bookingDate.Day, 9, 0, 0);
                    break;
                case TimeSlots.NineThirty:
                    bookingDate = new DateTime(bookingDate.Year, bookingDate.Month, bookingDate.Day, 9, 30, 0);
                    break;
                case TimeSlots.Ten:
                    bookingDate = new DateTime(bookingDate.Year, bookingDate.Month, bookingDate.Day, 10, 0, 0);
                    break;
                case TimeSlots.TenThirty:
                    bookingDate = new DateTime(bookingDate.Year, bookingDate.Month, bookingDate.Day, 10, 30, 0);
                    break;
                default:
                    break;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectStr))
                {
                    // SQL query to insert a new booking
                    string query = @"
                        INSERT INTO Bookings (BookingId, DateTime, MedicalNotes, DoctorEmployeeNumber, PatientMedicalNumber)
                        VALUES (@BookingId, @DateTime, @MedicalNotes, @DoctorEmployeeNumber, @PatientMedicalNumber)";

                    // Open the connection
                    connection.Open();

                    // Create a command
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Add parameters
                        command.Parameters.AddWithValue("@BookingId", bookingId);
                        command.Parameters.AddWithValue("@DateTime", bookingDate);
                        command.Parameters.AddWithValue("@MedicalNotes", "");
                        command.Parameters.AddWithValue("@DoctorEmployeeNumber", doctorEmployeeId);
                        command.Parameters.AddWithValue("@PatientMedicalNumber", patientMedicalNumber);

                        // Execute the command
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Booking inserted successfully.");
                        }
                        else
                        {
                            Console.WriteLine("Failed to insert booking.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
