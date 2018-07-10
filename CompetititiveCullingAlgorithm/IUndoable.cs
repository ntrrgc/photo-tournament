using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentSort
{
    public interface IUndoable
    {
        string Name { get; }
        void Do();
        void Undo();
    }
}
