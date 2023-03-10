using System.Threading;
using System.Threading.Tasks;

namespace AdminCli.HttpAccess;

public interface IHttpService
{
    Task<T?> GetAsync<T>(string relativeUrl, CancellationToken cancellationToken = default);
    Task PostAsync<T>(string relativeUrl, T content, CancellationToken cancellationToken = default);
}
