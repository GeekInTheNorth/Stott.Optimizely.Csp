﻿namespace Stott.Security.Optimizely.Test.Features.Header;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using EPiServer.Core;
using EPiServer.ServiceLocation;

using Moq;

using NUnit.Framework;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Caching;
using Stott.Security.Optimizely.Features.Header;
using Stott.Security.Optimizely.Features.Nonce;
using Stott.Security.Optimizely.Features.Pages;
using Stott.Security.Optimizely.Features.Permissions.Repository;
using Stott.Security.Optimizely.Features.Sandbox;
using Stott.Security.Optimizely.Features.Sandbox.Repository;
using Stott.Security.Optimizely.Features.SecurityHeaders.Enums;
using Stott.Security.Optimizely.Features.SecurityHeaders.Repository;
using Stott.Security.Optimizely.Features.Settings.Repository;

[TestFixture]
public sealed class HeaderCompilationServiceTests
{
    private Mock<ICspPermissionRepository> _cspPermissionRepository;

    private Mock<ICspSettingsRepository> _cspSettingsRepository;

    private Mock<ICspSandboxRepository> _cspSandboxRepository;

    private Mock<ISecurityHeaderRepository> _securityHeaderRepository;

    private Mock<ICspContentBuilder> _headerBuilder;

    private Mock<ICspReportUrlResolver> _mockReportUrlResolver;

    private Mock<INonceProvider> _mockNonceProvider;

    private Mock<ICacheWrapper> _cacheWrapper;

    private Mock<IServiceProvider> _mockServiceProvider;

    private HeaderCompilationService _service;

    [SetUp]
    public void SetUp()
    {
        _cspPermissionRepository = new Mock<ICspPermissionRepository>();

        _cspSettingsRepository = new Mock<ICspSettingsRepository>();
        _cspSettingsRepository.Setup(x => x.GetAsync()).ReturnsAsync(new CspSettings());

        _cspSandboxRepository = new Mock<ICspSandboxRepository>();

        _securityHeaderRepository = new Mock<ISecurityHeaderRepository>();
        _securityHeaderRepository.Setup(x => x.GetAsync()).ReturnsAsync(new SecurityHeaderSettings());

        _headerBuilder = new Mock<ICspContentBuilder>();

        _mockReportUrlResolver = new Mock<ICspReportUrlResolver>();

        _mockNonceProvider = new Mock<INonceProvider>();

        _cacheWrapper = new Mock<ICacheWrapper>();

        _mockServiceProvider = new Mock<IServiceProvider>();
        _mockServiceProvider.Setup(x => x.GetService(typeof(ISecurityHeaderRepository))).Returns(_securityHeaderRepository.Object);
        _mockServiceProvider.Setup(x => x.GetService(typeof(ICspPermissionRepository))).Returns(_cspPermissionRepository.Object);
        _mockServiceProvider.Setup(x => x.GetService(typeof(ICspSettingsRepository))).Returns(_cspSettingsRepository.Object);
        _mockServiceProvider.Setup(x => x.GetService(typeof(ICspSandboxRepository))).Returns(_cspSandboxRepository.Object);

        ServiceLocator.SetServiceProvider(_mockServiceProvider.Object);

        _service = new HeaderCompilationService(
            _headerBuilder.Object,
            _mockReportUrlResolver.Object,
            _mockNonceProvider.Object,
            _cacheWrapper.Object);
    }

