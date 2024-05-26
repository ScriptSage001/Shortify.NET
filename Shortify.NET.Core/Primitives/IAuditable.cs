namespace Shortify.NET.Core.Primitives
{
    public interface IAuditable
    {
        /// <summary>
        /// CreatedOn DateTime UTC
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// UpdatedOn DateTime UTC
        /// </summary>
        public DateTime? UpdatedOnUtc { get; set; }

        /// <summary>
        /// RowStatus
        /// </summary>
        public bool RowStatus { get; set; }
    }
}
