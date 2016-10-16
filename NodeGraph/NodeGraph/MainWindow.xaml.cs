using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApplication1
{
	/// <summary>
	/// MainWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindow : Window
	{
		#region Properties

		public NodeEditViewModel NodeEditViewModel { get; private set; }
		public PaletteViewModel PaletteViewModel { get; private set; }

		public ICommand UndoCommand
		{
			get
			{
				return Undoable.UndoableContext.CurrentContext.UndoExecuteCommand;
			}
		}

		public ICommand RedoCommand
		{
			get
			{
				return Undoable.UndoableContext.CurrentContext.RedoExecuteCommand;
			}
		}

		#endregion


		/// <summary>
		/// 
		/// </summary>
		public MainWindow()
		{
			DataContext = this;
			NodeEditViewModel = new NodeEditViewModel();
			PaletteViewModel = new PaletteViewModel();

			InitializeComponent();

			// いったんヒストリクリア
			Undoable.UndoableContext.CurrentContext.ClearHistory();
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void NodeEditControl_ConnectionDragStarted(object sender, ConnectionDragStartedEventArgs e)
		{
			if (!(e.ConnectorDraggedOut is NodeConnectorViewModel)) {
				return;
			}

			var link = NodeEditViewModel.ConnectionDragStarted(e.ConnectorDraggedOut as NodeConnectorViewModel);
			e.Connection = link;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void NodeEditControl_ConnectionDragging(object sender, ConnectionDraggingEventArgs e)
		{
			NodeEditViewModel.ConnectionDragging(e);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void NodeEditControl_ConnectionDragCompleted(object sender, ConnectionDragCompletedEventArgs e)
		{
			NodeEditViewModel.ConnectionDragCompleted(e);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MenuItem_Click(object sender, RoutedEventArgs e)
		{
			NodeViewModel m = new NodeViewModel();
			Point p = Mouse.GetPosition(this);
			m.X = p.X;
			m.Y = p.Y;
			m.Name = "Add";
			m.Outputs.Add(new NodeConnectorViewModel("Output", NodeConnectorViewModel.ConnectorType.Output, m));
			NodeEditViewModel.Nodes.Add(m);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void NodeMenuItem_Click(object sender, RoutedEventArgs e)
		{
			NodeViewModel n = (sender as FrameworkElement).DataContext as NodeViewModel;
			if (n != null) {
				NodeEditViewModel.RemoveNode(n);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ConnectorMenuItem_Click(object sender, RoutedEventArgs e)
		{
			NodeConnectorViewModel c = (sender as FrameworkElement).DataContext as NodeConnectorViewModel;
			if (c != null) {
				while (c.AttachedLinks.Count > 0) {
					var l = c.AttachedLinks[0];
					NodeEditViewModel.RemoveLink(l);
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void NodeEditControl_DragOver(object sender, DragEventArgs e)
		{
			try {
				Point currentPosition = e.GetPosition(this);

				var item = e.OriginalSource as FrameworkElement;
				if (item != null) {
					e.Effects = DragDropEffects.Move;
				} else {
					e.Effects = DragDropEffects.None;
				}
				e.Handled = true;
			} catch (Exception) {
			}

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void NodeEditControl_Drop(object sender, DragEventArgs e)
		{
			try {
				e.Effects = DragDropEffects.None;
				e.Handled = true;

				var item = e.Data.GetData(typeof(PaletteItemViewModel)) as PaletteItemViewModel;
				if (item != null) {
					Point p = e.GetPosition(nodeEditControl.MainCanvas);
					NodeEditViewModel.AddNode(item, p);
				}
			} catch (Exception) {
			}
		}
	}
}
