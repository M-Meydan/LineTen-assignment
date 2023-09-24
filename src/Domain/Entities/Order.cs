using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    using Domain.Enums;
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Order
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

        public Guid CustomerId { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

        public Product Product { get; set; }
        public Customer Customer { get; set; }
    }

}
