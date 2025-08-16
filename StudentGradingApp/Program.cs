using System;
using System.Collections.Generic;
using System.IO;

namespace StudentGradingSystem
{
    // Student Class
    public class Student
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public int Score { get; set; }

        public Student(int id, string fullName, int score)
        {
            Id = id;
            FullName = fullName;
            Score = score;
        }

        public string GetGrade()
        {
            if (Score >= 80 && Score <= 100)
                return "A";
            else if (Score >= 70 && Score <= 79)
                return "B";
            else if (Score >= 60 && Score <= 69)
                return "C";
            else if (Score >= 50 && Score <= 59)
                return "D";
            else
                return "F";
        }
    }

    // Custom Exception Classes
    public class InvalidScoreFormatException : Exception
    {
        public InvalidScoreFormatException(string message) : base(message) { }
    }

    public class MissingFieldException : Exception
    {
        public MissingFieldException(string message) : base(message) { }
    }

    // StudentResultProcessor Class
    public class StudentResultProcessor
    {
        public List<Student> ReadStudentsFromFile(string inputFilePath)
        {
            List<Student> students = new List<Student>();
            
            using (StreamReader reader = new StreamReader(inputFilePath))
            {
                string line;
                int lineNumber = 0;
                
                while ((line = reader.ReadLine()) != null)
                {
                    lineNumber++;
                    
                    // Skip empty lines
                    if (string.IsNullOrWhiteSpace(line))
                        continue;
                    
                    string[] fields = line.Split(',');
                    
                    // Validate number of fields
                    if (fields.Length != 3)
                    {
                        throw new MissingFieldException($"Line {lineNumber}: Expected 3 fields (ID, Name, Score) but found {fields.Length} fields.");
                    }
                    
                    // Trim whitespace from all fields
                    for (int i = 0; i < fields.Length; i++)
                    {
                        fields[i] = fields[i].Trim();
                    }
                    
                    // Validate that fields are not empty
                    if (string.IsNullOrWhiteSpace(fields[0]) || 
                        string.IsNullOrWhiteSpace(fields[1]) || 
                        string.IsNullOrWhiteSpace(fields[2]))
                    {
                        throw new MissingFieldException($"Line {lineNumber}: One or more fields are empty or contain only whitespace.");
                    }
                    
                    // Try to parse ID
                    int id;
                    if (!int.TryParse(fields[0], out id))
                    {
                        throw new InvalidScoreFormatException($"Line {lineNumber}: Student ID '{fields[0]}' is not a valid integer.");
                    }
                    
                    string fullName = fields[1];
                    
                    // Try to parse score
                    int score;
                    if (!int.TryParse(fields[2], out score))
                    {
                        throw new InvalidScoreFormatException($"Line {lineNumber}: Score '{fields[2]}' is not a valid integer.");
                    }
                    
                    // Validate score range (optional - you might want scores to be 0-100)
                    if (score < 0 || score > 100)
                    {
                        throw new InvalidScoreFormatException($"Line {lineNumber}: Score {score} is out of valid range (0-100).");
                    }
                    
                    // Create and add student
                    Student student = new Student(id, fullName, score);
                    students.Add(student);
                }
            }
            
            return students;
        }

        public void WriteReportToFile(List<Student> students, string outputFilePath)
        {
            using (StreamWriter writer = new StreamWriter(outputFilePath))
            {
                writer.WriteLine("STUDENT GRADE REPORT");
                writer.WriteLine("===================");
                writer.WriteLine();
                
                foreach (Student student in students)
                {
                    string reportLine = $"{student.FullName} (ID: {student.Id}): Score = {student.Score}, Grade = {student.GetGrade()}";
                    writer.WriteLine(reportLine);
                    Console.WriteLine(reportLine); // Also display to console
                }
                
                writer.WriteLine();
                writer.WriteLine($"Total students processed: {students.Count}");
                Console.WriteLine($"\nTotal students processed: {students.Count}");
            }
        }
    }

    // Main Program Class
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Student Grading System");
            Console.WriteLine("=====================");
            
            // File paths - you can modify these as needed
            string inputFilePath = "students.txt";
            string outputFilePath = "student_report.txt";
            
            StudentResultProcessor processor = new StudentResultProcessor();
            
            try
            {
                Console.WriteLine($"Reading student data from: {inputFilePath}");
                
                // Read students from file
                List<Student> students = processor.ReadStudentsFromFile(inputFilePath);
                
                Console.WriteLine($"Successfully read {students.Count} student records.");
                Console.WriteLine("\nGenerating report...\n");
                
                // Write report to file
                processor.WriteReportToFile(students, outputFilePath);
                
                Console.WriteLine($"\nReport successfully written to: {outputFilePath}");
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Error: Input file not found - {ex.Message}");
                Console.WriteLine("Please ensure the input file exists and the path is correct.");
            }
            catch (InvalidScoreFormatException ex)
            {
                Console.WriteLine($"Error: Invalid score format - {ex.Message}");
            }
            catch (MissingFieldException ex)
            {
                Console.WriteLine($"Error: Missing or incomplete data - {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Error: Access denied - {ex.Message}");
                Console.WriteLine("Please check file permissions.");
            }
            catch (DirectoryNotFoundException ex)
            {
                Console.WriteLine($"Error: Directory not found - {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error occurred: {ex.Message}");
                Console.WriteLine($"Error type: {ex.GetType().Name}");
            }
            
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
