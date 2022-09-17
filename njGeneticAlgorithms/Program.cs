using System;

namespace njGeneticAlgorithms
{
    class Program
    {
        static EightPuzzle t;
        static bool running;
        static DateTime oldtime;

        static void Main(string[] args)
        {
            start:;

            t = new EightPuzzle();
            t.FinishAlgorithms += _FinishAlgorithms;
            Console.Clear();

            Console.WriteLine("Select a Cross Over Algorithms (1, 2, 3): ");
            Console.WriteLine("1." + CROSSOVER.CROSSOVER_ONE_POINT.ToString());
            Console.WriteLine("2." + CROSSOVER.CROSSOVER_TWO_POINT.ToString());
            Console.WriteLine("3." + CROSSOVER.CROSSOVER_UNIFORM.ToString());
            Console.WriteLine("4.EXIT");
            Console.WriteLine("");

            switch (Console.ReadKey().KeyChar)
            {
                case '1':
                    t.CrossOver = CROSSOVER.CROSSOVER_ONE_POINT;
                    break;
                case '2':
                    t.CrossOver = CROSSOVER.CROSSOVER_TWO_POINT;
                    break;
                case '3':
                    t.CrossOver = CROSSOVER.CROSSOVER_UNIFORM;
                    break;
                default:
                    return;
            }

            Console.WriteLine("Please wait...");

            oldtime = DateTime.Now;

            t.StartAlgorithms();

            running = true;

            while (running)
            {
                Console.Clear();

                Console.WriteLine("");
                Console.WriteLine("Algorithm starting with multi thread. Please select a option (1, 2, 3, ...): ");
                Console.WriteLine("1. Best Fitness");
                Console.WriteLine("2. Worst Fitness");
                Console.WriteLine("3. Display Best Chromosomes");
                Console.WriteLine("4. Display Worst Chromosomes");
                Console.WriteLine("5. Abort Algorithms");
                Console.WriteLine("");

                switch (Console.ReadKey().KeyChar)
                {
                    case '1':
                        Console.WriteLine("  Best Fitness Value = " + t.GeneticAlgorithms.bestFitness.Value.ToString());
                        break;
                    case '2':
                        Console.WriteLine("  Worst Fitness Value = " + t.GeneticAlgorithms.WorstFitness.Value.ToString());
                        break;
                    case '3':
                        Console.WriteLine("Best Chromosomes is > ");
                        var bChromosome = t.GeneticAlgorithms.Chromosomes(t.GeneticAlgorithms.bestFitness.Index);
                        for (int i = 0; i < t.ChromosomesRow; i++)
                        {
                            for (int j = 0; j < t.ChromosomesCol; j++)
                                Console.Write(bChromosome[i, j] + " ");
                            Console.WriteLine("");
                        }
                        break;
                    case '4':
                        Console.WriteLine("Worst Chromosomes is > ");
                        var wChromosome = t.GeneticAlgorithms.Chromosomes(t.GeneticAlgorithms.WorstFitness.Index);
                        for (int i = 0; i < t.ChromosomesRow; i++)
                        {
                            for (int j = 0; j < t.ChromosomesCol; j++)
                                Console.Write(wChromosome[i, j] + " ");
                            Console.WriteLine("");
                        }
                        break;
                    case '5':
                        t.AbortAlgorithms();
                        goto start;
                }
                Console.ReadKey();
            }

            goto start;
        }
        static void _FinishAlgorithms(object sender, EventArgs e)
        {
            running = false;

            var rem = DateTime.Now - oldtime;

            int[,] Chromosome = t.Result;

            Console.Clear();

            Console.WriteLine("Result is > ");

            for (int i = 0; i < t.ChromosomesRow; i++)
            {
                for (int j = 0; j < t.ChromosomesCol; j++)
                    Console.Write(Chromosome[i, j] + " ");
                Console.WriteLine("");
            }

            Console.WriteLine("");
            Console.WriteLine("Total Time by " + t.CrossOver.ToString() + " Algorithm");
            Console.WriteLine("              " + rem.ToString());
            Console.ReadKey();
        }
    }
}
