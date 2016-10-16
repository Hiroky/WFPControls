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
	/// </summary>
	public class NodeListItem : ListBoxItem
	{
		public static readonly DependencyProperty XProperty =
			DependencyProperty.Register("X", typeof(double), typeof(NodeListItem));

		public static readonly DependencyProperty YProperty =
			DependencyProperty.Register("Y", typeof(double), typeof(NodeListItem));

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


		static NodeListItem()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(NodeListItem), new FrameworkPropertyMetadata(typeof(NodeListItem)));
		}
	}
}
