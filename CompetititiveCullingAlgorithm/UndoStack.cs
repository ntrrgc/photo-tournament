using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompetititiveCullingAlgorithm
{
    public class UndoStack
    {
        List<IUndoable> stack = new List<IUndoable>();
        int cursorPosition = 0; // index of a new undoable action

        public void Do(IUndoable undoable)
        {
            stack.RemoveRange(cursorPosition, stack.Count - cursorPosition);
            stack.Add(undoable);
            cursorPosition++;
            undoable.Do();
        }

        public bool CanUndo { get { return cursorPosition > 0; } }
        public bool CanRedo { get { return cursorPosition < stack.Count; } }

        public void Undo()
        {
            Debug.Assert(CanUndo);
            cursorPosition--;
            stack[cursorPosition].Undo();
        }

        public void Redo()
        {
            Debug.Assert(CanRedo);
            stack[cursorPosition].Do();
            cursorPosition++;
        }

        public void Clear()
        {
            stack.Clear();
            cursorPosition = 0;
        }
    }
}
