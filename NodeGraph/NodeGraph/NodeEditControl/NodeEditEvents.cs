using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace WpfApplication1
{
	///
	/// Connector Events
	///

	/// <summary>
	/// Arguments for event raised when the user starts to drag a connector out from a node.
	/// </summary>
	internal class ConnectorDragStartedEventArgs : RoutedEventArgs
	{
		internal ConnectorDragStartedEventArgs(RoutedEvent routedEvent, object source) :
			base(routedEvent, source)
		{
		}

		/// <summary>
		/// Cancel dragging out of the connector.
		/// </summary>
		public bool Cancel
		{
			get;
			set;
		}
	}

	/// <summary>
	/// Defines the event handler for ConnectorDragStarted events.
	/// </summary>
	internal delegate void ConnectorDragStartedEventHandler(object sender, ConnectorDragStartedEventArgs e);

	/// <summary>
	/// Arguments for event raised while user is dragging a node in the network.
	/// </summary>
	internal class ConnectorDraggingEventArgs : RoutedEventArgs
	{
		/// <summary>
		/// The amount the connector has been dragged horizontally.
		/// </summary>
		private double horizontalChange = 0;

		/// <summary>
		/// The amount the connector has been dragged vertically.
		/// </summary>
		private double verticalChange = 0;

		public ConnectorDraggingEventArgs(RoutedEvent routedEvent, object source, double horizontalChange, double verticalChange) :
			base(routedEvent, source)
		{
			this.horizontalChange = horizontalChange;
			this.verticalChange = verticalChange;
		}

		/// <summary>
		/// The amount the node has been dragged horizontally.
		/// </summary>
		public double HorizontalChange
		{
			get
			{
				return horizontalChange;
			}
		}

		/// <summary>
		/// The amount the node has been dragged vertically.
		/// </summary>
		public double VerticalChange
		{
			get
			{
				return verticalChange;
			}
		}
	}

	/// <summary>
	/// Defines the event handler for ConnectorDragStarted events.
	/// </summary>
	internal delegate void ConnectorDraggingEventHandler(object sender, ConnectorDraggingEventArgs e);

	/// <summary>
	/// Arguments for event raised when the user has completed dragging a connector.
	/// </summary>
	internal class ConnectorItemDragCompletedEventArgs : RoutedEventArgs
	{
		public ConnectorItemDragCompletedEventArgs(RoutedEvent routedEvent, object source) :
			base(routedEvent, source)
		{
		}
	}

	/// <summary>
	/// Defines the event handler for ConnectorDragCompleted events.
	/// </summary>
	internal delegate void ConnectorDragCompletedEventHandler(object sender, ConnectorItemDragCompletedEventArgs e);


	/// ============================================================================================

	///
	/// Connection Events
	///

	/// <summary>
	/// Base class for connection dragging event args.
	/// </summary>
	public class ConnectionDragEventArgs : RoutedEventArgs
	{
		/// <summary>
		/// The NodeItem or it's DataContext (when non-NULL).
		/// </summary>
		private object node = null;

		/// <summary>
		/// The ConnectorItem or it's DataContext (when non-NULL).
		/// </summary>
		private object draggedOutConnector = null;

		/// <summary>
		/// The connector that will be dragged out.
		/// </summary>
		protected object connection = null;

		/// <summary>
		/// 
		/// </summary>
		protected Point position;

		/// <summary>
		/// The NodeItem or it's DataContext (when non-NULL).
		/// </summary>
		public object Node
		{
			get
			{
				return node;
			}
		}

		/// <summary>
		/// The ConnectorItem or it's DataContext (when non-NULL).
		/// </summary>
		public object ConnectorDraggedOut
		{
			get
			{
				return draggedOutConnector;
			}
		}

		public Point Position
		{
			get
			{
				return position;
			}
		}

		protected ConnectionDragEventArgs(RoutedEvent routedEvent, object source, object node, object connection, object connector, Point position) :
			base(routedEvent, source)
		{
			this.node = node;
			this.draggedOutConnector = connector;
			this.connection = connection;
			this.position = position;
		}
	}

	/// <summary>
	/// Arguments for event raised when the user starts to drag a connection out from a node.
	/// </summary>
	public class ConnectionDragStartedEventArgs : ConnectionDragEventArgs
	{
		/// <summary>
		/// The connection that will be dragged out.
		/// </summary>
		public object Connection
		{
			get
			{
				return connection;
			}
			set
			{
				connection = value;
			}
		}

		internal ConnectionDragStartedEventArgs(RoutedEvent routedEvent, object source, object node, object connector, Point position) :
			base(routedEvent, source, node, null, connector, position)
		{
		}
	}

	/// <summary>
	/// Defines the event handler for the ConnectionDragStarted event.
	/// </summary>
	public delegate void ConnectionDragStartedEventHandler(object sender, ConnectionDragStartedEventArgs e);
	/// <summary>
	/// Arguments for event raised while user is dragging a node in the network.
	/// </summary>
	public class ConnectionDraggingEventArgs : ConnectionDragEventArgs
	{
		/// <summary>
		/// The connection being dragged out.
		/// </summary>
		public object Connection
		{
			get
			{
				return connection;
			}
		}

		internal ConnectionDraggingEventArgs(RoutedEvent routedEvent, object source, object node, object connection, object connector, Point position) :
			base(routedEvent, source, node, connection, connector, position)
		{
		}
	}

	/// <summary>
	/// Defines the event handler for the ConnectionDragging event.
	/// </summary>
	public delegate void ConnectionDraggingEventHandler(object sender, ConnectionDraggingEventArgs e);

	/// <summary>
	/// Arguments for event raised when the user has completed dragging a connector.
	/// </summary>
	public class ConnectionDragCompletedEventArgs : ConnectionDragEventArgs
	{
		/// <summary>
		/// The ConnectorItem or it's DataContext (when non-NULL).
		/// </summary>
		private object connectorDraggedOver = null;

		/// <summary>
		/// The ConnectorItem or it's DataContext (when non-NULL).
		/// </summary>
		public object ConnectorDraggedOver
		{
			get
			{
				return connectorDraggedOver;
			}
		}

		/// <summary>
		/// The connection that will be dragged out.
		/// </summary>
		public object Connection
		{
			get
			{
				return connection;
			}
		}

		internal ConnectionDragCompletedEventArgs(RoutedEvent routedEvent, object source, object node, object connection, object connector, object connectorDraggedOver, Point position) :
			base(routedEvent, source, node, connection, connector, position)
		{
			this.connectorDraggedOver = connectorDraggedOver;
		}
	}

	/// <summary>
	/// Defines the event handler for the ConnectionDragCompleted event.
	/// </summary>
	public delegate void ConnectionDragCompletedEventHandler(object sender, ConnectionDragCompletedEventArgs e);


	/// ============================================================================================

	///
	/// Node Events
	///
	/// 
	/// <summary>
	/// Base class for node dragging event args.
	/// </summary>
	public class NodeDragEventArgs : RoutedEventArgs
	{
		/// <summary>
		/// The NodeItem's or their DataContext (when non-NULL).
		/// </summary>
		public ICollection nodes = null;

		protected NodeDragEventArgs(RoutedEvent routedEvent, object source, ICollection nodes) :
			base(routedEvent, source)
		{
			this.nodes = nodes;
		}

		/// <summary>
		/// The NodeItem's or their DataContext (when non-NULL).
		/// </summary>
		public ICollection Nodes
		{
			get
			{
				return nodes;
			}
		}
	}


	/// <summary>
	/// Arguments for event raised while user is dragging a node in the network.
	/// </summary>
	public class NodeDraggingEventArgs : NodeDragEventArgs
	{
		/// <summary>
		/// The amount the node has been dragged horizontally.
		/// </summary>
		public double horizontalChange = 0;

		/// <summary>
		/// The amount the node has been dragged vertically.
		/// </summary>
		public double verticalChange = 0;

		internal NodeDraggingEventArgs(RoutedEvent routedEvent, object source, ICollection nodes, double horizontalChange, double verticalChange) :
			base(routedEvent, source, nodes)
		{
			this.horizontalChange = horizontalChange;
			this.verticalChange = verticalChange;
		}

		/// <summary>
		/// The amount the node has been dragged horizontally.
		/// </summary>
		public double HorizontalChange
		{
			get
			{
				return horizontalChange;
			}
		}

		/// <summary>
		/// The amount the node has been dragged vertically.
		/// </summary>
		public double VerticalChange
		{
			get
			{
				return verticalChange;
			}
		}
	}

	/// <summary>
	/// Defines the event handler for NodeDragStarted events.
	/// </summary>
	public delegate void NodeDraggingEventHandler(object sender, NodeDraggingEventArgs e);
}
