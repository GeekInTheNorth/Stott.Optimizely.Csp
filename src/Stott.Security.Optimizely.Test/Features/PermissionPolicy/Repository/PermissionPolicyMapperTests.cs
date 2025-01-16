﻿using System;
using System.Collections.Generic;

using NUnit.Framework;

using Stott.Security.Optimizely.Features.PermissionPolicy;
using Stott.Security.Optimizely.Features.PermissionPolicy.Models;
using Stott.Security.Optimizely.Features.PermissionPolicy.Repository;

namespace Stott.Security.Optimizely.Test.Features.PermissionPolicy.Repository;

[TestFixture]
public sealed class PermissionPolicyMapperTests
{
    [Test]
    [TestCase("None", PermissionPolicyEnabledState.None)]
    [TestCase("All", PermissionPolicyEnabledState.All)]
    [TestCase("ThisSite", PermissionPolicyEnabledState.ThisSite)]
    [TestCase("ThisAndSpecificSites", PermissionPolicyEnabledState.ThisAndSpecificSites)]
    [TestCase("SpecificSites", PermissionPolicyEnabledState.SpecificSites)]
    public void ToModel_CorrectlyConvertsAnEntityToAModel(string enabledState, PermissionPolicyEnabledState expectedState)
    {
        // Arrange
        var entity = new Entities.PermissionPolicy
        {
            Directive = PermissionPolicyConstants.Accelerometer,
            EnabledState = enabledState,
            Origins = "https://www.example.com,https://www.test.com"
        };

        // Act
        var result = PermissionPolicyMapper.ToModel(entity);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo(entity.Directive));
        Assert.That(result.EnabledState, Is.EqualTo(expectedState));
        Assert.That(result.Sources, Has.Count.EqualTo(2));
        Assert.That(result.Sources[0].Url, Is.EqualTo("https://www.example.com"));
        Assert.That(result.Sources[1].Url, Is.EqualTo("https://www.test.com"));
    }

    [Test]
    [TestCase(PermissionPolicyEnabledState.None, "None")]
    [TestCase(PermissionPolicyEnabledState.All, "All")]
    [TestCase(PermissionPolicyEnabledState.ThisSite, "ThisSite")]
    [TestCase(PermissionPolicyEnabledState.ThisAndSpecificSites, "ThisAndSpecificSites")]
    [TestCase(PermissionPolicyEnabledState.SpecificSites, "SpecificSites")]
    public void ToEntity_MapsAModelOnToAnEntity(PermissionPolicyEnabledState enabledState, string expectedState)
    {
        // Arrange
        var modifiedBy = Guid.NewGuid().ToString();

        var model = new SavePermissionPolicyModel
        {
            Name = PermissionPolicyConstants.Accelerometer.ToString(),
            EnabledState = enabledState,
            Sources = ["https://www.example.com", "https://www.test.com"]
        };

        var entity = new Entities.PermissionPolicy();

        // Act
        PermissionPolicyMapper.ToEntity(model, entity, modifiedBy);

        // Assert
        Assert.That(entity.Directive, Is.EqualTo(model.Name));
        Assert.That(entity.EnabledState, Is.EqualTo(expectedState));
        Assert.That(entity.Origins, Is.EqualTo("https://www.example.com,https://www.test.com"));
        Assert.That(entity.Modified, Is.Not.Null);
        Assert.That(entity.ModifiedBy, Is.EqualTo(modifiedBy));
    }

    [Test]
    [TestCaseSource(typeof(PermissionPolicyMapperTestCases), nameof(PermissionPolicyMapperTestCases.ToPolicyFragmentTestCases))]
    public void ToPolicyFragment_MapsAnEntityToAPolicyFragment(string directiveName, string enabledState, string origins, string expectedResult)
    {
        // Arrange
        var entity = new Entities.PermissionPolicy
        {
            Directive = directiveName,
            EnabledState = enabledState,
            Origins = origins
        };

        // Act
        var result = PermissionPolicyMapper.ToPolicyFragment(entity);

        // Assert
        Assert.That(result, Is.EqualTo(expectedResult));
    }
}

public static class PermissionPolicyMapperTestCases
{
    public static IEnumerable<TestCaseData> ToPolicyFragmentTestCases
    {
        get
        {
            yield return new TestCaseData(PermissionPolicyConstants.Accelerometer.ToString(), PermissionPolicyEnabledState.None.ToString(), string.Empty, "accelerometer=()");
            yield return new TestCaseData(PermissionPolicyConstants.AmbientLightSensor.ToString(), PermissionPolicyEnabledState.All.ToString(), string.Empty, "ambient-light-sensor=*");
            yield return new TestCaseData(PermissionPolicyConstants.Autoplay.ToString(), PermissionPolicyEnabledState.ThisSite.ToString(), string.Empty, "autoplay=(self)");
            yield return new TestCaseData(PermissionPolicyConstants.Bluetooth.ToString(), PermissionPolicyEnabledState.ThisAndSpecificSites.ToString(), string.Empty, "bluetooth=(self )");
            yield return new TestCaseData(PermissionPolicyConstants.Camera.ToString(), PermissionPolicyEnabledState.ThisAndSpecificSites.ToString(), "https://www.example.com", "camera=(self \"https://www.example.com\")");
            yield return new TestCaseData(PermissionPolicyConstants.Fullscreen.ToString(), PermissionPolicyEnabledState.ThisAndSpecificSites.ToString(), "https://www.example.com,https://www.test.com", "fullscreen=(self \"https://www.example.com\" \"https://www.test.com\")");
            yield return new TestCaseData(PermissionPolicyConstants.Gamepad.ToString(), PermissionPolicyEnabledState.SpecificSites.ToString(), string.Empty, "gamepad=()");
            yield return new TestCaseData(PermissionPolicyConstants.Geolocation.ToString(), PermissionPolicyEnabledState.SpecificSites.ToString(), "https://www.example.com", "geolocation=(\"https://www.example.com\")");
            yield return new TestCaseData(PermissionPolicyConstants.Gyroscope.ToString(), PermissionPolicyEnabledState.SpecificSites.ToString(), "https://www.example.com,https://www.test.com", "gyroscope=(\"https://www.example.com\" \"https://www.test.com\")");
        }
    }
}