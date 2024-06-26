using System;
using System.Collections.Generic;
using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using BudgetManager.Repository;

namespace BudgetManager.Services.Contracts
{
    /// <summary>
    /// Service class for managing operations related to payments.
    /// </summary>
    public interface IPaymentService : IBaseService<PaymentModel, Payment, IRepository<Payment>>
    {
        /// <summary>
        /// Retrieves all payment categories.
        /// </summary>
        /// <returns>A list of payment category models.</returns>
        List<PaymentCategoryModel> GetPaymentCategories();

        /// <summary>
        /// Retrieves payments data based on specified filters.
        /// </summary>
        /// <param name="fromDate">Optional. Minimum date filter.</param>
        /// <param name="toDate">Optional. Maximum date filter.</param>
        /// <param name="userId">The ID of the user whose payments to retrieve.</param>
        /// <param name="bankAccountId">Optional. The ID of the bank account to filter payments.</param>
        /// <returns>A list of payment models matching the criteria.</returns>
        List<PaymentModel> GetPaymentsData(DateTime? fromDate, DateTime? toDate, int userId, int? bankAccountId);

        /// <summary>
        /// Retrieves all payment types.
        /// </summary>
        /// <returns>A list of payment type models.</returns>
        List<PaymentTypeModel> GetPaymentTypes();

        /// <summary>
        /// Clones an existing payment.
        /// </summary>
        /// <param name="id">The ID of the payment to clone.</param>
        /// <returns>The ID of the newly cloned payment.</returns>
        int ClonePayment(int id);

        /// <summary>
        /// Checks if a user has rights to access a specific payment.
        /// </summary>
        /// <param name="paymentId">The ID of the payment.</param>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>True if the user has access rights; otherwise, false.</returns>
        bool UserHasRightToPayment(int paymentId, int userId);
    }
}