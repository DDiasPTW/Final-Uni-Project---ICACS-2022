using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EnemyGeneration : MonoBehaviour
{
    private WorldGeneration worldGen;

    public bool checkEnemiesToSpawn = false;
    public bool canSpawnBoss;
    public bool hasStartedSpawning = false;
    [Range(1,30)]
    public int enemiesPerWave; //Quantos inimigos irao dar spawn por cada wave
    public int howManyEnemies; //Quantos inimigos vao dar spawn nesta wave
    public int enemiesToSpawn; //Quantos inimigos faltam dar spawn

    public List<GameObject> enemiesThatWillSpawn = new List<GameObject>();

    [SerializeField] private List<GameObject> enemies = new List<GameObject>(); //Todos os inimigos do jogo (SEM BOSSES)
    [SerializeField] private List<GameObject> BossList = new List<GameObject>(); //Todos os bosses do jogo
    public List<GameObject> spawnedEnemies = new List<GameObject>();


    [Range(1, 4)]
    public int currentDifficulty;

    public bool isSpawning = false;
    public float spawnCooldown; //cooldown entre spawn de cada inimigo
    [SerializeField] private float spawnTime;


    [Header("UI Stuff")]
    public GameObject EnemyContainer;
    public TextMeshProUGUI defaultNumber, assassinNumber, healerNumber, tankNumber;


    private string EnemyDefault = "Default";
    [SerializeField] private int howManyDefaults = 0;
    private string EnemyAssassin = "Assassin";
    [SerializeField] private int howManyAssassins = 0;
    private string EnemyHealer = "Healer";
    [SerializeField]private int howManyHealers = 0;
    private string EnemyTank = "Tank";
    [SerializeField]private int howManyTanks = 0;

    private void Awake()
    {
        enemiesPerWave = enemiesPerWave * currentDifficulty;
        EnemyContainer.SetActive(false);
    }

    void Start()
    {
        worldGen = GameObject.FindGameObjectWithTag("GridManager").GetComponent<WorldGeneration>();
        spawnTime = 0;
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.K)) //"Mata" a current wave
        {
            ClearWave();
        }

        if (worldGen.CurrentWave != worldGen.MaxWave) //Para garantir que boss so da spawn na ultima ronda
        {
            canSpawnBoss = true;
        }

        if (isSpawning) //é mudado no script WorldGeneration
        {
            SpawnEnemies();
        }
        if (!isSpawning && !hasStartedSpawning)
        {
            enemiesToSpawn = howManyEnemies;
        }

        if (checkEnemiesToSpawn)
        {
            DefineEnemies();
        }


        spawnTime -= Time.deltaTime;
    }

    public void StartWave()
    {
        worldGen.UpdateNavMesh();
        EnemyContainer.SetActive(false);
        if (worldGen.CurrentWave > 0)
        {
            isSpawning = true;
            hasStartedSpawning = true;
        }
    }

    private void ClearWave()
    {
        for (int i = 0; i < spawnedEnemies.Count; i++)
        {
            Destroy(spawnedEnemies[i]);
        }
        enemiesToSpawn = 0;
        isSpawning = false;
        spawnedEnemies.Clear();
        if (worldGen.CurrentWave < worldGen.MaxWave)
        {
            howManyEnemies = 0;
        }
        
    }

    public void DefineEnemies()
    {
        howManyHealers = 0; howManyDefaults = 0; howManyAssassins = 0; howManyTanks = 0;

        for (int i = 0; i < howManyEnemies; i++)
        {
            int whatEnemy = Random.Range(0, enemies.Count);
            enemiesThatWillSpawn.Add(enemies[whatEnemy]);
        }

        for (int i = 0; i < enemiesThatWillSpawn.Count; i++)
        {
            if (enemiesThatWillSpawn[i].tag == EnemyDefault)
            {
                howManyDefaults++;
            }else if (enemiesThatWillSpawn[i].tag == EnemyAssassin)
            {
                howManyAssassins++;
            }else if (enemiesThatWillSpawn[i].tag == EnemyHealer)
            {
                howManyHealers++;
            }else if (enemiesThatWillSpawn[i].tag == EnemyTank)
            {
                howManyTanks++;
            }
        }

        EnemyContainer.SetActive(true);
        defaultNumber.text = "x" + howManyDefaults.ToString();
        assassinNumber.text = "x" + howManyAssassins.ToString();
        healerNumber.text = "x" + howManyHealers.ToString();
        tankNumber.text = "x" + howManyTanks.ToString();

        checkEnemiesToSpawn = false;
    }

    void SpawnEnemies() //da loop por todos os spawnpoints de modo a que haja um numero igual de inimigos por spawnPoint.
        //TO DO: Definir percentagem de chance de cada inimigo, tendo em conta as waves e a dificuldade
    {
        if (spawnTime <= 0)
        {
            for (int i = 0; i < worldGen.spawnPoints.Count; i++)
            {
                int whatEnemy = Random.Range(0, enemiesThatWillSpawn.Count);
                GameObject enemy;

                //enemy = Instantiate(enemies[whatEnemy], worldGen.spawnPoints[i].transform.position - new Vector3(0, worldGen.spawnPoints[i].transform.localScale.y / 2, 0), Quaternion.identity);
                enemy = Instantiate(enemiesThatWillSpawn[whatEnemy], worldGen.spawnPoints[i].transform.position - new Vector3(0, worldGen.spawnPoints[i].transform.localScale.y / 2, 0), Quaternion.identity);
                enemiesThatWillSpawn.RemoveAt(whatEnemy);


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
            GameObject boss = Instantiate(BossList[randomBoss], worldGen.spawnPoints[ChosenSpawnPoint].transform.position,Quaternion.identity);
            spawnedEnemies.Add(boss);
            canSpawnBoss = false;
        }
    }
}
