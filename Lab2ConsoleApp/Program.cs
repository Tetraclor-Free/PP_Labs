using System;
using System.IO;

namespace Lab2ConsoleApp
{
    class Program
    {
        static BusinessLogic logic;

        static void Main(string[] args)
        {
            var path = @"..\..\..\bd.bin";
            IDataSource dataSource = new FileDataSource(path);
            logic = new BusinessLogic(dataSource);


            logic.Save(new SampleEmployeeRecord("Александр Ф. Ю.;Разработчик;Разработка;5555") { id = 1});

            bool exit = false;
            while (!exit)
            {
                PrintMenu();
                string command = Console.ReadLine();
                switch (command)
                {
                    case "1":
                        AddRecord();
                        break;
                    case "2":
                        PrintList();
                        break;
                    case "3":
                        ChangeRecord();
                        break;
                    case "4":
                        DeleteRecord();
                        break;
                    case "5":
                        CreateReport();
                        break;
                    case "6":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Неизвестная команда");
                        break;
                }
            }
        }

        static void PrintMenu()
        {
            var menu = @"
1) Добавить запись (выбор подвида записи и ввод данных)
2) Просмотреть записи (список записей, отсортированных по заданному критерию)
3) Изменить запись (поиск записи по номеру/идентификатору и ввод измененной
версии записи)
4) Удалить запись (поиск записи по номеру/идентификатору и подтверждение
удаления)
5) Составить отчет
6) Выход. Выход из программы
";
            Console.Write(menu);
        }

        static void CreateReport()
        {
            Console.WriteLine("Составление отчета");
            var from = GetNumberAnswer("Укажите оклад от", int.MaxValue);
            var to = GetNumberAnswer("Укажите оклад до", int.MaxValue);
            var result = logic.GetReport(from, to);
            Console.WriteLine(result);
            Console.WriteLine("Укажите путь сохранения отчета");
            var path = Console.ReadLine();
            File.WriteAllText(path, result);
        }

        static void AddRecord()
        {
            var ans = GetNumberAnswer("Выбрать подвид записи: \n1)Обычный сотрудник\n2)Временный работник\n3)Стажер", 3);
            if (ans == -1) return;

            AddOrUpdateFromConsole(ans);
        }

        static void AddOrUpdateFromConsole(int recordType, int id = 0)
        {
            Console.Write("Введите через точку с запятой значения следующих полей: ФИО;Должность;Отдел;Оклад");
            EmployeeRecord record = null;
            try
            {
                if (recordType == 1)
                {
                    Console.WriteLine();
                    record = new SampleEmployeeRecord(Console.ReadLine());
                }
                else if (recordType == 2)
                {
                    Console.WriteLine(";Дата оконачания договора");
                    record = new TempWorkerRecord(Console.ReadLine());
                }
                else if (recordType == 3)
                {
                    Console.WriteLine(";Учебное заведение");
                    record = new TraineeRecord(Console.ReadLine());
                }
                record.id = id;
                Console.WriteLine($"Добавлена/Изменена запись:\n{logic.Save(record)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка заполнения данных: " + ex.Message);
                return;
            }
        }

        static void PrintList()
        {
            var records = logic.GetAll();
            if(records.Count == 0)
            {
                Console.WriteLine("Записей нет");
                return;
            }
            var str = string.Join("\n", records);
            Console.WriteLine(str);
        }

        static void ChangeRecord()
        {
            var id = GetNumberAnswer("Изменение записи, введите идентификатор записи", int.MaxValue);
            if (id == -1) return;
            var record = logic.Get(id);
            if (record == null)
            {
                Console.WriteLine("По переданному идентификатору запись не найдена");
                return;
            }
            Console.WriteLine($"Изменение записи:{record}");
            AddOrUpdateFromConsole(record.GetRecordType(), id);
        }

        static void DeleteRecord()
        {
            var id = GetNumberAnswer("Удаление записи, введите идентификатор записи", int.MaxValue);
            if (id == -1) return;
            var record = logic.Get(id);
            if (record == null)
            {
                Console.WriteLine("По переданному идентификатору запись не найдена");
                return;
            }
            var ans = GetNumberAnswer($"Вы точно желаете удалить эту запись? 1.Да  2.Нет\n{record}", 2);
            if (ans == -1) return;
            if (ans == 2) return;
            logic.Delete(id);
        }

        static int GetNumberAnswer(string message, int maxNum)
        {
            Console.WriteLine(message);

            var ans = Console.ReadLine();

            if(int.TryParse(ans, out int result) && maxNum >= result && result > 0)
            {
                return result;
            }
            else
            {
                Console.WriteLine("Неизвестная команда");
                return -1;
            }
        }
    }
}
