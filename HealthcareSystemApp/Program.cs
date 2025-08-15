using System;
using System.Collections.Generic;
using System.Linq;

// Generic repository for entity management
public class Repository<T>
{
    private List<T> items = new List<T>();

    public void Add(T item) => items.Add(item);
    public List<T> GetAll() => new List<T>(items);
    public T? GetById(Func<T, bool> predicate) => items.FirstOrDefault(predicate);
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

// Patient class
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

    public override string ToString() => $"ID: {Id}, Name: {Name}, Age: {Age}, Gender: {Gender}";
}

// Prescription class
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

    public override string ToString() => $"Prescription ID: {Id}, Medication: {MedicationName}, Date Issued: {DateIssued:yyyy-MM-dd}";
}

// HealthSystemApp class
public class HealthSystemApp
{
    private Repository<Patient> _patientRepo = new Repository<Patient>();
    private Repository<Prescription> _prescriptionRepo = new Repository<Prescription>();
    private Dictionary<int, List<Prescription>> _prescriptionMap = new Dictionary<int, List<Prescription>>();

    public void SeedData()
    {
        // Add patients
        _patientRepo.Add(new Patient(1, "Alice Smith", 30, "Female"));
        _patientRepo.Add(new Patient(2, "Bob Johnson", 45, "Male"));
        _patientRepo.Add(new Patient(3, "Carol Lee", 28, "Female"));

        // Add prescriptions
        _prescriptionRepo.Add(new Prescription(1, 1, "Amoxicillin", DateTime.Now.AddDays(-10)));
        _prescriptionRepo.Add(new Prescription(2, 1, "Ibuprofen", DateTime.Now.AddDays(-5)));
        _prescriptionRepo.Add(new Prescription(3, 2, "Paracetamol", DateTime.Now.AddDays(-7)));
        _prescriptionRepo.Add(new Prescription(4, 3, "Cetirizine", DateTime.Now.AddDays(-2)));
        _prescriptionRepo.Add(new Prescription(5, 2, "Metformin", DateTime.Now.AddDays(-1)));
    }

    public void BuildPrescriptionMap()
    {
        _prescriptionMap.Clear();
        foreach (var prescription in _prescriptionRepo.GetAll())
        {
            if (!_prescriptionMap.ContainsKey(prescription.PatientId))
                _prescriptionMap[prescription.PatientId] = new List<Prescription>();
            _prescriptionMap[prescription.PatientId].Add(prescription);
        }
    }

    public void PrintAllPatients()
    {
        Console.WriteLine("All Patients:");
        foreach (var patient in _patientRepo.GetAll())
        {
            Console.WriteLine(patient);
        }
    }

    public List<Prescription> GetPrescriptionsByPatientId(int patientId)
    {
        return _prescriptionMap.ContainsKey(patientId) ? _prescriptionMap[patientId] : new List<Prescription>();
    }

    public void PrintPrescriptionsForPatient(int patientId)
    {
        var patient = _patientRepo.GetById(p => p.Id == patientId);
        if (patient == null)
        {
            Console.WriteLine($"No patient found with ID {patientId}.");
            return;
        }
        Console.WriteLine($"Prescriptions for {patient.Name} (ID: {patient.Id}):");
        var prescriptions = GetPrescriptionsByPatientId(patientId);
        if (prescriptions.Count == 0)
        {
            Console.WriteLine("No prescriptions found.");
        }
        else
        {
            foreach (var prescription in prescriptions)
            {
                Console.WriteLine(prescription);
            }
        }
    }
}

// Program entry point
class Program
{
    static void Main(string[] args)
    {
        var app = new HealthSystemApp();
        app.SeedData();
        app.BuildPrescriptionMap();
        app.PrintAllPatients();
        Console.WriteLine();
        // For demo, print prescriptions for patient with ID 2
        app.PrintPrescriptionsForPatient(2);
    }
}
