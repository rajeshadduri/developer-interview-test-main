using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Interfaces
{
    public interface IRebateCalculatorFactory
    {
        IRebateCalculator GetCalculator(Rebate rebate, Product product, CalculateRebateRequest request);
    }
}