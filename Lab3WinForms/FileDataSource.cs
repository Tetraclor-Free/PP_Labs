using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Lab3WinForms
{
    public class FileDataSource : IDataSource
    {
        FileStream fileStream;
        string signature = "nikitaku";
        Dictionary<int, int> typeToBitesLength = new Dictionary<int, int>();
        Dictionary<int, EmployeeRecord> typeToSample = new Dictionary<int, EmployeeRecord>();
        int currentId = 0;

        public FileDataSource(string path)
        {
            if (File.Exists(path))
            {
                // если файл существует то проверяем сигнатуру
                fileStream = new FileStream(path, FileMode.Open);
                if (ValidateSignature() == false)
                    throw new Exception("Неверная сигнатура файла");
            }
            else
            {
                // Если файлан еще не существует создаем его
                fileStream = new FileStream(path, FileMode.Create);
                // И  записываем сигнатуру
                var buf = Encoding.ASCII.GetBytes(signature);
                fileStream.Write(buf);
                fileStream.Flush();
            }

            // Определеяем текущий последний ид записи
            currentId = GetCurrentId();

            // Инициализация словарей, которые понадобятся при создании новых записей
            var samples = new List<EmployeeRecord>() { new TempWorkerRecord(), new SampleEmployeeRecord(), new TraineeRecord() };
            foreach (var item in samples)
            {
                typeToBitesLength[item.GetRecordType()] = item.GetBitesLength();
                typeToSample[item.GetRecordType()] = item;
            }
        }

        /// <summary>
        /// Метод проверки сигнатуры файла, читаются первый восемь байт и сверяются с образцом
        /// </summary>
        /// <returns></returns>
        private bool ValidateSignature()
        {
            var buf = new byte[8];
            fileStream.Read(buf);
            var str = Encoding.Default.GetString(buf);
            return signature == str;
        }

        /// <summary>
        /// Устанавливает указатель потока на начало пропустив сигнатуру
        /// </summary>
        private void StreamSetStart()
        {
            fileStream.Seek(8, SeekOrigin.Begin);
        }

        public bool Delete(int id)
        {
            StreamSetStart(); // Идем в начало
            // Пропускаем все записи с меньшим id
            if (SkipRecords(id - 1) == false) return false;
            // Перезаписываем первый байт указывая на то что запись удалена
            fileStream.WriteByte(1);
            fileStream.Flush();
            return true;
        }

        public EmployeeRecord Get(int id)
        {
            StreamSetStart(); // В начало
            // Пропускаем все предыдущие записи
            if (SkipRecords(id - 1) == false) return null;
            return ReadNextRecord();
        }

        public List<EmployeeRecord> GetAll()
        {
            StreamSetStart();// В начало
            var result = new List<EmployeeRecord>();
            while(fileStream.Position < fileStream.Length)
            {
                // Пока не конец файла читаем записи
                var record = ReadNextRecord();
                if (record != null) result.Add(record);
            }
            return result;
        }

        private EmployeeRecord ReadNextRecord()
        {
            
            var deleted = fileStream.ReadByte(); // признак удаления
            var type = fileStream.ReadByte(); // тип записи
            if (deleted == 1)
            {
                // Если запись помечена как удаленная пропусккаем ее возвращаем null
                fileStream.Seek(typeToBitesLength[type], SeekOrigin.Current);
                return null;
            }
            else
            {
                // Клонируем пустую запись 
                var record = typeToSample[type].Clone();
                // Записываем в нее считанные из файла данные 
                record.ReadFromBites(new StreamReaderHelper(fileStream));
                return record;
            }
        }

        public EmployeeRecord Save(EmployeeRecord record)
        {

            if (record.id == 0) // Если запись новая то добавляем в конец файла иначе обновляем
            {
                return Append(record);
            }
            else
            {
                return Update(record);
            }
        }

        /// <summary>
        /// Метод добавления записи в конец файла
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        private EmployeeRecord Append(EmployeeRecord record)
        {
            record = record.Clone(); // Клонируем переданную запись
            record.id = ++currentId; // Присваем ей id
            fileStream.Seek(0, SeekOrigin.End); // Ставим указатель в конец файла
            WriteRecord(record); // Записываем запись в файл
            return record;
        }

       /// <summary>
       /// Метод обновления записи в файле
       /// </summary>
       /// <param name="record"></param>
       /// <returns></returns>
        private EmployeeRecord Update(EmployeeRecord record)
        {
            StreamSetStart();
            // Находим запись по id
            if (SkipRecords(record.id - 1) == false) return null;
            var newRecord = record.Clone();
            WriteRecord(record); // Обновляем
            return newRecord;
        }
        /// <summary>
        /// Добавление или перезапись записи в файл
        /// </summary>
        /// <param name="record"></param>
        private void WriteRecord(EmployeeRecord record)
        {
            fileStream.WriteByte(0); // Указание что запись не удалена
            fileStream.WriteByte((byte)record.GetRecordType()); // указываем тип записи
            record.WriteBites(new StreamWriterHelper(fileStream)); // Записываем саму запись
            fileStream.Flush(); // Сохраняем изменения
        }

        // Пропусккает указанное количество записей начинаю с текущего положения
        private bool SkipRecords(int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (fileStream.Length < fileStream.Position) return false;
                SkipNextRecord();
            }
            return true;
        }

        /// <summary>
        /// Метод пропуска записи в файле 1 байт удаления 1 байт типа и length длина записи
        /// </summary>
        private void SkipNextRecord()
        {
            var deleted = fileStream.ReadByte();
            var type = fileStream.ReadByte();
            var length = typeToBitesLength[type];
            fileStream.Seek(length, SeekOrigin.Current);
        }

        /// <summary>
        /// Метод для определения текущего ид последней записи
        /// </summary>
        /// <returns></returns>
        private int GetCurrentId()
        {
            StreamSetStart(); // на старт потока
            var count = 0;
            while (fileStream.Length < fileStream.Position)
            {
                // Пока есть что читать, пропускаем следующую запись и прибавляем id
                SkipNextRecord();
                count++;
            }
            return count;
        }
    }

    /// <summary>
    /// Вспомогательный класс для записи в файл
    /// </summary>
    public class StreamWriterHelper
    {
        FileStream fileStream;

        public StreamWriterHelper(FileStream fileStream)
        {
            this.fileStream = fileStream;
        }

        public void Write(string value)
        {
            var buf = Encoding.Default.GetBytes(value);
            var d = 100 - buf.Length;
            fileStream.Write(buf);
            for (int i = 0; i < d; i++) fileStream.WriteByte(0);
        }

        public void Write(int value)
        {
            fileStream.Write(BitConverter.GetBytes(value));
        }

        public void Write(DateTime value)
        {
            fileStream.Write(BitConverter.GetBytes(value.ToBinary()));
        }
    }

    /// <summary>
    /// Вспомогательный класс для чтения из файла
    /// </summary>
    public class StreamReaderHelper
    {
        FileStream fileStream;

        public StreamReaderHelper(FileStream fileStream)
        {
            this.fileStream = fileStream;
        }

        public string ReadString()
        {
            return ReadString(100).Trim('\0');
        }

        public int ReadInt()
        {
            var buf = new byte[4];
            fileStream.Read(buf);
            return BitConverter.ToInt32(buf);
        }

        public DateTime ReadDateTime()
        {
            var buf = new byte[8];
            fileStream.Read(buf);
            return DateTime.FromBinary(BitConverter.ToInt64(buf));
        }

        private string ReadString(int count)
        {
            var buf = new byte[count];
            fileStream.Read(buf, 0, count);
            return Encoding.Default.GetString(buf);
        }
    }
}
