using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace njGeneticAlgorithms
{
    public delegate void ComputeFitness(POPULATION WhichOne);
    public delegate void ComputeRandomChromosomes(POPULATION WhichOne, int Id);
    public delegate void ComputeRandomCell(POPULATION WhichOne, int Id, int Row, int Col);

    public enum CROSSOVER
    {
        CROSSOVER_ONE_POINT,
        CROSSOVER_TWO_POINT,
        CROSSOVER_UNIFORM,
        CROSSOVER_NONE
    }

    public enum POPULATION
    {
        CURRPOP, NEWPOP
    }

    public class Fitness { public int Value; public int Index; public Fitness(int value = 0, int index = 0) { Value = value; Index = index; } }

    public class GeneticAlgorithms
    {

        public ComputeFitness ComputeFitness;
        public ComputeRandomChromosomes ComputeRandomChromosomes;
        public ComputeRandomCell ComputeRandomCell;
        public event EventHandler FinishAlgorithms;

        public int popSize;
        public int ChromosomesCol;
        public int ChromosomesRow;

        public CROSSOVER CrossOver { set; get; }

        public object[] pop;
        public object[] newpop;

        public int[] fitness;
        public int[] newfitness;

        public Fitness bestFitness;
        public Fitness WorstFitness;

        private Random rnd;

        public GeneticAlgorithms(int Chromosomesrow, int Chromosomescol, int popsize = 80, CROSSOVER crossover = CROSSOVER.CROSSOVER_TWO_POINT)
        {
            popSize = popsize;
            ChromosomesCol = Chromosomescol;
            ChromosomesRow = Chromosomesrow;
            CrossOver = crossover;
            pop = new object[popSize];
            newpop = new object[popSize];
            fitness = new int[popSize];
            newfitness = new int[popSize];
            bestFitness = new Fitness();
            WorstFitness = new Fitness();
            rnd = new Random();
        }

        private void InitialPopulation()
        {
            for (int i = 0; i < popSize; i++)
            {
                var Chromosome = new int[ChromosomesRow, ChromosomesCol];
                pop[i] = Chromosome;
                ComputeRandomChromosomes(POPULATION.CURRPOP, i);
            }
        }

        private void FindBestChromosomes(POPULATION WhichOne)
        {
            bestFitness.Value = (WhichOne == POPULATION.CURRPOP ? fitness[0] : newfitness[0]);
            bestFitness.Index = 0;

            for (int i = 1; i < popSize; i++)
                if (bestFitness.Value > (WhichOne == POPULATION.CURRPOP ? fitness[i] : newfitness[i]))
                {
                    bestFitness.Value = (WhichOne == POPULATION.CURRPOP ? fitness[i] : newfitness[i]);
                    bestFitness.Index = i;
                }
        }

        private void FindWorstChromosomes(POPULATION WhichOne)
        {
            WorstFitness.Value = (WhichOne == POPULATION.CURRPOP ? fitness[0] : newfitness[0]);
            WorstFitness.Index = 0;
            for (int i = 1; i < popSize; i++)
                if (WorstFitness.Value < (WhichOne == POPULATION.CURRPOP ? fitness[i] : newfitness[i]))
                {
                    WorstFitness.Value = (WhichOne == POPULATION.CURRPOP ? fitness[i] : newfitness[i]);
                    WorstFitness.Index = i;
                }
        }

        private int doCrossOver(int index, int index1, int index2)
        {
            int[,] chrom1 = new int[ChromosomesRow, ChromosomesCol];
            int[,] chrom2 = new int[ChromosomesRow, ChromosomesCol];
            int[,] Chromosome1 = (int[,])pop[index1];
            int[,] Chromosome2 = (int[,])pop[index2];

            int ColCombinePoint1, ColCombinePoint2;
            const double Possibility = 0.5;

            switch (CrossOver)
            {
                case CROSSOVER.CROSSOVER_ONE_POINT:

                    ColCombinePoint1 = rnd.Next(1, ChromosomesCol - 1);

                    for (int i = 0; i < ChromosomesRow; i++)
                    {
                        for (int j = 0; j < ColCombinePoint1; j++)
                        {
                            chrom1[i, j] = Chromosome1[i, j];
                            chrom2[i, j] = Chromosome2[i, j];
                        }

                        for (int j = ColCombinePoint1; j < ChromosomesCol; j++)
                        {
                            chrom1[i, j] = Chromosome2[i, j];
                            chrom2[i, j] = Chromosome1[i, j];
                        }
                    }

                    break;
                case CROSSOVER.CROSSOVER_TWO_POINT:

                    ColCombinePoint1 = rnd.Next(0, ChromosomesCol);
                    ColCombinePoint2 = rnd.Next(0, ChromosomesCol);

                    int temp;
                    if (ColCombinePoint1 > ColCombinePoint2)
                    {
                        temp = ColCombinePoint1;
                        ColCombinePoint1 = ColCombinePoint2;
                        ColCombinePoint2 = temp;
                    }

                    for (int i = 0; i < ChromosomesRow; i++)
                    {

                        for (int j = 0; j < ColCombinePoint1; j++)
                        {
                            chrom1[i, j] = Chromosome1[i, j];
                            chrom2[i, j] = Chromosome2[i, j];
                        }

                        for (int j = ColCombinePoint1; j < ColCombinePoint2; j++)
                        {
                            chrom1[i, j] = Chromosome2[i, j];
                            chrom2[i, j] = Chromosome1[i, j];
                        }

                        for (int j = ColCombinePoint2; j < ChromosomesCol; j++)
                        {
                            chrom1[i, j] = Chromosome1[i, j];
                            chrom2[i, j] = Chromosome2[i, j];
                        }
                    }

                    break;
                case CROSSOVER.CROSSOVER_UNIFORM:
                    for (int i = 0; i < ChromosomesRow; i++)
                        for (int j = 0; j < ChromosomesCol; j++)
                        {
                            double p = rnd.NextDouble();
                            if (p < Possibility)
                            {
                                chrom1[i, j] = Chromosome1[i, j];
                                chrom2[i, j] = Chromosome2[i, j];
                            }
                            else
                            {
                                chrom1[i, j] = Chromosome2[i, j];
                                chrom2[i, j] = Chromosome1[i, j];
                            }
                        }
                    break;
            }

            newpop[index++] = chrom1;
            newpop[index++] = chrom2;
            return index;
        }

        private void Selection()
        {
            int index = 0, index1, index2;
            for (int i = 0; i < popSize / 2; i++)
            {
                index1 = rnd.Next(0, popSize);
                index2 = rnd.Next(0, popSize);

                while (index1 == index2)
                    index2 = rnd.Next(0, popSize);

                index = this.doCrossOver(index, index1, index2);
            }
        }

        private void Mutation()
        {
            int ColPoint, RowPoint;
            const double PossibilityMutation = 0.3;

            for (int i = 0; i < popSize; i++)
                if (rnd.NextDouble() < PossibilityMutation)
                {
                    int[,] Chromosome = (int[,])newpop[i];

                    for (RowPoint = 0; RowPoint < ChromosomesRow; RowPoint++)
                    {
                        ColPoint = rnd.Next(0, ChromosomesCol);
                        ComputeRandomCell(POPULATION.NEWPOP, i, RowPoint, ColPoint);
                    }
                }
        }

        private void ReplaceWorstChromosome()
        {
            this.FindWorstChromosomes(POPULATION.NEWPOP);
            this.FindBestChromosomes(POPULATION.CURRPOP);
            //انتقال بهترین کروموزوم از نسل قبلی به نسل جدید
            // Replace worst chromosome of newpop by best chromosome of pop
            newpop[WorstFitness.Index] = pop[bestFitness.Index];
            newfitness[WorstFitness.Index] = fitness[bestFitness.Index];

        }

        private void Nextpopulation()
        {//نسل جدید را به عنوان نسل قعلی در نظر گرفته شود
            for (int i = 0; i < popSize; i++)
            {
                pop[i] = newpop[i];
                fitness[i] = newfitness[i];
            }
        }

        public void StartAlgorithms()
        {
            InitialPopulation();
            ComputeFitness(POPULATION.CURRPOP);
            FindBestChromosomes(POPULATION.CURRPOP);
            while (bestFitness.Value > 0)
            {
                Selection(); //Crossover
                Mutation();
                ComputeFitness(POPULATION.NEWPOP);
                ReplaceWorstChromosome();
                Nextpopulation();
                FindBestChromosomes(POPULATION.CURRPOP);
            }

            if (FinishAlgorithms != null) FinishAlgorithms(this, new EventArgs());

        }

        public int[,] Chromosomes(int Index)
        {
            return (int[,])pop[Index];
        }
    }
}
