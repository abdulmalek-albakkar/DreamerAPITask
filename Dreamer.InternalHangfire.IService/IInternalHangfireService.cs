using System;

namespace Dreamer.InternalHangfire.IService
{
    public interface IInternalHangfireService
    {
        void EnqueueMarkOrderAsShipped(Guid orderId);
    }
}
