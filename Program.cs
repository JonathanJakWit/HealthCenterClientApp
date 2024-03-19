using Microsoft.VisualBasic;
using System.Numerics;
using System.Xml.Linq;
using HealthCenterClientApp.Data;
using HealthCenterClientApp.Models;

namespace HealthCenterClientApp
{
    public enum ClientTypes
    {
        Admin, Doctor, Patient
    }

    public class Program
    {
        static DoctorRepository doctorRepository = new DoctorRepository("Data Source=(localdb)\\MSSqlLocalDB;Initial Catalog=HealthCenterDB;Integrated Security=True;Pooling=False;Encrypt=True;Trust Server Certificate=False");
        static SpecializationRepository specializationRepository = new SpecializationRepository("Data Source=(localdb)\\MSSqlLocalDB;Initial Catalog=HealthCenterDB;Integrated Security=True;Pooling=False;Encrypt=True;Trust Server Certificate=False");

        private bool isConnected = false;
        private ClientTypes clientTypes = ClientTypes.Admin;

        #region Admin
        static void AdminOption1()
        {
            // Add a list of specialization of its doctors such as dentist, cardiologist, or
            //psychiatrists, and their visit cost
            specializationRepository.GetSpecializationList().ForEach(x => { x.ToString(); });

            Console.WriteLine("Which EmployeeNumber do you want to edit?: ");
            string? employNr = Console.ReadLine();
            Console.WriteLine("SpecializationId: ");
            string specialName = Console.ReadLine();

            doctorRepository.ChangeDoctorSpecialization(Convert.ToInt32(employNr), Convert.ToInt32(specialName));

            //Doctor pogDoc = doctorRepository.GetDoctorWithSpecialization(Convert.ToInt32(employNr));
            //Console.WriteLine(pogDoc.ToString());

            Console.WriteLine("Op 1");
            Console.ReadLine();
        }
        static void AdminOption2()
        {
            //Add the information about the health center’s doctors, including
            //employee number, full name, one specialization for each doctor, and
            //phone

            Console.WriteLine("Op 2");
            Console.ReadLine();
        }
        static void AdminOption3()
        {
            //Admin can also delete a doctor from the health center

            Console.WriteLine("Op 3");
            Console.ReadLine();
        }

        static void HandleAdmin()
        {
            bool adminLoggedIn = true;
            while (adminLoggedIn)
            {
                Console.WriteLine("-----");
                Console.WriteLine("[1] - Add specialization and visit cost");
                Console.WriteLine("[2] - Add doctors information");
                Console.WriteLine("[3] - Delete doctor");
                Console.WriteLine("[4] - Log out");
                Console.WriteLine("-----");

                Console.WriteLine("Option: ");
                var _ = Console.ReadLine();
                switch (_)
                {
                    case "1":
                        AdminOption1();
                        break;
                    case "2":
                        AdminOption2();
                        break;
                    case "3":
                        AdminOption3();
                        break;
                    case "4":
                        adminLoggedIn = false;
                        break;

                    default:
                        break;
                }
                Console.ReadLine();
                Console.Clear();
            }
        }
        #endregion Admin

        #region Doctor
        static void HandleDoctor()
        {

        }
        #endregion Doctor

        #region Patient
        static void HandlePatient()
        {

        }
        #endregion Patient

        static void Main(string[] args)
        {
            bool isClientActive = true;

            // Connect to database


            // Program Loop
            while (isClientActive)
            {
                // Ask which client type it is
                Console.WriteLine("-----");
                Console.WriteLine("[1] - Admin");
                Console.WriteLine("[2] - Doctor");
                Console.WriteLine("[3] - Patient");
                Console.WriteLine("[4] - Exit");
                Console.WriteLine("-----");

                Console.WriteLine("Option: ");
                var _ = Console.ReadLine();
                switch (_)
                {
                    case "1":
                        HandleAdmin();
                        break;
                    case "2":
                        HandleDoctor();
                        break;
                    case "3":
                        HandlePatient();
                        break;
                    case "4":
                        isClientActive = false;
                        break;

                    default:
                        break;
                }
                Console.ReadLine();
                Console.Clear();
            }

            // Disconnect database

            // Exit program
        }
    }
}
