using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using Common.Logging;
using Helios.Domain;

namespace Helios.Data
{
    /// <summary>
    /// Entity Framework repository
    /// </summary>
    public partial class EfRepository<T> : IRepository<T> where T : BaseEntity
    {
        #region Fields

        private static readonly ILog Logger = LogManager.GetLogger("Helios.Data.Repository");

        private readonly IDbContext _context;
        private IDbSet<T> _entities;

        #endregion

        #region Ctor

        public EfRepository(IDbContext context)
        {
            this._context = context;
        }

        #endregion

        #region Utilities

        protected string GetFullErrorText(DbEntityValidationException exc)
        {
            var msg = string.Empty;
            foreach (var validationErrors in exc.EntityValidationErrors)
                foreach (var error in validationErrors.ValidationErrors)
                    msg += $"Property: {error.PropertyName} Error: {error.ErrorMessage}" + Environment.NewLine;
            return msg;
        }

        /// <summary>
        /// 确保实体对象在本地对象上下文中，如果对象不在上下文中，直接删除时会报错
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected T EnsureEntityWasAttachToContext(T entity)
        {
            var alreadyAttached = this.Entities.Local.FirstOrDefault(x => x.Id == entity.Id);
            if (alreadyAttached == null)
            {
                Logger.Debug($"将对象[{entity.GetType().FullName}#{entity.Id}]附加到数据容器上下文中，否则直接删除会报错");
                this.Entities.Attach(entity);
                alreadyAttached = entity;
            }

            return alreadyAttached;
        }

        #endregion

        #region Methods
        public virtual void Insert(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                this.Entities.Add(entity);

                this._context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                var fullErrorTest = GetFullErrorText(dbEx);
                Logger.Error(fullErrorTest, dbEx);
                throw new Exception(fullErrorTest, dbEx);
            }
        }

        public virtual void Insert(IEnumerable<T> entities)
        {
            try
            {
                if (entities == null)
                    throw new ArgumentNullException(nameof(entities));

                foreach (var entity in entities)
                    this.Entities.Add(entity);

                this._context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                var fullErrorTest = GetFullErrorText(dbEx);
                Logger.Error(fullErrorTest, dbEx);
                throw new Exception(fullErrorTest, dbEx);
            }
        }

        public virtual void Update(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                this._context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                var fullErrorTest = GetFullErrorText(dbEx);
                Logger.Error(fullErrorTest, dbEx);
                throw new Exception(fullErrorTest, dbEx);
            }
        }

        public virtual void Update(IEnumerable<T> entities)
        {
            try
            {
                if (entities == null)
                    throw new ArgumentNullException(nameof(entities));

                this._context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                var fullErrorTest = GetFullErrorText(dbEx);
                Logger.Error(fullErrorTest, dbEx);
                throw new Exception(fullErrorTest, dbEx);
            }
        }

        public virtual void Delete(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                entity = EnsureEntityWasAttachToContext(entity);
                
                this.Entities.Remove(entity);

                this._context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                var fullErrorTest = GetFullErrorText(dbEx);
                Logger.Error(fullErrorTest, dbEx);
                throw new Exception(fullErrorTest, dbEx);
            }
        }

        public virtual void Delete(IEnumerable<T> entities)
        {
            try
            {
                if (entities == null)
                    throw new ArgumentNullException(nameof(entities));

                foreach (var entity in entities)
                {
                    EnsureEntityWasAttachToContext(entity);
                    this.Entities.Remove(entity);
                }

                this._context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                var fullErrorTest = GetFullErrorText(dbEx);
                Logger.Error(fullErrorTest, dbEx);
                throw new Exception(fullErrorTest, dbEx);
            }
        }

        #endregion

        #region Properties

        public virtual IQueryable<T> Table => this.Entities;

        public virtual IQueryable<T> TableNoTracking => this.Entities.AsNoTracking();

        protected virtual IDbSet<T> Entities => _entities ?? (_entities = _context.Set<T>());

        #endregion
    }
}