using Dreamer.EmailNotifier.Dto;
using Dreamer.EmailNotifier.IService;
using Dreamer.Models.Main;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Net.Mail;
using System.Text;

namespace Dreamer.EmailNotifier.Service
{
    public class EmailNotifierService : IEmailNotifierService
    {
        private MailConfig MailConfig { get; }
        public EmailNotifierService(IOptionsMonitor<MailConfig> mailConfig)
        {
            MailConfig = mailConfig.CurrentValue;
        }

        public bool SendPaidSuccessfully(string to, OrderSet order)
        {
            StringBuilder body = new StringBuilder();
            body.AppendLine("Dear Customer,");
            body.AppendLine();
            body.AppendLine("Thank you for being our customer.");
            body.AppendLine();
            body.AppendLine($"The following order has been payed successfully:");
            body.AppendLine();
            body.AppendLine($"#{order.SerialNumber}        {order.PaymentDateTime}      [Delivary Cost:{order.OrderShipment.ShipmentCost} USD]      [Net Discount:{order.NetItemsDiscountValue} USD]       [Net items price:{order.NetItemsPrice} USD]");
            body.AppendLine();
            body.AppendLine($"Total: {order.Total} USD");
            body.AppendLine();
            body.AppendLine($"and will be shipped to your address {order.OrderShipment.Address} on {order.OrderShipment.ShippedTime}.");
            body.AppendLine();
            body.AppendLine($"Thanks.");
            return SendEmail("Customer Invoice", body.ToString(), to);
        }

        public bool SendShippedSuccessfully(string to, int orderSerialNumber)
        {
            StringBuilder body = new StringBuilder();
            body.AppendLine("Dear Customer,");
            body.AppendLine();
            body.AppendLine("Thank you for being our customer.");
            body.AppendLine();
            body.AppendLine($"Your order #{orderSerialNumber} has been shipped successfully and will be yours soon.");
            body.AppendLine();
            body.AppendLine($"Thanks.");
            return SendEmail("Customer Invoice", body.ToString(), to);
        }

        public bool SendDeliveredSuccessfully(string to, int orderSerialNumber)
        {
            StringBuilder body = new StringBuilder();
            body.AppendLine("Dear Customer,");
            body.AppendLine();
            body.AppendLine("Thank you for being our customer.");
            body.AppendLine();
            body.AppendLine($"Your order #{orderSerialNumber} has been delivered successfully, enjoy it.");
            body.AppendLine();
            body.AppendLine($"Thanks.");
            return SendEmail("Customer Invoice", body.ToString(), to);
        }

        private bool SendEmail(string subject, string body,string to)
        {
            SmtpClient client = new SmtpClient();
            client.Port = MailConfig.STMP_ClientPort;
            client.Host = MailConfig.STMP_ClientHost;
            client.EnableSsl = true;
            client.Timeout = 10000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(MailConfig.CompanyMail, MailConfig.CompanyMailPassword);

            MailMessage mm = new MailMessage(MailConfig.CompanyMail, to)
            {
                Subject = subject,
                Body = body.ToString()
            };
            mm.BodyEncoding = Encoding.UTF8;
            mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

            try
            {
                client.Send(mm);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }
    }
}
