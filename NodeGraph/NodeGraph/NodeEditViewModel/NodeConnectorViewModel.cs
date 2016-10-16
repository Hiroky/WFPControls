using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.ObjectModel;

using Newtonsoft.Json;

namespace WpfApplication1
{
	/// <summary>
	/// 
	/// </summary>
	[JsonObject(MemberSerialization.OptIn)]
	public class NodeConnectorViewModel : ViewModelBase
	{
		public enum ConnectorType
		{
			Input,
			Output,
		};


		Point position_;
		bool isConnected_;
		ObservableCollection<LinkViewModel> attachedLinks_;


		#region Properties

		[JsonProperty]
		public string Name {
			get; set;
		}

		public NodeViewModel Parent {
			get; set;
		}

		[JsonProperty]
		public ConnectorType Type {
			get; set;
		}

		[JsonProperty]
		public Point Position
		{
			get { return position_; }
			set
			{
				position_ = value;

				OnPropertyChanged("Position");

				// 更新イベント
				if (PositionUpdated != null) {
					PositionUpdated(this, EventArgs.Empty);
				}
			}
		}

		[JsonProperty]
		public ObservableCollection<LinkViewModel> AttachedLinks
		{
			get { return attachedLinks_; }
			set
			{
				attachedLinks_ = value;
				OnPropertyChanged("AttachedLinks");
			}
		}

		[JsonProperty]
		public bool IsConnected
		{
			get { return isConnected_; }
			set
			{
				isConnected_ = value;
				OnPropertyChanged("IsConnected");
			}
		}

		public event EventHandler<EventArgs> PositionUpdated;

		#endregion



		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="parent"></param>
		public NodeConnectorViewModel(string name, ConnectorType type, NodeViewModel parent)
		{
			Name = name;
			Parent = parent;
			Type = type;
			attachedLinks_ = new ObservableCollection<LinkViewModel>();
			IsConnected = false;

			// 接続を監視
			attachedLinks_.CollectionChanged += (o, e) => {
				IsConnected = attachedLinks_.Count > 0;
				//Console.WriteLine("AttachNum : {0}", attachedLinks_.Count);
			};
        }
	}
}
