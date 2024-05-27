namespace Shortify.NET.Applicaion.Abstractions
{
    public interface IUrlShorteningService
    {
        /// <summary>
        /// Generates Unique Alpha-Numeric Code
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<string> GenerateUniqueCode(CancellationToken cancellationToken = default);
    }
}
