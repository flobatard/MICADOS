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

        public string toString()
        {
            return ("nom : " + nom + ", prix : " + prix + ", stock : " + stock);
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

        public string toString()
        {
            string ret = "";
            for(int i = 0 ; i < listM.Count; i++)
            {
                ret = ret + listM[i].toString() + "\n";
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
                    for(int i = 0; i < listM.Count; i++)
                    {
                        Xamarin.Forms.Internals.Log.Warning("micados", listM[i].ToString());
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