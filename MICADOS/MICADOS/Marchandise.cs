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
    class Marchandise
    {
        public string nom;
        public double prix;
        public int stock;
        public Marchandise(string n, double p, int s)
        {
            nom = n;
            prix = p;
            stock = s;
        }

        public Marchandise(string n, string p, string s)
        {
            nom = n;
            stock = Int32.Parse(s);
            prix = float.Parse(p, CultureInfo.InvariantCulture.NumberFormat);
        }

        public string toString(bool full=false)
        {
            if (full)
            {
                return (nom + " " + Math.Round(prix, 2) + "€ (" + stock + " en stock)");
            }
            else return nom;
        }

    }
    class ListeMarchandises
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

        public double PrixTotal()
        {
            double prix = 0;
            for(int i = 0; i < listM.Count; i++)
            {
                prix = prix + listM[i].prix;
            }
            return Math.Round(prix, 2);
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
    }
}