using System;
using System.Collections.Generic;

namespace Lab1ConsoleApp
{
    public class BusinessLogic
    {
        List<string> allowDepartaments = new List<string>();
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
            if(allowDepartaments.Contains(record.department) == false)
            {
                throw new ArgumentException(@"Отдел один из следующих: " + string.Join(", ", allowDepartaments));
            }

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

        private int RecordCompare(EmployeeRecord a, EmployeeRecord b)
        {
            // Сначала сравниваем по департаменту
            var departamentCompare = a.department.CompareTo(b.department);
            // Если департаммент одинаоквы то сравниваем по ФИО
            return departamentCompare == 0 ? a.fullname.CompareTo(b.fullname) : departamentCompare;
        }
    }
}
