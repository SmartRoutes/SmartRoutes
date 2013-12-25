using System;
using System.IO;
using CsvHelper;

namespace SmartRoutes.Database.Data
{
    public class CsvWriter<T> : IDisposable where T : new()
    {
        private readonly CsvWriter _writer;
        private bool _headersWritten;

        public CsvWriter(Stream stream)
        {
            Record.Initialize(typeof (T));
            _writer = new CsvWriter(new StreamWriter(stream));
            _headersWritten = false;
        }

        public void Dispose()
        {
            _writer.Dispose();
        }

        public void Write(T obj)
        {
            Record record = Record.Create(obj);

            if (!_headersWritten)
            {
                foreach (string name in record.Names)
                {
                    _writer.WriteField(name);
                }
                _writer.NextRecord();
                _headersWritten = true;
            }

            foreach (object value in record.Values)
            {
                _writer.WriteField(value);
            }
            _writer.NextRecord();
        }
    }
}