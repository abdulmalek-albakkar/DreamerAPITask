using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dreamer.API.Dtos.Admin
{
    public class MarkOrderAsDeliveredDto
    {
        public Guid OrderId { get; set; }
    }
}
