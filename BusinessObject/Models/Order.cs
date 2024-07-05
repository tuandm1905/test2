﻿using System;
using System.Collections.Generic;

namespace BusinessObject.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public string CodeOrder { get; set; } = null!;

    public string Fullname { get; set; } = null!;

    public DateTime OrderDate { get; set; }

    public DateTime? ShippedDate { get; set; }

    public DateTime? RequiredDate { get; set; }

    public string Phone { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Note { get; set; } = null!;

    public double TotalAmount { get; set; }

    public int AccountId { get; set; }

    public int OwnerId { get; set; }

    public int StatusId { get; set; }

    public string? VoucherId { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual Owner Owner { get; set; } = null!;

    public virtual OrderStatus Status { get; set; } = null!;

    public virtual Voucher? Voucher { get; set; }
}
