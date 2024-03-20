using HealthCenterClientApp.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCenterClientApp.Data
{
    public class DoctorRepository
    {
        private string connectStr;

        public DoctorRepository(string connectionString)
        {
            connectStr = connectionString;
        }

        public int GetDoctorCount()
        {
            int docCount = 0;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectStr))
                {
                    string searchQuery = "SELECT COUNT(*) FROM Doctors";
                    SqlCommand command = new SqlCommand(searchQuery, connection);
                    connection.Open();
                    docCount = (int)command.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return docCount;
        }

        public void ShowDoctorAvailability(int doctorEmployeeNumber)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectStr))
                {
                    // Query creation
                    string query = $@"
                        SELECT Monday_09_00, Monday_09_30, Monday_10_00, Monday_10_30,
                               Tuesday_09_00, Tuesday_09_30, Tuesday_10_00, Tuesday_10_30,
                               Wednesday_09_00, Wednesday_09_30, Wednesday_10_00, Wednesday_10_30,
                               Thursday_09_00, Thursday_09_30, Thursday_10_00, Thursday_10_30,
                               Friday_09_00, Friday_09_30, Friday_10_00, Friday_10_30
                        FROM DoctorAvailability
                        WHERE DoctorEmployeeNumber = @DoctorEmployeeNumber";

                    // Open the connection
                    connection.Open();

                    // Create a command
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Add parameter for the doctor's EmployeeNumber
                        command.Parameters.AddWithValue("@DoctorEmployeeNumber", doctorEmployeeNumber);

                        // Execute the command and read the data
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Check if there are rows
                            if (reader.HasRows)
                            {
                                // Output availability for each day and time slot
                                while (reader.Read())
                                {
                                    Console.WriteLine("Availability for Doctor Employee Number: " + doctorEmployeeNumber);
                                    Console.WriteLine("Monday: 09:00 - " + reader.GetBoolean(0));
                                    Console.WriteLine("Monday: 09:30 - " + reader.GetBoolean(1));
                                    Console.WriteLine("Monday: 10:00 - " + reader.GetBoolean(2));
                                    Console.WriteLine("Monday: 10:30 - " + reader.GetBoolean(3));
                                    Console.WriteLine("Tuesday: 09:00 - " + reader.GetBoolean(4));
                                    Console.WriteLine("Tuesday: 09:30 - " + reader.GetBoolean(5));
                                    Console.WriteLine("Tuesday: 10:00 - " + reader.GetBoolean(6));
                                    Console.WriteLine("Tuesday: 10:30 - " + reader.GetBoolean(7));
                                    Console.WriteLine("Wednesday: 09:00 - " + reader.GetBoolean(8));
                                    Console.WriteLine("Wednesday: 09:30 - " + reader.GetBoolean(9));
                                    Console.WriteLine("Wednesday: 10:00 - " + reader.GetBoolean(10));
                                    Console.WriteLine("Wednesday: 10:30 - " + reader.GetBoolean(11));
                                    Console.WriteLine("Thursday: 09:00 - " + reader.GetBoolean(12));
                                    Console.WriteLine("Thursday: 09:30 - " + reader.GetBoolean(13));
                                    Console.WriteLine("Thursday: 10:00 - " + reader.GetBoolean(14));
                                    Console.WriteLine("Thursday: 10:30 - " + reader.GetBoolean(15));
                                    Console.WriteLine("Friday: 09:00 - " + reader.GetBoolean(16));
                                    Console.WriteLine("Friday: 09:30 - " + reader.GetBoolean(17));
                                    Console.WriteLine("Friday: 10:00 - " + reader.GetBoolean(18));
                                    Console.WriteLine("Friday: 10:30 - " + reader.GetBoolean(19));
                                }
                            }
                            else
                            {
                                Console.WriteLine("DoctorEmployeeNumber: " + doctorEmployeeNumber + " Doesnt have any available sluts");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void SearchDoctorsWithSpecialization(int specialId)
        {
            using (SqlConnection connection = new SqlConnection(connectStr))
            {
                string query = "SELECT * FROM Doctors WHERE SpecializationId = @SpecializationId";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@SpecializationId", specialId);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Console.WriteLine($"EmployeeNunber: {reader["EmployeeNumber"]}, FirstName: {reader["FirstName"]}, LastName: {reader["LastName"]}, SpecializationId: {reader["SpecializationId"]}, PhoneNumber: {reader["PhoneNumber"]}");
                }

                reader.Close();
            }
        }

        public void SeeSpecializationAndVisitCost()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectStr))
                {
                    // Retrieve specialization names and visit costs
                    string retQuery = "SELECT SpecializationName, VisitCost FROM Specialization";

                    connection.Open();
                    using (SqlCommand command = new SqlCommand(retQuery, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Check if there are rows
                            if (reader.HasRows)
                            {
                                // Output columns
                                Console.WriteLine("Specialization Name\tVisit Cost");

                                // Read and output each row
                                while (reader.Read())
                                {
                                    string specializationName = reader.GetString(0);
                                    decimal visitCost = reader.GetDecimal(1);

                                    Console.WriteLine($"{specializationName}\t\t{visitCost}");
                                }
                            }
                            else
                            {
                                Console.WriteLine("No existing data.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public Doctor GetDoctorFromEmployeeNumber(int employeeNumber)
        {
            Doctor doctor = null;

            using (var connection = new SqlConnection(connectStr))
            {
                connection.Open();

                string query = @"
                SELECT d.EmployeeNumber, d.FirstName, d.LastName, d.SpecializationId, d.PhoneNumber,
                       s.SpecializationId, s.SpecializationName, s.VisitCost
                FROM Doctors d
                LEFT JOIN Specialization s ON d.SpecializationId = s.SpecializationId
                WHERE d.EmployeeNumber = @EmployeeNumber";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EmployeeNumber", employeeNumber);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (doctor == null) // If doctor is deleted then dont include their data
                            {
                                doctor = new Doctor
                                {
                                    EmployeeNumber = Convert.ToInt32(reader["EmployeeNumber"]),
                                    FirstName = reader["FirstName"].ToString(),
                                    LastName = reader["LastName"].ToString(),
                                    PhoneNumber = reader["PhoneNumber"].ToString(),
                                    SpecializationId = reader["SpecializationId"] == DBNull.Value ? null : (int?)reader["SpecializationId"],
                                    Specialization = new Specialization
                                    {
                                        SpecializationId = Convert.ToInt32(reader["SpecializationId"]),
                                        SpecializationName = reader["SpecializationName"].ToString(),
                                        VisitCost = Convert.ToDecimal(reader["VisitCost"])
                                    }
                                };
                            }
                            else
                            {
                                doctor.Specialization = new Specialization
                                {
                                    SpecializationId = Convert.ToInt32(reader["SpecializationId"]),
                                    SpecializationName = reader["SpecializationName"].ToString(),
                                    VisitCost = Convert.ToDecimal(reader["VisitCost"])
                                };
                            }
                        }
                    }
                }
            }

            return doctor;
        }

        public void ChangeDoctorSpecialization(int employeeNumber, int specializationId)
        {
            using (var connection = new SqlConnection(connectStr))
            {
                connection.Open();

                // Update the doctor's specialization
                string updateQuery = "UPDATE Doctors SET SpecializationId = @SpecializationId WHERE EmployeeNumber = @EmployeeNumber";

                using (var updateCommand = new SqlCommand(updateQuery, connection))
                {
                    updateCommand.Parameters.AddWithValue("@SpecializationId", specializationId);
                    updateCommand.Parameters.AddWithValue("@EmployeeNumber", employeeNumber);
                    updateCommand.ExecuteNonQuery();
                }
            }
        }
        
        public void ChangeDoctorStatus(int employeeNumber, string statusDisplayString)
        {
            using (var connection = new SqlConnection(connectStr))
            {
                connection.Open();

                // Update the doctor's status
                string updateQuery = "UPDATE Doctors SET Status = @Status WHERE EmployeeNumber = @EmployeeNumber";

                using (var updateCommand = new SqlCommand(updateQuery, connection))
                {
                    updateCommand.Parameters.AddWithValue("@Status", statusDisplayString);
                    updateCommand.Parameters.AddWithValue("@EmployeeNumber", employeeNumber);
                    updateCommand.ExecuteNonQuery();
                }
            }
        }

        public int GetNextEmployeeNumber(SqlConnection connection)
        {
            string countQuery = "SELECT COUNT(*) FROM Doctors";

            using (var countCommand = new SqlCommand(countQuery, connection))
            {
                int employeeCount = (int)countCommand.ExecuteScalar();
                return employeeCount + 1;
            }
        }

        public void AddDoctor(string firstName, string lastName, int specializationId, string phoneNumber)
        {
            using (var connection = new SqlConnection(connectStr))
            {
                connection.Open();

                // Get the current count of employees
                int newEmployeeNumber = GetNextEmployeeNumber(connection);

                // Add the doctor
                string insertQuery = "INSERT INTO Doctors (EmployeeNumber, FirstName, LastName, SpecializationId, PhoneNumber) " +
                                     "VALUES (@EmployeeNumber, @FirstName, @LastName, @SpecializationId, @PhoneNumber)";

                using (var insertCommand = new SqlCommand(insertQuery, connection))
                {
                    insertCommand.Parameters.AddWithValue("@EmployeeNumber", newEmployeeNumber);
                    insertCommand.Parameters.AddWithValue("@FirstName", firstName);
                    insertCommand.Parameters.AddWithValue("@LastName", lastName);
                    insertCommand.Parameters.AddWithValue("@SpecializationId", specializationId);
                    insertCommand.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                    insertCommand.ExecuteNonQuery();
                }

                insertQuery = "INSERT INTO DoctorAvailability (DoctorEmployeeNumber) " +
                              "VALUES (@DoctorEmployeeNumber)";

                using (var insertCommand = new SqlCommand(insertQuery, connection))
                {
                    insertCommand.Parameters.AddWithValue("@DoctorEmployeeNumber", newEmployeeNumber);
                    insertCommand.ExecuteNonQuery();
                }
            }
        }

        public void ChangeDoctorAvailability(int employeeNumber, AvailabilitySlot availabilitySlot, int bitValue) // 0 or 1 where 0 = FALSE and 1 = TRUE
        {
            using (var connection = new SqlConnection(connectStr))
            {
                connection.Open();

                // Define which day and time (which column in the availability-table)
                string updateQuery = "UPDATE DoctorAvailability SET ";
                switch (availabilitySlot.WeekDay)
                {
                    case WeekDays.Monday:
                        updateQuery += "Monday_";
                        break;
                    case WeekDays.Tuesday:
                        updateQuery += "Tuesday_";
                        break;
                    case WeekDays.Wednesday:
                        updateQuery += "Wednesday_";
                        break;
                    case WeekDays.Thursday:
                        updateQuery += "Thursday_";
                        break;
                    case WeekDays.Friday:
                        updateQuery += "Friday_";
                        break;
                    default:
                        break;
                }

                switch (availabilitySlot.TimeSlot)
                {
                    case TimeSlots.Nine:
                        updateQuery += "09_00";
                        break;
                    case TimeSlots.NineThirty:
                        updateQuery += "09_30";
                        break;
                    case TimeSlots.Ten:
                        updateQuery += "10_00";
                        break;
                    case TimeSlots.TenThirty:
                        updateQuery += "10_30";
                        break;
                    default:
                        break;
                }

                updateQuery += " = @AvailabilityStatus WHERE DoctorEmployeeNumber = @DoctorEmployeeNumber";

                // Update the doctor's availability
                using (var updateCommand = new SqlCommand(updateQuery, connection))
                {
                    updateCommand.Parameters.AddWithValue("@AvailabilityStatus", bitValue);
                    updateCommand.Parameters.AddWithValue("@DoctorEmployeeNumber", employeeNumber);
                    updateCommand.ExecuteNonQuery();
                }
            }
        }
    }
}
