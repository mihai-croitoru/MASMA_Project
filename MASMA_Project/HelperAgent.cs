using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActressMas;


namespace MASMA_Project
{
    class HelperAgent : TurnBasedAgent
    {

        int myLimit = Utils.helperLimit;        // limita e egala cu limita standard pentru HelperAgent
        string excedText;

        public override void Act(Queue<Message> messages)
        {

            int index;
            string text;
            while (messages.Count > 0)
            {
                Message message = messages.Dequeue();
                // preia mesajul de la dispatcher, il parseaza
                // daca textul depaseste limita agentului, textul in exces este trimis la dispatcher
                // textul criptat e trimis la distributor
                if (message.Sender == Utils.dispatcher)
                {
                    index = 0;
                    text = "";
                    this.parseMessage(message.Content, out index, out text);
                    Console.WriteLine("[{0}] : Received task from Dispatcher Agent !", this.Name);
                    Send(Utils.distributor, index.ToString() + Utils.delimiter + Utils.Encipher(text, Utils.publicKey));
                    Console.WriteLine("[{0}] : Sending encrypted content to Distributor Agent !", this.Name);
                }

            }
            this.Stop();
        }


        // metoda pentru preluarea din mesaj a indexului si a textului de criptat
        private void parseMessage(string msg, out int initialIndex, out string text)
        {
            initialIndex = 0;
            text = "";
            // sunt extrase din mesaj indexul si textul
            string[] myString = msg.Split(new string[] { Utils.delimiter }, StringSplitOptions.None);
            if (Int32.TryParse(myString[0], out initialIndex))
            {
                // daca lungimea textului e mai mare decat limita, este pastrat o parte din text de lungimea limitei, iar textul in exces e trimis la dispatcher
                if (myString[1].Length >= this.myLimit && this.myLimit > 0)
                {
                    text = myString[1].Substring(0, this.myLimit);
                    excedText = myString[1].Substring(this.myLimit);
                    int index = initialIndex + text.Length;
                    Console.WriteLine("[{0}] : Received text exceds the  proccessing capacity limit!", this.Name);
                    Send(Utils.dispatcher, index.ToString() + Utils.delimiter + excedText);
                    Console.WriteLine("[{0}] : Sending the exceded ammount back to the dispatcher!",this.Name);
                }
                // altfel textul este pastrat 
                else
                {   
                    text = myString[1];
                }
            }
            else
                text = "String could not be parsed.";
        }

    }
}
