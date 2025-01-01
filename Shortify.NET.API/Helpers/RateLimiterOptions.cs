namespace Shortify.NET.API.Helpers
{
    /// <summary>
    /// Defines the options to configure the rate limiter
    /// </summary>
    public class RateLimiterOptions
    {
        /// <summary>
        /// Gets or Sets the number of request permitted per window
        /// </summary>
        public int PermitLimit { get; init; }
        
        /// <summary>
        /// Gets or Sets the timespan of one window in seconds
        /// </summary>
        public int WindowInSeconds { get; init; }
        
        /// <summary>
        /// Gets or Sets the number of segments the window is divided into
        /// </summary>
        public int SegmentsPerWindow { get; init; }
        
        /// <summary>
        /// Gets or Sets the number of requests permitted in the queue.
        /// Pass 0 for no queue.
        /// </summary>
        public int QueueLimit { get; init; }
    }
}