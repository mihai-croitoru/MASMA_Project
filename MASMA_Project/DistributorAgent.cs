using ControlzEx.Standard;
using EO.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActressMas;

namespace MASMA_Project
{
    class DistributorAgent : TurnBasedAgent
    {
        int countp = 0;     // variabila ce retine numarul de ProcessorAgent
        Dictionary<int, string> encryptedBlocks = new Dictionary<int, string> { };  // dictionar ce retine ca cheie indexul de pornire a unui substring in textul initial, iar ca valoare are substringul
        string initialText;     // variabila ce retine textul initial, citit din fisier
        string encryptedText;   // variabila ce retine textul criptat pentur a fi scris in fisier
        int initialTextLength;  // variabiala ce retine lungimea textului initial
        int encryptedTextLength;    //variabila ce retine lungimea textului criptat
     


        public override void Setup()
        {
            
            string splittext = null;
            Console.WriteLine("[{0}]: Sending text to participants ...", this.Name);
            
            this.countp = Utils.processorAgents.Count;     
            
            this.initialText = ReadFromFile("../../Input.txt"); // se citeste fisierul si se retine textul in variabila initialText
            this.initialTextLength = this.initialText.Length;   // se calculeaza lungimea textului initial
            int standardSize = initialTextLength / this.countp; // se calculeaza impartirea numarului de caractere la numarul de ProcessorAgent
            int index = 0;      // variabila pivot care retine indexul de start al textului nedistribuit agentilor
            // se distribuie fiecarui agent un text de lungime aleatoare
            for (int i = 0; i < this.countp; ++i)
            {
                // se alege random un numar cuprins intre jumatate din standardSize (pentru a nu fi prea mic) si standardSize + standardSize/numarul de agenti+1 (pentru a nu exista agenti carora sa nu li se atribuie un subtext)
                int randomSize = Utils.RandNoGen.Next(standardSize / 2, standardSize + (standardSize / (this.countp + 1)));
                // daca a mai ramas un singur agent caruia nu i-a fost atribuit un subtext, acesta primeste tot textul ramas
                if (i == (this.countp - 1))
                {
                    splittext = initialText.Substring(index);
                }
                // altfel se ia subtextul de lungime random incepand de la pivot
                else
                {
                    splittext = initialText.Substring(index, randomSize);
                }
                // la inceputul textului de criptat se adauga pivotul + delimitatorul pentru ca textul sa poata fi reconstruit in ordinea corecta
                this.Send(Utils.processorAgents[i], index.ToString() + Utils.delimiter + splittext);
                Console.WriteLine("Sending text to the proccesor[{0}] agent.", i+1);
                index += splittext.Length;  // pivotul este incrementat cu lungimea textului trimis
            }
           // createDispatcherAgent();

        }

        public override void Act(Queue<Message> messages)
        {
            
            while (messages.Count > 0)
            {
                // mesajele primite sunt procesate si stocate in dictionar
                Message message = messages.Dequeue();
                int index;
                string text;
                this.parseMessage(message.Content, out index, out text);
                if (index >= 0)
                {
                    encryptedBlocks.Add(index, text);
                }

            }
            // periodic, cand coada de mesaje se goleste, se incearca construirea textului criptat
            this.encryptedText = this.getEncryptedText();
            this.encryptedTextLength = this.encryptedText.Length;
            // daca lungimea textului initial este egala cu lungimea textului criptat inseamna ca nu se mai asteapta mesaje
            if(this.initialTextLength == this.encryptedTextLength)
            {   Console.WriteLine("The size of encrypted text is equl to the size of innitial text !");
                this.WriteToFile("../../Output.txt", this.encryptedText);   // textul e scris in fisier
                this.Send(Utils.dispatcher, "stop");    // DispatcherAgent este notifict ca se poate opri
                this.Stop();        
            }
            // altfel steapta pana primeste toate mesajele
            else
            {
                
             //   this.distrAgentEnv.Add(new DispatcherAgent(), "dispatcherAgent1");
                 
                Console.WriteLine("[{0}] : Waiting for messages ...", this.Name);
            }

        }

        // metoda utilizata la extragerea indexului si a textului din mesajul primit
        private void parseMessage(string msg, out int index, out string text)
        {
            index = 0;
            text = "";
            string[] myString = msg.Split(new string[] { Utils.delimiter }, StringSplitOptions.None);
            if (Int32.TryParse(myString[0], out index))
            {
                text = myString[1];
            }
            else
            {
                text = "String could not be parsed.";
                index = -1;
            }
        }

        // metoda utilizata la construirea, in ordine, a textului criptat
        private string getEncryptedText()
        {
            List<int> keys = new List<int>(this.encryptedBlocks.Keys); // se preiau cheile(care reprezinta indexul de start textului) din dictionar
            keys.Sort();        // se sorteaza pentru a construi textul in ordinea corecta
            string enchriptedText = "";
            foreach (int key in keys)
            {
                enchriptedText += this.encryptedBlocks[key];        //se construieste textul in ordinea crescatoare a cheilor
            }
            return enchriptedText;
        }

        // metoda pentru citirea din fisier
        public static String ReadFromFile(string filename)
        {
           
            TextReader tr = new StreamReader(@filename);
            string text = tr.ReadToEnd();
            tr.Close();

            return text;
        }

        // metoda pentru scrierea in fisier
        public void WriteToFile(string filename, string text)
        {

            Console.WriteLine("Writing content to the {0} file.", filename);
            TextWriter tw = new StreamWriter(@filename);
            tw.WriteLine(text);
            tw.Close();

        }

  

    }
}
