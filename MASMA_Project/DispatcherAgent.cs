using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActressMas;


namespace MASMA_Project
{
    class DispatcherAgent : TurnBasedAgent
    {
        List<string> excedTextList = new List<string> { };
        List<string> helperAgentList = new List<string> { };
        int helperCount = 0;
        ActressMas.TurnBasedEnvironment myEnv;
        // primeste un parametru de tipul ActressMas.TurnBasedEnvironment necesar la adaugarea de HelperAgent
        public DispatcherAgent(ActressMas.TurnBasedEnvironment env)
        {
            this.myEnv = env;
        }

        public override void Act(Queue<Message> messages)
        {

        
                while (messages.Count > 0)
            {
                Message message = messages.Dequeue();
                // daca primeste notificare "stop" de la DistributorAgent, procesul este oprit
                if (message.Sender == Utils.distributor && message.Content == "stop")
                    this.Stop();
                else
                {
                    switch (message.Content)
                    {
                        // notificare ca un ProcessorAgent mai pote prelua un subtext pentru criptare
                        case "yes":
                            // daca exista blocuri de text care nu au fost atribuite, trimite agentului un subtext
                            if (this.excedTextList.Count > 0)
                            {
                                this.Send(message.Sender, this.excedTextList[0]);
                                this.excedTextList.RemoveAt(0); //dupa ce a fost trimis, subtextul este sters din lista te asteptare
                            }
                            break;
                        // notificare ca un ProcessorAgent nu mai pote prelua alt subtext pentru criptare
                        case "no":
                            // verifica daca agentul mai exista in lista de agenti disponibil de a prelua task-uri suplimentare
                            if (Utils.processorAgents.Contains(message.Sender))
                            {
                                int idx = Utils.processorAgents.IndexOf(message.Sender);
                                Utils.processorAgents.RemoveAt(idx);    // agentul este sters din lista
                                this.Send(message.Sender, "stop");      // agentului i se trimite notificare ca poate opri procesul
                            }
                            break;
                        // text in exces trimis de un agent a carui limita a fost depasita
                        default:
                            excedTextList.Add(message.Content);     // textul e adaugat in lista de asteptare
                            // daca mai exista agenti in lista de agenti disponibili, acestia sunt informati ca exista task-uri suplimentare
                            if (Utils.processorAgents.Count > 0)
                            {
                                this.informProcessors();
                            }
                            break;
                    }
                }
               
            }
            // daca mai exista task-uri suplimentare, lista de mesaje e goala si nu mai exista agenti disponibili, se creaza agenti de tip HelperAgent
            if(messages.Count <= 0 && excedTextList.Count > 0 && Utils.processorAgents.Count <=0)
            {
                // se creaza cate un HelperAgent pentru fiecare text din lista de task-uri suplimentare si i se atribuie cate un text
                helperCount = excedTextList.Count;
              //  createEnvironement(myEnv);
                createHelperAgents(helperCount);
                for(int i = 0; i < helperCount; ++i)
                {
                    this.Send(helperAgentList[i], excedTextList[i]);
                }
                // dupa atribuire, lista de task-uri este golita
                excedTextList.Clear();
            }
        }

        private void informProcessors()
        {
            // se trimite mesaj "help" tuturor agentilor disponibili
            for (int i = 0; i < Utils.processorAgents.Count; ++i)
            {
                this.Send(Utils.processorAgents[i], "help");
                Console.WriteLine("[{0}] : Asking all processor agents for help !", this.Name);
            }

        }
     
        private void createHelperAgents( int number)
        {
            Console.WriteLine("Start creating HelperAgents");
            string name;
            for(int i = 0; i < number; ++i)
            {
                name = "helper" + i;
                
                this.myEnv.Add(new HelperAgent(), name);
                helperAgentList.Add(name);
                Console.WriteLine("[{0}] Created {1}",this.Name , name);
            }
        }

        
    }
}
