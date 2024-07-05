﻿using System;
using System.Collections.Generic;

namespace BusinessObject.Models;

public partial class Brand
{
    public int BrandId { get; set; }

    public string Name { get; set; } = null!;

    public string Image { get; set; } = null!;

    public bool Isdelete { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
