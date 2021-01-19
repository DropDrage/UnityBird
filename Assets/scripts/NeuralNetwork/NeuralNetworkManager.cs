using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

namespace NeuralNetwork
{
    public class NeuralNetworkManager : MonoBehaviour
    {
        public const float BirdSpawnX = 0.5f;
        private const float PiDiv3 = Mathf.PI / 2.95f;
        private const float MinDynamicParam = 0.05f;

        [ReadOnly] [SerializeField] private int generation;
        [ReadOnly] [SerializeField] private int bestGeneration;
        [ReadOnly] [SerializeField] private int generation100;
        [ReadOnly] [SerializeField] private float bestScore;
        [ReadOnly] [SerializeField] private float bestScorePrev;
        [ReadOnly] [SerializeField] private float bestScoreNow;

        [ReadOnly] [SerializeField] private float dynamicMutationChance;
        [ReadOnly] [SerializeField] private float dynamicMutationStrength;

        [Space] public int populationSize;
        public GameObject prefab;
        [SerializeField] private GameMain gameMain;

        [Range(0.0001f, 1f)] public float mutationChance = 0.25f;
        [Range(0f, 1f)] public float mutationStrength = 0.5f;
        [Range(0.1f, 10f)] public float gameSpeed = 4f;

        [SerializeField] private int targetScore = 500;

        private readonly int[] _layers = {4, 8, 8, 1}; //initializing network to the right size
        // Manual: 16 = 27, 29; 32 = 13, 64 = 23
        // Auto chance 0.1, strength 0.15: 4 = 62, 120; 64 = 94
        // Dynamic: 8 = 30, 47, 58; 12 = 67; 16 = 60;

        private int _deadCount;

        [SerializeField] private List<NeuralNetwork> networks;
        private List<BirdControl> _birds;


        private void Start()
        {
            dynamicMutationChance = mutationChance;
            dynamicMutationStrength = mutationStrength;

            InitNetworks();
            CreateBirdControls();
        }

        public void InitNetworks()
        {
            networks = new List<NeuralNetwork>();
            for (var i = 0; i < populationSize; i++)
            {
                var network = new NeuralNetwork(_layers);
                // network.Load("Assets/NN_Model_Best.txt");
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
                bird.pipeSpawner = gameMain.pipeSpawner;
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
                bestGeneration = generation - 1;
                if (bestScore > 100 && generation100 == 0)
                {
                    generation100 = generation - 1;
                }

                var targetDifference = (bestScore / targetScore - 1) * PiDiv3;
                var doubleDifference = targetDifference * targetDifference;
                dynamicMutationChance = mutationChance * Mathf.Sin(Mathf.Pow(doubleDifference, 2f) + MinDynamicParam);
                dynamicMutationStrength =
                    mutationStrength * Mathf.Sin(Mathf.Pow(doubleDifference, 3f) + MinDynamicParam);
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
                networks[i].Mutate((int) (1 / dynamicMutationChance), dynamicMutationStrength);
            }

            for (var i = networkHalfCount; i < networkHalfCount * 1.25; i++)
            {
                networks[i] = topHalfNetworks[i % topHalfNetworks.Count].Copy(new NeuralNetwork(_layers));
            }

            for (var i = (int) (networkHalfCount * 1.25); i < networks.Count; i++)
            {
                networks[i] = topHalfNetworks[i % topHalfNetworks.Count].Copy(new NeuralNetwork(_layers));
                networks[i].Mutate((int) (1 / dynamicMutationChance), dynamicMutationStrength);
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