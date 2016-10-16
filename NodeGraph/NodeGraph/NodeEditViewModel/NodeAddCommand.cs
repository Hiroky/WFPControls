using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApplication1.Undoable;

namespace WpfApplication1
{
	/// <summary>
	/// 
	/// </summary>
	class NodeAddCommand : IUndoable
	{
		NodeEditViewModel parentObject_;
		NodeViewModel value_;


		#region Properties

		public string Name { get; private set; }

		public bool CommandMerge { get { return false; } }

		#endregion


		/// <summary>
		/// 
		/// </summary>
		public NodeAddCommand(NodeEditViewModel parent, string name, NodeViewModel value)
		{
			parentObject_ = parent;
			Name = name;
			value_ = value;
        }


		/// <summary>
		/// 
		/// </summary>
		public object Undo(object undoData)
		{
			parentObject_.Nodes.Remove(value_);
			return null;
		}


		/// <summary>
		/// 
		/// </summary>
		public object Redo(object redoData)
		{
			parentObject_.Nodes.Add(value_);
			return null;
		}

	}
}
