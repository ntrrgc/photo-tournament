﻿using System.Threading;
using System.Threading.Tasks;

namespace PhotoTournament
{
    public interface IAsyncComparator<T>
    {
        Task<int> CompareAsync(T item, T other, CancellationToken cancellationToken);
    }
}