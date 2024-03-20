using Microsoft.VisualBasic;
using System.Numerics;
using System.Xml.Linq;
using HealthCenterClientApp.Data;
using HealthCenterClientApp.Models;
using System.Data.SqlTypes;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        static PatientRepository patientRepository = new PatientRepository("Data Source=(localdb)\\MSSqlLocalDB;Initial Catalog=HealthCenterDB;Integrated Security=True;Pooling=False;Encrypt=True;Trust Server Certificate=False");
        static BookingRepository bookingRepository = new BookingRepository("Data Source=(localdb)\\MSSqlLocalDB;Initial Catalog=HealthCenterDB;Integrated Security=True;Pooling=False;Encrypt=True;Trust Server Certificate=False");

        private bool isConnected = false;
        private ClientTypes clientTypes = ClientTypes.Admin;

        #region Admin
        static void AdminOption1()
        {
            // Add a list of specialization of its doctors such as dentist, cardiologist, or
            //psychiatrists, and their visit cost
            //specializationRepository.GetSpecializationList().ForEach(x => { x.ToString(); });

            Console.WriteLine("----------");
            Console.WriteLine("Which EmployeeNumber do you want to edit?: ");
            string? employNr = Console.ReadLine();
            Console.WriteLine("----------");
            Console.WriteLine("[1] - Dentist");
            Console.WriteLine("[2] - Cardiologist");
            Console.WriteLine("[3] - Psychiatrist");
            Console.WriteLine("----------");
            Console.WriteLine("SpecializationId: ");
            string specialName = Console.ReadLine();

            doctorRepository.ChangeDoctorSpecialization(Convert.ToInt32(employNr), Convert.ToInt32(specialName));

            Doctor curDoctor = doctorRepository.GetDoctorFromEmployeeNumber(Convert.ToInt32(employNr));
            Console.WriteLine("----------");
            Console.WriteLine("Changed Doctor");
            Console.WriteLine(curDoctor.ToString());
            Console.WriteLine("----------");
            Console.WriteLine("Press [Enter] To Continue...");
            Console.ReadLine();
        }
        static void AdminOption2()
        {
            //Add the information about the health center’s doctors, including
            //employee number, full name, one specialization for each doctor, and
            //phone
            Console.WriteLine("----------");
            Console.WriteLine("First Name: ");
            string firstName = Console.ReadLine();
            Console.WriteLine("Last Name: ");
            string lastName = Console.ReadLine();
            Console.WriteLine("Specialization ID: ");
            string specialId = Console.ReadLine();
            Console.WriteLine("Phone Number: ");
            string phoneNumber = Console.ReadLine();
            Console.WriteLine("----------");

            doctorRepository.AddDoctor(firstName, lastName, Convert.ToInt32(specialId), phoneNumber);

            Console.WriteLine("Successfully added new Doctor!");
        }
        static void AdminOption3()
        {
            //Admin can also delete (make inactive) a doctor from the health center
            Console.WriteLine("----------");
            Console.WriteLine("Which EmployeeNumber do you want to Delete?: ");
            string? employNr = Console.ReadLine();
            Console.WriteLine("----------");
            Console.WriteLine("[1] - Active");
            Console.WriteLine("[2] - Inactive");
            Console.WriteLine("----------");
            Console.WriteLine("Status: ");
            string statusInput = Console.ReadLine();
            string statusDisplayStr = "";
            if (statusInput == "1")
            {
                statusDisplayStr = "ACTIVE";
            }
            else if (statusInput == "2")
            {
                statusDisplayStr = "INACTIVE";
            }

            doctorRepository.ChangeDoctorStatus(Convert.ToInt32(employNr), statusDisplayStr);

            Doctor curDoctor = doctorRepository.GetDoctorFromEmployeeNumber(Convert.ToInt32(employNr));
            Console.WriteLine("----------");
            Console.WriteLine("Changed Doctor");
            Console.WriteLine(curDoctor.ToString());
            Console.WriteLine("----------");
            Console.WriteLine("Press [Enter] To Continue...");
            Console.ReadLine();
        }

        static void SeeAllPatientsAndVisitCosts()
        {
            patientRepository.ShowAllPatientsWithCosts();
        }

        static void HandleAdmin()
        {
            bool adminLoggedIn = true;
            while (adminLoggedIn)
            {
                Console.WriteLine("-----");
                Console.WriteLine("[1] - Add/Edit specialization and visit cost");
                Console.WriteLine("[2] - Add doctor's information and or create new doctor");
                Console.WriteLine("[3] - Delete doctor");
                Console.WriteLine("[4] - See All Bookings");
                Console.WriteLine("[5] - See All Medical Records of specific patient");
                Console.WriteLine("[6] - See all patients and their total visit costs");
                Console.WriteLine("[7] - Log out");
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
                        // See all upcoming bookings
                        Console.WriteLine("-----");
                        int doctorCount = doctorRepository.GetDoctorCount();
                        for (int i = 0; i < doctorCount; i++)
                        {
                            SeeDoctorBookings(i);
                        }
                        Console.WriteLine("-----");
                        Console.ReadLine();
                        break;
                    case "5":
                        // See list of all medical records of a specific patient
                        Console.WriteLine("-----");
                        Console.WriteLine("Enter patientId: ");
                        string pickedPatientIdStr = Console.ReadLine();
                        int pickedPatientId = int.Parse(pickedPatientIdStr);
                        patientRepository.ShowMedicalNotesForPatient(pickedPatientId);
                        Console.WriteLine("-----");
                        Console.ReadLine();
                        break;
                    case "6":
                        // See list of all patients including medical number and full name and sum of all visit costs
                        SeeAllPatientsAndVisitCosts();
                        break;
                    case "7":
                        adminLoggedIn = false;
                        break;

                    default:
                        break;
                }
                Console.WriteLine("Press [Enter] To Continue...");
                Console.ReadLine();
                Console.Clear();
            }
        }
        #endregion Admin

        #region Doctor
        static void SeeDoctorBookings(int employeeNumber)
        {
            List<Booking> bookings = bookingRepository.GetDoctorBookings(employeeNumber);
            Console.WriteLine("-----");
            foreach (Booking booking in bookings)
            {
                Console.WriteLine(booking.ToString());
            }
            Console.WriteLine("-----");
        }

        static void SeeDoctorsPatients(int employeeNumber)
        {
            SeeDoctorBookings(employeeNumber);
        }

        static void ChangeDoctorAvailability(int employeeNumber)
        {
            WeekDays dayToChange = WeekDays.Monday;
            TimeSlots timeToChange = TimeSlots.Nine;
            int bitValue = 0;

            Console.WriteLine("----------");
            Console.WriteLine("[1] - Monday");
            Console.WriteLine("[2] - Tuesday");
            Console.WriteLine("[3] - Wednesday");
            Console.WriteLine("[4] - Thursday");
            Console.WriteLine("[5] - Friday");
            Console.WriteLine("Which day's availability do you wish to change?: ");
            Console.WriteLine("Option: ");
            string dayToChangeStr = Console.ReadLine();

            switch (dayToChangeStr)
            {
                case "1":
                    dayToChange = WeekDays.Monday;
                    break;
                case "2":
                    dayToChange = WeekDays.Tuesday;
                    break;
                case "3":
                    dayToChange = WeekDays.Wednesday;
                    break;
                case "4":
                    dayToChange = WeekDays.Thursday;
                    break;
                case "5":
                    dayToChange = WeekDays.Friday;
                    break;

                default:
                    break;
            }

            Console.WriteLine("----------");
            Console.WriteLine("[1] - 09:00");
            Console.WriteLine("[2] - 09:30");
            Console.WriteLine("[3] - 10:00");
            Console.WriteLine("[4] - 10:30");
            Console.WriteLine("Which time do you wish to change?: ");
            Console.WriteLine("Option: ");
            string timeToChangeStr = Console.ReadLine();

            switch (timeToChangeStr)
            {
                case "1":
                    timeToChange = TimeSlots.Nine;
                    break;
                case "2":
                    timeToChange = TimeSlots.NineThirty;
                    break;
                case "3":
                    timeToChange = TimeSlots.Ten;
                    break;
                case "4":
                    timeToChange = TimeSlots.TenThirty;
                    break;

                default:
                    break;
            }

            AvailabilitySlot availabilitySlot = new AvailabilitySlot();
            {
                availabilitySlot.WeekDay = dayToChange;
                availabilitySlot.TimeSlot = timeToChange;
            }

            Console.WriteLine("----------");
            Console.WriteLine("[1] - Unavailable");
            Console.WriteLine("[2] - Available");
            Console.WriteLine("Option: ");
            string bitValueStr = Console.ReadLine();
            switch (bitValueStr)
            {
                case "1":
                    bitValue = 0; // FALSE
                    break;
                case "2":
                    bitValue = 1; // TRUE
                    break;

                default:
                    break;
            }

            doctorRepository.ChangeDoctorAvailability(employeeNumber, availabilitySlot, bitValue);
        }

        static void AddMedicalNotesToPastAppointment(int employeeNumber)
        {
            SeeDoctorBookings(employeeNumber);
            Console.WriteLine("-----");
            Console.WriteLine("Enter bookingId: ");
            int bookingId = int.Parse(Console.ReadLine());
            Console.WriteLine("-----");
            Console.WriteLine("Enter MedicalNote: "); // Lägg till dom olika specifika notesen
            string medicalNotes = Console.ReadLine();
            bookingRepository.AddMedicalNote(bookingId, medicalNotes);

            Console.Clear();
            SeeDoctorBookings(employeeNumber);
        }

        static void HandleDoctor()
        {
            Console.WriteLine("Enter EmployeeId: ");
            int employeeId = int.Parse(Console.ReadLine());
            bool doctorLoggedIn = true;

            Console.WriteLine("-----");
            Console.WriteLine("Welcome Doctor"); // byt till namn på doktorn senare

            while (doctorLoggedIn)
            {
                Console.WriteLine("-----");
                Console.WriteLine("[1] - See Upcoming Bookings");
                Console.WriteLine("[2] - Edit Availability");
                Console.WriteLine("[3] - See List Of Patients");
                Console.WriteLine("[4] - Add Medical Notes To Past Appointment");
                Console.WriteLine("[5] - Log out");
                Console.WriteLine("-----");
                Console.WriteLine("Option: ");
                var _ = Console.ReadLine();
                Console.Clear();
                switch (_)
                {
                    case "1":
                        // See bookings
                        SeeDoctorBookings(employeeId);
                        break;
                    case "2":
                        // Change availability - om tid bokad redan
                        ChangeDoctorAvailability(employeeId);
                        break;
                    case "3":
                        // Se lista med patienter + medical record för dom
                        SeeDoctorsPatients(employeeId);
                        break;
                    case "4":
                        // Add medical notes to past appointment
                        AddMedicalNotesToPastAppointment(employeeId);
                        break;
                    case "5":
                        doctorLoggedIn = false;
                        break;

                    default:
                        break;
                }
            }
        }
        #endregion Doctor

        #region Patient
        static void HandleBookingAppointment(int patientMedicalNumber)
        {
            // - Book an appointment to visit a doctor
            // See list of doctors with specialization and visit cost
            Console.WriteLine("-----");
            doctorRepository.SeeSpecializationAndVisitCost();
            Console.WriteLine("-----");
            Console.ReadLine();

            // Search for doctors with specific specialization
            Console.WriteLine("-----");
            Console.WriteLine("[1] - Dentist");
            Console.WriteLine("[2] - Cardiologist");
            Console.WriteLine("[3] - Psychiatrist");
            Console.WriteLine("Which doctor-type do you want to see");
            string _ = Console.ReadLine();
            int specialId = int.Parse(_);
            doctorRepository.SearchDoctorsWithSpecialization(specialId);
            Console.ReadLine();
            // Select doctor
            Console.WriteLine("Enter DoctorID: ");
            string doctorPickedIDStr = Console.ReadLine();
            int doctorPickedID = int.Parse(doctorPickedIDStr);
            Console.Clear();
            // See the doctor's available times
            Console.WriteLine("-----");
            doctorRepository.ShowDoctorAvailability(doctorPickedID);
            Console.WriteLine("-----");
            Console.ReadLine();
            // CAN ONLY BOOK IF FRIDAY. OTHERWISE SAY ITS ONLY AVAILABLE ON FRIDAYS
            // Type "Adminpass" after error message then press enter to bypass restriction
            // Pick an appointment time and book it
            DateTime todaysDate = DateTime.Today;
            if (todaysDate.DayOfWeek == DayOfWeek.Friday) // Isnt friday
            {
                AvailabilitySlot pickedTimeSlot = new AvailabilitySlot();
                pickedTimeSlot.WeekDay = WeekDays.Monday;
                pickedTimeSlot.TimeSlot = TimeSlots.Nine;

                Console.WriteLine("Pick Day: ");
                string dayPicked = Console.ReadLine();
                switch (dayPicked)
                {
                    case "monday":
                        pickedTimeSlot.WeekDay = WeekDays.Monday;
                        break;
                    case "tuesday":
                        pickedTimeSlot.WeekDay = WeekDays.Tuesday;
                        break;
                    case "wednesday":
                        pickedTimeSlot.WeekDay = WeekDays.Wednesday;
                        break;
                    case "thursday":
                        pickedTimeSlot.WeekDay = WeekDays.Thursday;
                        break;
                    case "friday":
                        pickedTimeSlot.WeekDay = WeekDays.Friday;
                        break;

                    default:
                        break;
                }

                Console.WriteLine("Pick time (9-30): ");
                string timePicked = Console.ReadLine();
                switch (timePicked)
                {
                    case "9-0":
                        pickedTimeSlot.TimeSlot = TimeSlots.Nine;
                        break;
                    case "9-30":
                        pickedTimeSlot.TimeSlot = TimeSlots.NineThirty;
                        break;
                    case "10-0":
                        pickedTimeSlot.TimeSlot = TimeSlots.Ten;
                        break;
                    case "10-30":
                        pickedTimeSlot.TimeSlot = TimeSlots.TenThirty;
                        break;

                    default:
                        break;
                }

                bookingRepository.AddNewBooking(doctorPickedID, patientMedicalNumber, pickedTimeSlot);
                Console.WriteLine("Booking complete!");
            }
            else
            {
                Console.WriteLine("CAN ONLY BOOK ON FRIDAY'S");
                _ = Console.ReadLine();
                if (_ == "adminpass")
                {
                    // Bypass the friday restriction
                    AvailabilitySlot pickedTimeSlot = new AvailabilitySlot();
                    pickedTimeSlot.WeekDay = WeekDays.Monday;
                    pickedTimeSlot.TimeSlot = TimeSlots.Nine;

                    Console.WriteLine("Pick Day: ");
                    string dayPicked = Console.ReadLine();
                    switch (dayPicked)
                    {
                        case "monday":
                            pickedTimeSlot.WeekDay = WeekDays.Monday;
                            break;
                        case "tuesday":
                            pickedTimeSlot.WeekDay = WeekDays.Tuesday;
                            break;
                        case "wednesday":
                            pickedTimeSlot.WeekDay = WeekDays.Wednesday;
                            break;
                        case "thursday":
                            pickedTimeSlot.WeekDay = WeekDays.Thursday;
                            break;
                        case "friday":
                            pickedTimeSlot.WeekDay = WeekDays.Friday;
                            break;

                        default:
                            break;
                    }

                    Console.WriteLine("Pick time (9-30): ");
                    string timePicked = Console.ReadLine();
                    switch (timePicked)
                    {
                        case "9-0":
                            pickedTimeSlot.TimeSlot = TimeSlots.Nine;
                            break;
                        case "9-30":
                            pickedTimeSlot.TimeSlot = TimeSlots.NineThirty;
                            break;
                        case "10-0":
                            pickedTimeSlot.TimeSlot = TimeSlots.Ten;
                            break;
                        case "10-30":
                            pickedTimeSlot.TimeSlot = TimeSlots.TenThirty;
                            break;

                        default:
                            break;
                    }

                    bookingRepository.AddNewBooking(doctorPickedID, patientMedicalNumber, pickedTimeSlot);
                    Console.WriteLine("Booking complete!");

                }
            }
            Console.ReadLine();

        }

        static void HandlePatient()
        {
            int patientMedicalId = 0;
            bool patientActive = true;

            // -- Done --
            // Register
            // Medical number (9), FirstName, LastName, Gender, Address, PhoneNumber, Birthday

            // Sign in (just medical number)
            
            // See all information
            // Edit information (except medicalnumber and registrationdate)
            // -- Done --



            // - Book an appointment to visit a doctor
            // See list of doctors with specialization and visit cost
            // Search for doctors with specific specialization
            // Select doctor
            // See the doctor's available times
            // Pick an appointment time and book it
            // CAN ONLY BOOK IF FRIDAY. OTHERWISE SAY ITS ONLY AVAILABLE ON FRIDAYS
            // Type "Adminpass" after error message then press enter to bypass restriction

            while (patientActive)
            {
                Console.Clear();
                Console.WriteLine("-----");
                Console.WriteLine("[1] - Sign Up");
                Console.WriteLine("[2] - Sign In");
                Console.WriteLine("[3] - Update Information");
                Console.WriteLine("[4] - Book Appointment Menu");
                Console.WriteLine("[5] - Logout");

                Console.WriteLine("Option: ");
                var _ = Console.ReadLine();
                switch (_)
                {
                    case "1":
                        patientRepository.AddNewPatient();
                        break;
                    case "2":
                        Console.WriteLine("Enter medicalID: ");
                        string medicalIdInput = Console.ReadLine();
                        patientMedicalId = Convert.ToInt32(medicalIdInput);
                        Console.WriteLine("Signed in: " +  patientMedicalId);
                        Console.ReadLine();

                        //if (patientRepository.IsPatientRegistered(patientMedicalId))
                        //{
                        //    patientMedicalId = int.Parse(medicalIdInput);
                        //}
                        //else
                        //{
                        //    Console.WriteLine("This account doesnt exist!");
                        //    Console.ReadLine();
                        //}
                        break;
                    case "3":
                        patientRepository.UpdatePatientInformation(patientMedicalId);
                        break;
                    case "4":
                        HandleBookingAppointment(patientMedicalId);
                        break;
                    case "5":
                        patientActive = false;
                        Console.ReadLine();
                        break;

                    default:
                        break;
                }

            }

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
                Console.Clear();
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
                //Console.WriteLine("Press [Enter] To Continue...");
                //Console.ReadLine();
                Console.Clear();
            }

            // Exit program
        }
    }
}
