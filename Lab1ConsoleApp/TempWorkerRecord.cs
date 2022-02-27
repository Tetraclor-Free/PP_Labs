using System;

namespace Lab1ConsoleApp
{
    public class TempWorkerRecord : EmployeeRecord // Временный работник
    {
        public DateTime dateEnd; // Конец временного договора о работе

        public TempWorkerRecord(int id, string fullname, string post, string department, int salary, DateTime dateEnd) 
            : base(id, fullname, post, department, salary)
        {
            this.dateEnd = dateEnd;
        }

        public TempWorkerRecord(string record) : base(record)
        {
            dateEnd = DateTime.Parse(filds[index + 1]);
        }

        public override string ToString()
        {
            return $"Временный работник\n" + base.ToString() + $"Дата окончания временного договора:{dateEnd}\n"; 
        }
    }
}
