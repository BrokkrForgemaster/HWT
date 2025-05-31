
namespace HWT.Application.Interfaces
{
    /// <summary name="IUpdateService">
    /// This interface defines methods for checking and performing updates
    /// to the application using a remote service GitHub Releases.
    /// </summary>
    public interface IUpdateService
    {
        Task<bool> IsUpdateAvailableAsync();
        Task<bool> TryUpdateAsync(Action<double, string> reportProgress);
    }
}