namespace BudgetManager.Domain.DTOs
{
    public record EnumItemModelAdjusted : IDtoModel
    {
        /// <summary>
        /// Gets or sets the ID of enum item.
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// Gets or sets the code representing enum item.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the name of enum item.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name of enum item type ID
        /// </summary>
        public int EnumItemTypeId { get; set; }

        /// <summary>
        /// Gets or sets the name of enum item type
        /// </summary>
        public string EnumItemTypeCode { get; set; }

        /// <summary>
        /// Gets or sets the name of enum item type
        /// </summary>
        public string EnumItemTypeName { get; set; }

        /// <summary>
        /// Gets or sets the metadata
        /// </summary>
        public string Metadata { get; set; }
    }
}
