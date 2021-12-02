namespace Lab1ConsoleApp
{
    public class SampleEmployeeRecord : EmployeeRecord
    {
        public SampleEmployeeRecord(int id, string fullname, string post, string department, int salary) 
            : base(id, fullname, post, department, salary)
        {
        }

        public SampleEmployeeRecord(string record) : base(record)
        {
        }

        public override EmployeeRecord Clone()
        {
            return new SampleEmployeeRecord(id, fullname, post, department, salary);
        }

        public override string ToString()
        {
            return $"Обычный работник\n" + base.ToString();
        }
    }
}
