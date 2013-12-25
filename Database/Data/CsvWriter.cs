using System;
using System.IO;
using CsvHelper;

namespace SmartRoutes.Database.Data
{
    public class CsvWriter<T> : IDisposable where T : new()
    {
        private readonly CsvWriter _writer;
        private bool _needsHeaders;

        public CsvWriter(Stream stream, bool needsHeaders)
        {
            Record.Initialize(typeof (T));
            _writer = new CsvWriter(new StreamWriter(stream));
            _needsHeaders = needsHeaders;
        }

        public void Dispose()
        {
            _writer.Dispose();
        }

        public void Write(T obj)
        {
            Record record = Record.Create(obj);

            if (_needsHeaders)
            {
                foreach (string name in record.Names)
                {
                    _writer.WriteField(name);
                }
                _writer.NextRecord();
                _needsHeaders = false;
            }

            foreach (object value in record.Values)
            {
                _writer.WriteField(value);
            }
            _writer.NextRecord();
        }
    }
}