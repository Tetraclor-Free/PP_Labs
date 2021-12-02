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
                fileStream = new FileStream(path, FileMode.Open);
                if (ValidateSignature() == false)
                    throw new Exception("Неверная сигнатура файла");
            }
            else
            {
                fileStream = new FileStream(path, FileMode.Create);
                var buf = Encoding.ASCII.GetBytes(signature);
                fileStream.Write(buf);
                fileStream.Flush();
            }

            currentId = GetCurrentId();

            var samples = new List<EmployeeRecord>() { new TempWorkerRecord(), new SampleEmployeeRecord(), new TraineeRecord() };
            foreach (var item in samples)
            {
                typeToBitesLength[item.GetRecordType()] = item.GetBitesLength();
                typeToSample[item.GetRecordType()] = item;
            }
        }

        private bool ValidateSignature()
        {
            var buf = new byte[8];
            fileStream.Read(buf);
            var str = Encoding.Default.GetString(buf);
            return signature == str;
        }

        private void StreamSetStart()
        {
            fileStream.Seek(8, SeekOrigin.Begin);
        }

        public bool Delete(int id)
        {
            StreamSetStart();
            if (SkipRecords(id - 1) == false) return false;
            fileStream.WriteByte(1);
            fileStream.Flush();
            return true;
        }

        public EmployeeRecord Get(int id)
        {
            StreamSetStart();
            if (SkipRecords(id - 1) == false) return null;
            return ReadNextRecord();
        }

        public List<EmployeeRecord> GetAll()
        {
            StreamSetStart();
            var result = new List<EmployeeRecord>();
            while(fileStream.Position < fileStream.Length)
            {
                var record = ReadNextRecord();
                if (record != null) result.Add(record);
            }
            return result;
        }

        private EmployeeRecord ReadNextRecord()
        {
            var deleted = fileStream.ReadByte();
            var type = fileStream.ReadByte();
            if (deleted == 1)
            {
                fileStream.Seek(typeToBitesLength[type], SeekOrigin.Current);
                return null;
            }
            else
            {
                var record = typeToSample[type].Clone();
                record.ReadFromBites(new StreamReaderHelper(fileStream));
                return record;
            }
        }

        public EmployeeRecord Save(EmployeeRecord record)
        {

            if (record.id == 0)
            {
                return Append(record);
            }
            else
            {
                return Update(record);
            }
        }

        private EmployeeRecord Append(EmployeeRecord record)
        {
            record = record.Clone();
            record.id = ++currentId;
            fileStream.Seek(0, SeekOrigin.End);
            WriteRecord(record);
            return record;
        }

        private EmployeeRecord Update(EmployeeRecord record)
        {
            StreamSetStart();
            SkipRecords(record.id - 1);
            var newRecord = record.Clone();
            WriteRecord(record);
            return newRecord;
        }
        private void WriteRecord(EmployeeRecord record)
        {
            fileStream.WriteByte(0);
            fileStream.WriteByte((byte)record.GetRecordType());
            record.WriteBites(new StreamWriterHelper(fileStream));
            fileStream.Flush();
        }

        private bool SkipRecords(int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (fileStream.Length < fileStream.Position) return false;
                SkipNextRecord();
            }
            return true;
        }

        private void SkipNextRecord()
        {
            var deleted = fileStream.ReadByte();
            var type = fileStream.ReadByte();
            var length = typeToBitesLength[type];
            fileStream.Seek(length, SeekOrigin.Current);
        }

        private int GetCurrentId()
        {
            StreamSetStart();
            var count = 0;
            while (fileStream.Length < fileStream.Position)
            {
                SkipNextRecord();
                count++;
            }
            return count;
        }
    }

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
