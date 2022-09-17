using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace njGeneticAlgorithms
{
    public class EightPuzzle
    {
        public GeneticAlgorithms GeneticAlgorithms;
        public event EventHandler  FinishAlgorithms;
        private Random rnd;
        private Thread t;
        private int _ChromosomesCol = 3;
        private int _ChromosomesRow = 3;

        public int ChromosomesCol { get { return _ChromosomesCol; } }
        public int ChromosomesRow { get { return _ChromosomesRow; } }

        public CROSSOVER CrossOver { set { GeneticAlgorithms.CrossOver = value; } get { return GeneticAlgorithms.CrossOver; } }

        public EightPuzzle()
        {
            rnd = new Random();
            GeneticAlgorithms = new GeneticAlgorithms(ChromosomesRow, ChromosomesCol, 80, CROSSOVER.CROSSOVER_UNIFORM);
            GeneticAlgorithms.ComputeFitness = ComputeFitness;
            GeneticAlgorithms.ComputeRandomChromosomes = ComputeRandomChromosomes;
            GeneticAlgorithms.ComputeRandomCell = ComputeRandomCell;
            GeneticAlgorithms.FinishAlgorithms += _FinishAlgorithms;
        }

        public int[,] Result
        {
            get
            {
                return (int[,])GeneticAlgorithms.pop[GeneticAlgorithms.bestFitness.Index];
            }
        }

        public void StartAlgorithms()
        {
            ThreadStart ts = new ThreadStart(GeneticAlgorithms.StartAlgorithms);
              t = new Thread(ts);
            t.Start();
        }
        public void AbortAlgorithms()
        {
         if(t!=null)
            t.Abort();
        }

        private void ComputeFitness(POPULATION WhichOne)
        {
            for (int r = 0; r < GeneticAlgorithms.popSize; r++)
            {
                int[,] Chromosome = (int[,])(WhichOne == POPULATION.CURRPOP ? GeneticAlgorithms.pop[r] : GeneticAlgorithms.newpop[r]);

                if (WhichOne == POPULATION.CURRPOP) GeneticAlgorithms.fitness[r] = 0; else GeneticAlgorithms.newfitness[r] = 0;

                int row1, col1, row2, col2;
                int rowcol=GeneticAlgorithms.ChromosomesRow * GeneticAlgorithms.ChromosomesCol;

                for (int i = 0; i < rowcol; i++)
                {
                    row1 = i / GeneticAlgorithms.ChromosomesRow;
                    col1 = i % GeneticAlgorithms.ChromosomesCol;

                    for (int j = i + 1; j < rowcol; j++)
                    {
                        row2 = j / GeneticAlgorithms.ChromosomesRow;
                        col2 = j % GeneticAlgorithms.ChromosomesCol;

                        if (Chromosome[row1, col1] == Chromosome[row2, col2])
                            if (WhichOne == POPULATION.CURRPOP) GeneticAlgorithms.fitness[r]++; else GeneticAlgorithms.newfitness[r]++;
                    }
                }

                int sum;

                for (int x = 0; x < GeneticAlgorithms.ChromosomesRow; x++)
                {
                    // calculate rows  
                    // - - -
                    // - - -
                    // - - -
                    sum = Chromosome[x, 0] + Chromosome[x, 1] + Chromosome[x, 2];
                    if (sum != 15)
                        if (WhichOne == POPULATION.CURRPOP) GeneticAlgorithms.fitness[r]++; else GeneticAlgorithms.newfitness[r]++;

                    // calculate cols  
                    // | | |
                    // | | |
                    // | | |
                    sum = Chromosome[0, x] + Chromosome[1, x] + Chromosome[2, x];
                    if (sum != 15)
                        if (WhichOne == POPULATION.CURRPOP) GeneticAlgorithms.fitness[r]++; else GeneticAlgorithms.newfitness[r]++;
                }

                // calculate Diameter  
                // |   |
                //   |  
                // |   |
                sum = Chromosome[0, 0] + Chromosome[1, 1] + Chromosome[2, 2];
                if (sum != 15)
                    if (WhichOne == POPULATION.CURRPOP) GeneticAlgorithms.fitness[r]++; else GeneticAlgorithms.newfitness[r]++;
                sum = Chromosome[0, 2] + Chromosome[1, 1] + Chromosome[2, 0];
                if (sum != 15)
                    if (WhichOne == POPULATION.CURRPOP) GeneticAlgorithms.fitness[r]++; else GeneticAlgorithms.newfitness[r]++;

            }
        }

        public void ComputeRandomChromosomes(POPULATION WhichOne, int Id)
        {
            List<int> list = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var Chromosomes = (int[,])(WhichOne == POPULATION.CURRPOP ? GeneticAlgorithms.pop[Id] : GeneticAlgorithms.newpop[Id]);
            for (int k = 0; k < ChromosomesRow; k++)
            {
                for (int l = 0; l < ChromosomesCol; l++)
                {
                    int index = rnd.Next(0, list.Count - 1);
                    Chromosomes[k, l] = list[index];
                    list.RemoveAt(index);
                }
            }
        }

        public void ComputeRandomCell(POPULATION WhichOne, int Id, int Row, int Col)
        {
            List<int> list = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var Chromosomes = (int[,])(WhichOne == POPULATION.CURRPOP ? GeneticAlgorithms.pop[Id] : GeneticAlgorithms.newpop[Id]);

            for (int k = 0; k < GeneticAlgorithms.ChromosomesRow; k++)
                for (int l = 0; l < GeneticAlgorithms.ChromosomesCol; l++)
                    if (Row != k || Col != l)
                    {
                        list.Remove(Chromosomes[k, l]);
                    }
            int index = rnd.Next(0, list.Count - 1);
            if (list.Count == 1)
            {
                int i = rnd.Next(1, ChromosomesRow);
                int j = rnd.Next(1, ChromosomesCol);
                Chromosomes[Row, Col] = Chromosomes[i, j];
                Chromosomes[i, j] = list[index];
            }
            else
                Chromosomes[Row, Col] = list[index];
        }

        private void _FinishAlgorithms(object sender, EventArgs e)
        {
            if (FinishAlgorithms != null) FinishAlgorithms(this, new EventArgs());
        }
    }
}