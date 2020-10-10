using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Xamarin.Forms;

namespace MICADOS
{
    class Comptes
    {
        public double caisse, banque;
        public Comptes() { }
        public Comptes(float c, float b)
        {
            caisse = c;
            banque = b;
        }
        public void ReadComptes()
        {
            var dataPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "comptes.micados");
            if (File.Exists(dataPath))
            {
                using (var reader = new StreamReader(dataPath, true))
                {
                    string l;
                    if ((l = reader.ReadLine()) != null)
                    {
                        caisse = float.Parse(l, CultureInfo.InvariantCulture.NumberFormat);
                        if ((l = reader.ReadLine()) != null)
                        {
                            banque = float.Parse(l, CultureInfo.InvariantCulture.NumberFormat);
                        }
                    }
                }
            }
        }
        public void SaveMarchandises()
        {
            var dataPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "comptes.micados");
            using (var writer = File.CreateText(dataPath))
            {
                writer.WriteLine(Math.Round(caisse, 2).ToString());
                writer.WriteLine(Math.Round(banque, 2).ToString());
            }
        }
    }
}
