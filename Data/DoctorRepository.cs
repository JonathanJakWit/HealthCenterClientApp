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
                            if (doctor == null)
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
                //string updateQuery = "UPDATE Doctors SET Status = @Status WHERE EmployeeNumber = @EmployeeNumber";

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
