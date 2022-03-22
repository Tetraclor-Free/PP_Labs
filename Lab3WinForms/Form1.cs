using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab3WinForms
{
    public partial class Form1 : Form
    {
        BusinessLogic logic;

        public Form1()
        {
            InitializeComponent();

            logic = new BusinessLogic(new FileDataSource(@"..\..\..\bd.bin"));

            if(logic.GetAll().Count == 0) // Для теситрования
            {
                logic.Save(new SampleEmployeeRecord("Дмитрий Г. Д.;Разработчик;Разработка;50000"));
                logic.Save(new TempWorkerRecord("Алексей А. Ж.;Разработчик;Разработка;100000;20.12.2022"));
                logic.Save(new TraineeRecord("Александр Ф. Ю.;Стажер;Бухгалтерия;5555;УДГУ"));
            }
  

            // Привязываем к событиям обработчики
            AddButton.Click += AddButton_Click;
            ChangeButton.Click += ChangeButton_Click;
            DeleteButton.Click += DeleteButton_Click;
            CreateReportButton.Click += CreateReportButton_Click;

            // Отображаем все текщие записи
            ViewRecords();
        }

        private void CreateReportButton_Click(object sender, EventArgs e)
        {
            var dialog = new SaveFileDialog();
            // Отркываем диалог сохранения файла
            var result = dialog.ShowDialog();
            if (result != DialogResult.OK) return;
            // Если файл указан верно считываем данны для формирования отчета с формы
            var min = (int)FromNumeric.Value;
            var max = (int)ToNumeric.Value;
            // Создаем отчет
            var report = logic.GetReport(min, max);
            // Записываем его в файл
            File.WriteAllText(dialog.FileName, report);
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            // Берем выделенный элемент на форме
            var record = (EmployeeRecord)ViewRecordsListBox.SelectedItem;
            if (record == null) return;
            // Удалем его
            logic.Delete(record.id);
            // Обнволяем отображение всех записей
            ViewRecords();
        }

        private void ChangeButton_Click(object sender, EventArgs e)
        {
            // Берем выделенный элемент
            var record = (EmployeeRecord)ViewRecordsListBox.SelectedItem;
            // Передаем для обновелния 
            OpenAddUpdateRecordForm(record);
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            // передача null Указывает на то что создается новая запись
            OpenAddUpdateRecordForm(null);
        }

        public void ViewRecords()
        {
            // Указываем listBox что нужно отображать
            ViewRecordsListBox.DataSource = logic.GetAll();
        }

        public void OpenAddUpdateRecordForm(EmployeeRecord record)
        {
          
            var form = new AddUpdateRecordForm(record);
            DialogResult result = DialogResult.Cancel;
            try
            {
                // Открываем форму для редактирования записей
                result = form.ShowDialog();
                if (result != DialogResult.OK) return;

                // Если нажата кнопка ок, то сохраняем переданную запись
                var newRecord = form.EmployeeRecord;
                logic.Save(newRecord);
            }
            catch(ArgumentException e)
            {
                // Если возникнут ошибки отображем во всплывающем сообщении
                MessageBox.Show(e.Message);
                return;
            }

            // Обновление отображения
            ViewRecords();
        }
    }
}
