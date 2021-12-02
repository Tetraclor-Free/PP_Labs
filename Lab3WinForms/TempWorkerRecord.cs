using System;

namespace Lab3WinForms
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

        public TempWorkerRecord()
        {
        }

        public override EmployeeRecord Clone()
        {
            return new TempWorkerRecord(id, fullname, post, department, salary, dateEnd);
        }

        public override string ToString()
        {
            return $"Временный работник\n" + base.ToString() + $"Дата окончания временного договора:{dateEnd}\n"; 
        }

        public override int GetRecordType()
        {
            return 2;
        }

        public override int GetBitesLength()
        {
            return base.GetBitesLength() + 8; 
        }

        public override void WriteBites(StreamWriterHelper streamWriterHelper)
        {
            base.WriteBites(streamWriterHelper);
            streamWriterHelper.Write(dateEnd);
        }

        public override void ReadFromBites(StreamReaderHelper streamReaderHelper)
        {
            base.ReadFromBites(streamReaderHelper);
            dateEnd = streamReaderHelper.ReadDateTime();
        }
    }
}
