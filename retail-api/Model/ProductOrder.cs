﻿using System;
using System.Collections.Generic;

namespace RetailApi.Model
{
    public partial class ProductOrder
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public int? ProductId { get; set; }
        public int? OrderId { get; set; }

        public virtual Orders Order { get; set; }
        public virtual Products Product { get; set; }
    }
}