using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace MICADOS
{
    public partial class MainPage : TabbedPage
    {
        public MainPage()
        {
            InitializeComponent();
            string tag = "micados";
            ObservableCollection<string> itemList;
            itemList = new ObservableCollection<string>();
            itemList.Add("je suis à vendre !");
            itemsRecettes.ItemsSource = itemList;
            Marchandise m = new Marchandise("coca", 1.2, 4);
            List<Marchandise> l = new List<Marchandise>();
            l.Add(m);
            ListeMarchandises lm = new ListeMarchandises(l);
            lm.SaveMarchandises();
            Log.Warning(tag, lm.toString());
            ListeMarchandises listmar = new ListeMarchandises();
            listmar.ReadMarchandises();
            ObservableCollection<string> cocalist = new ObservableCollection<string>();
            for(int i = 0; i < listmar.listM.Count; i++)
            {
                cocalist.Add(listmar.listM[i].nom);
            }
            Log.Warning(tag, listmar.toString());
            itemsRecettes.ItemsSource = cocalist;
        }

        private void itemsRecettes_itemtapped(object sender, FocusEventArgs e)
        {
            /// TODO
            /// itemsVenteEnCours.ItemsSource
        }
    }
}
