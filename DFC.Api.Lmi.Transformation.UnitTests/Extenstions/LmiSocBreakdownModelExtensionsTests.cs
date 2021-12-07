using DFC.Api.Lmi.Transformation.Common;
using DFC.Api.Lmi.Transformation.Extensions;
using DFC.Api.Lmi.Transformation.Models.LmiImportApiModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DFC.Api.Lmi.Transformation.UnitTests.Extenstions
{
    [Trait("Category", "LmiSocBreakdownModelExtensions - LmiSocBreakdownModel extensions Unit Tests")]
    public class LmiSocBreakdownModelExtensionsTests
    {
        [Fact]
        public void LmiSocBreakdownModelExtensionsWithFullDataIsSuccessful()
        {
            // Arrange
            var testData = new LmiSocBreakdownModel
            {
                Note = "a note",
                Measure = Constants.MeasureForIndustry,
                PredictedEmployment = new List<LmiSocBreakdownYearModel>
                {
                    new LmiSocBreakdownYearModel
                    {
                        Year = DateTime.UtcNow.Year,
                        Breakdown = new List<LmiSocBreakdownYearValueModel>
                        {
                            new LmiSocBreakdownYearValueModel
                            {
                                Code = 123,
                                Note = "a note",
                                Name = "a name",
                                Employment = 123,
                            },
                        },
                    },
                },
            };

            // Act
            testData.SetMeasures();

            // Assert
            Assert.Equal(testData.Measure, testData.PredictedEmployment.First().Breakdown.First().Measure);
        }

        [Fact]
        public void LmiSocBreakdownModelExtensionsWithNoBreakdownIsSuccessful()
        {
            // Arrange
            var testData = new LmiSocBreakdownModel
            {
                Note = "a note",
                Measure = Constants.MeasureForIndustry,
                PredictedEmployment = new List<LmiSocBreakdownYearModel>
                {
                    new LmiSocBreakdownYearModel
                    {
                        Year = DateTime.UtcNow.Year,
                        Breakdown = null,
                    },
                },
            };

            // Act
            testData.SetMeasures();

            // Assert
            Assert.True(true);
        }

        [Fact]
        public void LmiSocBreakdownModelExtensionsWithNoPredictedEmploymentIsSuccessful()
        {
            // Arrange
            var testData = new LmiSocBreakdownModel
            {
                Note = "a note",
                Measure = Constants.MeasureForIndustry,
                PredictedEmployment = null,
            };

            // Act
            testData.SetMeasures();

            // Assert
            Assert.True(true);
        }

        [Fact]
        public void LmiSocBreakdownModelExtensionsWithNullDataIsSuccessful()
        {
            // Arrange
            const LmiSocBreakdownModel? testData = null;

            // Act
            testData.SetMeasures();

            // Assert
            Assert.True(true);
        }
    }
}
