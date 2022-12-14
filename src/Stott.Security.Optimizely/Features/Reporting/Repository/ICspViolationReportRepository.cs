﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stott.Security.Optimizely.Features.Reporting.Repository
{
    public interface ICspViolationReportRepository
    {
        Task SaveAsync(string blockedUri, string violatedDirective);

        Task<IList<ViolationReportSummary>> GetReportAsync(DateTime threshold);

        Task<int> DeleteAsync(DateTime threshold);
    }
}
