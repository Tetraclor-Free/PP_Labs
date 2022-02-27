namespace Lab1ConsoleApp
{
    /// <summary>
    /// Ваш класс основной записи
    /// </summary>
    abstract public class EmployeeRecord
    {
        public int id; // Должен генерироваться автоматически после записи в хранилище.
                       // 0 обозначает новую, еще не сохраненную запись

        public string fullname; // ФИО
        public string post; // Должность
        public string department; // Отдел
        public int salary; // Оклад

        protected string[] filds;
        protected int index;

        protected EmployeeRecord(int id, string fullname, string post, string department, int salary)
        {
            this.id = id;
            this.fullname = fullname;
            this.post = post;
            this.department = department;
            this.salary = salary;
        }

        // Парсим запись и зстроки
        protected EmployeeRecord(string record)
        {
            filds = record.Split(';');
            this.fullname = filds[0];
            this.post = filds[1];
            this.department = filds[2];
            this.salary =int.Parse(filds[3]);
            index = 3;
        }
        // Метод для полного клонирования, чтобы не было ссылочной зависисмости
        public virtual EmployeeRecord Clone() =>(EmployeeRecord)MemberwiseClone();

        //Переопределяем метод для преобразования записи в строку
        public override string ToString()
        {
            return $"ИД:{id}\nФИО:{fullname}\nДолжность:{post}\nОтдел:{department}\nОклад:{salary}\n";
        }
    }
}
