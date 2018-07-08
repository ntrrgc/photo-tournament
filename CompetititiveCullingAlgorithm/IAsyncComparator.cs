using System.Threading.Tasks;

namespace CompetititiveCullingAlgorithm
{
    internal interface IAsyncComparator<T>
    {
        Task<int> CompareAsync(T item, T other);
    }
}