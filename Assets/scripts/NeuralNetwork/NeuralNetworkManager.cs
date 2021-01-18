using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

namespace NeuralNetwork
{
    public class NeuralNetworkManager : MonoBehaviour
    {
        public const float BirdSpawnX = 0.5f;

        [ReadOnly] [SerializeField] private int generation;
        [ReadOnly] [SerializeField] private float bestScore;
        [ReadOnly] [SerializeField] private float bestScorePrev;
        [ReadOnly] [SerializeField] private float bestScoreNow;

        public int populationSize;
        public GameObject prefab;
        [SerializeField] private GameMain gameMain;

        [Range(0.0001f, 1f)] public float mutationChance = 0.01f;
        [Range(0f, 1f)] public float mutationStrength = 0.5f;
        [Range(0.1f, 10f)] public float gameSpeed = 1f;

        private readonly int[] _layers = {4, 8, 8, 1}; //initializing network to the right size

        private int _deadCount;

        [SerializeField] private List<NeuralNetwork> networks;
        private List<BirdControl> _birds;


        private void Start()
        {
            InitNetworks();
            CreateBirdControls();
        }

        public void InitNetworks()
        {
            networks = new List<NeuralNetwork>();
            for (var i = 0; i < populationSize; i++)
            {
                var network = new NeuralNetwork(_layers);
                network.Load("Assets/NN_Model_Best.txt");
                networks.Add(network);
            }
        }

        public void CreateBirdControls()
        {
            ++generation;
            _deadCount = 0;
            bestScorePrev = bestScoreNow;
            Time.timeScale = gameSpeed;

            if (_birds != null)
            {
                _birds.ForEach(bird => Destroy(bird.gameObject));

                SortNetworks(); //this sorts networks and mutates them
            }

            _birds = new List<BirdControl>();
            for (var i = 0; i < populationSize; i++)
            {
                var bird = Instantiate(prefab, new Vector3(BirdSpawnX, 3.5f, 0), Quaternion.identity)
                    .GetComponent<BirdControl>();
                bird.network = networks[i];
                bird.networkManager = this;
                bird.pipeSpawner = gameMain.pipeSpawner.GetComponent<PipeSpawner>();
                _birds.Add(bird);
            }

            gameMain.StartGame();
        }

        public void SortNetworks()
        {
            networks.Sort();
            networks.Reverse();
            if (networks[0].fitness >= bestScore)
            {
                networks[0].Save("Assets/NN_Model.txt");
            }

            var topNetworks = networks.GetRange(0, 3);
            if (topNetworks[0].fitness - topNetworks[1].fitness > 3
                && topNetworks[0].fitness - topNetworks[2].fitness > 3)
            {
                topNetworks.RemoveRange(1, 2);
            }

            var networkHalfCount = networks.Count / 2;
            var topHalfNetworks = networks.GetRange(0, networkHalfCount);
            for (var i = 0; i < networks.Count / 2; i++)
            {
                networks[i] = topNetworks[i % topNetworks.Count].Copy(new NeuralNetwork(_layers));
                networks[i].Mutate((int) (1 / mutationChance), mutationStrength);
            }

            for (var i = networkHalfCount; i < networkHalfCount * 1.25; i++)
            {
                networks[i] = topHalfNetworks[i % topHalfNetworks.Count].Copy(new NeuralNetwork(_layers));
            }

            for (var i = (int) (networkHalfCount * 1.25); i < networks.Count; i++)
            {
                networks[i] = topHalfNetworks[i % topHalfNetworks.Count].Copy(new NeuralNetwork(_layers));
                networks[i].Mutate((int) (1 / mutationChance), mutationStrength);
            }

            if (networks.Count < populationSize)
            {
                for (var i = networks.Count; i < populationSize; i++)
                {
                    networks.Add(topNetworks[i % topNetworks.Count].Copy(new NeuralNetwork(_layers)));
                }
            }
            else if (networks.Count > populationSize)
            {
                networks.RemoveRange(populationSize - 1, populationSize - networks.Count);
            }
        }


        private void FixedUpdate()
        {
            bestScoreNow = networks.Aggregate(0f, (acc, network) => network.fitness > acc ? network.fitness : acc);
            if (bestScoreNow > bestScore)
            {
                gameMain.scoreManager.SetScore((int) bestScoreNow);
                bestScore = bestScoreNow;
            }
        }


        public void IncreaseDead()
        {
            ++_deadCount;
            if (_deadCount >= networks.Count)
            {
                CreateBirdControls();
            }
        }
    }
}