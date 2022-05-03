﻿using System;
using System.Threading.Tasks;

using EPiServer.Logging;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Stott.Optimizely.Csp.Common;
using Stott.Optimizely.Csp.Features.Reporting.Repository;
using Stott.Optimizely.Csp.Features.Whitelist;

namespace Stott.Optimizely.Csp.Features.Reporting
{
    [Authorize(Roles = "CmsAdmin,WebAdmins,Administrators")]
    public class CspReportingController : BaseController
    {
        private readonly ICspViolationReportRepository _repository;

        private readonly IWhitelistService _whitelistService;

        private ILogger _logger = LogManager.GetLogger(typeof(CspReportingController));

        public CspReportingController(
            ICspViolationReportRepository repository, 
            IWhitelistService whitelistService)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _whitelistService = whitelistService ?? throw new ArgumentNullException(nameof(whitelistService));
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("[controller]/[action]")]
        public async Task<IActionResult> Report([FromBody]ReportModel cspReport)
        {
            try
            {
                _repository.Save(cspReport);

                var isOnWhitelist = await _whitelistService.IsOnWhitelist(cspReport.BlockedUri, cspReport.ViolatedDirective);
                if (isOnWhitelist)
                {
                    _whitelistService.AddToWhitelist(cspReport.BlockedUri, cspReport.ViolatedDirective);
                }

                return Ok();
            }
            catch (Exception exception)
            {
                _logger.Error($"{CspConstants.LogPrefix} Failed to save CSP Report.", exception);
                throw;
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("[controller]/[action]")]
        public IActionResult ReportSummary()
        {
            try
            {
                var reportDate = DateTime.Today.AddDays(0 - CspConstants.LogRetentionDays);
                var model = _repository.GetReport(reportDate);

                return CreateSuccessJson(model);
            }
            catch (Exception exception)
            {
                _logger.Error($"{CspConstants.LogPrefix} Failed to retrieve CSP Report.", exception);
                throw;
            }
        }
    }
}
