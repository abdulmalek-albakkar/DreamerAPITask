using Dreamer.Models.Main;

namespace Dreamer.EmailNotifier.IService
{
    public interface IEmailNotifierService
    {
        bool SendPaidSuccessfully(string to, OrderSet order);
        bool SendShippedSuccessfully(string to, int orderSerialNumber);
        bool SendDeliveredSuccessfully(string to, int orderSerialNumber);
    }
}
