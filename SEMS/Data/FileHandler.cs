using System;
using System.Collections.Generic;
using System.IO;
using SEMS.Models;

namespace SEMS.Data
{
    public static class FileHandler
    {
        private static string filePath = "employees.txt";

        public static void Save(List<Employee> employees)
        {
            List<string> lines = new List<string>();

            foreach (var emp in employees)
            {
                // ✅ Include ImagePath in saved line
                string line = $"{emp.Id},{emp.Name},{emp.Department},{emp.Salary},{emp.JoiningDate},{emp.ImagePath},{emp.IsDeleted}";
                lines.Add(line);
            }

            File.WriteAllLines(filePath, lines);
        }

        public static List<Employee> Load()
        {
            List<Employee> employees = new List<Employee>();

            if (!File.Exists(filePath))
                return employees;

            var lines = File.ReadAllLines(filePath);

            foreach (var line in lines)
            {
                var parts = line.Split(',');

                // ✅ Ensure we have at least 7 parts (including ImagePath)
                if (parts.Length >= 7)
                {
                    employees.Add(new Employee
                    {
                        Id = int.Parse(parts[0]),
                        Name = parts[1],
                        Department = parts[2],
                        Salary = double.Parse(parts[3]),
                        JoiningDate = DateTime.Parse(parts[4]),
                        ImagePath = parts[5],          // ✅ Load ImagePath
                        IsDeleted = bool.Parse(parts[6])
                    });
                }
            }

            return employees;
        }
    }
}