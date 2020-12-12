using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace MASMA_Project
{
    class Program
    { 
        static void Main(string[] args)
        {
            
            var env = new ActressMas.TurnBasedEnvironment(0, 500, false);
            Console.WriteLine("Created new Turn-Based Environment !");
            var distributorAgent = new DistributorAgent(); env.Add(distributorAgent, "distributor");
            Console.WriteLine("Created new DistributorAgent .");
                   
            var processorAgent1 = new ProcessorAgent(1000); env.Add(processorAgent1, "processor1");
           Console.WriteLine("Created new ProcessorAgent : processor1.");            
            var processorAgent2 = new ProcessorAgent(5000); env.Add(processorAgent2, "processor2");
            Console.WriteLine("Created new ProcessorAgent : processor2.");
            var processorAgent3 = new ProcessorAgent(4000); env.Add(processorAgent3, "processor3");
            Console.WriteLine("Created new ProcessorAgent : processor3.");
            var processorAgent4 = new ProcessorAgent(3015); env.Add(processorAgent4, "processor4");
            Console.WriteLine("Created new ProcessorAgent : processor4.");
            var processorAgent5 = new ProcessorAgent(1545); env.Add(processorAgent5, "processor5");
            Console.WriteLine("Created new ProcessorAgent : processor5.");

            var dispatcherAgent = new DispatcherAgent(env); env.Add(dispatcherAgent, "dispatcher");
            Console.WriteLine("Created new DispatcherAgent.");
            env.Start();
            

        }
    }
}
