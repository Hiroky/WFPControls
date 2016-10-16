using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfApplication1
{
	public class ConnectorControl : ContentControl
	{
		#region DependancyProperties

		public static readonly DependencyProperty PositionProperty =
			DependencyProperty.Register("Position", typeof(Point), typeof(ConnectorControl));

		public static readonly DependencyProperty IsConnectedProperty =
			DependencyProperty.Register("IsConnected", typeof(bool), typeof(ConnectorControl));

		internal static readonly DependencyProperty ParentEditControlProperty =
			DependencyProperty.Register("ParentEditControl", typeof(FrameworkElement), typeof(ConnectorControl),
				new FrameworkPropertyMetadata(ParentEditContol_PropertyChanged));

		internal static readonly DependencyProperty ParentNodeControlProperty =
			DependencyProperty.Register("ParentNodeControl", typeof(FrameworkElement), typeof(ConnectorControl),
				new FrameworkPropertyMetadata(ParentNodeContol_PropertyChanged));

		internal static readonly RoutedEvent ConnectorDragStartedEvent =
			EventManager.RegisterRoutedEvent("ConnectorDragStarted", RoutingStrategy.Bubble,
				typeof(ConnectorDragStartedEventHandler), typeof(ConnectorControl));

		internal static readonly RoutedEvent ConnectorDraggingEvent =
			EventManager.RegisterRoutedEvent("ConnectorDragging", RoutingStrategy.Bubble,
				typeof(ConnectorDraggingEventHandler), typeof(ConnectorControl));

		internal static readonly RoutedEvent ConnectorDragCompletedEvent =
			EventManager.RegisterRoutedEvent("ConnectorDragCompleted", RoutingStrategy.Bubble,
				typeof(ConnectorDragCompletedEventHandler), typeof(ConnectorControl));

		#endregion

		#region Properties

		public Point Position
		{
			get
			{
				return (Point)GetValue(PositionProperty);
			}
			set
			{
				SetValue(PositionProperty, value);
			}
		}

		public bool IsConnected
		{
			get
			{
				return (bool)GetValue(IsConnectedProperty);
			}
			set
			{
				SetValue(IsConnectedProperty, value);
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

		public FrameworkElement ParentNodeControl
		{
			get
			{
				return (FrameworkElement)GetValue(ParentNodeControlProperty);
			}
			set
			{
				SetValue(ParentNodeControlProperty, value);
			}
		}

		#endregion


		private static readonly double DragThreshold = 2;
		bool isDragging_ = false;
		bool isLeftMouseDown_ = false;
		Point lastMousePoint_;

		/// <summary>
		/// 
		/// </summary>
		static ConnectorControl()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ConnectorControl), new FrameworkPropertyMetadata(typeof(ConnectorControl)));
		}

		/// <summary>
		/// 
		/// </summary>
		public ConnectorControl()
		{
			LayoutUpdated += ConnectorControl_LayoutUpdated;
		}


		/// <summary>
		/// 
		/// </summary>
		private void UpdatePosition()
		{
			if (ParentEditControl == null) {
				return;
			}

			// 子かどうか調べる
			// ノードが削除されたときにエラーが出るのでここでチェックする
			// 削除を検知できるならそれが一番いい
			if (!ParentEditControl.IsAncestorOf(this)) {
				ParentEditControl = null;
				return;
			}

			var centerPoint = new Point(ActualWidth / 2, ActualHeight / 2);
			Position = TransformToAncestor(ParentEditControl).Transform(centerPoint);
			//Console.WriteLine("Position / {0}, {1}", Position.X, Position.Y);
		}


		/// <summary>
		/// 
		/// </summary>
		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			base.OnMouseDown(e);

			//if (ParentNodeItem != null) {
			//	ParentNodeItem.BringToFront();
			//}

			if (e.ChangedButton == MouseButton.Left) {
				//if (ParentNodeItem != null) {
				//	ParentNodeItem.LeftMouseDownSelectionLogic();
				//}

				lastMousePoint_ = e.GetPosition(ParentEditControl);
				isLeftMouseDown_ = true;
				CaptureMouse();

				e.Handled = true;
			} else if (e.ChangedButton == MouseButton.Right) {
				//if (ParentNodeItem != null) {
				//	ParentNodeItem.RightMouseDownSelectionLogic();
				//}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			base.OnMouseUp(e);

			if (e.ChangedButton == MouseButton.Left) {
				if (isLeftMouseDown_) {
					if (isDragging_) {
						RaiseEvent(new ConnectorItemDragCompletedEventArgs(ConnectorDragCompletedEvent, this));
						isDragging_ = false;
					} else {
						//if (ParentNodeItem != null) {
						//	ParentNodeItem.LeftMouseUpSelectionLogic();
						//}
					}

					isLeftMouseDown_ = false;
					ReleaseMouseCapture();

					e.Handled = true;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			if (isDragging_) {
				// ドラッグ中
				Point curMousePoint = e.GetPosition(ParentEditControl);
				Vector offset = curMousePoint - lastMousePoint_;
				if (offset.X != 0.0 || offset.Y != 0.0) {
					lastMousePoint_ = curMousePoint;
					RaiseEvent(new ConnectorDraggingEventArgs(ConnectorDraggingEvent, this, offset.X, offset.Y));
				}
				e.Handled = true;
			} else if (isLeftMouseDown_) {
				// ドラッグ開始
				if (ParentEditControl != null 
					//&& ParentEditContol.EnableConnectionDragging
					) {
					Point curMousePoint = e.GetPosition(ParentEditControl);
					var dragDelta = curMousePoint - lastMousePoint_;
					double dragDistance = Math.Abs(dragDelta.Length);
					if (dragDistance > DragThreshold) {
						var eventArgs = new ConnectorDragStartedEventArgs(ConnectorDragStartedEvent, this);
						RaiseEvent(eventArgs);

						if (eventArgs.Cancel) {
							isLeftMouseDown_ = false;
							return;
						}

						isDragging_ = true;
						e.Handled = true;
					}
				}
			}
		}



		private static void ParentEditContol_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var c = (ConnectorControl)d;
			c.UpdatePosition();
		}

		private static void ParentNodeContol_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var c = (ConnectorControl)d;
			//c.UpdatePosition();
		}

		private void ConnectorControl_LayoutUpdated(object sender, EventArgs e)
		{
			UpdatePosition();
		}
	}
}
