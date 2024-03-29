﻿using System;
using System.Collections.Generic;
using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;

namespace BudgetManager.Services.Contracts
{
    public interface IPaymentService : IBaseService<PaymentModel, Payment>
    {
        List<PaymentCategoryModel> GetPaymentCategories();

        List<PaymentModel> GetPaymentsData(DateTime? fromDate, DateTime? toDate, int userId, int? bankAccountId);

        List<PaymentTypeModel> GetPaymentTypes();

        int ClonePayment(int id);

        bool UserHasRightToPayment(int paymentId, int userId);
    }
}