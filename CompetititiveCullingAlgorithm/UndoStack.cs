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
        LinkedList<IUndoable> stack = new LinkedList<IUndoable>();
        LinkedListNode<IUndoable> cursorPosition = null; // Nodes starting at this cursor will be removed by new actions.
        private readonly int MaxUndoElements = 10;

        public void Do(IUndoable undoable)
        {
            while (cursorPosition != null)
            {
                var nextNode = cursorPosition.Next;
                stack.Remove(cursorPosition);
                cursorPosition = nextNode;
            }

            while (stack.Count > MaxUndoElements)
            {
                stack.RemoveFirst();
            }

            stack.AddLast(undoable);
            undoable.Do();
        }

        public bool CanUndo { get {
                return (cursorPosition == null && stack.Count > 0) ||
                    (cursorPosition != null && cursorPosition.Previous != null);
            } }
        public bool CanRedo { get {
                return cursorPosition != null;
            } }

        public void Undo()
        {
            Debug.Assert(CanUndo);
            if (cursorPosition != null)
                cursorPosition = cursorPosition.Previous;
            else
                cursorPosition = stack.Last;
            cursorPosition.Value.Undo();
        }

        public void Redo()
        {
            Debug.Assert(CanRedo);
            cursorPosition.Value.Do();
            cursorPosition = cursorPosition.Next;
        }

        public void Clear()
        {
            stack.Clear();
            cursorPosition = null;
        }
    }
}
