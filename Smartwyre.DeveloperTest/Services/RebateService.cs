using Smartwyre.DeveloperTest.Interfaces;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services;

public class RebateService : IRebateService
{
    private readonly IRebateDataStore _rebateDataStore;
    private readonly IProductDataStore _productDataStore;
    private readonly IRebateCalculatorFactory _rebateCalculatorFactory;

    public RebateService(IRebateDataStore rebateDataStore, IProductDataStore productDataStore, IRebateCalculatorFactory rebateCalculatorFactory)
    {
        _rebateDataStore = rebateDataStore;
        _productDataStore = productDataStore;
        _rebateCalculatorFactory = rebateCalculatorFactory;
    }

    public CalculateRebateResult Calculate(CalculateRebateRequest request)
    {
        var rebate = _rebateDataStore.GetRebate(request.RebateIdentifier);
        var product = _productDataStore.GetProduct(request.ProductIdentifier);

        var result = new CalculateRebateResult();

        if (rebate == null || product == null)
        {
            return result;
        }

        var rebateCalculator = _rebateCalculatorFactory.GetCalculator(rebate, product, request);
        if (rebateCalculator == null || !rebateCalculator.CanCalculate(rebate, product, request))
        {
            return result;
        }

        var rebateAmount = rebateCalculator.Calculate(rebate, product, request);
        if (rebateAmount > 0)
        {
            result.Success = true;
            _rebateDataStore.StoreCalculationResult(rebate, rebateAmount);
        }

        return result;
    }
}
