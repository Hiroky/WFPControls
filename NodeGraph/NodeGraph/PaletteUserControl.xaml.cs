using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
	/// PaletteUserControl.xaml の相互作用ロジック
	/// </summary>
	public partial class PaletteUserControl : UserControl
	{
		Point _lastMouseDown;
		bool _isMouseDown;

		/// <summary>
		/// 
		/// </summary>
		public PaletteUserControl()
		{
			InitializeComponent();
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void border_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (!_isMouseDown) {
				var item = sender as Border;
				_isMouseDown = true;
				_lastMouseDown = e.GetPosition(this);
				item.CaptureMouse();
			}
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void border_MouseMove(object sender, MouseEventArgs e)
		{
			try {
				if (e.LeftButton == MouseButtonState.Pressed) {
					Point currentPosition = e.GetPosition(this);

					if ((Math.Abs(currentPosition.X - _lastMouseDown.X) > 3.0) ||
						(Math.Abs(currentPosition.Y - _lastMouseDown.Y) > 3.0)) {
						var draggedItem = sender as Border;
						if (draggedItem != null) {
							DragDropEffects finalDropEffect = DragDrop.DoDragDrop(draggedItem, draggedItem.DataContext, DragDropEffects.Move);
						}
					}
				}
			} catch (Exception) {
			}
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void border_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (_isMouseDown) {
				var item = sender as Border;
				item.ReleaseMouseCapture();
				_isMouseDown = false;
            }
		}
	}
}
