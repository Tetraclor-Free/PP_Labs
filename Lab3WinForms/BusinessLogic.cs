using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab3WinForms
{
    public class BusinessLogic
    {
        public static List<string> allowDepartaments = new List<string>();
        IDataSource dataSource;

        public BusinessLogic(IDataSource dataSource)
        {
            this.dataSource = dataSource;
            // Перечисляем все существующие отделы
            allowDepartaments = new List<string>()
            {
                "Разработка",
                "Исследование",
                "Маркетинг",
                "Бухгалтерия",
                "Юридический",
                "Управление"
            };
        }

        public EmployeeRecord Save(EmployeeRecord record)
        {
            // Проверка что в фио не менее трех слов
            if (record.fullname.Split(' ').Length < 3)
            {
                throw new ArgumentException("ФИО должно содержать не меньше трех слов");
            }

            // Проверяем что отдел существует
            if (allowDepartaments.Contains(record.department) == false)
            {
                throw new ArgumentException(@"Отдел один из следующих: " + string.Join(", ", allowDepartaments));
            }

            // Только после всех проверок сохраняем запись
            return dataSource.Save(record);
        }

        public EmployeeRecord Get(int id)
        {
            return dataSource.Get(id);
        }
        public bool Delete(int id)
        {
            return dataSource.Delete(id);
        }
        public List<EmployeeRecord> GetAll()
        {
            var records = dataSource.GetAll();
            // Сортируем указывая как сравнивать два элемента
            records.Sort(RecordCompare);
            return records;
        }

        /// <summary>
        /// Метод для создания отчета
        /// </summary>
        /// <param name="salaryFrom"></param>
        /// <param name="salaryTo"></param>
        /// <returns></returns>
        public string GetReport(int salaryFrom, int salaryTo)
        {
            var result = GetAll() // Берем все эелменты 
                .Where(v => v.salary >= salaryFrom && v.salary <= salaryTo) // Выбираем те из них у которых зарплата находится между переданными значениями
                .GroupBy(v => v.department) // Группировка по отделам
                .Select(v => (v.Key, v.Count())); // Подсчет элементов по каждым из групп

            // Формируем отчет
            return $"Групировка сотрудников по отделам, с окладом от {salaryFrom} до {salaryTo}:\n" + string.Join("\n", result);
        }

        private int RecordCompare(EmployeeRecord a, EmployeeRecord b)
        {
            // Сначала сравниваем по департаменту
            var departamentCompare = a.department.CompareTo(b.department);
            // Если департаммент одинаоквы то сравниваем по ФИО
            return departamentCompare == 0 ? a.fullname.CompareTo(b.fullname) : departamentCompare;
        }
    }
}
