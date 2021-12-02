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
            if (record.fullname.Split(' ').Length < 3)
            {
                throw new ArgumentException("ФИО должно содержать не меньше трех слов");
            }

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
            records.Sort(RecordCompare);
            return records;
        }

        private int RecordCompare(EmployeeRecord a, EmployeeRecord b)
        {
            var departamentCompare = a.department.CompareTo(b.department);
            return departamentCompare == 0 ? a.fullname.CompareTo(b.fullname) : departamentCompare;
        }
    }
}
