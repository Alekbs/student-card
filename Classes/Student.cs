using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentCard
{
    internal class Student
    {
        public string FIO { get; set; }
        public Curriculum Curriculum { get; set; }
        public Address Address { get; set; }
        public Contacts Contacts { get; set; }

        /// <summary>
        /// Выводит информацию о студенте.
        /// </summary>      
        public void Display()

        {
            // Вывод информации на экран
            Console.WriteLine("FIO: " + (FIO ?? "N/A"));
            Console.WriteLine("Curriculum:");
            Console.WriteLine("    Faculty: " + (Curriculum?.Faculty ?? "N/A"));
            Console.WriteLine("    Speciality: " + (Curriculum?.Speciality ?? "N/A"));
            Console.WriteLine("    Cource: " + (Curriculum?.Cource ?? "N/A"));
            Console.WriteLine("    Group: " + (Curriculum?.Group ?? "N/A"));
            Console.WriteLine("Address:");
            Console.WriteLine("    City: " + (Address?.City ?? "N/A"));
            Console.WriteLine("    PostIndex: " + (Address?.PostIndex ?? "N/A"));
            Console.WriteLine("    Street: " + (Address?.Street ?? "N/A"));
            Console.WriteLine("Contacts:");
            Console.WriteLine("    Phone: " + (Contacts?.Phone ?? "N/A"));
            Console.WriteLine("    Email: " + (Contacts?.Email ?? "N/A") + "\n");

        }
    }
}
