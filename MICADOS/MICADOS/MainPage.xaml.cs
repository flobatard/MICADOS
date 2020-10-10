using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
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
            achetables.addMarchandise(new Marchandise("coca", 1.2, 4));

            /// Log.Warning(tag, achetables.toString());

            itemsRecettes.ItemsSource = new ObservableCollection<string>(achetables.toStringList(true));
        }

        private void AddToMenu(ref ListeMarchandises menu, Marchandise l, bool check=true)
        {
            if (!check || !menu.toStringList(false).Contains(l.toString(false)))
            {
                menu.listM.Add(l);
            }
        }

        private void Autres_TextChanged(object sender, TextChangedEventArgs e)
        {
            PrixVenteEnCours.Text = (commandesEnCours.PrixTotal() + getAutresPrix()).ToString() + "€";
        }

        private double getAutresPrix()
        {
            Log.Warning("micados", Autres.Text);
            if(Autres.Text.Length != 0)
            {
                return Math.Round(float.Parse(Autres.Text, CultureInfo.InvariantCulture.NumberFormat), 2);
            }
            else
            {
                return 0;
            }
        }
        private void itemsRecettes_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            int i = e.ItemIndex;
            if (achetables.listM[i].stock > 0)
            {
                AddToMenu(ref commandesEnCours, achetables.listM[i], false);
                achetables.listM[i].stock = achetables.listM[i].stock - 1;
                itemsRecettes.ItemsSource = new ObservableCollection<string>(achetables.toStringList(true));
                itemsVenteEnCours.ItemsSource = new ObservableCollection<string>(commandesEnCours.toStringList());
                PrixVenteEnCours.Text = (commandesEnCours.PrixTotal() + getAutresPrix()).ToString() + "€";
            }
        }
        private void itemsDepenses_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            AddToMenu(ref commandesEnCours, achetables.listM[e.ItemIndex]);
            itemsVenteEnCours.ItemsSource = menuCommandeEnCours;
        }
    }
}
