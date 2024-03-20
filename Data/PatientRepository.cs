using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCenterClientApp.Data
{
    public class PatientRepository
    {
        private string connectStr;

        public PatientRepository(string connectionString)
        {
            connectStr = connectionString;
        }

        public void AddNewPatient()
        {
            // Ta info från den nya patienten
            Console.WriteLine("Enter information for the new patient:");

            Console.Write("Medical Number: ");
            int medicalNumber = int.Parse(Console.ReadLine());

            Console.Write("First Name: ");
            string firstName = Console.ReadLine();

            Console.Write("Last Name: ");
            string lastName = Console.ReadLine();

            Console.Write("Gender (M/F): ");
            char gender = char.Parse(Console.ReadLine());

            Console.Write("City: ");
            string city = Console.ReadLine();

            Console.Write("State/Province: ");
            string stateProvince = Console.ReadLine();

            Console.Write("Street: ");
            string street = Console.ReadLine();

            Console.Write("Post Zip Code: ");
            string postZipCode = Console.ReadLine();

            Console.Write("Phone: ");
            string phone = Console.ReadLine();

            Console.Write("Birthday (YYYY-MM-DD): ");
            DateTime birthday = DateTime.Parse(Console.ReadLine());

            // Connect
            using (SqlConnection connection = new SqlConnection(connectStr))
            {
                connection.Open();

                // Add the new patient by inserting them
                string insertQuery = @"
                    INSERT INTO Patients (MedicalNumber, FirstName, LastName, Gender, City, StateProvince, Street, PostZipCode, Phone, Birthday, RegisterDate)
                    VALUES (@MedicalNumber, @FirstName, @LastName, @Gender, @City, @StateProvince, @Street, @PostZipCode, @Phone, @Birthday, GETDATE());";

                using (SqlCommand command = new SqlCommand(insertQuery, connection))
                {
                    // Add parameters to the command
                    command.Parameters.AddWithValue("@MedicalNumber", medicalNumber);
                    command.Parameters.AddWithValue("@FirstName", firstName);
                    command.Parameters.AddWithValue("@LastName", lastName);
                    command.Parameters.AddWithValue("@Gender", gender);
                    command.Parameters.AddWithValue("@City", city);
                    command.Parameters.AddWithValue("@StateProvince", stateProvince);
                    command.Parameters.AddWithValue("@Street", street);
                    command.Parameters.AddWithValue("@PostZipCode", postZipCode);
                    command.Parameters.AddWithValue("@Phone", phone);
                    command.Parameters.AddWithValue("@Birthday", birthday);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Patient added successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Failed to add patient.");
                    }
                }
            }

            Console.ReadLine();
        }

        public void ShowMedicalNotesForPatient(int patientMedicalNumber)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectStr))
                {
                    string query = "SELECT MedicalNotes FROM Bookings WHERE PatientMedicalNumber = @PatientMedicalNumber";

                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PatientMedicalNumber", patientMedicalNumber);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            List<string> medicalNotesList = new List<string>();

                            while (reader.Read())
                            {
                                string medicalNotes = reader["MedicalNotes"] == DBNull.Value ? null : (string)reader["MedicalNotes"];
                                medicalNotesList.Add(medicalNotes);
                            }

                            if (medicalNotesList.Count > 0)
                            {
                                Console.WriteLine($"Medical Notes for Patient with Medical Number {patientMedicalNumber}:");
                                foreach (var medicalNotes in medicalNotesList)
                                {
                                    Console.WriteLine(medicalNotes);
                                }
                            }
                            else
                            {
                                Console.WriteLine($"No medical notes found for Patient with Medical Number {patientMedicalNumber}");
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

        public bool IsPatientRegistered(int medicalNumber)
        {
            string testQuery = "SELECT COUNT(*) FROM Patients WHERE MedicalNumber = @MedicalNumber";

            using (SqlConnection connection = new SqlConnection(connectStr))
            {
                connection.Open();

                // Räkna hur många gånger som medicalNumret finns i databasen
                using (SqlCommand command = new SqlCommand(testQuery, connection))
                {
                    command.Parameters.AddWithValue("@MedicalNumber", medicalNumber);
                    int count = (int)command.ExecuteScalar();

                    // om det finns mer än en så returna true för den finns
                    if (count > 0)
                    {
                        return true;
                    }

                    // annars returna false
                    return false;
                }
            }
        }

        public void ShowAllPatientsWithCosts()
        {
            string query = @"
                SELECT 
                    p.MedicalNumber,
                    p.FirstName,
                    p.LastName,
                    SUM(s.VisitCost) AS TotalCost
                FROM 
                    Bookings b
                LEFT JOIN 
                    Doctors d ON b.DoctorEmployeeNumber = d.EmployeeNumber
                LEFT JOIN 
                    Patients p ON b.PatientMedicalNumber = p.MedicalNumber
                LEFT JOIN 
                    Specialization s ON d.SpecializationId = s.SpecializationId
                GROUP BY 
                    p.MedicalNumber, p.FirstName, p.LastName;";

            try
            {
                // Connect to the database
                using (SqlConnection connection = new SqlConnection(connectStr))
                {
                    // Open the connection
                    connection.Open();

                    // Create a command to execute the query
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Execute the query and retrieve the results
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Check if there are rows returned
                            if (reader.HasRows)
                            {
                                // Iterate over the results and write them to the console
                                while (reader.Read())
                                {
                                    int medicalNumber = reader.GetInt32(0);
                                    string firstName = reader.GetString(1);
                                    string lastName = reader.GetString(2);
                                    decimal totalCost = reader.GetDecimal(3);

                                    Console.WriteLine($"Medical Number: {medicalNumber}, Name: {firstName} {lastName}, Total Cost: {totalCost}");
                                }
                            }
                            else
                            {
                                Console.WriteLine("No data found.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.ReadLine();
        }

        public void UpdatePatientInformation(int patientMedicalId)
        {
            Console.WriteLine("Enter the medical number of the patient to update:");
            int medicalNumberToUpdate = patientMedicalId;
            Console.WriteLine(medicalNumberToUpdate);

            using (SqlConnection connection = new SqlConnection(connectStr))
            {
                connection.Open();

                Console.WriteLine("Enter new information for the patient:");

                Console.Write("First Name: ");
                string firstName = Console.ReadLine();

                Console.Write("Last Name: ");
                string lastName = Console.ReadLine();

                Console.Write("Gender (M/F): ");
                char gender = char.Parse(Console.ReadLine());

                Console.Write("City: ");
                string city = Console.ReadLine();

                Console.Write("State/Province: ");
                string stateProvince = Console.ReadLine();

                Console.Write("Street: ");
                string street = Console.ReadLine();

                Console.Write("Post Zip Code: ");
                string postZipCode = Console.ReadLine();

                Console.Write("Phone: ");
                string phone = Console.ReadLine();

                Console.Write("Birthday (YYYY-MM-DD): ");
                DateTime birthday = DateTime.Parse(Console.ReadLine());

                // Update the patient's information in the database
                string updateQuery = @"
                    UPDATE Patients
                    SET FirstName = @FirstName, LastName = @LastName, Gender = @Gender,
                        City = @City, StateProvince = @StateProvince, Street = @Street,
                        PostZipCode = @PostZipCode, Phone = @Phone, Birthday = @Birthday
                        WHERE MedicalNumber = @MedicalNumberToUpdate";

                using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                {
                    updateCommand.Parameters.AddWithValue("@MedicalNumberToUpdate", medicalNumberToUpdate);
                    updateCommand.Parameters.AddWithValue("@FirstName", firstName);
                    updateCommand.Parameters.AddWithValue("@LastName", lastName);
                    updateCommand.Parameters.AddWithValue("@Gender", gender);
                    updateCommand.Parameters.AddWithValue("@City", city);
                    updateCommand.Parameters.AddWithValue("@StateProvince", stateProvince);
                    updateCommand.Parameters.AddWithValue("@Street", street);
                    updateCommand.Parameters.AddWithValue("@PostZipCode", postZipCode);
                    updateCommand.Parameters.AddWithValue("@Phone", phone);
                    updateCommand.Parameters.AddWithValue("@Birthday", birthday);

                    int rowsAffected = updateCommand.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Patient information updated successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Failed to update patient information.");
                    }
                }
            }
            Console.ReadLine();
        }
    }
}
