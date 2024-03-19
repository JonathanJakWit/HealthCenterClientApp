using HealthCenterClientApp.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCenterClientApp.Data
{
    public class SpecializationRepository
    {
        private string connectStr;

        public SpecializationRepository(string connectionString)
        {
            connectStr = connectionString;
        }

        public List<Specialization> GetSpecializationList()
        {
            List<Specialization> specializations = new List<Specialization>();

            using (var connection = new SqlConnection(connectStr))
            {
                connection.Open();

                string query = "SELECT SpecializationId, SpecializationName, VisitCost FROM Specialization";

                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            specializations.Add(new Specialization
                            {
                                SpecializationId = Convert.ToInt32(reader["SpecializationId"]),
                                SpecializationName = reader["SpecializationName"].ToString(),
                                VisitCost = Convert.ToDecimal(reader["VisitCost"])
                            }
                            );
                        }
                    }
                }
            }

            return specializations;
        }
    }
}
