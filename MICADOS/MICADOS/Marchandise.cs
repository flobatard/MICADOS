using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Xamarin.Forms;
using System.Linq;
using System.Globalization;
using System.Xml;
using System.Data;
using Xamarin.Forms.Markup;
using System.Threading;

namespace MICADOS
{
    public class Marchandise
    {
        public string nom { get; set; }
        public double prix { get; set; }
        public int stock { get; set; }
        public string prixstring { get; set; }
        public int vente { get; set; }
        public int achat { get; set; }
        public int dispo { get; set; }

        public Guid id;
        public Marchandise(string n, double p, int s)
        {
            nom = n;
            prix = p;
            prixstring = Math.Round(prix, 2).ToString() + "€";
            stock = s;
            vente = 0;
            achat = 0;
            dispo = 0;
            id = Guid.NewGuid();
        }

        public Marchandise(string n, string p, string s)
        {
            nom = n;
            stock = Int32.Parse(s);
            prix = float.Parse(p, CultureInfo.InvariantCulture.NumberFormat);
            prixstring = p + "€";
            vente = 0;
            achat = 0;
            dispo = 0;
            id = Guid.NewGuid();
        }

        public string toString(bool full=false)
        {
            string ret = "";
            ret += nom + " (" + Math.Round(prix, 2) + "€)\n";
            if (full)
            {
                ret += stock + " en stock\n" + dispo.ToString() + " dispos\n" + achat + " vont être achetés\n" + vente + " vont être vendus.";
            }
            return ret;
        }
        public string log(bool achat = false)
        {
            if (achat)
            { 
                return (this.achat.ToString() + " " + nom + " (" + Math.Round(prix, 2) + " €)");
            }
            else{
                return (this.vente.ToString() + " " + nom + " (" + Math.Round(prix, 2) + " €)");
            }
        }
        public void UpdateDispo()
        {
            dispo = stock + achat - vente;
        }
        public void reset()
        {
            achat = 0;
            vente = 0;
            dispo = 0;
        }
        public void DoTransaction()
        {
            stock = dispo;
            reset();
        }
    }
    public class ListeMarchandises
    {
        public List<Marchandise> listM = new List<Marchandise>();

        public ListeMarchandises(){}
        public ListeMarchandises(List<Marchandise> l)
        {
            for(int i = 0; i < l.Count; i++)
            {
                listM.Add(l[i]);
            }
        }

        public void UpdateDispos()
        {
            for(int i = 0; i < listM.Count(); i++)
            {
                listM[i].UpdateDispo();
            }
        }

        public string[] toStringArray(bool full=false)
        {
            string[] ret = new string[listM.Count()];
            for (int i = 0; i < listM.Count(); i++)
            {
                ret[i] = listM[i].toString(full);
            }
            return ret;
        }
        public List<string> toStringList(bool full=false)
        {
            List<string> ret = new List<string>();
            for (int i = 0; i < listM.Count(); i++)
            {
                ret.Add(listM[i].toString(full));
            }
            return ret;
        }

        public double PrixTotal(bool achat)
        {
            if (achat)
            {
                double prix = 0;
                for(int i = 0; i < listM.Count; i++)
                {
                    prix = prix + listM[i].prix * listM[i].achat;
                }
                return Math.Round(prix, 2);
            }
            else
            {
                double prix = 0;
                for (int i = 0; i < listM.Count; i++)
                {
                    prix = prix + listM[i].prix * listM[i].vente;
                }
                return Math.Round(prix, 2);
            }
        }
        public string toString(bool full=false)
        {
            string ret = "";
            for (int i = 0; i < listM.Count; i++)
            {
                ret = ret + listM[i].toString(full) + "\n";
            }
            return ret;
        }

        public string log(bool achat=false)
        {
            string ret = "";
            for (int i = 0; i < listM.Count; i++)
            {
                ret = ret + listM[i].log(achat) + "\n";
            }
            return ret;
        }

        public void addMarchandise(Marchandise m)
        {
            listM.Add(m);
        }

        public void ReadMarchandises()
        {
            var dataPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "data.micados");
            if (File.Exists(dataPath)) 
            {

                using (var reader = new StreamReader(dataPath, true))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] mar = line.Split(',');
                        string n = mar[0];
                        string p = mar[1];
                        string s = mar[2];
                        listM.Add(new Marchandise(n, p, s));
                    }
                }

            }
        }
        public void SaveMarchandises()
        {
            var dataPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "data.micados");
            using (var writer = File.CreateText(dataPath))
            {
                for (int i = 0; i < listM.Count; i++)
                {
                    writer.WriteLine(listM[i].nom.ToString() + "," + listM[i].prix.ToString() + "," + (string)listM[i].stock.ToString());
                }
            }
        }
        public int getIndexGuid(Guid i)
        {
            for(int j =0; j < listM.Count(); j++)
            {
                if(listM[j].id.CompareTo(i) == 0)
                {
                    return j;
                }
            }
            return -1;
        }
        public void ResetAll()
        {
            for(int i = 0; i < listM.Count(); i++)
            {
                listM[i].reset();
            }
        }
        public void DoTransaction()
        {
            for(int i = 0; i < listM.Count(); i++)
            {
                listM[i].DoTransaction();
            }
        }
    }
}