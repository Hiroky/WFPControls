using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

using WpfApplication1.Undoable;

namespace WpfApplication1
{
	/// <summary>
	/// NodeBox用コンテンツ
	/// </summary>
	public class NodeViewModel : ViewModelBase, IUndoableViewModel
	{
		string name_;
		public string Name
		{
			get { return name_; }
			set { name_ = value; OnPropertyChanged("Name"); }
		}

#if false
		double x_;
		public double X
		{
			get { return x_; }
			set { x_ = value; OnPropertyChanged("X"); }
		}

		double y_;
		public double Y
		{
			get { return y_; }
			set { y_ = value; OnPropertyChanged("Y"); }
		}
#else
		UndoableProperty<Point> position_;
		public double X
		{
			get { return position_.Value.X; }
			set {
				if (position_.Value.X != value) {
					var pos = position_.Value;
					pos.X = value;
					position_.Value = pos;
					OnPropertyChanged("X");
				}
			}
		}

		public double Y
		{
			get { return position_.Value.Y; }
			set
			{
				if (position_.Value.Y != value) {
					var pos = position_.Value;
					pos.Y = value;
					position_.Value = pos;
					OnPropertyChanged("Y");
				}
			}
		}
#endif

		ObservableCollection<NodeConnectorViewModel> args_;
		public ObservableCollection<NodeConnectorViewModel> Args
		{
			get { return args_; }
			private set { args_ = value; }
		}

		ObservableCollection<NodeConnectorViewModel> outputs_;
		public ObservableCollection<NodeConnectorViewModel> Outputs
		{
			get { return outputs_; }
			private set { outputs_ = value; }
		}

		bool isSelected_;
		public bool IsSelected
		{
			get { return isSelected_; }
			set { isSelected_ = value; OnPropertyChanged("IsSelected"); }
		}

		/// <summary>
		/// 
		/// </summary>
		public NodeViewModel()
			: this(new Point(0, 0))
		{
		}

		public NodeViewModel(Point position)
		{
			position_ = new UndoableProperty<Point>(this, "Position", position);
			Args = new ObservableCollection<NodeConnectorViewModel>();
			Outputs = new ObservableCollection<NodeConnectorViewModel>();
		}

		#region IUndoableViewModel implementation

		public void OnUndo(IUndoable data)
		{
			if (data.Name == "Position") {
				OnPropertyChanged("X");
				OnPropertyChanged("Y");
			} else {
				OnPropertyChanged(data.Name);
			}
		}

		public void OnRedo(IUndoable data)
		{
			if (data.Name == "Position") {
				OnPropertyChanged("X");
				OnPropertyChanged("Y");
			} else {
				OnPropertyChanged(data.Name);
			}
		}

		#endregion
	}
}
