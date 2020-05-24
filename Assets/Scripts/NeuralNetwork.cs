using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class NeuralNetwork
{
    public UInt32[] Topology 
    {
        get
        {
            UInt32[] Result = new UInt32[Topo.Count];

            Topo.CopyTo(Result, 0);

            return Result;
        }
    }

    ReadOnlyCollection<UInt32> Topo; 
    Layer[] Layers; 

    Random Random;

    private class Layer 
    {
        private double[][] weights;
        private Random random; 

  
        public Layer(UInt32 InputCount, UInt32 OutputCount, Random random)
        {
            this.random = random;

            weights = new double[InputCount + 1][]; 

            for (int i = 0; i < weights.Length; i++)
                weights[i] = new double[OutputCount];


            for (int i = 0; i < weights.Length; i++)
                for (int j = 0; j < weights[i].Length; j++)
                    weights[i][j] = this.random.NextDouble() - 0.5f;
        }


        public Layer(Layer inhereited)
        {

            random = inhereited.random;

            weights = new double[inhereited.weights.Length][];

            for (int i = 0; i < weights.Length; i++)
                weights[i] = new double[inhereited.weights[0].Length];

            for (int i = 0; i < weights.Length; i++)
            {
                for (int j = 0; j < weights[i].Length; j++)
                {
                    weights[i][j] = inhereited.weights[i][j];
                }
            }
        }

        public double[] FeedForward(double[] Input)
        {

     
            double[] Output = new double[weights[0].Length];

            for (int i = 0; i < weights.Length; i++)
            {
                for (int j = 0; j < weights[i].Length; j++)
                {
                    if (i == weights.Length - 1) 
                        Output[j] += weights[i][j]; 
                    else
                        Output[j] += weights[i][j] * Input[i];
                }
            }

          
            for (int i = 0; i < Output.Length; i++)
                Output[i] = ReLU(Output[i]);

          
            return Output;
        }


        public void Mutate (double MutationProbablity, double MutationAmount)
        {
            for (int i = 0; i < weights.Length; i++)
            {
                for (int j = 0; j < weights[i].Length; j++)
                {
                    if (random.NextDouble() < MutationProbablity)
                        weights[i][j] = random.NextDouble() * (MutationAmount * 2) - MutationAmount;
                }
            }
        }

        private double ReLU(double x)
        {
            if (x >= 0)
                return x;
            else
                return x / 20;
        }
    }

    public NeuralNetwork (UInt32[] Topology, Int32? Seed = 0)
    {

        if (Seed.HasValue)
            Random = new Random(Seed.Value);
        else
            Random = new Random();

        Topo = new List<uint>(Topology).AsReadOnly();
        Layers = new Layer[Topo.Count - 1];

        for (int i = 0; i < Layers.Length; i++)
        {
            Layers[i] = new Layer(Topo[i], Topo[i + 1], Random);
        }
    }


    public NeuralNetwork (NeuralNetwork Main)
    {
        Random = new Random(Main.Random.Next());
 
        Topo = Main.Topo;

        Layers = new Layer[Topo.Count - 1];

        for (int i = 0; i < Layers.Length; i++)
        {
            Layers[i] = new Layer (Main.Layers[i]);
        }
    }


    public double[] FeedForward(double[] Input)
    {


        double[] Output = Input;

        for (int i = 0; i < Layers.Length; i++)
        {
            Output = Layers[i].FeedForward(Output);
        }

        return Output;
    }


    public void Mutate (double MutationProbablity = 0.2, double MutationAmount = 1.0)
    {
        for (int i = 0; i < Layers.Length; i++)
        {
            Layers[i].Mutate(MutationProbablity, MutationAmount);
        }
    }
}