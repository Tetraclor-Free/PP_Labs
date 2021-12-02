using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab1ConsoleApp
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
                record.id = records.Count + 1;
                records.Add(record.Clone());
                return record.Clone();
            }
            else
            {
                if (CheckOutRange(record.id) == false)
                    return null;
                records[record.id] = record.Clone();
                return record.Clone();
            }
        }

        public EmployeeRecord Get(int id)
        {
            if (CheckOutRange(id) == false) 
                return null;

            return records[id - 1].Clone();
        }
        public bool Delete(int id)
        {
            if (CheckOutRange(id) == false) 
                return false;

            records.RemoveAt(id - 1);
            return true;
        }
        public List<EmployeeRecord> GetAll()
        {
            return records.Select(v => v.Clone()).ToList();
        }

        private bool CheckOutRange(int id)
        {
            return id > 0 && id <= records.Count;
        }
    }
}
