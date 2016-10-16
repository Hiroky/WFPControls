using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
	/// </summary>
	public class NodeControl : ContentControl
	{
		/// <summary>
		/// 
		/// </summary>
		bool isMouseLeftDrag_;

		/// <summary>
		/// 
		/// </summary>
		Point mouseDragStartPoint_;


		#region DependencyProperties

		public static readonly DependencyProperty TitleProperty =
			DependencyProperty.Register("Title", typeof(string), typeof(NodeControl));

		public static readonly DependencyProperty InputsProperty =
			DependencyProperty.Register("Inputs", typeof(IEnumerable), typeof(NodeControl));

		public static readonly DependencyProperty OutputsProperty =
			DependencyProperty.Register("Outputs", typeof(IEnumerable), typeof(NodeControl));

		public static readonly DependencyProperty XProperty =
			DependencyProperty.Register("X", typeof(double), typeof(NodeControl));

		public static readonly DependencyProperty YProperty =
			DependencyProperty.Register("Y", typeof(double), typeof(NodeControl));

		public static readonly DependencyProperty InputItemTemplateProperty =
			DependencyProperty.Register("InputItemTemplate", typeof(DataTemplate), typeof(NodeControl));

		public static readonly DependencyProperty OutputItemTemplateProperty =
			DependencyProperty.Register("OutputItemTemplate", typeof(DataTemplate), typeof(NodeControl));

		internal static readonly DependencyProperty ParentEditControlProperty =
			DependencyProperty.Register("ParentEditControl", typeof(FrameworkElement), typeof(NodeControl));

		#endregion

		#region properties

		public string Title
		{
			get
			{
				return (string)GetValue(TitleProperty);
			}
			set
			{
				SetValue(TitleProperty, value);
			}
		}

		public IEnumerable Inputs
		{
			get
			{
				return (IEnumerable)GetValue(InputsProperty);
			}
			set
			{
				SetValue(InputsProperty, value);
			}
		}

		public IEnumerable Outputs
		{
			get
			{
				return (IEnumerable)GetValue(OutputsProperty);
			}
			set
			{
				SetValue(OutputsProperty, value);
			}
		}

		public double X
		{
			get
			{
				return (double)GetValue(XProperty);
			}
			set
			{
				SetValue(XProperty, value);
			}
		}

		public double Y
		{
			get
			{
				return (double)GetValue(YProperty);
			}
			set
			{
				SetValue(YProperty, value);
			}
		}

		public DataTemplate InputItemTemplate
		{
			get
			{
				return (DataTemplate)GetValue(InputItemTemplateProperty);
			}
			set
			{
				SetValue(InputItemTemplateProperty, value);
			}
		}

		public DataTemplate OutputItemTemplate
		{
			get
			{
				return (DataTemplate)GetValue(OutputItemTemplateProperty);
			}
			set
			{
				SetValue(OutputItemTemplateProperty, value);
			}
		}

		public FrameworkElement ParentEditControl
		{
			get
			{
				return (FrameworkElement)GetValue(ParentEditControlProperty);
			}
			set
			{
				SetValue(ParentEditControlProperty, value);
			}
		}

		#endregion

		#region Events

		//internal static readonly RoutedEvent NodeDragStartedEvent =
		//	EventManager.RegisterRoutedEvent("NodeDragStarted", RoutingStrategy.Bubble,
		//		typeof(ConnectorDragStartedEventHandler), typeof(NodeControl));

		internal static readonly RoutedEvent NodeDraggingEvent =
			EventManager.RegisterRoutedEvent("NodeDragging", RoutingStrategy.Bubble,
				typeof(NodeDraggingEventHandler), typeof(NodeControl));

		//internal static readonly RoutedEvent NodeDragCompletedEvent =
		//	EventManager.RegisterRoutedEvent("NodeDragCompleted", RoutingStrategy.Bubble,
		//		typeof(ConnectorDragCompletedEventHandler), typeof(NodeControl));

		#endregion

		/// <summary>
		/// 
		/// </summary>
		static NodeControl()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(NodeControl), new FrameworkPropertyMetadata(typeof(NodeControl)));
		}


		/// <summary>
		/// 
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			// 必須コントロールにイベントを設定
			//var thumb = (Thumb)this.Template.FindName("PART_Thumb", this);
			//if (thumb == null) {
			//	throw new ApplicationException("Failed to find 'PART_Thumb' in the visual tree for 'NodeControl'.");
			//}
			//thumb.DragDelta += Thumb_DragDelta;
		}


		/// <summary>
		/// 
		/// </summary>
		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			base.OnMouseDown(e);

			if (e.ChangedButton == MouseButton.Left) {
				mouseDragStartPoint_ = Mouse.GetPosition(ParentEditControl);
				isMouseLeftDrag_ = true;
				CaptureMouse();
				e.Handled = true;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			if (isMouseLeftDrag_) {
				Point currentPos = Mouse.GetPosition(ParentEditControl);
				var diff = Point.Subtract(currentPos, mouseDragStartPoint_);
				mouseDragStartPoint_ = currentPos;

				// イベント発生
				RaiseEvent(new NodeDraggingEventArgs(NodeDraggingEvent, this, new object[] { DataContext }, diff.X, diff.Y));
				e.Handled = true;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			base.OnMouseUp(e);

			if (e.ChangedButton == MouseButton.Left) {
				if (isMouseLeftDrag_) {
					isMouseLeftDrag_ = false;
					ReleaseMouseCapture();
					e.Handled = true;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		private void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
		{
			//var p = VisualParent as ContentPresenter;
			//if (p != null) {
			//	// ItemsControlを使用した場合
			//	double x = Canvas.GetLeft(p);
			//	double y = Canvas.GetTop(p);
			//	if (double.IsNaN(x)) x = 0;
			//	if (double.IsNaN(y)) y = 0;
			//	Canvas.SetLeft(p, x + e.HorizontalChange);
			//	Canvas.SetTop(p, y + e.VerticalChange);
			//} else 
			//{
			//	// 通常時
			//	Canvas.SetLeft(this, Canvas.GetLeft(this) + e.HorizontalChange);
			//	Canvas.SetTop(this, Canvas.GetTop(this) + e.VerticalChange);
			//}

			// ドラッグイベント発生
			//RaiseEvent(new NodeDraggingEventArgs(NodeDraggingEvent, this, new object[] { DataContext }, e.HorizontalChange, e.VerticalChange));
			//e.Handled = true;
		}
	}

}
