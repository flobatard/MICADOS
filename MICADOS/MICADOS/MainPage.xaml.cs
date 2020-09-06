using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MICADOS
{
    public partial class MainPage : TabbedPage
    {
        public MainPage()
        {
            InitializeComponent();
            ObservableCollection<string> itemList;
            itemList = new ObservableCollection<string>();
            itemList.Add(prout.Text);
            itemsDepenses.ItemsSource = itemList;

        }
    }
}
