﻿namespace Stott.Security.Optimizely.Test.Features.Reporting;

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Moq;

using NUnit.Framework;

using Stott.Security.Optimizely.Features.Reporting;
using Stott.Security.Optimizely.Features.Reporting.Service;
using Stott.Security.Optimizely.Features.Whitelist;

[TestFixture]
public class CspReportingControllerTests
{
    private Mock<ICspViolationReportService> _mockReportService;

    private Mock<IWhitelistService> _mockWhitelistService;

    private Mock<ILogger<CspReportingController>> _mockLogger;

    private CspReportingController _controller;

    [SetUp]
    public void SetUp()
    {
        _mockReportService = new Mock<ICspViolationReportService>();
        _mockWhitelistService = new Mock<IWhitelistService>();
        _mockLogger = new Mock<ILogger<CspReportingController>>();

        _controller = new CspReportingController(
            _mockReportService.Object,
            _mockWhitelistService.Object,
            _mockLogger.Object);
    }

    [Test]
    public void Report_WhenTheCommandThrowsAnException_ThenTheErrorIsReThrown()
    {
        // Arrange
        var saveModel = new ReportModel();

        _mockReportService.Setup(x => x.SaveAsync(It.IsAny<ReportModel>()))
                          .ThrowsAsync(new Exception(string.Empty));

        // Assert
        Assert.ThrowsAsync<Exception>(() => _controller.Report(saveModel));
    }

    [Test]
    public async Task Report_WhenTheCommandIsSuccessful_ThenAnOkResponseIsReturned()
    {
        // Arrange
        var saveModel = new ReportModel();

        // Act
        var response = await _controller.Report(saveModel);

        // Assert
        Assert.That(response, Is.AssignableFrom<OkResult>());
    }

    [Test]
    public void ReportSummary_WhenTheCommandThrowsAnException_ThenTheErrorIsReThrown()
    {
        // Arrange
        _mockReportService.Setup(x => x.GetReportAsync(It.IsAny<DateTime>()))
                          .ThrowsAsync(new Exception(string.Empty));

        // Assert
        Assert.ThrowsAsync<Exception>(() => _controller.ReportSummary());
    }

    [Test]
    public async Task ReportSummary_WhenTheCommandIsSuccessful_ThenAnOkResponseIsReturned()
    {
        // Act
        var response = await _controller.ReportSummary();

        // Assert
        Assert.That(response, Is.AssignableFrom<ContentResult>());
        Assert.That((response as ContentResult).StatusCode, Is.EqualTo(200));
    }
}
