using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
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

            C = new Comptes();
            C.ReadComptes();

            /// Log.Warning(tag, achetables.toString());
            UpdateAll();

            itemsRecettes.ItemsSource = new List<Marchandise>(Achetables.listM);
            itemsDepenses.ItemsSource = new List<Marchandise>(Achetables.listM);
            itemsAjoute.ItemsSource = new List<Marchandise>(Achetables.listM);
        }

        private void AddToMenu(ref ListeMarchandises menu, Marchandise l, bool achat, bool check=true)
        {
            if(achat)
            {
                if(l.achat <= 0)
                {
                    if (!check || !menu.toStringList(false).Contains(l.toString(false))) menu.listM.Add(l);
                }
                l.achat += 1;
            }
            if(!achat)
            {
                if(l.vente <= 0)
                {
                    if (!check || !menu.toStringList(false).Contains(l.toString(false))) menu.listM.Add(l);
                }
                l.vente += 1;
            }
        }

        private void RemoveFromMenu(ref ListeMarchandises menu, int index, bool achat)
        {
            if (achat)
            {
                menu.listM[index].achat -= 1;
                if(menu.listM[index].achat <= 0)
                {
                    DepensesEnCours.listM.RemoveAt(index);
                }
            }
            else
            {
                menu.listM[index].vente -= 1;
                if (menu.listM[index].vente <= 0)
                {
                    CommandesEnCours.listM.RemoveAt(index);
                }
            }
        }

        private double getAutresVentePrix()
        {
            return (AutresVente.Text.Length != 0 && AutresVente.Text != "-") ? Math.Round(float.Parse(AutresVente.Text.Replace(',', '.'), CultureInfo.InvariantCulture.NumberFormat), 2) : 0;
        }

        private void DoLogs(string destination, bool achat = false)
        {
            if (achat)
            {
                string prixachat = (PrixDepenseEnCours.Text.Length != 0) ? PrixDepenseEnCours.Text : "0";
                string log = "\n---------------------\n" +
                    "dépense le " + DateTime.Now.ToString() +
                    "\n" + DepensesEnCours.log(achat) +
                    "\nplus " + AutresDepense.Text  + "€" +
                    "\nCommentaire : " + CommentairesDepense.Text +
                    "\nTotal : " + prixachat + "€ à " + destination + "\n\n";
                logs.Text += log;
            }
            else
            {
                string prixvente = (PrixVenteEnCours.Text.Length != 0) ? PrixVenteEnCours.Text : "0";
                string log = "\n---------------------\n" +
                    "vente le " + DateTime.Now.ToString() +
                    "\n" + CommandesEnCours.log(achat) +
                    "\nplus " + AutresVente.Text + "€" +
                    "\nCommentaire : " + CommentairesVente.Text +
                    "\nTotal : " + (prixvente + " € à " + destination + "\n\n");
                logs.Text += log;
            }
        }
        private void DoLogsTransaction(string from, string to)
        {
            string log = "\n---------------------\n" +
                "transaction le " + DateTime.Now.ToString() +
                "\nde " + from + " à " + to +
                "\nCommentaire : " + CommentairesDepense.Text +
                "\nValeur : " + MontantTransaction.Text + "€\n\n";
            logs.Text += log;
        }

        private void DoTransaction(string type, bool achat=false)
        {
            DoLogs(type, achat);
            double prixdep = (PrixDepenseEnCours.Text.Length != 0) ? Math.Round(float.Parse(PrixDepenseEnCours.Text.Replace("€", string.Empty), CultureInfo.InvariantCulture.NumberFormat), 2) : 0;
            if (achat)
            {
                switch (type)
                {
                    case "Banque":
                        if (PrixDepenseEnCours.Text.Length != 0)
                        C.banque -= prixdep;
                        break;
                    case "Caisse":
                        C.caisse -= prixdep;
                        break;
                    case "Don":
                        break;
                }
                DepensesEnCours = new ListeMarchandises();
            }
            else
            {
                switch (type)
                {
                    case "Banque":
                        C.banque += CommandesEnCours.PrixTotal(false) + getAutresVentePrix();
                        break;
                    case "Caisse":
                        C.caisse += CommandesEnCours.PrixTotal(false) + getAutresVentePrix();
                        break;
                    case "Don":
                        break;
                }
                CommandesEnCours = new ListeMarchandises();
            }
            Achetables.DoTransaction();
            Achetables.SaveMarchandises();
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
            itemsAjoute.ItemsSource = new List<Marchandise>(Achetables.listM);
            SoldeCaisse.Text = C.caisse.ToString();
            SoldeBanque.Text = C.banque.ToString();

        }

        private void clean()
        {
            CommentairesVente.Text = "";
            AutresVente.Text = "";
            AutresDepense.Text = "";
            PrixVenteEnCours.Text = "";
            PrixDepenseEnCours.Text = "";
            CommandesEnCours.ResetAll();
            UpdateAll();
            CadreAutreDepense.BorderColor = Color.Black;
            CadrePrixDepenseEnCours.BorderColor = Color.Black;
            ErreurAjout.Text = "";
            NomProduitAjoute.Text = "";
            PrixProduitAjoute.Text = "";
        }

        /// Ventes
        
        private void AutresVente_TextChanged(object sender, TextChangedEventArgs e)
        {
            PrixVenteEnCours.Text = (CommandesEnCours.PrixTotal(false) + getAutresVentePrix()).ToString() + "€";
        }
        private void Vente_Banque_Clicked(object sender, EventArgs e)
        {
            for(int i = 0; i < Achetables.listM.Count(); i++)
            {
                if(Achetables.listM[i].vente > Achetables.listM[i].stock)
                {
                    ErreurVente.Text = Achetables.listM[i].nom + " : plus de ventes que de stock";
                    return;
                }
            }
            DoTransaction("Banque", false);
            UpdateAll();
            clean();
            SoldeBanque.Text = C.banque.ToString();
            C.SaveComptes();
        }
        private void Vente_Caisse_Clicked(object sender, EventArgs e)
        {
            for (int i = 0; i < Achetables.listM.Count(); i++)
            {
                if (Achetables.listM[i].vente > Achetables.listM[i].stock)
                {
                    ErreurVente.Text = Achetables.listM[i].nom + " : plus de ventes que de stock";
                    return;
                }
            }
            DoTransaction("Caisse", false);
            UpdateAll();
            clean();
            SoldeCaisse.Text = C.caisse.ToString();
            C.SaveComptes();
        }
        private void Vente_Don_Clicked(object sender, EventArgs e)
        {
            for (int i = 0; i < Achetables.listM.Count(); i++)
            {
                if (Achetables.listM[i].vente > Achetables.listM[i].stock)
                {
                    ErreurVente.Text = Achetables.listM[i].nom + " : plus de ventes que de stock";
                    return;
                }
            }
            DoTransaction("Don", false);
            UpdateAll();
            clean();
            C.SaveComptes();
        }

        private void itemsRecettes_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            int i = e.ItemIndex;
            if (Achetables.listM[i].dispo > 0)
            {
                AddToMenu(ref CommandesEnCours, Achetables.listM[i], false, true);
                UpdateAll();
                PrixVenteEnCours.Text = (CommandesEnCours.PrixTotal(false) + getAutresVentePrix()).ToString() + "€";
            }
        }

        private void itemsVenteEnCours_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            int i = e.ItemIndex;
            RemoveFromMenu(ref CommandesEnCours, i, false);
            UpdateAll();
        }

        /// Depenses
        
        private void itemsDepenses_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            int i = e.ItemIndex;
            AddToMenu(ref DepensesEnCours, Achetables.listM[i], true, true);
            UpdateAll();
        }

        private void AutresDepense_TextChanged(object sender, TextChangedEventArgs e){}

        private void itemsDepenseEnCours_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            int i = e.ItemIndex;
            RemoveFromMenu(ref DepensesEnCours, i, true);
            UpdateAll();
        }
        private void Depense_Banque_Clicked(object sender, EventArgs e)
        {
            double autredep, prixdep;
            autredep = (AutresDepense.Text.Length != 0) ? Math.Round(float.Parse(AutresDepense.Text, CultureInfo.InvariantCulture.NumberFormat), 2) : 0;
            prixdep = (PrixDepenseEnCours.Text.Length != 0) ? Math.Round(float.Parse(PrixDepenseEnCours.Text, CultureInfo.InvariantCulture.NumberFormat), 2) : 0;

            if (prixdep < autredep)
            {
                CadreAutreDepense.BorderColor = Color.FromRgb(255, 255, 0);
                CadrePrixDepenseEnCours.BorderColor = Color.FromRgb(255, 0, 0);
            }
            else
            {
                DoTransaction("Banque", true);
                clean();
                SoldeBanque.Text = C.banque.ToString();
            }
            C.SaveComptes();
        }


        private void Depense_Caisse_Clicked(object sender, EventArgs e)
        {
            double autredep, prixdep;
            autredep = (AutresDepense.Text.Length != 0) ? Math.Round(float.Parse(AutresDepense.Text, CultureInfo.InvariantCulture.NumberFormat), 2) : 0;
            prixdep = (PrixDepenseEnCours.Text.Length != 0) ? Math.Round(float.Parse(PrixDepenseEnCours.Text, CultureInfo.InvariantCulture.NumberFormat), 2) : 0;
            if (prixdep<autredep)
            {
                CadreAutreDepense.BorderColor = Color.FromRgb(255, 255, 0);
                CadrePrixDepenseEnCours.BorderColor = Color.FromRgb(255, 0, 0);
            }
            else
            {
                DoTransaction("Caisse", true);
                clean();
                SoldeCaisse.Text = C.caisse.ToString();
            }
            C.SaveComptes();
        }
        private void Depense_Don_Clicked(object sender, EventArgs e)
        {
            double autredep = (AutresDepense.Text.Length != 0) ? Math.Round(float.Parse(AutresDepense.Text, CultureInfo.InvariantCulture.NumberFormat), 2) : 0;
            double prixdep = (PrixDepenseEnCours.Text.Length != 0) ? Math.Round(float.Parse(PrixDepenseEnCours.Text, CultureInfo.InvariantCulture.NumberFormat), 2) : 0;
            if (prixdep<autredep)
            {
                CadreAutreDepense.BorderColor = Color.FromRgb(255, 255, 0);
                CadrePrixDepenseEnCours.BorderColor = Color.FromRgb(255, 0, 0);
            }
            else
            {
                DoTransaction("Don", true);
                clean();
            }
            C.SaveComptes();
        }

        /// tresorerie
        
        private void cleanTresorerie()
        {
            Compte1.SelectedItem = null;
            Compte2.SelectedItem = null;
            CommentairesTransaction.Text = "";
            MontantTransaction.Text = "";
        }
        private void EnregistrerTransaction_Clicked(object sender, EventArgs e)
        {
            Log.Warning("micados", "-" + Compte1.SelectedItem.ToString() + "-");
            switch (Compte1.SelectedItem.ToString())
            {
                case "Banque":
                    C.banque -= Math.Round(float.Parse(MontantTransaction.Text.Replace(',', '.'), CultureInfo.InvariantCulture.NumberFormat), 2);
                    break;
                case "Caisse":
                    C.caisse -= Math.Round(float.Parse(MontantTransaction.Text.Replace(',', '.'), CultureInfo.InvariantCulture.NumberFormat), 2);
                    break;
            }
            switch (Compte2.SelectedItem.ToString())
            {
                case "Banque":
                    C.banque += Math.Round(float.Parse(MontantTransaction.Text.Replace(',', '.'), CultureInfo.InvariantCulture.NumberFormat), 2);
                    break;
                case "Caisse":
                    C.caisse += Math.Round(float.Parse(MontantTransaction.Text.Replace(',', '.'), CultureInfo.InvariantCulture.NumberFormat), 2);
                    break;
            }
            Log.Warning("micados", C.banque.ToString());
            DoLogsTransaction(Compte1.SelectedItem.ToString(), Compte2.SelectedItem.ToString());
            cleanTresorerie();
            C.SaveComptes();
            UpdateAll();
        }

        private void Ajoute_Clicked(object sender, EventArgs e)
        {
            Achetables.addMarchandise(new Marchandise(NomProduitAjoute.Text, Math.Round(float.Parse(PrixProduitAjoute.Text.Replace(',', '.'), CultureInfo.InvariantCulture.NumberFormat), 2), 0));
            Achetables.SaveMarchandises();
            UpdateAll();
        }

        private void itemsAjoute_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (Achetables.listM[e.ItemIndex].stock <= 0) Achetables.listM.RemoveAt(e.ItemIndex);
            else ErreurAjout.Text = "Erreur : retrait d'une marchandise dont le stock est non nul";
            Achetables.SaveMarchandises();
            UpdateAll();
        }
    }
}
