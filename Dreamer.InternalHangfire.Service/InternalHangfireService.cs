using Dreamer.EmailNotifier.IService;
using Dreamer.InternalHangfire.IService;
using Dreamer.Models.Main;
using Dreamer.SqlServer.Database;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Linq;

namespace Dreamer.InternalHangfire.Service
{
    public class InternalHangfireService : IInternalHangfireService
    {
        private DreamerDbContext DreamerDbContext { get; }
        private IEmailNotifierService EmailNotifierService { get; }
        public InternalHangfireService(DreamerDbContext dreamerDbContext, IEmailNotifierService emailNotifierService)
        {
            DreamerDbContext = dreamerDbContext;
            EmailNotifierService = emailNotifierService;
        }
        public void EnqueueMarkOrderAsShipped(Guid orderId)
        {
            try
            {
                BackgroundJob.Schedule(() => MarkOrderAsShipped(orderId), TimeSpan.FromMinutes(1));
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }
        public void MarkOrderAsShipped(Guid orderId)
        {
            try
            {
                var orderSet = DreamerDbContext.Orders.Include(order => order.OrderShipment).Include(order => order.DreamerUser).Where(order => order.Id == orderId).First();
                orderSet.Status = Models.Enums.OrderStatus.Shipped;
                orderSet.OrderShipment.ShippedTime = DateTimeOffset.UtcNow;
                DreamerDbContext.SaveChanges();


                EmailNotifierService.SendShippedSuccessfully(orderSet.DreamerUser.Email, orderSet.SerialNumber);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }
    }
}
