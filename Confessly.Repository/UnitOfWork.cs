using Confessly.Domain;
using Confessly.Logging.Extensions;
using Confessly.Logging.Interfaces;
using Confessly.Repository.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Confessly.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ConfesslyDbContext _dbContext;
        private readonly ILoggingService _logger;
        private readonly IPerformanceLogger _performanceLogger;

        public UnitOfWork(ConfesslyDbContext dbContext, ILoggingService logger, IPerformanceLogger performanceLogger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _performanceLogger = performanceLogger ?? throw new ArgumentNullException(nameof(performanceLogger));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _logger.LogDebug("Disposing UnitOfWork and DbContext");
                _dbContext.Dispose();
            }
        }

        public async Task<int> SaveChanges(CancellationToken cancellationToken = default)
        {
            using var performanceTracker = _performanceLogger.TrackDatabaseOperation("SaveChanges", "UnitOfWork");
            
            var stopwatch = Stopwatch.StartNew();

            try
            {
                _logger.LogDebug("Starting SaveChanges operation");

                var changeCount = await _dbContext.SaveChangesAsync(cancellationToken);
                
                stopwatch.Stop();

                if (changeCount > 0)
                {
                    _logger.LogRepositoryMetrics("SaveChanges", "UnitOfWork", changeCount, stopwatch.Elapsed);
                    _logger.LogInformation("SaveChanges completed successfully. {ChangeCount} changes committed to database", changeCount);
                }
                else
                {
                    _logger.LogDebug("SaveChanges completed with no changes to commit");
                }

                return changeCount;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogRepositoryException(ex, "SaveChanges", "UnitOfWork");
                throw;
            }
        }
    }
}
