﻿using System;
using System.Collections.Generic;

namespace BudgetManager.Data.DataModels
{
    public class Payment
    {
        public int Id { get; set; }

        public decimal Amount { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime Date { get; set; }

        public int BankAccountId { get; set; }

        public BankAccount BankAccount { get; set; }

        public int PaymentTypeId { get; set; }

        public PaymentType PaymentType { get; set; }

        public int PaymentCategoryId { get; set; }

        public PaymentCategory PaymentCategory { get; set; }

        public IList<PaymentTag> PaymentTags { get; set; }
    }
}