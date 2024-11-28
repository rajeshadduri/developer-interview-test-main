using System;
using System.Collections.Generic;
using Moq;
using Smartwyre.DeveloperTest.Interfaces;
using Smartwyre.DeveloperTest.Services;
using Smartwyre.DeveloperTest.Types;
using Smartwyre.DeveloperTest.Utils;
using Xunit;

namespace Smartwyre.DeveloperTest.Tests;

public class PaymentServiceTests
{
    [Fact]
    public void Calculate_ShouldReturnSuccess_WhenRebateIsValid()
    {
        var rebateDataStoreMock = new Mock<IRebateDataStore>();
        var productDataStoreMock = new Mock<IProductDataStore>();
        var rebateCalculatorFactoryMock = new Mock<IRebateCalculatorFactory>();
        var rebateCalculatorMock = new Mock<IRebateCalculator>();

        rebateCalculatorFactoryMock.Setup(f => f.GetCalculator(It.IsAny<Rebate>(), It.IsAny<Product>(), It.IsAny<CalculateRebateRequest>()))
            .Returns(rebateCalculatorMock.Object);
        rebateCalculatorMock.Setup(c => c.CanCalculate(It.IsAny<Rebate>(), It.IsAny<Product>(), It.IsAny<CalculateRebateRequest>()))
            .Returns(true);
        rebateCalculatorMock.Setup(c => c.Calculate(It.IsAny<Rebate>(), It.IsAny<Product>(), It.IsAny<CalculateRebateRequest>()))
            .Returns(100m);

        var rebateService = new RebateService(rebateDataStoreMock.Object, productDataStoreMock.Object, rebateCalculatorFactoryMock.Object);

        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "Rebate1",
            ProductIdentifier = "Product1",
            Volume = 100
        };

        rebateDataStoreMock.Setup(x => x.GetRebate(It.IsAny<string>())).Returns(new Rebate { Incentive = IncentiveType.FixedCashAmount, Amount = 50 });
        productDataStoreMock.Setup(x => x.GetProduct(It.IsAny<string>())).Returns(new Product { SupportedIncentives = SupportedIncentiveType.FixedCashAmount });

        var result = rebateService.Calculate(request);

        Assert.True(result.Success);
    }

    [Fact]
    public void Calculate_ShouldReturnFailure_WhenRebateCannotBeCalculated()
    {
        // Arrange
        var rebateDataStoreMock = new Mock<IRebateDataStore>();
        var productDataStoreMock = new Mock<IProductDataStore>();
        var rebateCalculatorFactoryMock = new Mock<IRebateCalculatorFactory>();

        // Mock the Rebate data
        rebateDataStoreMock.Setup(x => x.GetRebate(It.IsAny<string>())).Returns(new Rebate { Incentive = IncentiveType.FixedCashAmount, Amount = 50 });

        // Mock the Product data
        productDataStoreMock.Setup(x => x.GetProduct(It.IsAny<string>())).Returns(new Product { SupportedIncentives = SupportedIncentiveType.FixedCashAmount });

        // Mock the factory to return null (no calculator available)
        rebateCalculatorFactoryMock.Setup(f => f.GetCalculator(It.IsAny<Rebate>(), It.IsAny<Product>(), It.IsAny<CalculateRebateRequest>()))
            .Returns<IRebateCalculator>(null);  // Simulating the case where no calculator is available

        // Create the RebateService with mocked dependencies
        var rebateService = new RebateService(rebateDataStoreMock.Object, productDataStoreMock.Object, rebateCalculatorFactoryMock.Object);

        // Define the request
        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "Rebate1",
            ProductIdentifier = "Product1",
            Volume = 100
        };

        // Act
        var result = rebateService.Calculate(request);

        // Assert
        // Ensure the result is a failure
        Assert.False(result.Success, "The calculation should have failed because no calculator was found.");
    }

    [Fact]
    public void Calculate_ShouldReturnFailure_WhenCanCalculateReturnsFalse()
    {
        // Arrange
        var rebateDataStoreMock = new Mock<IRebateDataStore>();
        var productDataStoreMock = new Mock<IProductDataStore>();
        var rebateCalculatorFactoryMock = new Mock<IRebateCalculatorFactory>();
        var rebateCalculatorMock = new Mock<IRebateCalculator>();

        // Mock the Rebate data
        rebateDataStoreMock.Setup(x => x.GetRebate(It.IsAny<string>())).Returns(new Rebate { Incentive = IncentiveType.FixedRateRebate, Percentage = 10 });

        // Mock the Product data
        productDataStoreMock.Setup(x => x.GetProduct(It.IsAny<string>())).Returns(new Product { SupportedIncentives = SupportedIncentiveType.FixedRateRebate, Price = 100 });

        // Mock the factory to return the FixedRateRebateCalculator
        rebateCalculatorFactoryMock.Setup(f => f.GetCalculator(It.IsAny<Rebate>(), It.IsAny<Product>(), It.IsAny<CalculateRebateRequest>()))
            .Returns(rebateCalculatorMock.Object);

        // Mock the CanCalculate method to return false (the rebate cannot be calculated)
        rebateCalculatorMock.Setup(c => c.CanCalculate(It.IsAny<Rebate>(), It.IsAny<Product>(), It.IsAny<CalculateRebateRequest>()))
            .Returns(false);

        // Create the RebateService with mocked dependencies
        var rebateService = new RebateService(rebateDataStoreMock.Object, productDataStoreMock.Object, rebateCalculatorFactoryMock.Object);

        // Define the request
        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "Rebate1",
            ProductIdentifier = "Product1",
            Volume = 100
        };

        // Act
        var result = rebateService.Calculate(request);

        // Assert
        // Ensure the result is a failure because CanCalculate returned false
        Assert.False(result.Success, "The calculation should have failed because the calculator's CanCalculate method returned false.");
    }
}
