﻿namespace Stott.Security.Optimizely.Features.Settings;

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Common.Validation;
using Stott.Security.Optimizely.Features.Settings.Service;

[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = CspConstants.AuthorizationPolicy)]
public class CspSettingsController : BaseController
{
    private readonly ICspSettingsService _settings;

    private readonly ILogger<CspSettingsController> _logger;

    public CspSettingsController(
        ICspSettingsService service,
        ILogger<CspSettingsController> logger)
    {
        _settings = service;
        _logger = logger;
    }

    [HttpGet]
    [Route("[controller]/[action]")]
    public async Task<IActionResult> Get()
    {
        try
        {
            var data = await _settings.GetAsync();

            return CreateSuccessJson(new CspSettingsModel
            {
                IsEnabled = data.IsEnabled,
                IsReportOnly = data.IsReportOnly,
                IsWhitelistEnabled = data.IsWhitelistEnabled,
                WhitelistAddress = data.WhitelistUrl
            });
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"{CspConstants.LogPrefix} Failed to retrieve CSP settings.");
            throw;
        }
    }

    [HttpPost]
    [Route("[controller]/[action]")]
    public async Task<IActionResult> Save(CspSettingsModel model)
    {
        if (!ModelState.IsValid)
        {
            var validationModel = new ValidationModel(ModelState);
            return CreateValidationErrorJson(validationModel);
        }

        try
        {
            await _settings.SaveAsync(model);

            return Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"{CspConstants.LogPrefix} Failed to save CSP settings.");
            throw;
        }
    }
}