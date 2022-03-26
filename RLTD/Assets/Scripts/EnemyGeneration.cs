using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGeneration : MonoBehaviour
{
    private WorldGeneration worldGen;

    public int howManyEnemies; //Quantos inimigos vao dar spawn nesta wave
    [SerializeField] private int enemiesToSpawn; //Quantos inimigos faltam dar spawn


    public List<GameObject> enemies = new List<GameObject>(); //Todos os inimigos do jogo (SEM BOSSES)

    [Range(1, 4)]
    public int currentDifficulty;

    public bool isSpawning = false;
    public float spawnCooldown;
    [SerializeField] private float spawnTime;

    void Start()
    {
        worldGen = GameObject.FindGameObjectWithTag("GridManager").GetComponent<WorldGeneration>();
        spawnTime = 0;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            isSpawning = true; 
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


    void SpawnEnemies()
    {
        if (spawnTime <= 0)
        {
            for (int i = 0; i < worldGen.spawnPoints.Count; i++)
            {
                int whatEnemy = Random.Range(0, enemies.Count);

                Instantiate(enemies[whatEnemy], worldGen.spawnPoints[i].transform.position, Quaternion.identity);

                enemiesToSpawn--;

                if (enemiesToSpawn > 0 && i == worldGen.spawnPoints.Count - 1)
                {
                    spawnTime = spawnCooldown;
                    //i = -1;
                }
                else if (enemiesToSpawn == 0)
                {
                    isSpawning = false;
                    break;
                }
            }
        }
    }
}
