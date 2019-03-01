using Dreamer.ShippingCostCalculator.IService;

namespace Dreamer.ShippingCostCalculator.Service
{
    /// <summary>
    /// This service should use an API to calculate the cost based on Address,Currency,Distance,Account plan (basic/premium),Shipping method, Shipping marchant
    /// </summary>
    public class ShippingCostCalculatorService : IShippingCostCalculatorService
    {
        public double Calculate(string Address)
        {
            return 10000;
        }
    }
}
