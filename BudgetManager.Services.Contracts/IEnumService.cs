using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using BudgetManager.Repository;
using System.Collections.Generic;

namespace BudgetManager.Services.Contracts
{
    /// <summary>
    /// Service for enum items
    /// </summary>
    public interface IEnumService : IBaseService<EnumItemModelAdjusted, EnumItem, IRepository<EnumItem>>
    {
        /// <summary>
        /// Get enum item by code
        /// </summary>
        /// <param name="enumItemCode">Enum item code</param>
        /// <returns>Model of enum item</returns>
        EnumItemModelAdjusted GetByCode(string enumItemCode);

        /// <summary>
        /// Get all enum items with specified enum type code
        /// </summary>
        /// <param name="enumItemTypeCode">Enum item type code</param>
        /// <returns>All enum items with specified type code</returns>
        IEnumerable<EnumItemModelAdjusted> GetAllByTypeCode(string enumItemTypeCode);
    }
}
