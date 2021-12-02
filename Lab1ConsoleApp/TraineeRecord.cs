namespace Lab1ConsoleApp
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

        public override EmployeeRecord Clone()
        {
            return new TraineeRecord(id, fullname, post, department, salary, educationInstitution);
        }

        public override string ToString()
        {
            return $"Стажер\n" + base.ToString() + $"Учебное заведение:{educationInstitution}\n";
        }
    }
}
