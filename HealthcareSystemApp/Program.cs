using System;
using System.Collections.Generic;
using System.Linq;

namespace HealthcareManagementSystem
{
    // Generic Repository for Entity Management
    public class Repository<T>
    {
        private List<T> items;

        public Repository()
        {
            items = new List<T>();
        }

        public void Add(T item)
        {
            items.Add(item);
        }

        public List<T> GetAll()
        {
            return items;
        }

        public T? GetById(Func<T, bool> predicate)
        {
            return items.FirstOrDefault(predicate);
        }

        public bool Remove(Func<T, bool> predicate)
        {
            var item = items.FirstOrDefault(predicate);
            if (item != null)
            {
                items.Remove(item);
                return true;
            }
            return false;
        }
    }

    // Patient Class
    public class Patient
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }

        public Patient(int id, string name, int age, string gender)
        {
            Id = id;
            Name = name;
            Age = age;
            Gender = gender;
        }

        public override string ToString()
        {
            return $"Patient ID: {Id}, Name: {Name}, Age: {Age}, Gender: {Gender}";
        }
    }

    // Prescription Class
    public class Prescription
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string MedicationName { get; set; }
        public DateTime DateIssued { get; set; }

        public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
        {
            Id = id;
            PatientId = patientId;
            MedicationName = medicationName;
            DateIssued = dateIssued;
        }

        public override string ToString()
        {
            return $"Prescription ID: {Id}, Patient ID: {PatientId}, Medication: {MedicationName}, Date Issued: {DateIssued:yyyy-MM-dd}";
        }
    }

    // Main Health System Application Class
    public class HealthSystemApp
    {
        private Repository<Patient> _patientRepo;
        private Repository<Prescription> _prescriptionRepo;
        private Dictionary<int, List<Prescription>> _prescriptionMap;

        public HealthSystemApp()
        {
            _patientRepo = new Repository<Patient>();
            _prescriptionRepo = new Repository<Prescription>();
            _prescriptionMap = new Dictionary<int, List<Prescription>>();
        }

        public void SeedData()
        {
            Console.WriteLine("=== Seeding Data ===");
            
            // Add patients
            _patientRepo.Add(new Patient(1, "John Smith", 45, "Male"));
            _patientRepo.Add(new Patient(2, "Sarah Johnson", 32, "Female"));
            _patientRepo.Add(new Patient(3, "Michael Brown", 58, "Male"));

            // Add prescriptions
            _prescriptionRepo.Add(new Prescription(1, 1, "Lisinopril 10mg", DateTime.Now.AddDays(-10)));
            _prescriptionRepo.Add(new Prescription(2, 1, "Metformin 500mg", DateTime.Now.AddDays(-5)));
            _prescriptionRepo.Add(new Prescription(3, 2, "Ibuprofen 400mg", DateTime.Now.AddDays(-3)));
            _prescriptionRepo.Add(new Prescription(4, 2, "Amoxicillin 250mg", DateTime.Now.AddDays(-1)));
            _prescriptionRepo.Add(new Prescription(5, 3, "Atorvastatin 20mg", DateTime.Now.AddDays(-7)));

            Console.WriteLine("Data seeding completed successfully!");
            Console.WriteLine();
        }

        public void BuildPrescriptionMap()
        {
            Console.WriteLine("=== Building Prescription Map ===");
            
            // Clear existing map
            _prescriptionMap.Clear();

            // Group prescriptions by PatientId
            var prescriptions = _prescriptionRepo.GetAll();
            foreach (var prescription in prescriptions)
            {
                if (!_prescriptionMap.ContainsKey(prescription.PatientId))
                {
                    _prescriptionMap[prescription.PatientId] = new List<Prescription>();
                }
                _prescriptionMap[prescription.PatientId].Add(prescription);
            }

            Console.WriteLine($"Prescription map built successfully! Mapped {_prescriptionMap.Count} patients.");
            Console.WriteLine();
        }

        public List<Prescription> GetPrescriptionsByPatientId(int patientId)
        {
            if (_prescriptionMap.ContainsKey(patientId))
            {
                return _prescriptionMap[patientId];
            }
            return new List<Prescription>();
        }

        public void PrintAllPatients()
        {
            Console.WriteLine("=== All Patients ===");
            var patients = _patientRepo.GetAll();
            
            if (patients.Count == 0)
            {
                Console.WriteLine("No patients found.");
                return;
            }

            foreach (var patient in patients)
            {
                Console.WriteLine(patient.ToString());
            }
            Console.WriteLine();
        }

        public void PrintPrescriptionsForPatient(int patientId)
        {
            Console.WriteLine($"=== Prescriptions for Patient ID: {patientId} ===");
            
            // First, get patient details
            var patient = _patientRepo.GetById(p => p.Id == patientId);
            if (patient == null)
            {
                Console.WriteLine($"Patient with ID {patientId} not found.");
                return;
            }

            Console.WriteLine($"Patient: {patient.Name}");
            Console.WriteLine("Prescriptions:");

            // Get prescriptions from the map
            var prescriptions = GetPrescriptionsByPatientId(patientId);
            
            if (prescriptions.Count == 0)
            {
                Console.WriteLine("No prescriptions found for this patient.");
                return;
            }

            foreach (var prescription in prescriptions)
            {
                Console.WriteLine($"  - {prescription.ToString()}");
            }
            Console.WriteLine();
        }

        public void PrintAllPrescriptions()
        {
            Console.WriteLine("=== All Prescriptions ===");
            var prescriptions = _prescriptionRepo.GetAll();
            
            if (prescriptions.Count == 0)
            {
                Console.WriteLine("No prescriptions found.");
                return;
            }

            foreach (var prescription in prescriptions)
            {
                Console.WriteLine(prescription.ToString());
            }
            Console.WriteLine();
        }
    }

    // Main Program
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Healthcare Management System");
            Console.WriteLine("============================");
            Console.WriteLine();

            // Instantiate HealthSystemApp
            HealthSystemApp healthSystem = new HealthSystemApp();

            // Call SeedData()
            healthSystem.SeedData();

            // Call BuildPrescriptionMap()
            healthSystem.BuildPrescriptionMap();

            // Print all patients
            healthSystem.PrintAllPatients();

            // Print all prescriptions for reference
            healthSystem.PrintAllPrescriptions();

            // Select one PatientId and display all prescriptions for that patient
            // Let's display prescriptions for Patient ID 1 (John Smith)
            healthSystem.PrintPrescriptionsForPatient(1);

            // Display prescriptions for Patient ID 2 (Sarah Johnson)
            healthSystem.PrintPrescriptionsForPatient(2);

            // Display prescriptions for Patient ID 3 (Michael Brown)
            healthSystem.PrintPrescriptionsForPatient(3);

            // Demonstrate error handling - try to get prescriptions for non-existent patient
            healthSystem.PrintPrescriptionsForPatient(999);

            Console.WriteLine("System demonstration completed!");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}