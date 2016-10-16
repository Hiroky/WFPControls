using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApplication1
{
	public class LinkControl : Shape
	{
		#region Dependancy properties

		public static readonly DependencyProperty StartProperty = 
			DependencyProperty.Register("Start", typeof(Point), typeof(LinkControl),
				new FrameworkPropertyMetadata(new Point(0.0, 0.0), FrameworkPropertyMetadataOptions.AffectsRender));

		public static readonly DependencyProperty EndProperty =
			DependencyProperty.Register("End", typeof(Point), typeof(LinkControl),
				new FrameworkPropertyMetadata(new Point(0.0, 0.0), FrameworkPropertyMetadataOptions.AffectsRender));

		#endregion

		#region Properties

		private PathGeometry geometory_;
		protected override Geometry DefiningGeometry
		{
			get {
				UpdatePath();			// TODO:ここで大丈夫か要検証
				return geometory_;
			}
		}

		public Point Start
		{
			get
			{
				return (Point)GetValue(StartProperty);
			}
			set
			{
				SetValue(StartProperty, value);
			}
		}

		public Point End
		{
			get
			{
				return (Point)GetValue(EndProperty);
			}
			set
			{
				SetValue(EndProperty, value);
			}
		}

		#endregion


		/// <summary>
		/// 
		/// </summary>
		public LinkControl()
		{
			geometory_ = new PathGeometry();
		}

		/// <summary>
		/// 
		/// </summary>
		private void UpdatePath()
		{
			PathFigure pf = new PathFigure();
			pf.StartPoint = Start;

			double half = Math.Abs(End.X - Start.X);
			half = Math.Min(100.0, half / 2);

			// イーズインイーズアウトっぽい感じにする
			var point0 = new Point(Start.X + half, Start.Y);
			var point1 = new Point(End.X - half, End.Y);
			pf.Segments.Add(new BezierSegment(point0, point1, End, true));

			geometory_.Figures.Clear();
			geometory_.Figures.Add(pf);
		}
	}
}
