using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WpfApplication1
{
	/// <summary>
	/// 
	/// </summary>
	public class PaletteItemViewModel : ViewModelBase
	{
		string name_;
		public string Name
		{
			get { return name_; }
			set { name_ = value; OnPropertyChanged("Name"); }
		}

		string category_;
		public string Category
		{
			get { return category_; }
			set { category_ = value; OnPropertyChanged("Category"); }
		}

		bool isSelected_;
		public bool IsSelected
		{
			get { return isSelected_; }
			set { isSelected_ = value; OnPropertyChanged("IsSelected"); }
		}


		public PaletteItemViewModel(string name, string category)
		{
			Name = name;
			Category = category;
		}
	}


	/// <summary>
	/// 
	/// </summary>
	public class PaletteViewModel : ViewModelBase
	{
		#region Properties

		ObservableCollection<PaletteItemViewModel> items_;
		public ObservableCollection<PaletteItemViewModel> Items
		{
			get { return items_; }
			set { items_ = value; OnPropertyChanged("Items"); }
		}

		ICollectionView itemsView_;
		public ICollectionView ItemsView
		{
			get
			{
				return itemsView_;
			}
			private set
			{
				itemsView_ = value;
				OnPropertyChanged("ItemsView");
			}
		}

		string filter_;
		public string Filter
		{
			get
			{
				return filter_;
			}
			set
			{
				filter_ = value;
				OnPropertyChanged("Filter");
				ItemsView.Refresh();
			}
		}

		#endregion

		public PaletteViewModel()
		{
			Items = new ObservableCollection<PaletteItemViewModel>();
			ItemsView = CollectionViewSource.GetDefaultView(Items);
			PropertyGroupDescription groupDescription = new PropertyGroupDescription("Category");
			ItemsView.GroupDescriptions.Add(groupDescription);
			ItemsView.Filter = FilterFunction;

			Items.Add(new PaletteItemViewModel("Position", "Attribute"));
			Items.Add(new PaletteItemViewModel("Normal", "Attribute"));
			Items.Add(new PaletteItemViewModel("Color", "Attribute"));
			Items.Add(new PaletteItemViewModel("Normalize", "Function"));
			Items.Add(new PaletteItemViewModel("Clamp", "Function"));
			Items.Add(new PaletteItemViewModel("Constant", "Constants"));
			Items.Add(new PaletteItemViewModel("ConstantVector2", "Constants"));
			Items.Add(new PaletteItemViewModel("ConstantVector3", "Constants"));
			Items.Add(new PaletteItemViewModel("ConstantVector4", "Constants"));
		}

		private bool FilterFunction(object item)
		{
			var pi = item as PaletteItemViewModel;
			pi.IsSelected = false;

            if (String.IsNullOrEmpty(Filter)) {
				return true;
			} else {
				return (pi.Name.IndexOf(Filter, StringComparison.OrdinalIgnoreCase) >= 0);
			}
		}
	}
}
