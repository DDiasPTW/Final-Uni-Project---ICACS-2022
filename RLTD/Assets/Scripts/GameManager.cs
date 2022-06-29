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
    public GameObject spawnButton;
    public GameObject mainMenuUI;
    public GameObject pauseMenu;
    public GameObject loseMenu;
    public GameObject winMenu;
    public bool canPause = false;
    public int coinsPerWave;


    private float gspeed;

    public int Health = 100;

    private void Awake()
    {
        worldGen = GetComponent<WorldGeneration>();
        enemyGen = GetComponent<EnemyGeneration>();
        bM = GetComponent<BuildManager>();
        Time.timeScale = 1;
        spawnButton.SetActive(false);
        pauseMenu.SetActive(false);
        loseMenu.SetActive(false);
        winMenu.SetActive(false);
    }

    void Update()
    {
        if (enemyGen.spawnedEnemies.Count == 0 && !enemyGen.isSpawning && worldGen.CurrentWave < worldGen.MaxWave && mainMenuUI.activeSelf == false && enemyGen.enemiesToSpawn == 0)
        {
            ExpandWorld();
        }else if (enemyGen.spawnedEnemies.Count == 0 && !enemyGen.isSpawning && worldGen.CurrentWave == worldGen.MaxWave && mainMenuUI.activeSelf == false && enemyGen.enemiesToSpawn == 0)
        {
            WinGame();
        }


        if (enemyGen.enemiesToSpawn != 0 && !enemyGen.isSpawning && enemyGen.spawnedEnemies.Count == 0 && canSpawn /*&& !worldGen.canSpawn*/)
        {
            //spawnButton.SetActive(true);
        }
        else spawnButton.SetActive(false);

        if (Health <= 0)
        {
            LoseGame();
        }

        waveText.text = "WAVE " + worldGen.CurrentWave.ToString() + "/" + worldGen.MaxWave.ToString();

        if (Input.GetKeyDown(KeyCode.Escape) && canPause)
        {
            pauseMenu.SetActive(true);
            gspeed = Time.timeScale;
            Time.timeScale = 0;
        }
    }

    public void ExpandWorld()
    {
        worldGen.NextWave();
        enemyGen.hasStartedSpawning = false;

        if (worldGen.CurrentWave < worldGen.MaxWave / 4 && worldGen.CurrentWave > 1)
        {
            bM.CurrentCoins += coinsPerWave * enemyGen.currentDifficulty;
        }

        enemyGen.DefineEnemy();
        canSpawn = true;
    }

    private void LoseGame()
    {
        //Abrir menu de derrota
        loseMenu.SetActive(true);
    }

    private void WinGame()
    {
        winMenu.SetActive(true);
    }

    public void LoseHealth(int damage)
    {
        Health -= damage;
    }

    public void SpawnNextWave()
    {
        enemyGen.StartWave();
        canSpawn = false;
    }

    //Pause Menu Button Stuff

    public void Resume()
    {
        Time.timeScale = gspeed;
        pauseMenu.SetActive(false);
    }
    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void Quit()
    {
        Application.Quit();
    }
}