    [Test]
    [TestCaseSource(typeof(HeaderCompilationServiceTestCases), nameof(HeaderCompilationServiceTestCases.GetEmptySourceTestCases))]
    public async Task GetSecurityHeaders_PassesEmptyCollectionIntoHeaderBuilderWhenRepositoryReturnsNullOrEmptySources(
        IList<CspSource> configuredSources)
    {
        // Arrange
        _cspPermissionRepository.Setup(x => x.GetAsync()).ReturnsAsync(configuredSources);
        _cspSettingsRepository.Setup(x => x.GetAsync()).ReturnsAsync(new CspSettings { IsEnabled = true });

        List<ICspSourceMapping> sourcesUsed = null;
        _headerBuilder.Setup(x => x.WithSettings(It.IsAny<CspSettings>())).Returns(_headerBuilder.Object);
        _headerBuilder.Setup(x => x.WithSandbox(It.IsAny<SandboxModel>())).Returns(_headerBuilder.Object);
        _headerBuilder.Setup(x => x.WithSources(It.IsAny<IEnumerable<ICspSourceMapping>>()))
                      .Returns(_headerBuilder.Object)
                      .Callback<IEnumerable<ICspSourceMapping>>(x => sourcesUsed = x.ToList());

        // Act
        await _service.GetSecurityHeadersAsync(null);

        // Assert
        Assert.That(sourcesUsed, Is.Not.Null);
        Assert.That(sourcesUsed, Is.Empty);
    }

    [Test]
    public async Task GetSecurityHeaders_MergesConfiguredAndPageSourcesToPassIntoTheHeaderBuilder()
    {
        // Arrange
        var configuredSources = new List<CspSource>
        {
            new() { Source = "https://www.google.com", Directives = $"{CspConstants.Directives.ScriptSource},{CspConstants.Directives.StyleSource}"},
            new() { Source = "https://www.example.com", Directives = $"{CspConstants.Directives.ScriptSource}"}
        };

        var pageSources = new List<PageCspSourceMapping>
        {
            new() { Source = CspConstants.Sources.SchemeBlob, Directives = $"{CspConstants.Directives.ImageSource},{CspConstants.Directives.MediaSource}"},
            new() { Source = CspConstants.Sources.SchemeData, Directives = $"{CspConstants.Directives.ImageSource}"}
        };

        var mockPageData = new Mock<TestPageData>(MockBehavior.Loose);
        mockPageData.Setup(x => x.ContentSecurityPolicySources).Returns(pageSources);

        _cspPermissionRepository.Setup(x => x.GetAsync()).ReturnsAsync(configuredSources);
        _cspSettingsRepository.Setup(x => x.GetAsync()).ReturnsAsync(new CspSettings { IsEnabled = true });

        List<ICspSourceMapping> sourcesUsed = null;
        _headerBuilder.Setup(x => x.WithSettings(It.IsAny<CspSettings>())).Returns(_headerBuilder.Object);
        _headerBuilder.Setup(x => x.WithSandbox(It.IsAny<SandboxModel>())).Returns(_headerBuilder.Object);
        _headerBuilder.Setup(x => x.WithSources(It.IsAny<IEnumerable<ICspSourceMapping>>()))
                      .Returns(_headerBuilder.Object)
                      .Callback<IEnumerable<ICspSourceMapping>>(x => sourcesUsed = x.ToList());

        // Act
        await _service.GetSecurityHeadersAsync(mockPageData.Object);

        // Assert
        Assert.That(sourcesUsed, Is.Not.Null);
        Assert.That(sourcesUsed.Count, Is.EqualTo(4));
        Assert.That(sourcesUsed.IndexOf(configuredSources[0]), Is.GreaterThanOrEqualTo(0));
        Assert.That(sourcesUsed.IndexOf(configuredSources[1]), Is.GreaterThanOrEqualTo(0));
        Assert.That(sourcesUsed.IndexOf(pageSources[0]), Is.GreaterThanOrEqualTo(0));
        Assert.That(sourcesUsed.IndexOf(pageSources[1]), Is.GreaterThanOrEqualTo(0));
    }

    [Test]
    public async Task GetSecurityHeaders_ContentSecurityHeaderIsAbsentWhenDisabled()
    {
        // Arrange
        _cspSettingsRepository.Setup(x => x.GetAsync()).ReturnsAsync(new CspSettings { IsEnabled = false });

        // Act
        var headers = await _service.GetSecurityHeadersAsync(null);

        // Assert
        Assert.That(headers.ContainsKey(CspConstants.HeaderNames.ContentSecurityPolicy), Is.False);
    }

