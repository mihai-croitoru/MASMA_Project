using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActressMas;


namespace MASMA_Project
{
    class ProcessorAgent : TurnBasedAgent
    {
        int myLimit;
        string excedText;

        public ProcessorAgent(int limit)
        {
            // limita specifica fiecarui agent
            this.myLimit = limit;
        }
        
        public override void Act(Queue<Message> messages)
        {

            int index;
            string text;
            while (messages.Count > 0)
            {
                Message message = messages.Dequeue();

                switch (message.Sender)
                {
                    // daca mesajul este primit de la distributor, acesta este parsat
                    // daca textul depaseste limita agentului, textul in exces este trimis la dispatcher
                    // textul criptat e trimis inapoi la distributor
                    case (Utils.distributor):
                        index = 0;
                        text = "";
                        Console.WriteLine("[{0}] : Received initial text from Distributor Agent !", this.Name);
                        this.parseMessage(message.Content, out index, out text);
                        Send(message.Sender, index.ToString() + Utils.delimiter + Utils.Encipher(text, Utils.publicKey));
                        Console.WriteLine("[{0}] : Sending encrypted text to Distributor Agent !", this.Name);
                        Console.WriteLine("**************************||**************************");
                        break;
                    // mesajul e primit de la dispatcher
                    case (Utils.dispatcher):
                        switch (message.Content)
                        {
                            // daca e "help": daca limita nu a ajuns la 0 atunci inseamna ca mai poate preluat task-uri suplimentare si trimite "yes", altfel trimite "no"
                            case "help":
                                Console.WriteLine("[{0}] Received help request from dispatcher!", this.Name);
                               
                                if (myLimit > 0)
                                {
                                    Send(message.Sender, "yes");
                                    Console.WriteLine("[{0}] : Can take additional task!", this.Name);
                                }
                                else
                                {
                                    Send(message.Sender, "no");
                                    Console.WriteLine("[{0}] : Cannot take additional task!", this.Name);
                                }
                                break;
                            // daca e "stop" inseamna ca poate opri procesul
                            case "stop":
                                Console.WriteLine("[{0}] Stop.", this.Name );
                                this.Stop();
                                break;
                            // altfel e text de criptat
                            // mesajul este parsat
                            // daca textul depaseste limita agentului, textul in exces este trimis la dispatcher
                            // textul criptat e trimis la distributor
                            default:
                                index = 0;
                                text = "";
                                this.parseMessage(message.Content, out index, out text);
                                Console.WriteLine("\n [{0}] : Received additional task from Dispatcher Agent !", this.Name);
                                Send(Utils.distributor, index.ToString() + Utils.delimiter + Utils.Encipher(text, Utils.publicKey));
                                Console.WriteLine("[{0}] : Sending encrypted content to Dispatcher Agent ! ", this.Name); Console.WriteLine("\n");
                                break;
                        }
                        break;
                }
            }
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
                // daca lungimea textului e mai mare decat limita: 
                // este pastrat o parte din text de lungimea limitei, limita devine 0, iar textul in exces e trimis la dispatcher
                if (myString[1].Length >= this.myLimit && this.myLimit > 0)
                {
                    text = myString[1].Substring(0, this.myLimit);
                    Console.WriteLine("[{0}] : Received text exceds the limit!", this.Name);
                    this.excedText = myString[1].Substring(this.myLimit);
                    Console.WriteLine("[{0}] : Retrieving part of text up until the limit!", this.Name);
                    int idx = initialIndex + text.Length;
                    
                    Send(Utils.dispatcher, idx.ToString() + Utils.delimiter + this.excedText);
                    Console.WriteLine("[{0}] : Sending the rest of text back to the dispatcher !", this.Name);
                    this.myLimit = 0;
                }
                // altfel textul este pastrat iar limita e decrementata cu lungimea textului
                else
                {
                    text = myString[1];
                    this.myLimit -= text.Length;
                }
            }
            else
                text = "String could not be parsed.";
        }


    }
}


