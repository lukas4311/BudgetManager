using Data.DataModels;
using ManagerWeb.Models.DTOs;
using ManagerWeb.Models.ViewModels;
using System;
using System.Collections.Generic;

namespace ManagerWeb.Services
{
    public interface IPaymentService
    {
        int AddPayment(PaymentViewModel paymentViewModel);

        List<BankAccount> GetBankAccounts();

        PaymentViewModel GetPayment(int id);

        List<PaymentCategoryModel> GetPaymentCategories();

        List<PaymentViewModel> GetPaymentsData(DateTime? fromDate, int? bankAccountId);

        List<PaymentTypeModel> GetPaymentTypes();

        void UpdatePayment(PaymentViewModel paymentViewModel);
    }
}