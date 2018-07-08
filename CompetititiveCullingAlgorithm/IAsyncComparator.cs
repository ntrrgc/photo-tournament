using System.Threading;
using System.Threading.Tasks;

namespace CompetititiveCullingAlgorithm
{
    public interface IAsyncComparator<T>
    {
        Task<int> CompareAsync(T item, T other, CancellationToken cancellationToken);
    }
}