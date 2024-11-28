using System;
using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Services;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Runner;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Enter RebateIdentifier");
        var rebateIdentifier = Console.ReadLine();
        Console.WriteLine("Enter ProductIdentifier");
        var productIdentifier = Console.ReadLine();
        Console.WriteLine("Write Volume");
        var volume = int.Parse(Console.ReadLine());

        var request = new CalculateRebateRequest
        {
            RebateIdentifier = rebateIdentifier,
            ProductIdentifier = productIdentifier,
            Volume = volume
        };

        var rebateService = new RebateService(new RebateDataStore(), new ProductDataStore(), new RebateCalculatorFactory());
        

        var result = rebateService.Calculate(new CalculateRebateRequest());
        Console.WriteLine("Result: " , result.Success);
        Console.ReadLine();
    }
}
