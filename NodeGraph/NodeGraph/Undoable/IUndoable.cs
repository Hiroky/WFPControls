using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1.Undoable
{
	/// <summary>
	/// Undo implementation interface
	/// </summary>
	public interface IUndoable
	{
		/// <summary>
		/// 
		/// </summary>
		string Name { get; }

		/// <summary>
		/// 
		/// </summary>
		bool CommandMerge { get; }

		/// <summary>
		/// 
		/// </summary>
		object Undo(object undoData);

		/// <summary>
		/// 
		/// </summary>
		object Redo(object redoData);
	}
}
