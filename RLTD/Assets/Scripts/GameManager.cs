using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    private WorldGeneration worldGen;
    private EnemyGeneration enemyGen;
    private BuildManager bM;
    public TextMeshProUGUI waveText;
    public ShopManager shopM;

    public bool canSpawn = false;
    //public GameObject nextPositionButton;
    public GameObject spawnButton;
    public GameObject mainMenuUI;

    public int coinsPerWave;

    public int Health = 100;

    private void Awake()
    {
        worldGen = GetComponent<WorldGeneration>();
        enemyGen = GetComponent<EnemyGeneration>();
        bM = GetComponent<BuildManager>();
        Time.timeScale = 1;
        //shopM = GameObject.FindGameObjectWithTag("ShopM").GetComponent<ShopManager>();

        //nextPositionButton.SetActive(false);
        spawnButton.SetActive(false);
    }

    void Update()
    {
        if (enemyGen.spawnedEnemies.Count == 0 && !enemyGen.isSpawning && worldGen.CurrentWave < worldGen.MaxWave && mainMenuUI.activeSelf == false && enemyGen.enemiesToSpawn == 0)
        {
            //nextPositionButton.SetActive(true);
            ExpandWorld();
        }
        //else nextPositionButton.SetActive(false);

        if (enemyGen.enemiesToSpawn != 0 && !enemyGen.isSpawning && enemyGen.spawnedEnemies.Count == 0 && canSpawn /*&& !worldGen.canSpawn*/)
        {
            spawnButton.SetActive(true);
        }
        else spawnButton.SetActive(false);

        if (enemyGen.isSpawning)
        {
            //waveText.gameObject.SetActive(false);
        }
        else if (!enemyGen.isSpawning && enemyGen.spawnedEnemies.Count == 0)
        {
            //waveText.gameObject.SetActive(true);
        }

        if (Health <= 0)
        {
            LoseGame();
        }

        waveText.text = "WAVE " + worldGen.CurrentWave.ToString() + "/" + worldGen.MaxWave.ToString();

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void ExpandWorld()
    {
        worldGen.NextWave();
        enemyGen.hasStartedSpawning = false;

        if (worldGen.CurrentWave < worldGen.MaxWave / 5)
        {
            bM.CurrentCoins +=(int) ((worldGen.CurrentWave - 1) * coinsPerWave * 1.5f);
        }else if (worldGen.CurrentWave >= (worldGen.MaxWave / 5) && worldGen.CurrentWave < (worldGen.MaxWave / 2f))
        {
            bM.CurrentCoins +=(worldGen.CurrentWave - 1) * coinsPerWave;
        }
        else
        {
            bM.CurrentCoins += coinsPerWave;
        }
        //bM.SellTowers();

        //worldGen.ShowSpawnPos();
        enemyGen.DefineEnemy();
        canSpawn = true;
    }

    private void LoseGame()
    {
        //MELHORAR ISTO
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoseHealth(int damage)
    {
        Health -= damage;
    }

    public void SpawnNextWave()
    {
        enemyGen.StartWave();
        //worldGen.ShowSpawnPos();
        canSpawn = false;
    }
}
