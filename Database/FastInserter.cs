using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;

namespace Database
{
    /// <summary>
    ///     This is a convenience class is the ideas introduced here:
    ///     http://stackoverflow.com/a/15662102/52749
    /// </summary>
    /// <typeparam name="T">The concrete <see cref="DbContext" /> child class.</typeparam>
    public class FastInserter<T> : IDisposable where T : DbContext
    {
        private readonly int _refreshFrequency;
        private readonly int _saveFrequency;
        private T _dbContext;
        private IDictionary<Type, object> _dbSets = new Dictionary<Type, object>();
        private int _untilRefresh;
        private int _untilSave;

        public FastInserter(int saveFrequency, int refreshFrequency)
        {
            _saveFrequency = saveFrequency;
            _untilSave = _saveFrequency;
            _refreshFrequency = refreshFrequency;
            _untilRefresh = _refreshFrequency;
        }

        public void Dispose()
        {
            if (_dbContext != null)
            {
                if (_untilSave < _saveFrequency)
                {
                    _dbContext.SaveChanges();
                    Console.WriteLine("Saved before dispose.");
                }
                _dbContext.Dispose();
            }
        }

        private void RefreshDbContext()
        {
            Dispose();

            _dbContext = Activator.CreateInstance<T>();
            _dbContext.Configuration.AutoDetectChangesEnabled = false;
            _dbContext.Configuration.ValidateOnSaveEnabled = false;

            _dbSets = _dbContext
                .GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.DeclaredOnly)
                .Where(p => p.PropertyType.GetGenericTypeDefinition() == typeof (DbSet<>))
                .ToDictionary(p => p.PropertyType.GetGenericArguments().First(), p => p.GetGetMethod().Invoke(_dbContext, null));
        }

        public void Add<TEntity>(TEntity entity) where TEntity : class
        {
            // make sure the context is initialized
            if (_dbContext == null)
            {
                RefreshDbContext();
            }

            // get the DbSet
            if (!_dbSets.ContainsKey(typeof (TEntity)))
            {
                throw new ArgumentException("No DbSet of the provided type could be found.");
            }
            var dbSet = (DbSet<TEntity>) _dbSets[typeof (TEntity)];

            // add the entity
            dbSet.Add(entity);

            // increment the counter
            _untilSave -= 1;
            _untilRefresh -= 1;

            // save if necessary
            if (_untilSave <= 0)
            {
                _dbContext.SaveChanges();
                Console.WriteLine("Saved.");
                _untilSave = _saveFrequency;
            }

            // refresh if necessary
            if (_untilRefresh <= 0)
            {
                RefreshDbContext();
                _untilRefresh = _refreshFrequency;
            }
        }
    }
}