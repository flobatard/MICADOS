using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace MICADOS
{
    public partial class MainPage : TabbedPage
    {
        private ListeMarchandises achetables, commandesEnCours;
        ObservableCollection<string> menuAchetables, menuCommandeEnCours;
        public MainPage()
        {
            InitializeComponent();
            string tag = "micados";
            achetables = new ListeMarchandises();
            commandesEnCours = new ListeMarchandises();
            achetables.ReadMarchandises();
            achetables.addMarchandise(new Marchandise("oasis", 1.2, 5));

            /// Log.Warning(tag, achetables.toString());

            menuAchetables = new ObservableCollection<string>();
            Log.Warning(tag, menuAchetables.Count().ToString());
            FillMenu(ref menuAchetables, ref achetables);
            itemsRecettes.ItemsSource = menuAchetables;

            menuCommandeEnCours = new ObservableCollection<string>();
            FillMenu(ref menuAchetables, ref achetables);
            Log.Warning(tag, achetables.toString());
            itemsRecettes.ItemsSource = menuAchetables;
        }

        private void FillMenu(ref ObservableCollection<string> menu, ref ListeMarchandises l)
        {
            menu = new ObservableCollection<string>();
            for (int i = 0; i < l.listM.Count; i++)
            {
                menu.Add(l.listM[i].nom);
            }
        }
        private void AddToMenu(ref ObservableCollection<string> menu, Marchandise l)
        {
            if (!menu.Contains(l.nom))
            {
                menu.Add(l.nom);
            }
        }

        private void itemsRecettes_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            Debug.WriteLine("item tapped : " + e.ItemIndex.ToString());
            AddToMenu(ref menuCommandeEnCours, achetables.listM[e.ItemIndex]);
            itemsVenteEnCours.ItemsSource = menuCommandeEnCours;
        }
    }
}
