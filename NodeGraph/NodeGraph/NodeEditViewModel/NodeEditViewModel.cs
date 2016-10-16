using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfApplication1.Undoable;
using Newtonsoft.Json;

namespace WpfApplication1
{
	/// <summary>
	/// 
	/// </summary>
	public class NodeEditViewModel : ViewModelBase
	{
		#region History data class

		/// <summary>
		/// ヒストリ保持用オブジェクト
		/// </summary>
		class ConnectorHistoryData
		{
			public LinkViewModel linkObject;
			public NodeConnectorViewModel source;
			public NodeConnectorViewModel target;
		}

		/// <summary>
		/// ノードヒストリ保持用オブジェクト
		/// </summary>
		class NodeHistoryData
		{
			public NodeViewModel node;
			public List<ConnectorHistoryData> linkList;
		}

		#endregion

		#region Properties

		// ノード
		ObservableCollection<NodeViewModel> nodes_;
		public ObservableCollection<NodeViewModel> Nodes
		{
			get { return nodes_; }
			private set { nodes_ = value; }
		}

		// リンク
		ObservableCollection<LinkViewModel> links_;
		public ObservableCollection<LinkViewModel> Links
		{
			get { return links_; }
			private set { links_ = value; }
		}

		#endregion


		/// <summary>
		/// 
		/// </summary>
		public NodeEditViewModel()
		{
			nodes_ = new ObservableCollection<NodeViewModel>();
			links_ = new ObservableCollection<LinkViewModel>();

			// テスト
			var n = new NodeViewModel();
			n.Name = "Test1";
			n.X = 10;
			n.Y = 100;
			n.Args.Add(new NodeConnectorViewModel("Position", NodeConnectorViewModel.ConnectorType.Input, n));
			n.Args.Add(new NodeConnectorViewModel("Normal", NodeConnectorViewModel.ConnectorType.Input, n));
			n.Args.Add(new NodeConnectorViewModel("Tangent", NodeConnectorViewModel.ConnectorType.Input, n));
			n.Args.Add(new NodeConnectorViewModel("Tangent", NodeConnectorViewModel.ConnectorType.Input, n));
			n.Outputs.Add(new NodeConnectorViewModel("Result", NodeConnectorViewModel.ConnectorType.Output, n));
			nodes_.Add(n);

			n = new NodeViewModel();
			n.Name = "Test2";
			n.X = 300;
			n.Y = 150;
			n.Args.Add(new NodeConnectorViewModel("Position", NodeConnectorViewModel.ConnectorType.Input, n));
			n.Args.Add(new NodeConnectorViewModel("Normal", NodeConnectorViewModel.ConnectorType.Input, n));
			n.Args.Add(new NodeConnectorViewModel("Tangent", NodeConnectorViewModel.ConnectorType.Input, n));
			n.Outputs.Add(new NodeConnectorViewModel("Result", NodeConnectorViewModel.ConnectorType.Output, n));
			nodes_.Add(n);

			n = new NodeViewModel();
			n.Name = "Shading model";
			n.X = 500;
			n.Y = 150;
			n.Args.Add(new NodeConnectorViewModel("Position", NodeConnectorViewModel.ConnectorType.Input, n));
			n.Args.Add(new NodeConnectorViewModel("Normal", NodeConnectorViewModel.ConnectorType.Input, n));
			n.Args.Add(new NodeConnectorViewModel("Tangent", NodeConnectorViewModel.ConnectorType.Input, n));
			nodes_.Add(n);

			string json = JsonConvert.SerializeObject(nodes_, Formatting.Indented);
			Console.WriteLine(json);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="connector"></param>
		/// <returns></returns>
		public LinkViewModel ConnectionDragStarted(NodeConnectorViewModel connector)
		{
			LinkViewModel link = new LinkViewModel();
			switch (connector.Type) {
				case NodeConnectorViewModel.ConnectorType.Input:
					link.TargetConnector = connector;
					break;

				case NodeConnectorViewModel.ConnectorType.Output:
					link.SourceConnector = connector;
					break;
			}

			// 追加
			Links.Add(link);

			return link;
		}

		/// <summary>
		/// 
		/// </summary>
		public void ConnectionDragging(ConnectionDraggingEventArgs arg)
		{
			LinkViewModel m = arg.Connection as LinkViewModel;
			if (m == null) {
				return;
			}

			// 位置を更新
			if (m.SourceConnector != null) {
				m.End = arg.Position;
			} else {
				m.Start = arg.Position;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public void ConnectionDragCompleted(ConnectionDragCompletedEventArgs arg)
		{
			LinkViewModel m = arg.Connection as LinkViewModel;
			if (m == null) {
				return;
			}

			bool isConnect = false;
			if (arg.ConnectorDraggedOver != null) {
				NodeConnectorViewModel connector = arg.ConnectorDraggedOver as NodeConnectorViewModel;
				if (connector != null) {
					var linkSource = m.SourceConnector != null ? m.SourceConnector : m.TargetConnector;

					// リンク元と同じタイプ、同じ親ならdiscard
					if (linkSource.Type != connector.Type
						&& linkSource.Parent != connector.Parent
						) {
						if (connector.Type == NodeConnectorViewModel.ConnectorType.Input) {
							m.TargetConnector = connector;
						} else {
							m.SourceConnector = connector;
						}
						isConnect = true;
					}

				}
			}

			// 接続されない場合リンクを削除する
			if (!isConnect) {
				m.SourceConnector = null;
				m.TargetConnector = null;
				Links.Remove(m);
			} else {
				// 接続された場合Undo登録する
				UndoableContext.CurrentContext.CommandStacking(new UndoableDelegate("ConnectAttribute", false,
					// Undo
					(o) => {
						// 内部でコールバック登録等を行っているのでちゃんといったんクリアする
						var d = o as ConnectorHistoryData;
						d.linkObject.SourceConnector = null;
						d.linkObject.TargetConnector = null;
                        Links.Remove(d.linkObject);
						return o;
					},
					// Redo
					(o) => {
						var d = o as ConnectorHistoryData;
						d.linkObject.SourceConnector = d.source;
						d.linkObject.TargetConnector = d.target;
						Links.Add(d.linkObject);
						return o;
					}), 
					// History data
					new ConnectorHistoryData() {
						linkObject = m,
						source = m.SourceConnector,
						target = m.TargetConnector
					}
					);
			}
		}


		/// <summary>
		/// PaletteItemViewModelを引数に取っているのは仮
		/// </summary>
		/// <param name="item"></param>
		public void AddNode(PaletteItemViewModel item, Point position)
		{
			NodeViewModel m = new NodeViewModel(position);
			m.Name = item.Name;
			m.Outputs.Add(new NodeConnectorViewModel("Output", NodeConnectorViewModel.ConnectorType.Output, m));
			Nodes.Add(m);

			// Undoコマンド登録
			//UndoableContext.CurrentContext.CommandStacking(new NodeAddCommand(this, "NodeAdd", m));
			UndoableContext.CurrentContext.CommandStacking(new UndoableDelegate("NodeAdd", false,
				// Undo
				(o) => {
					Nodes.Remove(o as NodeViewModel);
					return o;
				},
				// Redo
				(o) => {
					Nodes.Add(o as NodeViewModel);
					return o;
				}), m);
		}


		/// <summary>
		/// ノードを削除する
		/// </summary>
		public void RemoveNode(NodeViewModel node)
		{
			var history = new NodeHistoryData();
			history.node = node;
			history.linkList = new List<ConnectorHistoryData>();
			foreach (var a in node.Args) {
				foreach (var l in a.AttachedLinks) {
					history.linkList.Add(new ConnectorHistoryData() { linkObject = l, source = l.SourceConnector, target = l.TargetConnector });
				}
			}
			foreach (var a in node.Outputs) {
				foreach (var l in a.AttachedLinks) {
					history.linkList.Add(new ConnectorHistoryData() { linkObject = l, source = l.SourceConnector, target = l.TargetConnector });
				}
			}

			// ヒストリ登録
			UndoableContext.CurrentContext.CommandStacking(new UndoableDelegate("RemoveNode", false,
				// Undo
				(o) => {
					var d = o as NodeHistoryData;
					// node復元
					Nodes.Add(d.node);
					// link復元
					foreach (var a in d.linkList) {
						a.linkObject.SourceConnector = a.source;
						a.linkObject.TargetConnector = a.target;
						Links.Add(a.linkObject);
					}
					return o;
				},
				// Redo
				(o) => {
					var d = o as NodeHistoryData;
					Nodes.Remove(d.node);
					foreach (var a in d.linkList) {
						a.linkObject.SourceConnector = null;
						a.linkObject.TargetConnector = null;
						Links.Remove(a.linkObject);
					}
					return o;
				}),
				// History data
				history);

			// リンク解除
			foreach (var a in node.Args) {
				while (a.AttachedLinks.Count > 0) {
					var l = a.AttachedLinks[0];
					l.SourceConnector = null;
					l.TargetConnector = null;
					Links.Remove(l);
				}
			}
			foreach (var a in node.Outputs) {
				while (a.AttachedLinks.Count > 0) {
					var l = a.AttachedLinks[0];
					l.SourceConnector = null;
					l.TargetConnector = null;
					Links.Remove(l);
				}
			}

			// ノード削除
			Nodes.Remove(node);
		}

		/// <summary>
		/// リンクを削除する
		/// </summary>
		public void RemoveLink(LinkViewModel link)
		{
			UndoableContext.CurrentContext.CommandStacking(new UndoableDelegate("RemoveLink", false,
				// Undo
				(o) => {
					var d = o as ConnectorHistoryData;
					d.linkObject.SourceConnector = d.source;
					d.linkObject.TargetConnector = d.target;
					Links.Add(d.linkObject);
					return o;
				},
				// Redo
				(o) => {
					// 内部でコールバック登録等を行っているのでちゃんといったんクリアする
					var d = o as ConnectorHistoryData;
					d.linkObject.SourceConnector = null;
					d.linkObject.TargetConnector = null;
					Links.Remove(d.linkObject);
					return o;
				}),
				// History data
				new ConnectorHistoryData() {
					linkObject = link,
					source = link.SourceConnector,
					target = link.TargetConnector
				}
				);

			// リンク解除
			link.SourceConnector = null;
			link.TargetConnector = null;
			Links.Remove(link);
		}
	}
}
