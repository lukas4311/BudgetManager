﻿using System.Collections.Generic;

namespace Data.DataModels
{
    public class PaymentCategory
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public List<Payment> Payments { get; set; }
    }
}