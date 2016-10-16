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
	public class NodeListControl : ListBox
	{
		//static NodeListControl()
		//{
		//	DefaultStyleKeyProperty.OverrideMetadata(typeof(NodeListControl), new FrameworkPropertyMetadata(typeof(NodeListControl)));
		//}


		/// <summary>
		/// </summary>
		internal NodeListItem FindAssociatedNodeItem(object nodeDataContext)
		{
			return (NodeListItem)ItemContainerGenerator.ContainerFromItem(nodeDataContext);
		}

		/// <summary>
		/// </summary>
		protected override DependencyObject GetContainerForItemOverride()
		{
			return new NodeListItem();
		}

		/// <summary>
		/// </summary>
		protected override bool IsItemItsOwnContainerOverride(object item)
		{
			return item is NodeListItem;
		}

	}
}
