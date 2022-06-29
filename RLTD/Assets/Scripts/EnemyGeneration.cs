using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EnemyGeneration : MonoBehaviour
{
    private WorldGeneration worldGen;

    public bool canSpawnBoss;
    public bool hasStartedSpawning = false;
    [Range(1,50)]
    public int enemiesPerWave; //Quantos inimigos irao dar spawn por cada wave
    public int howManyEnemies; //Quantos inimigos vao dar spawn nesta wave
    public int enemiesToSpawn; //Quantos inimigos faltam dar spawn

    public GameObject enemyPref;
    [Header("Enemy composition")]
    [SerializeField] private List<GameObject> en_acessorio = new List<GameObject>();
    [SerializeField] private List<GameObject> en_cabeca = new List<GameObject>();
    [SerializeField] private List<GameObject> en_corpo = new List<GameObject>();
    [SerializeField] private List<GameObject> en_pes = new List<GameObject>();
    
    public GameObject chosen_acessorio;
    public GameObject chosen_cabeca;
    public GameObject chosen_corpo;
    public GameObject chosen_pes;
    public Sprite slowResSprite;
    public Sprite poisonResSprite;

    [Header("UI Stuff")]
    public GameObject enemyVisualizer;
    public GameObject acess_Icon;
    public Image resUISprite;
    public GameObject cabeca_Icon;
    public TextMeshProUGUI damageText;
    public GameObject corpo_Icon;
    public TextMeshProUGUI healthText;
    public GameObject pes_Icon;
    public TextMeshProUGUI speedText;

    [SerializeField] private List<GameObject> BossList = new List<GameObject>(); //Todos os bosses do jogo


    public List<GameObject> spawnedEnemies = new List<GameObject>();


    [Range(1, 4)]
    public int currentDifficulty;

    public bool isSpawning = false;
    public float spawnCooldown; //cooldown entre spawn de cada inimigo
    [SerializeField] private float spawnTime;


    private void Awake()
    {
        enemiesPerWave = enemiesPerWave * currentDifficulty;
    }

    void Start()
    {
        worldGen = GameObject.FindGameObjectWithTag("GridManager").GetComponent<WorldGeneration>();
        spawnTime = 0;
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.K)) //"Mata" a current wave, remover na versão final
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


        spawnTime -= Time.deltaTime;
    }

    public void StartWave()
    {
        worldGen.UpdateNavMesh();
        enemyVisualizer.SetActive(false);
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

    public void DefineEnemy()
    {
        //Limpa inimigo gerado anteriormente
        chosen_acessorio = null;
        chosen_cabeca = null;
        chosen_corpo = null;
        chosen_pes = null;

        //ESCOLHE UM ELEMENTO DE CADA CATEGORIA PARA CRIAR INIMIGO
        int randomNumber = Random.Range(0,100); //0-99
        int whatAcess;
        if (randomNumber < 75)
        {
            whatAcess = 0; //0 - null, 1- ice, 2- poison
        }
        else whatAcess = Random.Range(1,en_acessorio.Count);

        int whatCabeca = Random.Range(0, en_cabeca.Count); 
        int whatCorpo = Random.Range(0, en_corpo.Count);
        int whatPes = Random.Range(0, en_pes.Count);

        //Define inimigo
        chosen_acessorio = en_acessorio[whatAcess];
        chosen_cabeca = en_cabeca[whatCabeca];
        chosen_corpo = en_corpo[whatCorpo];
        chosen_pes = en_pes[whatPes];

        //Atualiza UI
        enemyVisualizer.SetActive(true);
        acess_Icon.GetComponent<MeshFilter>().mesh = chosen_acessorio.GetComponent<MeshFilter>().sharedMesh;
        cabeca_Icon.GetComponent<MeshFilter>().mesh = chosen_cabeca.GetComponent<MeshFilter>().sharedMesh;
        corpo_Icon.GetComponent<MeshFilter>().mesh = chosen_corpo.GetComponent<MeshFilter>().sharedMesh;
        pes_Icon.GetComponent<MeshFilter>().mesh = chosen_pes.GetComponent<MeshFilter>().sharedMesh;
        
        pes_Icon.transform.localPosition = new Vector3(0, chosen_pes.GetComponent<BoxCollider>().size.y / 2, 0);

        corpo_Icon.transform.localPosition = new Vector3(0, chosen_corpo.GetComponent<BoxCollider>().size.y / 2 + chosen_pes.GetComponent<BoxCollider>().size.y, 0);

        cabeca_Icon.transform.localPosition = new Vector3(0, chosen_cabeca.GetComponent<BoxCollider>().size.y / 2 + chosen_corpo.GetComponent<BoxCollider>().size.y + chosen_pes.GetComponent<BoxCollider>().size.y, 0);

        acess_Icon.transform.localPosition = new Vector3(0, chosen_acessorio.GetComponent<BoxCollider>().size.y / 2 + chosen_cabeca.GetComponent<BoxCollider>().size.y + chosen_corpo.GetComponent<BoxCollider>().size.y + chosen_pes.GetComponent<BoxCollider>().size.y, 0);


        if (chosen_acessorio.GetComponent<Acessorio>().resSlow)
        {
            resUISprite.sprite = slowResSprite;
        }
        else if (chosen_acessorio.GetComponent<Acessorio>().resPoison)
        {
            resUISprite.sprite = poisonResSprite;
        }
        else
        {
            resUISprite.sprite = null;
        }
        damageText.text = chosen_cabeca.GetComponent<Head>().damage.ToString();
        healthText.text = chosen_corpo.GetComponent<Corpo>().health.ToString();
        speedText.text = ((int)(chosen_pes.GetComponent<Pes>().speed * 10 )).ToString();

    }

    void SpawnEnemies() //da loop por todos os spawnpoints de modo a que haja um numero igual de inimigos por spawnPoint.
    {
        if (spawnTime <= 0)
        {
            for (int i = 0; i < worldGen.spawnPoints.Count; i++)
            {
                GameObject enemy;

                enemy = Instantiate(enemyPref, worldGen.spawnPoints[i].transform.position - new Vector3(0, worldGen.spawnPoints[i].transform.localScale.y / 2, 0), Quaternion.identity);

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
        if (worldGen.CurrentWave == worldGen.MaxWave && canSpawnBoss && enemiesToSpawn == 0) //Spawn de boss
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

            int randomBoss = Random.Range(0, BossList.Count);
            GameObject boss = Instantiate(BossList[randomBoss], worldGen.spawnPoints[ChosenSpawnPoint].transform.position, Quaternion.identity);
            spawnedEnemies.Add(boss);
            canSpawnBoss = false;
        }

    }
}