    [Test]
    [TestCaseSource(typeof(HeaderCompilationServiceTestCases), nameof(HeaderCompilationServiceTestCases.GetCspReportOnlyTestCases))]
    public async Task GetSecurityHeaders_ContentSecurityHeaderIsCorrentlySetToReportOnly(bool isReportOnlyMode, string expectedHeader)
    {
        // Arrange
        var configuredSources = new List<CspSource>
        {
            new() { Source = "https://www.google.com", Directives = $"{CspConstants.Directives.ScriptSource},{CspConstants.Directives.StyleSource}"},
            new() { Source = "https://www.example.com", Directives = $"{CspConstants.Directives.ScriptSource}"}
        };

        _cspPermissionRepository.Setup(x => x.GetAsync()).ReturnsAsync(configuredSources);
        _cspSettingsRepository.Setup(x => x.GetAsync()).ReturnsAsync(new CspSettings { IsEnabled = true, IsReportOnly = isReportOnlyMode });

        _headerBuilder.Setup(x => x.WithSettings(It.IsAny<CspSettings>())).Returns(_headerBuilder.Object);
        _headerBuilder.Setup(x => x.WithSandbox(It.IsAny<SandboxModel>())).Returns(_headerBuilder.Object);
        _headerBuilder.Setup(x => x.WithSources(It.IsAny<IEnumerable<ICspSourceMapping>>()))
                      .Returns(_headerBuilder.Object);

        // Act
        var headers = await _service.GetSecurityHeadersAsync(null);

        // Assert
        Assert.That(headers.ContainsKey(expectedHeader), Is.True);
    }

    [Test]
    [TestCaseSource(typeof(HeaderCompilationServiceTestCases), nameof(HeaderCompilationServiceTestCases.GetReportingEndpointsTestCases))]
    public async Task GetSecurityHeaders_CorrectlyAssemblesReportingEndpointsHeader(
        bool useInternalReporting, 
        bool useExternalReporting, 
        string externalReportingUrl, 
        bool shouldExist, 
        string expectedValue)
    {
        // Arrange
        _mockReportUrlResolver.Setup(x => x.GetReportToPath()).Returns("htts://www.example.com/report-to");
        _cspSettingsRepository.Setup(x => x.GetAsync())
                              .ReturnsAsync(new CspSettings 
                              { 
                                  IsEnabled = true, 
                                  UseInternalReporting = useInternalReporting,
                                  UseExternalReporting = useExternalReporting,
                                  ExternalReportToUrl = externalReportingUrl,
                              });

        _headerBuilder.Setup(x => x.WithSettings(It.IsAny<CspSettings>())).Returns(_headerBuilder.Object);
        _headerBuilder.Setup(x => x.WithSandbox(It.IsAny<SandboxModel>())).Returns(_headerBuilder.Object);
        _headerBuilder.Setup(x => x.WithSources(It.IsAny<IEnumerable<ICspSourceMapping>>()))
                      .Returns(_headerBuilder.Object);

        // Act
        var headers = await _service.GetSecurityHeadersAsync(null);

        var headerExists = headers.TryGetValue(CspConstants.HeaderNames.ReportingEndpoints, out var actualValue);

        // Assert
        Assert.That(headerExists, Is.EqualTo(shouldExist));
        Assert.That(actualValue, Is.EqualTo(expectedValue));
    }

    [Test]
    public async Task GetSecurityHeaders_MiscellaneousSecurityHeadersAreAbsentWhenSecurityHeaderRepositoryReturnsANull()
    {
        // Arrange
        _securityHeaderRepository.Setup(x => x.GetAsync()).ReturnsAsync((SecurityHeaderSettings)null);

        // Act
        var headers = await _service.GetSecurityHeadersAsync(null);

        // Assert
        Assert.That(headers.ContainsKey(CspConstants.HeaderNames.ContentTypeOptions), Is.False);
        Assert.That(headers.ContainsKey(CspConstants.HeaderNames.XssProtection), Is.False);
        Assert.That(headers.ContainsKey(CspConstants.HeaderNames.ReferrerPolicy), Is.False);
        Assert.That(headers.ContainsKey(CspConstants.HeaderNames.FrameOptions), Is.False);
        Assert.That(headers.ContainsKey(CspConstants.HeaderNames.CrossOriginEmbedderPolicy), Is.False);
        Assert.That(headers.ContainsKey(CspConstants.HeaderNames.CrossOriginOpenerPolicy), Is.False);
        Assert.That(headers.ContainsKey(CspConstants.HeaderNames.CrossOriginResourcePolicy), Is.False);
        Assert.That(headers.ContainsKey(CspConstants.HeaderNames.StrictTransportSecurity), Is.False);
    }

