using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab3WinForms
{
    public partial class AddUpdateRecordForm : Form
    {
        public EmployeeRecord EmployeeRecord;
        EmployeeRecord Record;
        Dictionary<string, EmployeeRecord> humanTypeToSample = new Dictionary<string, EmployeeRecord>();

        public AddUpdateRecordForm(EmployeeRecord record)
        {
            InitializeComponent();
            if (record != null)
                RecordTypesComboBox.Enabled = false;
            record = record ?? new SampleEmployeeRecord(0, "", "", "", 0);
            Record = record;
            // Определение какие типы записей можно будет выбрать
            var samples = new List<EmployeeRecord>() 
                {  new SampleEmployeeRecord(), new TempWorkerRecord(), new TraineeRecord()};
            var startItem = "";
            foreach (var sample in samples)
            {
                var str = sample.ToString();
                var type = str.Substring(0, str.IndexOf("ИД:"));
                humanTypeToSample[type] = sample;
                if(sample.GetType() == record.GetType())
                {
                    startItem = type;
                    humanTypeToSample[type] = record.Clone();
                }
            }
            // Инициализация элементов формы
            RecordTypesComboBox.DataSource = humanTypeToSample.Keys.ToList();
            RecordTypesComboBox.SelectedIndexChanged += RecordTypesComboBox_SelectedIndexChanged;
            RecordTypesComboBox.SelectedItem = startItem;

            SaveButton.Click += SaveButton_Click;
            CancelButton.Click += CancelButton_Click;

            tableLayoutPanel1.Controls.Clear();
            GenerateBaseFileds(record);
            GenerateExtFields(record);
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            // Отмена изменений закрытие формы
            DialogResult = DialogResult.Cancel;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            var humanType = (string)RecordTypesComboBox.SelectedItem;
            // Так как тип может быть любой то используется dynamic
            dynamic newRecord = humanTypeToSample[humanType].Clone();// dynamic, тип определится во время работы программы

            if(RecordTypesComboBox.Enabled == false)
                newRecord.id = Record.id;
            
            // Проставляем в поля значения из элементов формы
            newRecord.fullname = tableLayoutPanel1.GetControlFromPosition(1, 1).Text;
            newRecord.post = tableLayoutPanel1.GetControlFromPosition(1, 2).Text;
            newRecord.department = tableLayoutPanel1.GetControlFromPosition(1, 3).Text;
            newRecord.salary = int.Parse(tableLayoutPanel1.GetControlFromPosition(1, 4).Text);

            // В зависимотс от типа записи записываем дополнительные поля 
            if (newRecord is TempWorkerRecord)
            {
                newRecord.dateEnd = DateTime.Parse(tableLayoutPanel1.GetControlFromPosition(1, 5).Text);
            }
            else if (newRecord is TraineeRecord)
            {
                newRecord.educationInstitution = tableLayoutPanel1.GetControlFromPosition(1, 5).Text;
            }
            EmployeeRecord = newRecord;
        }

        /// <summary>
        /// Медот для обработки смены типов записи
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RecordTypesComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var humanType = (string)RecordTypesComboBox.SelectedItem;
            // Очищаем все элементы
            tableLayoutPanel1.Controls.Clear();
            // Создаем новые элементы
            GenerateBaseFileds(Record);
            GenerateExtFields(humanTypeToSample[humanType]);
        }
        /// <summary>
        /// Создание полей формы для базовый значений записи
        /// </summary>
        /// <param name="employeeRecord"></param>
        void GenerateBaseFileds(EmployeeRecord employeeRecord)
        {
            AddLabelInputPair("Id:", new Label() { Text = employeeRecord.id.ToString()});
            AddLabelInputPair("ФИО:", new TextBox() { Text = employeeRecord.fullname, Width = 300 });
            AddLabelInputPair("Должность:", new TextBox() { Text = employeeRecord.post });

            // Создание выпаюдающего списка для удобства пользователя
            var allow = BusinessLogic.allowDepartaments.ToList();
            if(allow.Remove(employeeRecord.department))
                allow.Insert(0, employeeRecord.department);

            var comboBox = new ComboBox() { DataSource = allow, SelectedItem = employeeRecord.department };
            
            AddLabelInputPair("Отдел:", comboBox);
            AddLabelInputPair("Зарплата:", new NumericUpDown() { Maximum = decimal.MaxValue, Value = employeeRecord.salary });
        }

        /// <summary>
        /// Создание элементов формы для дополнительных полей записи
        /// </summary>
        /// <param name="employeeRecord"></param>
        void GenerateExtFields(EmployeeRecord employeeRecord)
        {
            // В зависимости от класса записи создаются соответсвущие поля
            if (employeeRecord is TempWorkerRecord)
            {
                var date = (employeeRecord as TempWorkerRecord).dateEnd;
                date = date == default ? DateTime.Now : date;
                AddLabelInputPair("Дата окончания контракта: ",
                    new DateTimePicker() { Value = date });
            } 
            else if (employeeRecord is TraineeRecord) 
            {
                AddLabelInputPair("Учебное заведение: ",
                    new TextBox() { Text = (employeeRecord as TraineeRecord).educationInstitution});
            }
        }

        /// <summary>
        /// Вспомогательный метод для добавление пары элементов формы: пояснение - поле ввода 
        /// </summary>
        /// <param name="labeltext"></param>
        /// <param name="inputControl"></param>
        void AddLabelInputPair(string labeltext, Control inputControl)
        {
            tableLayoutPanel1.Controls.Add(new Label() { Text = labeltext });
            tableLayoutPanel1.Controls.Add(inputControl);
        }
    }
}
