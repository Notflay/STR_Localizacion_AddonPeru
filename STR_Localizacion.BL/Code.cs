using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace STR_Localizacion.BL
{
    public static class Code
    {

        private static string salt = "STRATCONSULTING";
        private static string saltnumbers =
            string.Join(string.Empty,
            string.Join(string.Empty, salt.Select(c => (int)c).Distinct()).Distinct());

        public static string[] generate(string companyDB, string addonID, string hardwarekey)
        {
            if (addonID.Length < 1)
                throw new System.ArgumentOutOfRangeException("El ID del AddOn debe tener al menos un digito");
            else if (Regex.Replace(hardwarekey, @"\D", "").Length < 1)
                throw new System.ArgumentOutOfRangeException("El Hardwarekey debe tener al menos un digito");

            bool primo(int number) =>
                Enumerable.Range(1, number).Where(v => number % v == 0).Count() == 2;

            var codes = (hardwarekey + addonID + companyDB + saltnumbers).Select(c => (int)c).ToList();
            var asciicode = Enumerable.Range(65, 26).Union(Enumerable.Range(48, 10)).ToList();

            string[] serial = new string[1000];
            for (int i = 0; i < serial.Length; i++)
            {
                asciicode = asciicode.OrderBy(n => n % (i + 1)).Reverse().OrderBy(n => primo(n)).ToList();

                codes.ForEach(item =>
                {
                    if (serial[i]?.Length > 4) return;
                    if (int.TryParse(((char)item).ToString(), out int result))
                        serial[i] += (char)asciicode[result];
                    else
                    {
                        int inx = asciicode.FindIndex(s => ((char)s).Equals((char)item));
                        asciicode = asciicode.OrderBy(n => n % (i + 1)).Reverse().OrderBy(n => primo(n)).ToList();
                        serial[i] += (char)asciicode[inx];
                    }
                });
            }
            serial = serial.Distinct().ToArray();

            var hash = codes.Distinct().OrderBy(n => n % codes.Count()).Select(c => (int)c).ToArray();
            var end = new string[hash.Length];
            for (int i = 0; i < hash.Length; i++)
            {
                asciicode = asciicode.OrderBy(n => n % (i + 1)).Reverse().OrderBy(n => primo(n)).ToList();
                var idx = asciicode.FindIndex(e => e.Equals((int)hash[i]));

                end[i] = idx != -1 ?
                serial[idx] :
                serial[(int)((System.Math.Abs(hash[i] * (idx + i)) / (i + 1)) / serial.Length)];
            }

            end = end.Distinct().ToArray();
            hash = string.Join(string.Empty, hash).Select(s => int.Parse(s.ToString())).Distinct().ToArray();
            var values = new string[hash.Length];
            for (int i = 0; i < hash.Length; i++)
                values[i] = end.ElementAt(hash[i]);
            return values.Take(5).ToArray();
        }

    }
}
