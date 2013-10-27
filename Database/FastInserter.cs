using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Database
{
    /// <summary>
    ///     This is a convenience class is the ideas introduced here:
    ///     http://stackoverflow.com/a/15662102/52749
    /// </summary>
    /// <typeparam name="T">The concrete <see cref="DbContext" /> child class.</typeparam>
    public class FastInserter<T> : IDisposable where T : DbContext
    {
        private readonly bool _refresh;
        private readonly int _saveFrequency;
        private readonly bool _userProvidedDbContext;
        private T _dbContext;
        private IDictionary<Type, object> _dbSets = new Dictionary<Type, object>();
        private int _untilSave;

        public FastInserter(T dbContext, int saveFrequency)
        {
            _saveFrequency = saveFrequency;
            _refresh = false;
            _untilSave = _saveFrequency;
            _userProvidedDbContext = true;

            _dbContext = dbContext;
            SetDbSets();
        }

        public FastInserter(int saveFrequency, bool refresh)
        {
            _saveFrequency = saveFrequency;
            _refresh = refresh;
            _untilSave = _saveFrequency;
            _userProvidedDbContext = false;
        }

        public void Dispose()
        {
            if (_dbContext != null)
            {
                if (_untilSave < _saveFrequency)
                {
                    SaveChangesAsync().Wait();
                }

                if (!_userProvidedDbContext)
                {
                    _dbContext.Dispose();
                }
            }
        }

        private void RefreshDbContext()
        {
            Dispose();

            _dbContext = Activator.CreateInstance<T>();
            _dbContext.Configuration.AutoDetectChangesEnabled = false;
            _dbContext.Configuration.ValidateOnSaveEnabled = false;

            SetDbSets();
        }

        private void SetDbSets()
        {
            _dbSets = _dbContext
                .GetEntitySetProperties()
                .ToDictionary(p => p.PropertyType.GetGenericArguments().First(), p => p.GetGetMethod().Invoke(_dbContext, null));
        }

        public async Task AddAsync<TEntity>(TEntity entity) where TEntity : class
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
            var dbSet = (IDbSet<TEntity>) _dbSets[typeof (TEntity)];

            // add the entity
            dbSet.Add(entity);

            // increment the counter
            _untilSave -= 1;

            // save if necessary
            if (_untilSave <= 0)
            {
                await SaveChangesAsync();
                if (_refresh)
                {
                    RefreshDbContext();
                }
                _untilSave = _saveFrequency;
            }
        }

        public async Task AddRangeAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            foreach (TEntity entity in entities)
            {
                await AddAsync(entity);
            }
        }

        public async Task SaveChangesAsync()
        {
            if (_dbContext != null)
            {
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}