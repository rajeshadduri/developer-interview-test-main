using Smartwyre.DeveloperTest.Interfaces;
using Smartwyre.DeveloperTest.Types;
using Smartwyre.DeveloperTest.Utils;

namespace Smartwyre.DeveloperTest.Services
{
    public class RebateCalculatorFactory : IRebateCalculatorFactory
    {
        public IRebateCalculator GetCalculator(Rebate rebate, Product product, CalculateRebateRequest request)
        {
            switch (rebate.Incentive)
            {
                case IncentiveType.FixedCashAmount:
                    return new FixedCashAmountRebateCalculator();
                case IncentiveType.FixedRateRebate:
                    return new FixedRateRebateCalculator();
                case IncentiveType.AmountPerUom:
                    return new AmountPerUomRebateCalculator();
                default:
                    return null;
            }
        }
    }
}
