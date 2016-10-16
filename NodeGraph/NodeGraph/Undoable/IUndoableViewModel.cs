using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1.Undoable
{
	public interface IUndoableViewModel
	{
		void OnUndo(IUndoable data);

		void OnRedo(IUndoable data);
	}
}
