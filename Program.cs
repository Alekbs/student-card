using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace StudentCard
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                String filePath = "students.json";
                Help();
                while (true)
                {

                    string input = GetInput("\nВведите команду: ");
                    string[] parts = input.Split(' ');

                    if (parts.Length > 0)
                    {
                        ParseCommand(parts, filePath);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла ошибка: " + ex.Message);
            }
            GetInput();
        }

        /// <summary>
        /// Парсинг команды.
        /// </summary>      
        static void ParseCommand(string[] parts, string filePath)
        {
            List<Student> students = LoadStudentsFromJson(filePath);
            string command = parts[0];
            string parameter = parts.Length > 1 ? string.Join(" ", parts.Skip(1)) : null;
            switch (command)
            {
                case "help":
                    Help();
                    break;
                case "view_all":
                    ViewAll(students);
                    break;
                case "view":
                    ViewOne(students, parameter);
                    break;
                case "edit":
                    EditStudent(students, parameter, filePath);
                    break;
                case "filter":
                    var paramDict = ParseParameters(parameter);

                    // Проверяем наличие хотя бы одного параметра
                    if (paramDict.ContainsKey("faculty") ||
                        paramDict.ContainsKey("speciality") ||
                        paramDict.ContainsKey("cource") ||
                        paramDict.ContainsKey("group"))
                    {
                        var filteredStudents = FilterStudents(
                            students,
                            paramDict.GetValueOrDefault("faculty"),
                            paramDict.GetValueOrDefault("speciality"),
                            paramDict.GetValueOrDefault("cource"),
                            paramDict.GetValueOrDefault("group")
                        );
                        if (filteredStudents.Count == 0)
                        {
                            Console.WriteLine("Таких студентов нет");
                        }
                        else
                        {
                            ViewAll(filteredStudents);
                        }
                    }
                    else
                    {
                        // Если параметры отсутствуют, отображаем всех студентов
                        Console.WriteLine("Введите параметры фильтрации.");
                    }
                    break;
                case "view_name":
                    ViewName(students);
                    break;
                case "add_student":
                    AddStudent(students, filePath);
                    break;
                default:
                    Console.WriteLine("Неправильная команда.");
                    break;
            }
        }

        /// <summary>
        /// Вывод списка команд.
        /// </summary> 
        static void Help()
        {
            Console.Write("Команды:\n\thelp - вывод списка команд\n\tview_all - отображение всех карточек студентов \n\tview {fio} - отображение карточки студента по ФИО \n\tedit {fio} - изменение карточки студента по ФИО \n\tfilter {faculty=/speciality=/cource=/group=} - список студентов по фильтру \n\tview_name - Список имен студентов \n\tadd_student - добавление нового студента");
        }
        /// <summary>
        /// Парсинг параметров.
        /// </summary>   
        static Dictionary<string, string> ParseParameters(string parameters)
        {
            var paramDict = new Dictionary<string, string>();
            if (parameters != null)
            {
                var paramPairs = parameters.Split(' ');
                foreach (var pair in paramPairs)
                {
                    var keyValue = pair.Split('=');
                    if (keyValue.Length == 2)
                    {
                        paramDict[keyValue[0]] = keyValue[1];
                    }
                }
            }
            return paramDict;
        }

        /// <summary>
        /// Вывод списка имен.
        /// </summary>   
        static void ViewName(List<Student> students)
        // Вывод списка имен
        {
            foreach (var student in students)
            {
                Console.WriteLine("FIO: " + (student.FIO ?? "N/A"));
            }
        }

        /// <summary>
        /// Вывод конкретного студента.
        /// </summary>
        static void ViewOne(List<Student> students, String fio = "John Doe")
        {
            Console.WriteLine("Информация о студенте:");
            students.Where(student => (student.FIO == fio)).ToList()[0].Display();
        }

        /// <summary>
        /// Выводит информацию о всех студентах.
        /// </summary>      
        static void ViewAll(List<Student> students)
        // Вывод всех студентов
        {
            Console.WriteLine("Информация о студентах:");
            foreach (Student student in students)
            {
                student.Display();
            }

        }

        /// <summary>
        /// Добавление нового студента.
        /// </summary>   
        static void AddStudent(List<Student> students, String filePath)
        {

            Student student = new Student();
            student.FIO = GetInput("Введите ФИО: ");

            student.Curriculum = new Curriculum();
            student.Curriculum.Faculty = GetInput("Введите факультет: ");
            student.Curriculum.Speciality = GetInput("Введите специальность: ");
            student.Curriculum.Cource = GetInput("Введите курс: ");
            student.Curriculum.Group = GetInput("Введите группу: ");

            student.Address = new Address();
            student.Address.City = GetInput("Введите город: ");
            student.Address.PostIndex = GetInput("Введите почтовый индекс: ");
            student.Address.Street = GetInput("Введите улицу: ");

            student.Contacts = new Contacts();
            student.Contacts.Phone = GetInput("Введите телефон: ");
            student.Contacts.Email = GetInput("Введите email: ");

            student.Display();
            students.Add(student);

            SaveStudentsToJson(students, filePath);
            Console.WriteLine("Студент добавлен.");
        }

        /// <summary>
        /// Проверка ввода на пустое значение.
        /// </summary> 
        static string GetInput(string prompt = null)
        {
            string input;
            do
            {
                Console.Write(prompt);
                input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Ввод не должен быть пустым. Пожалуйста, попробуйте снова.");
                    input = GetInput(prompt);
                }
            } while (string.IsNullOrWhiteSpace(input));
            return input;
        }

        /// <summary>
        /// Фильтрация студентов по заданным параметрам.
        /// </summary>   
        static List<Student> FilterStudents(List<Student> students, string faculty = null, string speciality = null, string cource = null, string group = null)
        // Фильтрация по параметру адреса
        {
            // Применяем фильтрацию по переданным критериям
            var filteredStudents = students.Where(student =>
                (faculty == null || student.Curriculum?.Faculty == faculty) &&
                (speciality == null || student.Curriculum?.Speciality == speciality) &&
                (cource == null || student.Curriculum?.Cource == cource) &&
                (group == null || student.Curriculum?.Group == group)
            ).ToList();
            return filteredStudents;
        }

        /// <summary>
        /// Редактирование карточки студента.
        /// </summary>   
        static void EditStudent(List<Student> students, string studentName, String filePath)
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
            Console.WriteLine("Введите параметры для изменения (например, street Pushkina), для завершения введите 'end'\n");
            Console.WriteLine("Изменяемые поля:\n\tfacutlty\n\tspeciality\n\tcource\n\tgroup\n\tcity\n\tpostindex\n\tstreet\n\tphone\n\temail");

            while (true)
            {
                Console.Write("\nВведите команду: ");
                string input = GetInput();
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
            SaveStudentsToJson(students, filePath);
            Console.WriteLine("Изменения сохранены.");
        }

        /// <summary>
        /// Сохраняет список студентов в файл.
        /// </summary>      
        static void SaveStudentsToJson(List<Student> students, string filePath)
        // Сохранение файла
        {
            string json = JsonConvert.SerializeObject(students, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        /// <summary>
        /// Загрузка списка студентов из файла.
        /// </summary>      
        static List<Student> LoadStudentsFromJson(string filePath)
        // Сохранение файла
        {
            string json = File.ReadAllText(filePath);
            List<Student> students = JsonConvert.DeserializeObject<List<Student>>(json);
            return students;
        }
    }
}
