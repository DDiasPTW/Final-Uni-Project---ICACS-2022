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

    public bool canSpawn = false;
    public GameObject nextPositionButton;
    public GameObject spawnButton;
    public GameObject mainMenuUI;

    public int Health = 100;
    public List<TextMeshProUGUI> healthText;

    private void Awake()
    {
        worldGen = GetComponent<WorldGeneration>();
        enemyGen = GetComponent<EnemyGeneration>();
        bM = GetComponent<BuildManager>();

        nextPositionButton.SetActive(false);
        spawnButton.SetActive(false);
    }

    void Update()
    {
        for (int i = 0; i < healthText.Count; i++)
        {
            healthText[i].text = Health.ToString();
        }


        if (enemyGen.spawnedEnemies.Count == 0 && !enemyGen.isSpawning && worldGen.CurrentWave < worldGen.MaxWave && mainMenuUI.activeSelf == false && enemyGen.enemiesToSpawn == 0)
        {
            nextPositionButton.SetActive(true);
        }
        else nextPositionButton.SetActive(false);

        if (enemyGen.enemiesToSpawn != 0 && !enemyGen.isSpawning && enemyGen.spawnedEnemies.Count == 0 && canSpawn)
        {
            spawnButton.SetActive(true);
        }
        else spawnButton.SetActive(false);

        waveText.text = worldGen.CurrentWave.ToString() + " / " + worldGen.MaxWave.ToString();

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void ExpandWorld()
    {
        worldGen.NextWave();
        enemyGen.hasStartedSpawning = false;
        bM.CurrentCoins += (worldGen.CurrentWave - 1) * 50 * enemyGen.currentDifficulty;
        canSpawn = true;
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
}
