using HWT.Domain.Entities;

namespace HWT.Application.Interfaces
{
    public interface IRefineryJobsService
    {
        /// <summary>
        /// Calls the UEXCorp “user_refineries_jobs” endpoint and returns a list of RefineryJob objects.
        /// </summary>
        /// <param name="secretKey">Your UEX secret key (passed via Authorization header).</param>
        Task<IReadOnlyList<RefineryJob>> GetRefineryJobsAsync(string? secretKey);
    }
}