using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfApplication1.Undoable
{
	/// <summary>
	/// 
	/// </summary>
	class UndoableContext
	{
		Stack<Tuple<IUndoable, object>> undoStack_;
		Stack<Tuple<IUndoable, object>> redoStack_;


		static UndoableContext currentContext_;
		public static UndoableContext CurrentContext
		{
			get
			{
				if (currentContext_ == null) {
					currentContext_ = new UndoableContext();
				}
				return currentContext_;
			}
			private set
			{
				currentContext_ = value;
			}
        }

		public ICommand UndoExecuteCommand
		{
			get
			{
				return new CommandDelegate(
					// execute
					(o) => {
						var com = undoStack_.Pop();
						var redoData = com.Item1.Undo(com.Item2);
						redoStack_.Push(new Tuple<IUndoable, object>(com.Item1, redoData));
					},
					// canExecute
					(o) => {
						return undoStack_.Count > 0;
					});
			}
		}

		public ICommand RedoExecuteCommand
		{
			get
			{
				return new CommandDelegate(
					// execute
					(o) => {
						var com = redoStack_.Pop();
						var undoData = com.Item1.Redo(com.Item2);
						undoStack_.Push(new Tuple<IUndoable, object>(com.Item1, undoData));
					},
					// canExecute
					(o) => {
						return redoStack_.Count > 0;
					});
			}
		}


		/// <summary>
		/// 
		/// </summary>
		public UndoableContext()
		{
			undoStack_ = new Stack<Tuple<IUndoable, object>>();
			redoStack_ = new Stack<Tuple<IUndoable, object>>();
		}


		/// <summary>
		/// 
		/// </summary>
		public void ClearHistory()
		{
			undoStack_.Clear();
			redoStack_.Clear();
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="command"></param>
		public void CommandStacking(IUndoable command)
		{
			CommandStacking(command, null);
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="command"></param>
		/// <param name="data"></param>
		public void CommandStacking(IUndoable command, object data)
		{
			// 同一オブジェクトに対する連続変更をマージする
			object preData = data;
			if (command.CommandMerge) {
				if (undoStack_.Count > 0) {
					if (command == undoStack_.Peek().Item1) {
						preData = undoStack_.Pop().Item2;
					}
				}
            }

			// push
			undoStack_.Push(new Tuple<IUndoable, object>(command, preData));

			// redoをクリア
			redoStack_.Clear();
        }

	}
}
