using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab3WinForms
{
    /// <summary>
    /// Хранение записей в оперативной памяти
    /// ! Для предотвращения изменения записей извне,
    /// все методы, возвращающие записи должны возвращать
    /// их копии
    /// </summary>
    public class MemoryDataSource : IDataSource
    {
        private List<EmployeeRecord> records = new List<EmployeeRecord>();
        public EmployeeRecord Save(EmployeeRecord record)
        {
            if (record.id == 0)
            {
                //Если id = 0 то значит новая запись увличием id и добавляем в лист
                record.id = records.Count + 1;
                records.Add(record.Clone());
                return record.Clone();
            }
            else
            {
                // Если id не ноль то проверяем что существует и если существует обновляем по id и возвращаем клон
                if (CheckOutRange(record.id) == false)
                    return null;
                records[record.id] = record.Clone();
                return record.Clone();
            }
        }

        public EmployeeRecord Get(int id)
        {
            //Если вышло за границы то возврта false
            if (CheckOutRange(id) == false)
                return null;

            return records[id - 1].Clone();
        }
        public bool Delete(int id)
        {
            // если вышло за границы массива возврта false
            if (CheckOutRange(id) == false)
                return false;

            records.RemoveAt(id - 1);
            return true;
        }
        public List<EmployeeRecord> GetAll()
        {
            // Клонируем все засписи
            return records.Select(v => v.Clone()).ToList();
        }

        /// <summary>
        /// Метод проверки на выход за пределы массива записей
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool CheckOutRange(int id)
        {
            return id > 0 && id <= records.Count;
        }
    }
}
