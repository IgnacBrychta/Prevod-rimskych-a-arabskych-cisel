#define PodporaVelkychCisel
//#undef PodporaVelkychCisel

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RomanNumbers
{
    internal static class Program
    {
        static readonly Dictionary<int, char> prevodyProRimskaCisla = new Dictionary<int, char> {
            { 1000, 'M' },
            { 500, 'D' },
            { 100, 'C' },
            { 50, 'L' },
            { 10, 'X' },
            { 5, 'V' },
            { 1, 'I' }
        };
        static Dictionary<char, int> reverzniPrevodyProRimskaCisla = new Dictionary<char, int>();
        static bool running = true;
        static void Main(string[] args)
        {
            Config();
            while (running)
            {
                MainLoop();
            }
        }
        private static void MainLoop()
        {
            Console.WriteLine("Co chcete dělat?\n1. Převést arabské číslo na římské\n2.Převést římské číslo na arabské\n3.Vypsat převodovou tabulku\n4. Konec");
            switch (Console.ReadLine())
            {
                case "1":
                    Menu_PrevodArabskehoCislaNaRimske();
                    break;
               case "2":
                    Menu_PrevodRimskehoCislaNaArabske();
                    break;
                case "3":
                    VypisTabulky();
                    break;
                case "4":
                    running = false;
                    break;
                default:
                    break;
            }
        }
        private static void VypisTabulky()
        {
            foreach (var item in prevodyProRimskaCisla)
            {
                Console.WriteLine(item);
            }
        }
        private static void Menu_PrevodArabskehoCislaNaRimske()
        {
            Console.WriteLine("Vložte arabské číslo.");
            string vstup = Console.ReadLine().ToUpper();
            int rimskeCislo;
            rimskeCislo = ArabskeCisloNaRimske(vstup);
            Console.WriteLine(rimskeCislo);
        }

        private static void Menu_PrevodRimskehoCislaNaArabske()
        {
            Console.WriteLine("Vložte římské číslo.");
            int rimskeCislo;
            while (true)
            {
                string vstup = Console.ReadLine().ToUpper();
                if (int.TryParse(vstup, out rimskeCislo))
                {
                    break;
                } else
                {
                    Console.WriteLine("Neplatný vstup.");
                }
            }
            string arabskeCislo = RimskeCisloNaArabske(rimskeCislo);
            Console.WriteLine(arabskeCislo);
        }
        private static void Config()
        {
            foreach (var item in prevodyProRimskaCisla)
            {
                reverzniPrevodyProRimskaCisla.Add(item.Value, item.Key);
            }
        }
        private static void TestFunkcnosti()
        {
            for (int i = 1; i <= 1000; i++)
            {
                string arabskeCislo = RimskeCisloNaArabske(i);
                int prevedeneRimskeCislo = ArabskeCisloNaRimske(arabskeCislo);
                Console.WriteLine($"{i}: {arabskeCislo} | {prevedeneRimskeCislo}");
            }
        }
        static string RimskeCisloNaArabske(int rimskeCislo)
        {
#if !PodporaVelkychCisel
            if (rimskeCislo > 1999) { throw new ArgumentException("Čísla větší nebo rovna 2000 nejsou podporována."); }
#endif
            int[] rimskeCislo_pole = rimskeCislo
                .ToString()
                .Reverse()
                .Select(x => (int)char.GetNumericValue(x))
                .ToArray();
            List<char> arabskeCislo_vysledek = new List<char>();
            int[] prevodyProRimskaCisla_Klice = prevodyProRimskaCisla.Keys.ToArray();
            int NejvyssiVyssiCifra(int cislo)
            {
                int pocetCiferCisla = cislo.ToString().Length;
                int vyssiCislo = (int)Math.Pow(10, pocetCiferCisla);
                if (vyssiCislo/2 > cislo)
                {
                    return vyssiCislo/2;
                } else
                {
                    return vyssiCislo;
                }
            }
            int NejvyssiNizsiCifra(int cislo)
            {
                int lastItem = -1;
                foreach (var item in prevodyProRimskaCisla_Klice.Reverse())
                {
                    if (cislo < item)
                    {
                        return lastItem;
                    }
                    lastItem = item;
                }
                return -1;
            }
            for (int i = rimskeCislo_pole.Length - 1; i >= 0; i--)
            {
                int cifra_nasobek = rimskeCislo_pole[i] * (int)Math.Pow(10, i);
                if (cifra_nasobek == 0) { continue; }
#if PodporaVelkychCisel
                while (cifra_nasobek > 1999) // podpora velkých čísel
                {
                    arabskeCislo_vysledek.Add('M');
                    cifra_nasobek -= 1000;
                }
#endif
                if (prevodyProRimskaCisla.ContainsKey(cifra_nasobek)) // je s cifrou nutno cokoli dělat?
                {
                    arabskeCislo_vysledek.Add(prevodyProRimskaCisla[cifra_nasobek]);
                } else
                {
                    int nejvyssiVyssiCifra = NejvyssiVyssiCifra(cifra_nasobek);
                    int rozdil = nejvyssiVyssiCifra - cifra_nasobek;
                    if (prevodyProRimskaCisla.ContainsKey(rozdil)) // cifra = 4 * nizsiCifraa
                    {
                        arabskeCislo_vysledek.Add(prevodyProRimskaCisla[rozdil]);
                        arabskeCislo_vysledek.Add(prevodyProRimskaCisla[nejvyssiVyssiCifra]);
                    } else
                    {
                        int nejvyssiNizsiCifra = NejvyssiNizsiCifra(cifra_nasobek); // 80 => 50
                        arabskeCislo_vysledek.Add(prevodyProRimskaCisla[nejvyssiNizsiCifra]);
                        cifra_nasobek -= nejvyssiNizsiCifra; // 80 - 50 = 30
                        nejvyssiNizsiCifra = NejvyssiNizsiCifra(cifra_nasobek); // 30 => 10
                        while (cifra_nasobek > 0)
                        {
                            arabskeCislo_vysledek.Add(prevodyProRimskaCisla[nejvyssiNizsiCifra]);
                            cifra_nasobek -= nejvyssiNizsiCifra;
                        }
                    }
                }
            }
            return string.Join("", arabskeCislo_vysledek);
        }
        static int ArabskeCisloNaRimske(string arabskeCislo)
        {
            Regex regex = new Regex(@"^(?=[MDCLXVI])M*(C[MD]|D?C{0,3})(X[CL]|L?X{0,3})(I[XV]|V?I{0,3})$");
            if(!regex.IsMatch(arabskeCislo)) { throw new ArgumentException("Nebylo možné převést vložené arabské číslo na římské: neplatný tvar."); };

            char[] arabskeCislo_charArr = arabskeCislo
                .ToArray();
            int rimskeCislo = 0;
            for (int i = 0; i < arabskeCislo_charArr.Length; i++)
            {
                foreach (KeyValuePair<int, char> item in prevodyProRimskaCisla)
                {
                    char currenctChar = arabskeCislo[i];
                    char nextChar;
                    if (currenctChar == item.Value)
                    {
                        if (i + 1 < arabskeCislo.Length) // je iterace na posledním prvku?
                        {
                            nextChar = arabskeCislo[i + 1];

                            if (item.Key >= reverzniPrevodyProRimskaCisla[nextChar])
                            {
                                rimskeCislo += item.Key;
                            }
                            else
                            {
                                rimskeCislo -= item.Key; // příští iterace bude stejného prvku
                            }
                        } else
                        {
                            rimskeCislo += item.Key;
                        }
                        break;
                    }
                }
            }
            return rimskeCislo;
        }
    }
}