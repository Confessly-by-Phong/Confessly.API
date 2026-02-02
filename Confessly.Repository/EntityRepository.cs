using Confessly.Domain;
using Confessly.Domain.Core;
using Confessly.Logging.Extensions;
using Confessly.Logging.Interfaces;
using Confessly.Repository.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Confessly.Repository
{
    public partial class EntityRepository<TEntity>(ConfesslyDbContext dbContext, ILoggingService logger) : BaseRepository<TEntity>(dbContext, logger)
        where TEntity : BaseEntity
    {
        /// <inheritdoc />
        public override async Task Delete(TEntity entity,
            bool isSoftDeleted = true,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(entity);

            try
            {
                Logger.LogInformation("Deleting {EntityName} with ID {EntityId} (SoftDelete: {IsSoftDeleted})", 
                    EntityName, entity.Id, isSoftDeleted);

                if (!isSoftDeleted)
                {
                    Table.Remove(entity);
                    return;
                }

                if (entity.IsDeleted) 
                {
                    Logger.LogWarning("{EntityName} with ID {EntityId} is already marked as deleted", EntityName, entity.Id);
                    return;
                }

                entity.IsDeleted = true;
                Table.Update(entity);

                Logger.LogInformation("Successfully marked {EntityName} with ID {EntityId} for deletion", EntityName, entity.Id);
            }
            catch (Exception ex)
            {
                Logger.LogRepositoryException(ex, "Delete", EntityName, entity.Id);
                throw;
            }
        }

        /// <inheritdoc />
        public override async Task Delete(Expression<Func<TEntity, bool>> predicate, bool isSoftDeleted = true,
            CancellationToken cancellationToken = default)
        {
            try
            {
                Logger.LogInformation("Searching for {EntityName} to delete using predicate (SoftDelete: {IsSoftDeleted})", 
                    EntityName, isSoftDeleted);

                TEntity? entity = await Table.SingleOrDefaultAsync(predicate, cancellationToken);
                if (entity is null) 
                {
                    Logger.LogWarning("No {EntityName} found matching the delete predicate", EntityName);
                    return;
                }

                await Delete(entity, isSoftDeleted, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogRepositoryException(ex, "DeleteByPredicate", EntityName);
                throw;
            }
        }

        /// <inheritdoc />
        public override async Task<TEntity?> Get(Expression<Func<TEntity, bool>> predicate,
            bool includeDeleted = false,
            CancellationToken cancellationToken = default)
        {
            try
            {
                Logger.LogDebug("Getting {EntityName} using predicate (IncludeDeleted: {IncludeDeleted})", 
                    EntityName, includeDeleted);

                var query = AddDeletedFilter(Table, includeDeleted);
                TEntity? entity = await query.SingleOrDefaultAsync(predicate, cancellationToken);

                if (entity != null)
                {
                    Logger.LogDebug("Found {EntityName} with ID {EntityId}", EntityName, entity.Id);
                }
                else
                {
                    Logger.LogDebug("No {EntityName} found matching the predicate", EntityName);
                }

                return entity;
            }
            catch (Exception ex)
            {
                Logger.LogRepositoryException(ex, "GetByPredicate", EntityName);
                throw;
            }
        }

        /// <inheritdoc />
        public override async Task<TEntity?> Get(Guid id,
            bool includeDeleted = false,
            CancellationToken cancellationToken = default)
        {
            try
            {
                Logger.LogDebug("Getting {EntityName} by ID {EntityId} (IncludeDeleted: {IncludeDeleted})", 
                    EntityName, id, includeDeleted);

                var query = AddDeletedFilter(Table, includeDeleted);
                TEntity? entity = await query.SingleOrDefaultAsync(e => e.Id.Equals(id), cancellationToken);

                if (entity != null)
                {
                    Logger.LogDebug("Successfully retrieved {EntityName} with ID {EntityId}", EntityName, id);
                }
                else
                {
                    Logger.LogDebug("{EntityName} with ID {EntityId} not found", EntityName, id);
                }

                return entity;
            }
            catch (Exception ex)
            {
                Logger.LogRepositoryException(ex, "GetById", EntityName, id);
                throw;
            }
        }

        /// <inheritdoc />
        public override async Task<IEnumerable<TEntity>> Get(Func<IQueryable<TEntity>, IQueryable<TEntity>> func,
            bool includeDeleted = false,
            CancellationToken cancellationToken = default)
        {
            try
            {
                Logger.LogDebug("Getting {EntityName} collection using custom query (IncludeDeleted: {IncludeDeleted})", 
                    EntityName, includeDeleted);

                var query = AddDeletedFilter(Table, includeDeleted);
                query = func(query);

                var results = await query.ToListAsync(cancellationToken);
                
                Logger.LogInformation("Retrieved {Count} {EntityName} records using custom query", 
                    results.Count, EntityName);

                return results;
            }
            catch (Exception ex)
            {
                Logger.LogRepositoryException(ex, "GetWithCustomQuery", EntityName);
                throw;
            }
        }

        /// <inheritdoc />
        public override async Task<IEnumerable<TEntity>> Get(bool includeDeleted = false,
            CancellationToken cancellationToken = default)
        {
            try
            {
                Logger.LogDebug("Getting all {EntityName} entities (IncludeDeleted: {IncludeDeleted})", 
                    EntityName, includeDeleted);

                var query = AddDeletedFilter(Table, includeDeleted);
                var results = await query.ToListAsync(cancellationToken);
                
                Logger.LogInformation("Retrieved {Count} {EntityName} records", results.Count, EntityName);

                return results;
            }
            catch (Exception ex)
            {
                Logger.LogRepositoryException(ex, "GetAll", EntityName);
                throw;
            }
        }

        /// <inheritdoc />
        public override async Task<TEntity> Insert(TEntity entity,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(entity);

            try
            {
                Logger.LogInformation("Inserting new {EntityName} with ID {EntityId}", EntityName, entity.Id);

                await Table.AddAsync(entity, cancellationToken);

                Logger.LogInformation("Successfully prepared {EntityName} with ID {EntityId} for insertion", EntityName, entity.Id);

                return entity;
            }
            catch (Exception ex)
            {
                Logger.LogRepositoryException(ex, "Insert", EntityName, entity.Id);
                throw;
            }
        }

        /// <inheritdoc />
        public override async Task<IEnumerable<TEntity>> Insert(IList<TEntity> entities,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(entities);
            
            if (!entities.Any()) 
            {
                Logger.LogWarning("Attempted to insert empty collection of {EntityName}", EntityName);
                return [];
            }

            try
            {
                Logger.LogInformation("Inserting {Count} {EntityName} entities", entities.Count, EntityName);

                await Table.AddRangeAsync(entities, cancellationToken);

                Logger.LogInformation("Successfully prepared {Count} {EntityName} entities for insertion", entities.Count, EntityName);

                return entities;
            }
            catch (Exception ex)
            {
                Logger.LogRepositoryException(ex, "BulkInsert", EntityName);
                throw;
            }
        }

        /// <inheritdoc />
        public override async Task<TEntity> Update(TEntity entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            try
            {
                Logger.LogInformation("Updating {EntityName} with ID {EntityId}", EntityName, entity.Id);

                Table.Update(entity);

                Logger.LogInformation("Successfully marked {EntityName} with ID {EntityId} for update", EntityName, entity.Id);

                return entity;
            }
            catch (Exception ex)
            {
                Logger.LogRepositoryException(ex, "Update", EntityName, entity.Id);
                throw;
            }
        }
    }
}