    [Test]
    [TestCaseSource(typeof(HeaderCompilationServiceTestCases), nameof(HeaderCompilationServiceTestCases.GetXContentTypeOptionsHeaderTestCases))]
    public async Task GetSecurityHeaders_XContentTypeOptionsHeaderIsPresentWhenEnabled(XContentTypeOptions xContentTypeOptions, bool shouldExist)
    {
        // Arrange
        _securityHeaderRepository.Setup(x => x.GetAsync())
                                 .ReturnsAsync(new SecurityHeaderSettings { XContentTypeOptions = xContentTypeOptions });

        // Act
        var headers = await _service.GetSecurityHeadersAsync(null);

        // Assert
        Assert.That(headers.ContainsKey(CspConstants.HeaderNames.ContentTypeOptions), Is.EqualTo(shouldExist));
    }

    [Test]
    [TestCaseSource(typeof(HeaderCompilationServiceTestCases), nameof(HeaderCompilationServiceTestCases.GetXssProtectionHeaderTestCases))]
    public async Task GetSecurityHeaders_XssProtectionHeaderIsPresentWhenNotSetToNone(XssProtection xssProtection, bool shouldHeaderExist)
    {
        // Arrange
        _securityHeaderRepository.Setup(x => x.GetAsync())
                                 .ReturnsAsync(new SecurityHeaderSettings { XssProtection = xssProtection });

        // Act
        var headers = await _service.GetSecurityHeadersAsync(null);

        // Assert
        Assert.That(headers.ContainsKey(CspConstants.HeaderNames.XssProtection), Is.EqualTo(shouldHeaderExist));
    }

    [Test]
    [TestCaseSource(typeof(HeaderCompilationServiceTestCases), nameof(HeaderCompilationServiceTestCases.GetReferrerPolicyTestCases))]
    public async Task GetSecurityHeaders_ReferrerPolicyHeaderIsPresentWhenNotSetToNone(ReferrerPolicy referrerPolicy, bool headerShouldExist)
    {
        // Arrange
        _securityHeaderRepository.Setup(x => x.GetAsync())
                                 .ReturnsAsync(new SecurityHeaderSettings { ReferrerPolicy = referrerPolicy });

        // Act
        var headers = await _service.GetSecurityHeadersAsync(null);

        // Assert
        Assert.That(headers.ContainsKey(CspConstants.HeaderNames.ReferrerPolicy), Is.EqualTo(headerShouldExist));
    }

    [Test]
    [TestCaseSource(typeof(HeaderCompilationServiceTestCases), nameof(HeaderCompilationServiceTestCases.GetFrameOptionsTestCases))]
    public async Task GetSecurityHeaders_FrameOptionsHeaderIsPresentWhenNotSetToNone(XFrameOptions frameOptions, bool headerShouldExist)
    {
        // Arrange
        _securityHeaderRepository.Setup(x => x.GetAsync())
                                 .ReturnsAsync(new SecurityHeaderSettings { FrameOptions = frameOptions });

        // Act
        var headers = await _service.GetSecurityHeadersAsync(null);

        // Assert
        Assert.That(headers.ContainsKey(CspConstants.HeaderNames.FrameOptions), Is.EqualTo(headerShouldExist));
    }

    [Test]
    [TestCaseSource(typeof(HeaderCompilationServiceTestCases), nameof(HeaderCompilationServiceTestCases.GetCrossOriginEmbedderPolicyTestCases))]
    public async Task GetSecurityHeaders_CrossOriginEmbedderPolicyHeaderIsPresentWhenNotSetToNone(CrossOriginEmbedderPolicy crossOriginEmbedderPolicy, bool headerShouldExist)
    {
        // Arrange
        _securityHeaderRepository.Setup(x => x.GetAsync())
                                 .ReturnsAsync(new SecurityHeaderSettings { CrossOriginEmbedderPolicy = crossOriginEmbedderPolicy });

        // Act
        var headers = await _service.GetSecurityHeadersAsync(null);

        // Assert
        Assert.That(headers.ContainsKey(CspConstants.HeaderNames.CrossOriginEmbedderPolicy), Is.EqualTo(headerShouldExist));
    }

