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

        //public List<Booking> GetDoctorBookings(int doctorEmployeeNumber)
        //{
        //    List<Booking> bookings = new List<Booking>();

        //    using (var connection = new SqlConnection(connectStr))
        //    {

        //    }

        //    return new List<Booking> { new Booking() }; // TEMP
        //}

        // Edit medical notes on booking (by doctor only)

        // Add booking function
    }
}
