using System;

namespace SEMS.Models
{
    public class Employee
    {
        public int Id { get; set; }                // Unique Employee ID
        public string Name { get; set; }           // Employee Name
        public string Department { get; set; }     // Department (HR, IT, Finance, etc.)
        public double Salary { get; set; }         // Salary
        public DateTime JoiningDate { get; set; }  // Date of Joining
        public string ImagePath { get; set; }      // Path to Profile Picture
        public bool IsDeleted { get; set; }        // Soft Delete Flag
    }
}