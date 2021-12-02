namespace Lab2ConsoleApp
{
    public class TraineeRecord : EmployeeRecord // Стажер студент
    {
        public string educationInstitution; // Учебное заведение стажера

        public TraineeRecord(int id, string fullname, string post, string department, int salary, string educationInstitution) 
            : base(id, fullname, post, department, salary)
        {
            this.educationInstitution = educationInstitution;
        }

        public TraineeRecord(string record) : base(record)
        {
            educationInstitution = filds[index + 1];
        }

        public TraineeRecord()
        {
        }

        public override EmployeeRecord Clone()
        {
            return new TraineeRecord(id, fullname, post, department, salary, educationInstitution);
        }

        public override string ToString()
        {
            return $"Стажер\n" + base.ToString() + $"Учебное заведение:{educationInstitution}\n";
        }

        public override int GetRecordType()
        {
            return 3;
        }

        public override int GetBitesLength()
        {
            return base.GetBitesLength() + 100;
        }

        public override void WriteBites(StreamWriterHelper streamWriterHelper)
        {
            base.WriteBites(streamWriterHelper);
            streamWriterHelper.Write(educationInstitution);
        }

        public override void ReadFromBites(StreamReaderHelper streamReaderHelper)
        {
            base.ReadFromBites(streamReaderHelper);
            educationInstitution = streamReaderHelper.ReadString();
        }
    }
}
