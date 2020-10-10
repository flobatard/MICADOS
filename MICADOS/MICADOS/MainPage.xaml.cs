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
        private Comptes C;
        public MainPage()
        {
            InitializeComponent();
            string tag = "micados";
            achetables = new ListeMarchandises();
            commandesEnCours = new ListeMarchandises();
            achetables.ReadMarchandises();
            achetables.addMarchandise(new Marchandise("oasis", 1.2, 5));
            achetables.addMarchandise(new Marchandise("coca", 1.2, 4));
            achetables.addMarchandise(new Marchandise("coca", 1, 4));

            C = new Comptes();
            C.ReadComptes();

            /// Log.Warning(tag, achetables.toString());

            itemsRecettes.ItemsSource = new List<Marchandise>(achetables.listM);
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
            if(Autres.Text.Length != 0)
            {
                return Math.Round(float.Parse(Autres.Text.Replace(',', '.'), CultureInfo.InvariantCulture.NumberFormat), 2);
            }
            else
            {
                return 0;
            }
        }

        private void DoLogs(string destination)
        {

            string log = "\n---------------------\n" +
                DateTime.Now.ToString() +
                "\n" + commandesEnCours.log() +
                "\nCommentaire : " + Commentaires.Text +
                "\nTotal : " + (commandesEnCours.PrixTotal() + getAutresPrix()).ToString() + "€ à " + destination + "\n\n";
            commandesEnCours = new ListeMarchandises();
            logs.Text += log;
            Log.Warning("micados", log);
        }

        private void clean()
        {
            Commentaires.Text = "";
            Autres.Text = "";
            PrixVenteEnCours.Text = "";
            commandesEnCours = new ListeMarchandises();
            itemsVenteEnCours.ItemsSource = new List<Marchandise>(commandesEnCours.listM);
        }
        private void Vente_Banque_Clicked(object sender, EventArgs e)
        {
            C.banque += commandesEnCours.PrixTotal() + getAutresPrix();
            DoLogs("banque");
            clean();
            SoldeBanque.Text = "solde de la banque : " + C.banque.ToString();
        }
        private void Vente_Caisse_Clicked(object sender, EventArgs e)
        {
            C.caisse += commandesEnCours.PrixTotal() + getAutresPrix();
            DoLogs("Caisse");
            clean();
            SoldeCaisse.Text = "solde de la caisse : " + C.caisse.ToString();
        }
        private void Vente_Don_Clicked(object sender, EventArgs e)
        {
            DoLogs("Don");
            clean();
        }

        private void itemsRecettes_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            int i = e.ItemIndex;
            if (achetables.listM[i].stock > 0)
            {
                AddToMenu(ref commandesEnCours, achetables.listM[i], false);
                achetables.listM[i].stock -= 1;
                itemsRecettes.ItemsSource = new List<Marchandise>(achetables.listM);
                itemsVenteEnCours.ItemsSource = new List<Marchandise>(commandesEnCours.listM);
                /// itemsVenteEnCours.ItemsSource = new ObservableCollection<string>(commandesEnCours.toStringList());
                PrixVenteEnCours.Text = (commandesEnCours.PrixTotal() + getAutresPrix()).ToString() + "€";
                Log.Warning("micados", achetables.listM[i].id.ToString());
            }
        }

        private void itemsVenteEnCours_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            int i = e.ItemIndex;
            commandesEnCours.listM[i].stock += 1;
            commandesEnCours.listM.RemoveAt(i);
            itemsVenteEnCours.ItemsSource = new List<Marchandise>(commandesEnCours.listM);
            itemsRecettes.ItemsSource = new List<Marchandise>(achetables.listM);
            PrixVenteEnCours.Text = (commandesEnCours.PrixTotal() + getAutresPrix()).ToString() + "€";
            Log.Warning("micados", achetables.listM[i].id.ToString());
        }

        private void itemsDepenses_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            AddToMenu(ref commandesEnCours, achetables.listM[e.ItemIndex]);
        }
    }
}
