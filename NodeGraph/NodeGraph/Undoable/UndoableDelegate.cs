using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1.Undoable
{
	/// <summary>
	/// Undoableなコマンドをクラス宣言なしで生成できるデリゲートクラス
	/// </summary>
	class UndoableDelegate : IUndoable
	{
		Func<object, object> undo_;
		Func<object, object> redo_;

		/// <summary>
		/// 
		/// </summary>
		public string Name { get; private set; }


		/// <summary>
		/// 
		/// </summary>
		public bool CommandMerge { get; private set; }


		/// <summary>
		/// 
		/// </summary>
		public UndoableDelegate(string name, bool mergeable, Func<object, object> undo, Func<object, object> redo)
		{
			undo_ = undo;
			redo_ = redo;
			Name = name;
			CommandMerge = mergeable;
		}


		/// <summary>
		/// 
		/// </summary>
		public object Undo(object undoData)
		{
			return undo_(undoData);
		}


		/// <summary>
		/// 
		/// </summary>
		public object Redo(object redoData)
		{
			return redo_(redoData);
		}

	}
}
