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
    }
}
