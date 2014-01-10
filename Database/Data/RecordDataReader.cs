using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using SmartRoutes.Model.Gtfs;

namespace SmartRoutes.Database.Data
{
    public class RecordDataReader<TEntity> : IDataReader where TEntity : class
    {
        private readonly RecordDataReaderConfiguration<TEntity> _configuration;
        private readonly IEnumerator<TEntity> _enumerator;
        private Record _currentRecord;
        private IDictionary<string, int> _nameToReaderOridinal;
        private Type[] _propertyTypes;
        private string[] _readerOrdinalToName;
        private int[] _readerOrdinalToRecordOrdinal;

        public RecordDataReader(IEnumerable<TEntity> entities)
        {
            // a pointer to the current entity
            _enumerator = entities.GetEnumerator();

            // initialize the configuration object
            _configuration = new RecordDataReaderConfiguration<TEntity>(this);
        }

        internal bool Configured { get; set; }

        public RecordDataReaderConfiguration<TEntity> Configuration
        {
            get { return _configuration; }
        }

        public void Dispose()
        {
        }

        public string GetName(int i)
        {
            Configure();
            return _readerOrdinalToName[i];
        }

        public Type GetFieldType(int i)
        {
            Configure();
            return _propertyTypes[i];
        }

        public object GetValue(int i)
        {
            Configure();
            return _currentRecord.Values.ElementAt(_readerOrdinalToRecordOrdinal[i]);
        }

        public int GetOrdinal(string name)
        {
            Configure();
            return _nameToReaderOridinal[name];
        }

        public bool Read()
        {
            Configure();
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


        public int FieldCount
        {
            get
            {
                Configure();
                return _propertyTypes.Length;
            }
        }

        #region Unimplemented Methods

        public string GetDataTypeName(int i)
        {
            throw new NotImplementedException();
        }

        public int GetValues(object[] values)
        {
            throw new NotImplementedException();
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

        object IDataRecord.this[int i]
        {
            get { throw new NotImplementedException(); }
        }

        object IDataRecord.this[string name]
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        private void Configure()
        {
            // only run when needed
            if (Configured)
            {
                return;
            }
            Configured = true;

            Type underlyingType = typeof (TEntity);

            ISet<string> ignoredPropertyNames = Configuration.IgnoredPropertyNames;

            string[] propertyNames = Record.GetPropertyNames(underlyingType).ToArray();
            Type[] propertyTypes = Record.GetPropertyTypes(underlyingType).ToArray();

            // since record properties can be excluded, keep track of the excluded indices
            IList<int> readerOrdinalToRecordOrdinal = new List<int>();
            for (int i = 0; i < propertyNames.Length; i++)
            {
                if (!ignoredPropertyNames.Contains(propertyNames[i]))
                {
                    readerOrdinalToRecordOrdinal.Add(i);
                }
            }
            _readerOrdinalToRecordOrdinal = readerOrdinalToRecordOrdinal.ToArray();

            // cache some meta information
            _readerOrdinalToName = _readerOrdinalToRecordOrdinal
                .Select(i => propertyNames[i])
                .ToArray();
            _propertyTypes = _readerOrdinalToRecordOrdinal
                .Select(i => propertyTypes[i])
                .ToArray();

            _nameToReaderOridinal = _readerOrdinalToName
                .Select((n, o) => new {Name = n, Ordinal = o})
                .ToDictionary(t => t.Name, t => t.Ordinal);
        }
    }
}