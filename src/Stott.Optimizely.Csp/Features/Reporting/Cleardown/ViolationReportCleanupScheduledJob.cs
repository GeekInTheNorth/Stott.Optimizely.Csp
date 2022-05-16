﻿using System;
using System.Threading.Tasks;

using EPiServer.DataAbstraction;
using EPiServer.Logging;
using EPiServer.PlugIn;
using EPiServer.Scheduler;

using Stott.Optimizely.Csp.Common;
using Stott.Optimizely.Csp.Features.Reporting.Repository;

namespace Stott.Optimizely.Csp.Features.Reporting.Cleardown
{
    [ScheduledPlugIn(
        DisplayName = "[CSP] Violation Report Clean Up", 
        Description = "Clears down CSP Violation Report entries that are older than 30 days.",
        GUID = "325aed37-58bb-4c14-b41f-5dc9af3d3696",
        DefaultEnabled = true,
        IntervalType = ScheduledIntervalType.Days,
        IntervalLength = 1,
        Restartable = false)]
    public class ViolationReportCleanupScheduledJob : ScheduledJobBase
    {
        private readonly ICspViolationReportRepository _repository;

        private readonly ILogger _logger = LogManager.GetLogger(typeof(ViolationReportCleanupScheduledJob));

        public ViolationReportCleanupScheduledJob(ICspViolationReportRepository repository)
        {
            _repository = repository;
        }

        public override string Execute()
        {
            try
            {
                var threshold = DateTime.Today.AddDays(0 - CspConstants.LogRetentionDays);
                var itemsDeleted = Task.Run(() => _repository.DeleteAsync(threshold)).Result;

                return $"{itemsDeleted} CSP Violation Record(s) were deleted.";
            }
            catch (Exception exception)
            {
                _logger.Error($"{CspConstants.LogPrefix} Failure encountered when clearing down CSP Violation Reports.", exception);

                return $"An error was encountered: {exception.Message}";
            }
        }
    }
}
