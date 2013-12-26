using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using SmartRoutes.Model.Gtfs;

namespace SmartRoutes.Database.Data
{
    public class RecordDataReader<T> : IDataReader where T : class
    {
        private readonly IEnumerator<T> _enumerator;
        private readonly Type _underlyingType;
        private readonly IDictionary<string, int> _nameToOridinal;
        private readonly string[] _ordinalToName;
        private readonly Type[] _propertyTypes;
        private Record _currentRecord;

        public RecordDataReader(IEnumerable<T> entities)
        {
            _underlyingType = typeof (T);

            // cache some meta information
            _ordinalToName = Record.GetPropertyNames(_underlyingType).ToArray();
            _nameToOridinal = _ordinalToName
                .Select((n, o) => new {Name = n, Ordinal = o})
                .ToDictionary(t => t.Name, t => t.Ordinal);
            _propertyTypes = Record.GetPropertyTypes(_underlyingType).ToArray();

            // the entity enumerator
            _enumerator = entities.GetEnumerator();
        }

        public void Dispose()
        {
        }

        public string GetName(int i)
        {
            return _ordinalToName[i];
        }

        public string GetDataTypeName(int i)
        {
            throw new NotImplementedException();
        }

        public Type GetFieldType(int i)
        {
            return _propertyTypes[i];
        }

        public object GetValue(int i)
        {
            return _currentRecord.Values.ElementAt(i);
        }

        public int GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        public int GetOrdinal(string name)
        {
            return _nameToOridinal[name];
        }

        public bool GetBoolean(int i)
        {
            throw new NotImplementedException();
        }

        public byte GetByte(int i)
        {
            throw new NotImplementedException();
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public char GetChar(int i)
        {
            throw new NotImplementedException();
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public Guid GetGuid(int i)
        {
            throw new NotImplementedException();
        }

        public short GetInt16(int i)
        {
            throw new NotImplementedException();
        }

        public int GetInt32(int i)
        {
            throw new NotImplementedException();
        }

        public long GetInt64(int i)
        {
            throw new NotImplementedException();
        }

        public float GetFloat(int i)
        {
            throw new NotImplementedException();
        }

        public double GetDouble(int i)
        {
            throw new NotImplementedException();
        }

        public string GetString(int i)
        {
            throw new NotImplementedException();
        }

        public decimal GetDecimal(int i)
        {
            throw new NotImplementedException();
        }

        public DateTime GetDateTime(int i)
        {
            throw new NotImplementedException();
        }

        public IDataReader GetData(int i)
        {
            throw new NotImplementedException();
        }

        public bool IsDBNull(int i)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        public bool NextResult()
        {
            throw new NotImplementedException();
        }

        public bool Read()
        {
            if (_enumerator.MoveNext())
            {
                _currentRecord = Record.Create(_enumerator.Current);
                if (_currentRecord.UnderlyingType == typeof (Stop))
                {
                    
                }
                return true;
            }

            return false;
        }

        public int Depth
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsClosed
        {
            get { throw new NotImplementedException(); }
        }

        public int RecordsAffected
        {
            get { throw new NotImplementedException(); }
        }

        public int FieldCount
        {
            get { return _propertyTypes.Length; }
        }

        object IDataRecord.this[int i]
        {
            get { throw new NotImplementedException(); }
        }

        object IDataRecord.this[string name]
        {
            get { throw new NotImplementedException(); }
        }
    }
}