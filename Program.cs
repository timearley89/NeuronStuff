using System.Globalization;
using System.Security.Cryptography.X509Certificates;

internal class Program
{
    
    private static void Main(string[] args)
    {
        int spawnMax = 50;
        List<Neuron> spawnlist = new List<Neuron>();
        double x = 0.1;
        while (x < spawnMax)
        {
            spawnlist.Add(new Neuron(x, spawnlist));
            x = x * 2.1;
        }
        foreach (Neuron spawned in spawnlist)
        {
            spawned.knownNeurons = spawnlist;
        }

        Neuron alpha = new Neuron(100, spawnlist);
        
        
        bool Stop = false;
        int counter = 0;
        while (!Stop)
        {
            alpha.Fire(alpha.knownNeurons[counter]);
            counter++;
            if (counter > alpha.knownNeurons.Count - 1) { counter = 0; }
            if (Console.ReadLine() == "Stop") { Stop = true; }
        }
        

    }
}


//now one could create the first copya class, the instance constructor, and copya could then copy itself to a new instance. 
public class Neuron
{
    public double value;
    public List<Neuron> knownNeurons;
    public bool ismemory;
   public Neuron(Neuron caller) //created by Neuron
    {
        Console.WriteLine(this.value.ToString() + " created by " + caller.value.ToString());
        this.value = caller.value;
        this.knownNeurons = new List<Neuron>();

        ismemory = true;
        //this.Fire(caller);
    }
    public Neuron(double value, List<Neuron> spawnlist) //created by human(me) or input
    {
        Console.WriteLine(value + " Spawned");
        this.value = value;
        this.knownNeurons = new List<Neuron>();
        foreach (Neuron n in spawnlist)
        {
            this.knownNeurons.Add(n);
            knownNeurons.Last().ismemory = true;
        }
        
        //it knows about itself
        
        ismemory = false;
        
    }
    public void CheckKnown(Neuron caller) //fired
    {
        Thread.Sleep(10);
        Console.WriteLine(this.value + " Fired By " + caller.value);

        if (caller.ismemory)
        {//this is a memory responding to me
            this.knownNeurons.Add(caller);
            this.Fire(this.knownNeurons[this.knownNeurons.IndexOf(this.knownNeurons.Last())]);
            //calls the second to last one in it's list?

        }
        else
        {//this is a real input


            double myvalue = this.value;
            double callervalue = caller.value;
            //if one is an exact match, fire it. if not, create one matching the calling one.
            int index = -1;
            int knowncount = 0;
            int callssincelast = 0;
            for (int i = (knownNeurons.Count - 1); i >= 0; i--) //memory stack
            {
                if (knownNeurons[i].value == callervalue)
                {
                    knowncount++;
                    if (callssincelast == 0) { callssincelast = (knownNeurons.Count - 1) - i; }
                }
            }
            if (knowncount == -1)
            { //didn't find any at all, first time this neuron has ever called me
              //i need to store this "encounter", then call it back.

                this.knownNeurons.Add(caller);
                this.knownNeurons.Last().ismemory = true;
                //this.Fire(caller);

            }
            else
            {//it's called me before. Let's see how much. 
                double temporalmatch = 0;
                temporalmatch = knowncount / this.knownNeurons.Count; //memory stack
                                                                      //temporalmatch is a percentage of how present the current value has been during this neuron's life. 
                double callssincelastpercent = callssincelast / this.knownNeurons.Count;
                //callssincelastpercent is a percentage of when in this neuron's life this happened last. Low percentage means it just happened, high percentage means long ago.
                //call back unles it's just called and it's been noisy
                //callbacksincelastpercent -- how long ago
                //temporalmatch -- how often
                if (callssincelastpercent > 0.8 && temporalmatch < 0.2) { this.Fire(caller); this.knownNeurons.Add(caller); this.knownNeurons.Last().ismemory = true; }
                if (callssincelastpercent > 0.8 && temporalmatch > 0.8) { this.Fire(caller); this.knownNeurons.Add(caller); this.knownNeurons.Last().ismemory = true; }
                if (callssincelastpercent < 0.2 && temporalmatch < 0.2) { this.Fire(caller); this.knownNeurons.Add(caller); this.knownNeurons.Last().ismemory = true; }
                //It's hitting the call above repeatedly
                else
                {//it just called and it's been noisy. Don't fire back, but log this.
                    this.knownNeurons.Add(caller);
                    this.knownNeurons.Last().ismemory = true;
                }
            }
        }
        
    }
    public void Fire(Neuron calling)
    {
        
        //
        //
        //
        Console.WriteLine("Firing " + calling.value.ToString());
        calling.CheckKnown(this); //this neuron fires it's value to that neuron
    }
    
}
    
