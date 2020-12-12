using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MASMA_Project
{
    public class Utils
    {
        public static int Delay = 100;
        public static Random RandNoGen = new Random();
        public const string delimiter = ">|#|>";   // string care delimiteaza indexul de text 
        public const int publicKey = 3;             // cheia utilizata la criptare
        public static List<string> processorAgents = new List<string> { "processor1", "processor2", "processor3", "processor4", "processor5" }; // lista cu numele obiectelor de tip ProcessorAgent implicate in procesul de criptare
        public const string dispatcher = "dispatcher";      // numele obiectului de tip DispatcherAgent implicat in procesul de criptare
        public const string distributor = "distributor";    // numele obiectului de tip DistributorAgent implicat in procesul de criptare
        public const int helperLimit = 1000;

        // metoda care returneaza caracterul ch criptat cu cheia key
        // criptarea consta in inlocuirea caracterelor de tip litere cu caracterul tip litera aflat la key pozitii distanta in codul ascii
        public static char cipher(char ch, int key)
        {
            // daca ch nu este litera, atunci ch ramane neschimbat
            if (!char.IsLetter(ch))
            {
                return ch;
            }
            // daca ch este litera atunci caracterul returnat este ch+key (ex: ch=B, key=3 => caraterul returnat este E
            char d = char.IsUpper(ch) ? 'A' : 'a';
            return (char)((((ch + key) - d) % 26) + d);


        }

        // metoda utilizata la criptarea textului dat ca parametru cu cheia key
        public static string Encipher(string input, int key)
        {
            Console.WriteLine("Encrypting ...");
            string output = string.Empty;

            foreach (char ch in input)
                output += cipher(ch, key);

            return output;
        }


        // metoda utilizata la decriptarea textului dat ca parametru cu cheia key
        public static string Decipher(string input, int key)
        {
            return Encipher(input, 26 - key);
        }
        
    }
}