    [Test]
    [TestCaseSource(typeof(HeaderCompilationServiceTestCases), nameof(HeaderCompilationServiceTestCases.GetCrossOriginEmbedderPolicyTestCases))]
    public async Task GetSecurityHeaders_CrossOriginOpenerPolicyHeaderIsPresentWhenNotSetToNone(CrossOriginOpenerPolicy crossOriginEmbedderPolicy, bool headerShouldExist)
    {
        // Arrange
        _securityHeaderRepository.Setup(x => x.GetAsync())
                                 .ReturnsAsync(new SecurityHeaderSettings { CrossOriginOpenerPolicy = crossOriginEmbedderPolicy });

        // Act
        var headers = await _service.GetSecurityHeadersAsync(null);

        // Assert
        Assert.That(headers.ContainsKey(CspConstants.HeaderNames.CrossOriginOpenerPolicy), Is.EqualTo(headerShouldExist));
    }

    [Test]
    [TestCaseSource(typeof(HeaderCompilationServiceTestCases), nameof(HeaderCompilationServiceTestCases.GetCrossOriginEmbedderPolicyTestCases))]
    public async Task GetSecurityHeaders_CrossOriginResourcePolicyHeaderIsPresentWhenNotSetToNone(CrossOriginResourcePolicy crossOriginEmbedderPolicy, bool headerShouldExist)
    {
        // Arrange
        _securityHeaderRepository.Setup(x => x.GetAsync())
                                 .ReturnsAsync(new SecurityHeaderSettings { CrossOriginResourcePolicy = crossOriginEmbedderPolicy });

        // Act
        var headers = await _service.GetSecurityHeadersAsync(null);

        // Assert
        Assert.That(headers.ContainsKey(CspConstants.HeaderNames.CrossOriginResourcePolicy), Is.EqualTo(headerShouldExist));
    }

    [Test]
    [TestCaseSource(typeof(HeaderCompilationServiceTestCases), nameof(HeaderCompilationServiceTestCases.GetStrictTransportSecurityTestCases))]
    public async Task GetSecurityHeaders_CorrectlySetsTheStrictTransportSecurityHeaderWhenEnabled(bool isEnabled, int maxAge, bool includeSubdomains, bool shouldExist, string expectedValue)
    {
        // Arrange
        _securityHeaderRepository.Setup(x => x.GetAsync())
                                 .ReturnsAsync(new SecurityHeaderSettings 
                                 { 
                                     IsStrictTransportSecurityEnabled = isEnabled,
                                     StrictTransportSecurityMaxAge = maxAge,
                                     IsStrictTransportSecuritySubDomainsEnabled = includeSubdomains
                                 });

        // Act
        var headers = await _service.GetSecurityHeadersAsync(null);

        var headerExists = headers.TryGetValue(CspConstants.HeaderNames.StrictTransportSecurity, out var actualValue);

        // Asser
        Assert.That(headerExists, Is.EqualTo(shouldExist));
        Assert.That(actualValue, Is.EqualTo(expectedValue));
    }

    [Test]
    public async Task GetSecurityHeadersAsync_UsesDefaultCacheKeyWhenThereIsNoPageData()
    {
        // Arrange
        string cacheKeyUsed = null;
        var headers = new Dictionary<string, string> { { "HeaderOne", "HeaderOneValues" } };

        _cacheWrapper.Setup(x => x.Get<Dictionary<string, string>>(It.IsAny<string>()))
                     .Returns(headers)
                     .Callback<string>(x => cacheKeyUsed = x);

        // Act
        _ = await _service.GetSecurityHeadersAsync(null);

        // Assert
        Assert.That(cacheKeyUsed, Is.EqualTo(CspConstants.CacheKeys.CompiledHeaders));
    }

