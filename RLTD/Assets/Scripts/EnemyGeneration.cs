using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGeneration : MonoBehaviour
{
    private WorldGeneration worldGen;
    
    public bool canSpawnBoss;
    
    [Range(5,30)]
    public int enemiesPerWave; //Quantos inimigos irao dar spawn por cada wave
    public int howManyEnemies; //Quantos inimigos vao dar spawn nesta wave
    [SerializeField] private int enemiesToSpawn; //Quantos inimigos faltam dar spawn


    [SerializeField] private List<GameObject> enemies = new List<GameObject>(); //Todos os inimigos do jogo (SEM BOSSES)
    [SerializeField] private List<GameObject> BossList = new List<GameObject>(); //Todos os bosses do jogo
    public List<GameObject> spawnedEnemies = new List<GameObject>();


    [Range(1, 4)]
    public int currentDifficulty;

    public bool isSpawning = false;
    public float spawnCooldown; //cooldown entre spawn de cada inimigo
    [SerializeField] private float spawnTime;

    void Start()
    {
        worldGen = GameObject.FindGameObjectWithTag("GridManager").GetComponent<WorldGeneration>();
        spawnTime = 0;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O) && !isSpawning) //Da spawn da wave
        {
            worldGen.UpdateNavMesh();
            if (worldGen.CurrentWave > 0)
            {
                isSpawning = true;
            }             
        }

        if (Input.GetKeyDown(KeyCode.K)) //"Mata" a current wave
        {
            for (int i = 0; i < spawnedEnemies.Count; i++)
            {
                Destroy(spawnedEnemies[i]);
            }
            enemiesToSpawn = 0;
            isSpawning = false;
            spawnedEnemies.Clear();
        }

        if (worldGen.CurrentWave != worldGen.MaxWave) //Para garantir que boss so da spawn na ultima ronda
        {
            canSpawnBoss = true;
        }

        if (isSpawning) //é mudado no script WorldGeneration
        {
            SpawnEnemies();
        }
        if (!isSpawning)
        {
            enemiesToSpawn = howManyEnemies;
        }

        spawnTime -= Time.deltaTime;
    }


    void SpawnEnemies() //da loop por todos os spawnpoints de modo a que haja um numero igual de inimigos por spawnPoint.
        //TO DO: Definir percentagem de chance de cada inimigo, tendo em conta as waves e a dificuldade
    {
        if (spawnTime <= 0)
        {
            for (int i = 0; i < worldGen.spawnPoints.Count; i++)
            {
                int whatEnemy = Random.Range(0, enemies.Count);
                GameObject enemy;
                enemy = Instantiate(enemies[whatEnemy], worldGen.spawnPoints[i].transform.position - new Vector3(0, worldGen.spawnPoints[i].transform.localScale.y / 2, 0), Quaternion.identity);
                spawnedEnemies.Add(enemy);
                enemiesToSpawn--;

                if (enemiesToSpawn > 0 && i == worldGen.spawnPoints.Count - 1)
                {
                    spawnTime = spawnCooldown;
                }
                else if (enemiesToSpawn == 0)
                {
                    isSpawning = false;
                    break;
                }
            }
        }

        if (worldGen.CurrentWave == worldGen.MaxWave && canSpawnBoss) //Spawn de boss
        {
            float distance = 0;
            int ChosenSpawnPoint = 0;
            for (int i = 0; i < worldGen.spawnPoints.Count; i++) //Garantir que o boss da spawn sempre no spawnPoint mais longe
            {
                if (Vector3.Distance(worldGen.spawnPoints[i].transform.position, worldGen.baseTile.transform.position) > distance)
                {
                    distance = Vector3.Distance(worldGen.spawnPoints[i].transform.position, worldGen.baseTile.transform.position);
                    ChosenSpawnPoint = i;
                }
                else continue;
            }

            int randomBoss = Random.Range(0,BossList.Count);
            Instantiate(BossList[randomBoss], worldGen.spawnPoints[ChosenSpawnPoint].transform.position,Quaternion.identity);
            canSpawnBoss = false;
        }
    }
}
