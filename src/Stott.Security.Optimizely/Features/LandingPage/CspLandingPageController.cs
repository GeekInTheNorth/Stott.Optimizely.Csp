﻿namespace Stott.Security.Optimizely.Features.LandingPage;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Stott.Security.Core.Common;

[Authorize(Policy = CspConstants.AuthorizationPolicy)]
[Route("[controller]/[action]")]
public class CspLandingPageController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Headers()
    {
        return View();
    }
}
