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
	public class NodeEditControl : Control
	{
		private static readonly double DragThreshold = 3;

		/// <summary>
		/// ドラッグ中のリンクコントロールのモデル
		/// </summary>
		object draggingLinkModel_;

		/// <summary>
		/// ドラッグオーバーしているコネクタ
		/// </summary>
		//ConnectorControl dragOverConnector_;

		/// <summary>
		/// 
		/// </summary>
		bool isMouseLeftDrag_;

		/// <summary>
		/// 
		/// </summary>
		bool isMouseRightDrag_;

		/// <summary>
		/// 
		/// </summary>
		Point mouseDragStartPoint_;
		
		/// <summary>
		/// 
		/// </summary>
		Canvas PART_Canvas_;

		/// <summary>
		/// 
		/// </summary>
		NodeListControl PART_NodeList_;

		/// <summary>
		/// 
		/// </summary>
		ItemsControl PART_LinkList_;

		/// <summary>
		/// 
		/// </summary>
		FrameworkElement PART_SelectionRect_;


		#region DependencyProperties

		public static readonly DependencyProperty NodesProperty =
			DependencyProperty.Register("Nodes", typeof(ICollection), typeof(NodeEditControl));

		public static readonly DependencyProperty LinksProperty =
			DependencyProperty.Register("Links", typeof(ICollection), typeof(NodeEditControl));

		public static readonly DependencyProperty IsScrollDraggingProperty =
			DependencyProperty.Register("IsScrollDragging", typeof(bool), typeof(NodeEditControl), new PropertyMetadata(false));

		#endregion


		#region properties

		public Canvas MainCanvas { get { return PART_Canvas_; } }

		public ICollection Nodes
		{
			get
			{
				return (ICollection)GetValue(NodesProperty);
			}
			set
			{
				SetValue(NodesProperty, value);
			}
		}

		public ICollection Links
		{
			get
			{
				return (ICollection)GetValue(LinksProperty);
			}
			set
			{
				SetValue(LinksProperty, value);
			}
		}

		public IList SelectedNodes
		{
			get
			{
				return PART_NodeList_.SelectedItems;
			}
		}

		public bool IsScrollDragging
		{
			get
			{
				return (bool)GetValue(IsScrollDraggingProperty);
			}
			set
			{
				SetValue(IsScrollDraggingProperty, value);
			}
		}

		#endregion


		#region Events

		public static readonly RoutedEvent ConnectionDragStartedEvent =
			EventManager.RegisterRoutedEvent("ConnectionDragStarted", RoutingStrategy.Bubble,
				typeof(ConnectionDragStartedEventHandler), typeof(NodeEditControl));

		public static readonly RoutedEvent ConnectionDraggingEvent =
			EventManager.RegisterRoutedEvent("ConnectionDragging", RoutingStrategy.Bubble,
				typeof(ConnectionDraggingEventHandler), typeof(NodeEditControl));

		public static readonly RoutedEvent ConnectionDragCompletedEvent =
			EventManager.RegisterRoutedEvent("ConnectionDragCompleted", RoutingStrategy.Bubble,
				typeof(ConnectionDragCompletedEventHandler), typeof(NodeEditControl));

		public event ConnectionDragStartedEventHandler ConnectionDragStarted
		{
			add { AddHandler(ConnectionDragStartedEvent, value); }
			remove { RemoveHandler(ConnectionDragStartedEvent, value); }
		}

		public event ConnectionDraggingEventHandler ConnectionDragging
		{
			add { AddHandler(ConnectionDraggingEvent, value); }
			remove { RemoveHandler(ConnectionDraggingEvent, value); }
		}

		public event ConnectionDragCompletedEventHandler ConnectionDragCompleted
		{
			add { AddHandler(ConnectionDragCompletedEvent, value); }
			remove { RemoveHandler(ConnectionDragCompletedEvent, value); }
		}

		#endregion



		/// <summary>
		/// 
		/// </summary>
		static NodeEditControl()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(NodeEditControl), new FrameworkPropertyMetadata(typeof(NodeEditControl)));
		}


		/// <summary>
		/// 
		/// </summary>
		public NodeEditControl()
		{
			// Regist event handler
			AddHandler(NodeControl.NodeDraggingEvent, new NodeDraggingEventHandler(NodeItem_Dragging));
			AddHandler(ConnectorControl.ConnectorDragStartedEvent, new ConnectorDragStartedEventHandler(ConnectorItem_DragStarted));
			AddHandler(ConnectorControl.ConnectorDraggingEvent, new ConnectorDraggingEventHandler(ConnectorItem_Dragging));
			AddHandler(ConnectorControl.ConnectorDragCompletedEvent, new ConnectorDragCompletedEventHandler(ConnectorItem_DragCompleted));
		}


		/// <summary>
		/// 
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			// 必須コントロールにイベントを設定
			PART_Canvas_ = (Canvas)this.Template.FindName("PART_Canvas", this);
			if (PART_Canvas_ == null) {
				throw new ApplicationException("Failed to find 'PART_Canvas' in the visual tree for 'NodeEditControl'.");
			}

			PART_NodeList_ = (NodeListControl)this.Template.FindName("PART_NodeList", this);
			if (PART_NodeList_ == null) {
				throw new ApplicationException("Failed to find 'PART_NodeList' in the visual tree for 'NodeEditControl'.");
			}
			//PART_NodeList_.SelectionChanged += PART_NodeList__SelectionChanged;

			PART_LinkList_ = (ItemsControl)this.Template.FindName("PART_LinkList", this);
			if (PART_LinkList_ == null) {
				throw new ApplicationException("Failed to find 'PART_LinkList' in the visual tree for 'NodeEditControl'.");
			}

			PART_SelectionRect_ = (FrameworkElement)this.Template.FindName("PART_SelectionRect", this);
			if (PART_SelectionRect_ == null) {
				throw new ApplicationException("Failed to find 'PART_SelectionRect' in the visual tree for 'FrameworkElement'.");
			}
		}


		/// <summary>
		/// 
		/// </summary>
		private void ItemSelectByDraggingRect()
		{
			double x = Canvas.GetLeft(PART_SelectionRect_);
			double y = Canvas.GetTop(PART_SelectionRect_);
			double width = PART_SelectionRect_.Width;
			double height = PART_SelectionRect_.Height;
			Rect dragRect = new Rect(x, y, width, height);

			// Ctrlが押されていた場合追加選択それ以外はクリア
			if ((Keyboard.Modifiers & ModifierKeys.Control) == 0) {
				PART_NodeList_.SelectedItems.Clear();
			}

			// 交差チェック
			for (int nodeIndex = 0; nodeIndex < Nodes.Count; ++nodeIndex) {
				var nodeItem = (NodeListItem)PART_NodeList_.ItemContainerGenerator.ContainerFromIndex(nodeIndex);
				var transformToAncestor = nodeItem.TransformToAncestor(PART_Canvas_);
				Point itemPt1 = transformToAncestor.Transform(new Point(0, 0));
				Point itemPt2 = transformToAncestor.Transform(new Point(nodeItem.ActualWidth, nodeItem.ActualHeight));
				Rect itemRect = new Rect(itemPt1, itemPt2);
				if (dragRect.IntersectsWith(itemRect)) {
					nodeItem.IsSelected = true;
				}
			}
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
		{
			base.OnPreviewMouseDown(e);

			// フォーカス取得
			Focus();

			if (e.ChangedButton == MouseButton.Right) {
				mouseDragStartPoint_ = Mouse.GetPosition(this);
				isMouseRightDrag_ = true;
				CaptureMouse();
				e.Handled = true;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
		{
			base.OnPreviewMouseUp(e);
			if (e.ChangedButton == MouseButton.Right) {
				if (isMouseRightDrag_) {
					isMouseRightDrag_ = false;
					ReleaseMouseCapture();
				}
				if (IsScrollDragging) {
					IsScrollDragging = false;
					e.Handled = true;
				}
			}
		}


		/// <summary>
		/// 
		/// </summary>
		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			base.OnMouseDown(e);

			// フォーカス取得
			Focus();

			if (e.ChangedButton == MouseButton.Left) {
				mouseDragStartPoint_ = Mouse.GetPosition(PART_Canvas_);
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
				if (PART_SelectionRect_.Visibility == System.Windows.Visibility.Collapsed) {
					PART_SelectionRect_.Visibility = System.Windows.Visibility.Visible;
				}
				Point currentPos = Mouse.GetPosition(PART_Canvas_);

				var diff = Point.Subtract(currentPos, mouseDragStartPoint_);
				if (diff.X > 0) {
					Canvas.SetLeft(PART_SelectionRect_, mouseDragStartPoint_.X);
					PART_SelectionRect_.Width = diff.X;
				} else {
					Canvas.SetLeft(PART_SelectionRect_, currentPos.X);
					PART_SelectionRect_.Width = -diff.X;
				}
				if (diff.Y > 0) {
					Canvas.SetTop(PART_SelectionRect_, mouseDragStartPoint_.Y);
					PART_SelectionRect_.Height = diff.Y;
				} else {
					Canvas.SetTop(PART_SelectionRect_, currentPos.Y);
					PART_SelectionRect_.Height = -diff.Y;
				}
			} else if (isMouseRightDrag_) {
				Point currentPos = Mouse.GetPosition(this);
				var diff = Point.Subtract(currentPos, mouseDragStartPoint_);

				if (IsScrollDragging
					|| (Math.Abs(diff.X) > DragThreshold || Math.Abs(diff.Y) > DragThreshold)) {
					IsScrollDragging = true;
					mouseDragStartPoint_ = currentPos;

					PART_Canvas_.Margin = new Thickness(PART_Canvas_.Margin.Left + diff.X,
						PART_Canvas_.Margin.Top + diff.Y,
						0,
						0);
				}
			}
			e.Handled = true;
		}

		/// <summary>
		/// 
		/// </summary>
		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			base.OnMouseUp(e);

			if (e.ChangedButton == MouseButton.Left) {
				if (isMouseLeftDrag_) {
					ItemSelectByDraggingRect();
					if (PART_SelectionRect_.Visibility == System.Windows.Visibility.Visible) {
						PART_SelectionRect_.Visibility = System.Windows.Visibility.Collapsed;
					}
					isMouseLeftDrag_ = false;
					ReleaseMouseCapture();
					e.Handled = true;
				}
			}
		}


		/// <summary>
		/// 
		/// </summary>
		void PART_NodeList__SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			//int i = 0;
		}


		#region Child control event handler

		/// <summary>
		/// 
		/// </summary>
		public static ParentT FindVisualParentWithType<ParentT>(FrameworkElement childElement)
			where ParentT : class
		{
			FrameworkElement parentElement = (FrameworkElement)VisualTreeHelper.GetParent(childElement);
			if (parentElement != null) {
				ParentT parent = parentElement as ParentT;
				if (parent != null) {
					return parent;
				}

				return FindVisualParentWithType<ParentT>(parentElement);
			}

			return null;
		}


		/// <summary>
		/// 
		/// </summary>
		private bool CheckConnectorControlDraggedOver(Point hitPoint, out ConnectorControl connectorDraggedOver)
		{
			connectorDraggedOver = null;

			// ヒットチェック
			HitTestResult result = null;
			VisualTreeHelper.HitTest(PART_NodeList_, null, (hitTestResult) => {
				result = hitTestResult;
				return HitTestResultBehavior.Stop;
			},
				new PointHitTestParameters(hitPoint));

			if (result == null || result.VisualHit == null) {
				// Hit test failed.
				return false;
			}

			// ヒットしたものがConnectorの子か調べる
			var hitItem = result.VisualHit as FrameworkElement;
			if (hitItem == null) {
				return false;
			}
			var connector = FindVisualParentWithType<ConnectorControl>(hitItem);
			if (connector == null) {
				return false;
			}
			connectorDraggedOver = connector;

			return true;
		}


		/// <summary>
		/// ドラッグ開始
		/// </summary>
		private void ConnectorItem_DragStarted(object sender, ConnectorDragStartedEventArgs e)
		{
			// フォーカス取得
			Focus();

			var ctrl = e.OriginalSource as ConnectorControl;
			if (ctrl == null) return;

			var position = Mouse.GetPosition(ctrl.ParentEditControl);

			// ノードとコネクタのコンテキストを入れてイベント起動
			var eventArgs = new ConnectionDragStartedEventArgs(ConnectionDragStartedEvent, this,
				ctrl.ParentNodeControl.DataContext, ctrl.DataContext, position);
			RaiseEvent(eventArgs);

			// ドラッグ中のリンクオブジェクトを保持
			draggingLinkModel_ = eventArgs.Connection;
			//Console.WriteLine("Drag started.");
		}

		/// <summary>
		/// ドラッグ中
		/// </summary>
		private void ConnectorItem_Dragging(object sender, ConnectorDraggingEventArgs e)
		{
			var ctrl = e.OriginalSource as ConnectorControl;
			if (ctrl == null || draggingLinkModel_ == null) return;

			// ドラッグ中イベント起動
			var position = Mouse.GetPosition(ctrl.ParentEditControl);
			//Console.WriteLine("{0}, {1}", position.X, position.Y);
			var connectionDraggingEventArgs = new ConnectionDraggingEventArgs(ConnectionDraggingEvent, this,
				ctrl.ParentNodeControl.DataContext, draggingLinkModel_, ctrl.DataContext, position);
			RaiseEvent(connectionDraggingEventArgs);

			// ドラッグオーバーチェック(現状必要ない)
			//position = Mouse.GetPosition(MainCanvas);
			//if (CheckConnectorControlDraggedOver(position, out dragOverConnector_)) {
			//Console.WriteLine("Over!");
			//}
		}

		/// <summary>
		/// ドラッグ終了
		/// </summary>
		private void ConnectorItem_DragCompleted(object sender, ConnectorItemDragCompletedEventArgs e)
		{
			var ctrl = e.OriginalSource as ConnectorControl;
			if (ctrl == null || draggingLinkModel_ == null) return;

			// ドラッグオーバーチェック
			Point mousePoint = Mouse.GetPosition(PART_Canvas_);
			ConnectorControl connectorDraggedOver = null;
			bool isHit = CheckConnectorControlDraggedOver(mousePoint, out connectorDraggedOver);

			// イベント起動
			var position = Mouse.GetPosition(ctrl.ParentEditControl);
			RaiseEvent(new ConnectionDragCompletedEventArgs(ConnectionDragCompletedEvent, this,
				ctrl.ParentNodeControl.DataContext,
				draggingLinkModel_,
				ctrl.DataContext,
				isHit ? connectorDraggedOver.DataContext : null,
				position));

			draggingLinkModel_ = null;
		}


		/// <summary>
		/// ノードドラッグ中
		/// </summary>
		private void NodeItem_Dragging(object sender, NodeDraggingEventArgs e)
		{
			// フォーカス取得
			Focus();

			var ctrl = e.OriginalSource as NodeControl;
			if (ctrl == null) return;

			//
			// 以下無駄が多いので仮
			//

			// 選択されていなければ選択状態にする
			foreach (var a in e.Nodes) {
				NodeListItem item = PART_NodeList_.FindAssociatedNodeItem(a);
				if (!item.IsSelected) {
					// Ctrlが押されていない場合セレクションリストをクリア
					if ((Keyboard.Modifiers & ModifierKeys.Control) == 0) {
						SelectedNodes.Clear();
					}

					item.IsSelected = true;
				} else {
				}
			}

			// 選択中オブジェクトに移動値を加算する
			// TODO:毎回検索するのが無駄なのでキャッシュする
			foreach (var n in SelectedNodes) {
				NodeListItem item = PART_NodeList_.FindAssociatedNodeItem(n);
				Canvas.SetLeft(item, Canvas.GetLeft(item) + e.HorizontalChange);
				Canvas.SetTop(item, Canvas.GetTop(item) + e.VerticalChange);
			}
		}

		#endregion
	}
}
