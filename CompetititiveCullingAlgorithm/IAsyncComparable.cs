using System.Threading.Tasks;

namespace CompetititiveCullingAlgorithm
{
    internal interface IAsyncComparable<T> where T : IAsyncComparable<T>
    {
        Task<int> CompareToAsync(T other);
    }
}