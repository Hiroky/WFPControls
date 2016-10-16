using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1.Undoable
{
	/// <summary>
	/// 
	/// </summary>
	public class UndoableProperty<TType> : IUndoable
	{
		IUndoableViewModel parentObject_;
		TType value_;


		#region Properties

		public string Name { get; private set; }

		public bool CommandMerge { get { return true; } }

		public TType Value
		{
			get
			{
				return value_;
			}
			set
			{
				// Undoデータ送信
				UndoableContext.CurrentContext.CommandStacking(this, value_);

                value_ = value;
			}
		}

		#endregion


		/// <summary>
		/// 
		/// </summary>
		public UndoableProperty(IUndoableViewModel parent, string name, TType defaultValue)
		{
			parentObject_ = parent;
			Name = name;
			value_ = defaultValue;
        }


		/// <summary>
		/// 
		/// </summary>
		public object Undo(object undoData)
		{
			var v = (TType)undoData;
			object previousValue = value_;
			value_ = v;
			parentObject_.OnUndo(this);
			return previousValue;
		}


		/// <summary>
		/// 
		/// </summary>
		public object Redo(object redoData)
		{
			var v = (TType)redoData;
			object previousValue = value_;
			value_ = v;
			parentObject_.OnRedo(this);
			return previousValue;
        }
	}
}
