using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace NeuralNetwork
{
    public class NeuralNetwork : IComparable<NeuralNetwork>
    {
        private readonly int[] layers;
        private float[][] neurons;
        private float[][] biases;
        private float[][][] weights;

        public float fitness = 0;


        #region Initialization

        public NeuralNetwork(int[] layers)
        {
            this.layers = new int[layers.Length];
            layers.CopyTo(this.layers, 0);

            InitNeurons();
            InitBiases();
            InitWeights();
        }

        //create empty storage array for the neurons in the network.
        private void InitNeurons()
        {
            neurons = layers.Select(t => new float[t]).ToArray();
        }

        //initializes and populates array for the biases being held within the network.
        private void InitBiases()
        {
            var biasList = new List<float[]>();
            foreach (var t in layers)
            {
                var bias = new float[t];
                for (var j = 0; j < t; j++)
                {
                    bias[j] = UnityEngine.Random.Range(-0.5f, 0.5f);
                }

                biasList.Add(bias);
            }

            biases = biasList.ToArray();
        }

        //initializes random array for the weights being held in the network.
        private void InitWeights()
        {
            var weightsList = new List<float[][]>();
            for (var i = 1; i < layers.Length; i++)
            {
                var layerWeightsList = new List<float[]>();
                var neuronsInPreviousLayer = layers[i - 1];
                for (var j = 0; j < neurons[i].Length; j++)
                {
                    var neuronWeights = new float[neuronsInPreviousLayer];
                    for (var k = 0; k < neuronsInPreviousLayer; k++)
                    {
                        neuronWeights[k] = UnityEngine.Random.Range(-0.5f, 0.5f);
                    }

                    layerWeightsList.Add(neuronWeights);
                }

                weightsList.Add(layerWeightsList.ToArray());
            }

            weights = weightsList.ToArray();
        }

        #endregion

        #region Neural Algorithms

        //feed forward, inputs >==> outputs.
        public float[] FeedForward(float[] inputs)
        {
            for (var i = 0; i < inputs.Length; i++)
            {
                neurons[0][i] = inputs[i];
            }

            for (var i = 1; i < layers.Length; i++)
            {
                var layer = i - 1;
                for (var j = 0; j < neurons[i].Length; j++)
                {
                    var value = 0f;
                    for (var k = 0; k < neurons[layer].Length; k++)
                    {
                        value += weights[layer][j][k] * neurons[layer][k];
                    }

                    neurons[i][j] = Activate(value + biases[i][j]);
                }
            }

            return neurons[neurons.Length - 1];
        }

        private static float Activate(float value) => (float) Math.Tanh(value);

        //used as a simple mutation function for any genetic implementations.
        public void Mutate(int chance, float val)
        {
            foreach (var bias in biases)
            {
                for (var j = 0; j < bias.Length; j++)
                {
                    bias[j] = (UnityEngine.Random.Range(0f, chance) <= 5)
                        ? bias[j] += UnityEngine.Random.Range(-val, val)
                        : bias[j];
                }
            }

            foreach (var i in weights)
            {
                foreach (var j in i)
                {
                    for (var k = 0; k < j.Length; k++)
                    {
                        j[k] = (UnityEngine.Random.Range(0f, chance) <= 5)
                            ? j[k] += UnityEngine.Random.Range(-val, val)
                            : j[k];
                    }
                }
            }
        }

        #endregion


        #region Utilities

        //Comparing For NeuralNetworks performance.
        public int CompareTo(NeuralNetwork other)
        {
            if (other == null)
                return 1;
            if (Math.Abs(fitness - other.fitness) < 0.001f)
                return 0;
            return (int) Mathf.Sign(fitness - other.fitness);
        }

        public NeuralNetwork Copy(NeuralNetwork nn) //For creatinga deep copy, to ensure arrays are serialzed.
        {
            for (var i = 0; i < biases.Length; i++)
            {
                for (var j = 0; j < biases[i].Length; j++)
                {
                    nn.biases[i][j] = biases[i][j];
                }
            }

            for (var i = 0; i < weights.Length; i++)
            {
                for (var j = 0; j < weights[i].Length; j++)
                {
                    for (var k = 0; k < weights[i][j].Length; k++)
                    {
                        nn.weights[i][j][k] = weights[i][j][k];
                    }
                }
            }

            return nn;
        }

        public void Save(string path) //this is used for saving the biases and weights within the network to a file.
        {
            File.Create(path).Close();
            var writer = new StreamWriter(path, true);

            foreach (var bias in biases)
            {
                foreach (var t1 in bias)
                {
                    writer.WriteLine(t1);
                }
            }

            foreach (var weight in weights)
            {
                foreach (var t1 in weight)
                {
                    foreach (var t2 in t1)
                    {
                        writer.WriteLine(t2);
                    }
                }
            }

            writer.Close();
        }

        //this loads the biases and weights from within a file into the neural network.
        public void Load(string path)
        {
            if (!File.Exists(path)) return;

            TextReader tr = new StreamReader(path);
            var numberOfLines = (int) new FileInfo(path).Length;
            var listLines = new string[numberOfLines];
            var index = 1;
            for (var i = 1; i < numberOfLines; i++)
            {
                listLines[i] = tr.ReadLine();
            }

            tr.Close();
            if (new FileInfo(path).Length <= 0) return;
            {
                foreach (var t in biases)
                {
                    for (var j = 0; j < t.Length; j++)
                    {
                        t[j] = float.Parse(listLines[index]);
                        index++;
                    }
                }

                foreach (var i in weights)
                {
                    foreach (var j in i)
                    {
                        for (var k = 0; k < j.Length; k++)
                        {
                            j[k] = float.Parse(listLines[index]);
                            index++;
                        }
                    }
                }
            }
        }

        #endregion
    }
}