    [Test]
    public async Task GetSecurityHeadersAsync_UsesDefaultCacheKeyWhenPageDataIsNotACspPage()
    {
        // Arrange
        string cacheKeyUsed = null;
        var headers = new Dictionary<string, string> { { "HeaderOne", "HeaderOneValues" } };
        var mockPageData = new Mock<PageData>(MockBehavior.Loose);

        _cacheWrapper.Setup(x => x.Get<Dictionary<string, string>>(It.IsAny<string>()))
                     .Returns(headers)
                     .Callback<string>(x => cacheKeyUsed = x);

        // Act
        _ = await _service.GetSecurityHeadersAsync(mockPageData.Object);

        // Assert
        Assert.That(cacheKeyUsed, Is.EqualTo(CspConstants.CacheKeys.CompiledHeaders));
    }

    [Test]
    public async Task GetSecurityHeadersAsync_UsesDefaultCacheKeyWhenPageDataIsACspPageWithANullSourceCollection()
    {
        // Arrange
        string cacheKeyUsed = null;
        var headers = new Dictionary<string, string> { { "HeaderOne", "HeaderOneValues" } };
        var mockPageData = new Mock<TestPageData>(MockBehavior.Loose);
        mockPageData.Setup(x => x.ContentSecurityPolicySources).Returns((IList<PageCspSourceMapping>)null);

        _cacheWrapper.Setup(x => x.Get<Dictionary<string, string>>(It.IsAny<string>()))
                     .Returns(headers)
                     .Callback<string>(x => cacheKeyUsed = x);

        // Act
        _ = await _service.GetSecurityHeadersAsync(mockPageData.Object);

        // Assert
        Assert.That(cacheKeyUsed, Is.EqualTo(CspConstants.CacheKeys.CompiledHeaders));
    }

    [Test]
    public async Task GetSecurityHeadersAsync_UsesDefaultCacheKeyWhenPageDataIsACspPageWithAnEmptySourceCollection()
    {
        // Arrange
        string cacheKeyUsed = null;
        var headers = new Dictionary<string, string> { { "HeaderOne", "HeaderOneValues" } };
        var mockPageData = new Mock<TestPageData>(MockBehavior.Loose);
        mockPageData.Setup(x => x.ContentSecurityPolicySources).Returns(new List<PageCspSourceMapping>(0));

        _cacheWrapper.Setup(x => x.Get<Dictionary<string, string>>(It.IsAny<string>()))
                     .Returns(headers)
                     .Callback<string>(x => cacheKeyUsed = x);

        // Act
        _ = await _service.GetSecurityHeadersAsync(mockPageData.Object);

        // Assert
        Assert.That(cacheKeyUsed, Is.EqualTo(CspConstants.CacheKeys.CompiledHeaders));
    }

    [Test]
    public async Task GetSecurityHeadersAsync_UsesPageSpecificCacheKeyWhenPageDataIsACspPageWithSources()
    {
        // Arrange
        string cacheKeyUsed = null;
        var headers = new Dictionary<string, string> { { "HeaderOne", "HeaderOneValues" } };

        var pageSources = new List<PageCspSourceMapping>
        {
            new() { Source = CspConstants.Sources.Self, Directives = CspConstants.Directives.DefaultSource }
        };

        var mockPageData = new Mock<TestPageData>(MockBehavior.Loose);
        mockPageData.Setup(x => x.ContentSecurityPolicySources).Returns(pageSources);

        _cacheWrapper.Setup(x => x.Get<Dictionary<string, string>>(It.IsAny<string>()))
                     .Returns(headers)
                     .Callback<string>(x => cacheKeyUsed = x);

        // Act
        _ = await _service.GetSecurityHeadersAsync(mockPageData.Object);

        // Assert
        Assert.That(cacheKeyUsed, Is.Not.EqualTo(CspConstants.CacheKeys.CompiledHeaders));
        Assert.That(cacheKeyUsed.Contains(CspConstants.CacheKeys.CompiledHeaders), Is.True);
    }
}