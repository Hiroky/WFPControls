using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApplication1
{
	/// <summary>
	/// 
	/// </summary>
	public class LinkViewModel : ViewModelBase
	{
		NodeConnectorViewModel source_;
		NodeConnectorViewModel target_;
		Point start_;
		Point end_;

		#region Properties

		public Point Start
		{
			get { return start_; }
			set {
				start_ = value;
				OnPropertyChanged("Start");
			}
		}
		public Point End
		{
			get { return end_; }
			set {
				end_ = value;
				OnPropertyChanged("End");
			}
		}

		public NodeConnectorViewModel SourceConnector
		{
			get { return source_; }
			set {
				// 位置更新イベント登録
				if (source_ != null) {
					// TODO:source側にもコネクタがつながっていることを通知
					source_.PositionUpdated -= new EventHandler<EventArgs>(SourceConnector_PositionUpdated);
					source_.AttachedLinks.Remove(this);
                }

				source_ = value;

				if (source_ != null) {
					source_.PositionUpdated += new EventHandler<EventArgs>(SourceConnector_PositionUpdated);
					source_.AttachedLinks.Add(this);

					Start = source_.Position;
					// 終端がない場合ひとまず同じ位置を入れておく
					if (target_ == null) {
						End = Start;
					}
				}
				OnPropertyChanged("SourceConnector");
			}
		}

		public NodeConnectorViewModel TargetConnector
		{
			get { return target_; }
			set
			{
				// 位置更新イベント登録
				if (target_ != null) {
					// TODO:source側にもコネクタがつながっていることを通知
					target_.PositionUpdated -= new EventHandler<EventArgs>(TargetConnector_PositionUpdated);
					target_.AttachedLinks.Remove(this);
				}

				target_ = value;

				if (target_ != null) {
					target_.PositionUpdated += new EventHandler<EventArgs>(TargetConnector_PositionUpdated);
					target_.AttachedLinks.Add(this);

					End = target_.Position;
					// 終端がない場合ひとまず同じ位置を入れておく
					if (source_ == null) {
						Start = End;
					}
				}

				OnPropertyChanged("TargetConnector");
			}
		}

		#endregion


		/// <summary>
		/// 
		/// </summary>
		public LinkViewModel()
		{
		}


		/// <summary>
		/// 
		/// </summary>
		private void SourceConnector_PositionUpdated(object sender, EventArgs e)
		{
			Start = SourceConnector.Position;
		}
		private void TargetConnector_PositionUpdated(object sender, EventArgs e)
		{
			End = TargetConnector.Position;
		}

	}
}
