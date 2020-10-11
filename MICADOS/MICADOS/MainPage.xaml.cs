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
        private ListeMarchandises Achetables, CommandesEnCours, DepensesEnCours;
        private Comptes C;
        public MainPage()
        {
            InitializeComponent();
            string tag = "micados";
            Achetables = new ListeMarchandises();
            CommandesEnCours = new ListeMarchandises();
            DepensesEnCours = new ListeMarchandises();
            Achetables.ReadMarchandises();
            Achetables.addMarchandise(new Marchandise("oasis", 1.2, 5));
            Achetables.addMarchandise(new Marchandise("coca", 1.2, 4));
            Achetables.addMarchandise(new Marchandise("coca", 1, 4));

            C = new Comptes();
            C.ReadComptes();

            /// Log.Warning(tag, achetables.toString());
            UpdateAll();

            itemsRecettes.ItemsSource = new List<Marchandise>(Achetables.listM);
            itemsDepenses.ItemsSource = new List<Marchandise>(Achetables.listM);
        }

        private void AddToMenu(ref ListeMarchandises menu, Marchandise l, bool achat, bool check=true)
        {
            if (!check || !menu.toStringList(false).Contains(l.toString(false)))
            {
                if(achat)
                {
                    if(l.achat == 0)
                    {
                        menu.listM.Add(l);
                    }
                    l.achat += 1;
                }
                if(!achat)
                {
                    if(l.vente == 0)
                    {
                        menu.listM.Add(l);
                    }
                    l.vente += 1;
                }
            }
        }

        private void RemoveFromMenu(ref ListeMarchandises menu, int index, bool achat)
        {
            if (achat)
            {
                menu.listM[index].achat -= 1;
                if(menu.listM[index].achat == 0)
                {
                    DepensesEnCours.listM.RemoveAt(index);
                }
            }
            else
            {
                menu.listM[index].vente -= 1;
                if (menu.listM[index].vente == 0)
                {
                    CommandesEnCours.listM.RemoveAt(index);
                }
            }
        }

        private double getAutresVentePrix()
        {
            if(AutresVente.Text.Length != 0)
            {
                return Math.Round(float.Parse(AutresVente.Text.Replace(',', '.'), CultureInfo.InvariantCulture.NumberFormat), 2);
            }
            else
            {
                return 0;
            }
        }

        private void DoLogs(string destination, bool recette = true)
        {
            if (recette)
            {
            string log = "\n---------------------\n" +
                "vente le " + DateTime.Now.ToString() +
                "\n" + CommandesEnCours.log() +
                "\nCommentaire : " + CommentairesVente.Text +
                "\nTotal : " + (CommandesEnCours.PrixTotal(false) + getAutresVentePrix()).ToString() + "€ à " + destination + "\n\n";
            CommandesEnCours = new ListeMarchandises();
            logs.Text += log;
            }
            else
            {
                string log = "\n---------------------\n" +
                    "dépense le " + DateTime.Now.ToString() +
                    "\n" + DepensesEnCours.log() +
                    "\nCommentaire : " + CommentairesDepense.Text +
                    "\nTotal : " + (DepensesEnCours.PrixTotal(false) + getAutresVentePrix()).ToString() + "€ à " + destination + "\n\n";
                DepensesEnCours = new ListeMarchandises();
                logs.Text += log;
            }
        }

        private void DoTransaction()
        {
            Achetables.DoTransaction();
        }
        private void UpdateAll()
        {
            Achetables.UpdateDispos();
            CommandesEnCours.UpdateDispos();
            DepensesEnCours.UpdateDispos();
            itemsRecettes.ItemsSource = new List<Marchandise>(Achetables.listM);
            itemsVenteEnCours.ItemsSource = new List<Marchandise>(CommandesEnCours.listM);
            itemsDepenseEnCours.ItemsSource = new List<Marchandise>(DepensesEnCours.listM);
            itemsDepenses.ItemsSource = new List<Marchandise>(Achetables.listM);

        }

        private void clean()
        {
            CommentairesVente.Text = "";
            AutresVente.Text = "";
            PrixVenteEnCours.Text = "";
            CommandesEnCours.ResetAll();
            UpdateAll();
        }

        /// Ventes
        
        private void AutresVente_TextChanged(object sender, TextChangedEventArgs e)
        {
            PrixVenteEnCours.Text = (CommandesEnCours.PrixTotal(false) + getAutresVentePrix()).ToString() + "€";
        }
        private void Vente_Banque_Clicked(object sender, EventArgs e)
        {
            C.banque += CommandesEnCours.PrixTotal(false) + getAutresVentePrix();
            DoLogs("banque");
            DoTransaction();
            UpdateAll();
            clean();
            SoldeBanque.Text = "solde de la banque : " + C.banque.ToString();
        }
        private void Vente_Caisse_Clicked(object sender, EventArgs e)
        {
            C.caisse += CommandesEnCours.PrixTotal(false) + getAutresVentePrix();
            DoLogs("Caisse");
            DoTransaction();
            UpdateAll();
            clean();
            SoldeCaisse.Text = "solde de la caisse : " + C.caisse.ToString();
        }
        private void Vente_Don_Clicked(object sender, EventArgs e)
        {
            DoTransaction();
            UpdateAll();
            DoLogs("Don");
            clean();
        }

        private void itemsRecettes_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            int i = e.ItemIndex;
            if (Achetables.listM[i].dispo > 0)
            {
                AddToMenu(ref CommandesEnCours, Achetables.listM[i], false, false);
                UpdateAll();
                PrixVenteEnCours.Text = (CommandesEnCours.PrixTotal(false) + getAutresVentePrix()).ToString() + "€";
                Log.Warning("micados", Achetables.listM[i].toString(true));
            }
        }

        private void itemsVenteEnCours_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            int i = e.ItemIndex;
            RemoveFromMenu(ref CommandesEnCours, i, false);
            PrixVenteEnCours.Text = (CommandesEnCours.PrixTotal(false) + getAutresVentePrix()).ToString() + "€";
            UpdateAll();
        }

        /// Depenses
        
        private void itemsDepenses_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            int i = e.ItemIndex;
            AddToMenu(ref DepensesEnCours, Achetables.listM[i], true, false);
            UpdateAll();
        }

        private void AutresDepense_TextChanged(object sender, TextChangedEventArgs e)
        {
        }


        private double getAutresDepensePrix()
        {
            if (AutresVente.Text.Length != 0)
            {
                return Math.Round(float.Parse(AutresDepense.Text.Replace(',', '.'), CultureInfo.InvariantCulture.NumberFormat), 2);
            }
            else
            {
                return 0;
            }
        }

        private void itemsDepenseEnCours_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            int i = e.ItemIndex;
            RemoveFromMenu(ref DepensesEnCours, i, true);
            UpdateAll();
        }
        private void Depense_Banque_Clicked(object sender, EventArgs e)
        {
            C.banque -= CommandesEnCours.PrixTotal(true) + getAutresDepensePrix();
            DoLogs("banque", false);
            clean();
            SoldeBanque.Text = "solde de la banque : " + C.banque.ToString();
        }
        private void Depense_Caisse_Clicked(object sender, EventArgs e)
        {
            C.caisse -= CommandesEnCours.PrixTotal(true) + getAutresDepensePrix();
            DoLogs("caisse", false);
            clean();
            SoldeCaisse.Text = "solde de la banque : " + C.banque.ToString();
        }
        private void Depense_Don_Clicked(object sender, EventArgs e)
        {
            DoLogs("don", false);
            clean();
        }
    }
}
