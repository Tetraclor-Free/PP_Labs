namespace Lab3WinForms
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

        protected EmployeeRecord()
        {
        }

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
        public virtual EmployeeRecord Clone() => (EmployeeRecord)MemberwiseClone();

        // Метод для определения типа записи
        public abstract int GetRecordType();


        /// <summary>
        /// Метод записывает поля данного объекта в поток
        /// </summary>
        /// <param name="streamWriterHelper"></param>
        public virtual void WriteBites(StreamWriterHelper streamWriterHelper)
        {
            streamWriterHelper.Write(id);
            streamWriterHelper.Write(fullname);
            streamWriterHelper.Write(post);
            streamWriterHelper.Write(department);
            streamWriterHelper.Write(salary);
        }

        /// <summary>
        /// Парсим из потока и записываем поля объекта 
        /// </summary>
        /// <param name="streamReaderHelper"></param>
        public virtual void ReadFromBites(StreamReaderHelper streamReaderHelper)
        {
            id = streamReaderHelper.ReadInt();
            fullname = streamReaderHelper.ReadString();
            post = streamReaderHelper.ReadString();
            department = streamReaderHelper.ReadString();
            salary = streamReaderHelper.ReadInt();
        }

        //Переопределяем метод для преобразования объекта в строку
        public override string ToString()
        {
            return $"ИД:{id}\nФИО:{fullname}\nДолжность:{post}\nОтдел:{department}\nОклад:{salary}\n";
        }
    }
}
