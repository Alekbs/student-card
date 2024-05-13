using Newtonsoft.Json;

namespace StudentCard
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string json = File.ReadAllText("Students.json");

                List<Student> students = JsonConvert.DeserializeObject<List<Student>>(json);
                Console.Write("Команды: \n\tview - отображение всех карточек студентов \n\t\t{edit} - изменение карточки студента по ФИО \n\tfilter {faculty/speciality/cource/group} - список студентов по фильтру ");
                while(true)
                {   
                    Console.Write("Введите команду: ");
                    string input = Console.ReadLine();
                    string[] parts = input.Split(' ');

                    if(parts.Length > 0)
                    {
                        string command = parts[0];
                        string parameter = parts.Length > 1 ? parts[1] : null;
                        ParseCommand(command, students, parameter);
                    }
                    else
                    {
                        Console.WriteLine("Пустая команда.");
                    }
                }
                
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла ошибка: " + ex.Message);
            }

            Console.ReadLine();
        }
    
        static void ParseCommand(string command, List<Student> students, string parameter)
        // Парсинг команды
        {
            switch (command)
            {
                case "view":
                    if(parameter == "edit")
                    {
                        Console.Write("Введите ФИО студента для редактирования: ");
                        string studentName = Console.ReadLine();
                        EditStudent(students, studentName);
                        break;
                    }
                    else {
                        view(students);
                        break;
                    }
                case "filter":
                    if(parameter != null)
                    {
                        switch(parameter)
                        {
                            case "faculty":
                                Console.Write("Введите факультет: ");
                                string faculty = Console.ReadLine();
                                viewName(FilterStudents(students, faculty: faculty));
                                break;
                            case "speciality":
                                Console.Write("Введите специальность: ");
                                string speciality = Console.ReadLine();
                                viewName(FilterStudents(students, speciality: speciality));
                                break;
                            case "cource":
                                Console.Write("Введите курс: ");
                                string course = Console.ReadLine();
                                viewName(FilterStudents(students, course: course));
                                break;
                            case "group":
                                Console.Write("Введите группу: ");
                                string group = Console.ReadLine();
                                viewName(FilterStudents(students, group: group));
                                break;
                            default:
                                Console.WriteLine("Недопустимый параметр для фильтрации.");
                                break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Параметр для фильтрации отсутствует.");
                    }
                    break;
                default:
                    Console.WriteLine("Неизвестная команда");
                    break;
            } 
        }
        static void viewName(List<Student> students)
        // Вывод списка имен
        {
            foreach (var student in students)
            {
                Console.WriteLine("FIO: " + (student.FIO ?? "N/A"));
            }
        }
        static void view(List<Student> students)
        // Вывод карточки студента
        {
            // Вывод информации на экран
            Console.WriteLine("Информация о студентах:");
            foreach (var student in students)
            {
                
                Console.WriteLine("FIO: " + (student.FIO ?? "N/A"));
                Console.WriteLine("Curriculum:");
                Console.WriteLine("    Faculty: " + (student.Curriculum?.Faculty ?? "N/A"));
                Console.WriteLine("    Speciality: " + (student.Curriculum?.Speciality ?? "N/A"));
                Console.WriteLine("    Cource: " + (student.Curriculum?.Cource ?? "N/A"));
                Console.WriteLine("    Group: " + (student.Curriculum?.Group ?? "N/A"));
                Console.WriteLine("Address:");
                Console.WriteLine("    City: " + (student.Address?.City ?? "N/A"));
                Console.WriteLine("    PostIndex: " + (student.Address?.PostIndex ?? "N/A"));
                Console.WriteLine("    Street: " + (student.Address?.Street ?? "N/A"));
                Console.WriteLine("Contacts:");
                Console.WriteLine("    Phone: " + (student.Contacts?.Phone ?? "N/A"));
                Console.WriteLine("    Email: " + (student.Contacts?.Email ?? "N/A") + "\n");
            }
                    
        }
        static List<Student> FilterStudents(List<Student> students, string faculty = null, string speciality = null, string course = null, string group = null)
        // Фильтрация по параметру адреса
        {
            // Применяем фильтрацию по переданным критериям
            var filteredStudents = students.Where(student =>
                (faculty == null || student.Curriculum?.Faculty == faculty) &&
                (speciality == null || student.Curriculum?.Speciality == speciality) &&
                (course == null || student.Curriculum?.Cource == course) &&
                (group == null || student.Curriculum?.Group == group)
            ).ToList();
            return filteredStudents;
        }
        static void EditStudent(List<Student> students, string studentName)
        // Изменение карточки студента
        {
            // Находим студента по ФИО
            Student studentToEdit = students.FirstOrDefault(student => student.FIO == studentName);
            if (studentToEdit == null)
            {
                Console.WriteLine("Студент с таким ФИО не найден.");
                return;
            }

            Console.WriteLine($"Выбран студент: {studentToEdit.FIO}");
            Console.WriteLine("Введите параметры для изменения (например, street Pushkina), для завершения введите 'end'");
            while (true)
            {
                string input = Console.ReadLine();
                if (input.ToLower() == "end")
                    break;

                string[] parts = input.Split(' ');
                if (parts.Length != 2)
                {
                    Console.WriteLine("Неверный формат ввода. Попробуйте снова.");
                    continue;
                }

                string parameter = parts[0].ToLower();
                string value = parts[1];

                // Изменяем соответствующий параметр студента
                switch (parameter)
                {
                    case "facutlty":
                        studentToEdit.Curriculum.Faculty = value;
                        break;
                    case "speciality":
                        studentToEdit.Curriculum.Speciality = value;
                        break;
                    case "cource":
                        studentToEdit.Curriculum.Cource = value;
                        break;
                    case "group":
                        studentToEdit.Curriculum.Group = value;
                        break;
                    case "city":
                        studentToEdit.Address.City = value;
                        break;
                    case "postindex":
                        studentToEdit.Address.PostIndex = value;
                        break;
                    case "street":
                        studentToEdit.Address.Street = value;
                        break;
                    case "phone":
                        studentToEdit.Contacts.Phone = value;
                        break;
                    case "email":
                        studentToEdit.Contacts.Email = value;
                        break;

                    default:
                        Console.WriteLine("Недопустимый параметр для редактирования.");
                        break;
                }
            }
            SaveStudentsToJson(students, "Students.json");
            Console.WriteLine("Изменения сохранены.");
        }
        
        static void SaveStudentsToJson(List<Student> students, string filePath)
        // Сохранение файла
        {
            string json = JsonConvert.SerializeObject(students, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }
    }


    public class Student
    {
        public string FIO { get; set; }
        public Curriculum Curriculum { get; set; }
        public Address Address { get; set; }
        public Contacts Contacts { get; set; }
    }

    public class Curriculum
    {
        public string Faculty { get; set; }
        public string Speciality { get; set; }
        public string Cource { get; set; }
        public string Group { get; set; }
    }

    public class Address
    {
        public string City { get; set; }
        public string PostIndex { get; set; }
        public string Street { get; set; }
    }

    public class Contacts
    {
        public string Phone { get; set; }
        public string Email { get; set; }
    }
}
