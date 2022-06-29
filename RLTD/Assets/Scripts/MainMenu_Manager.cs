using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenu_Manager : MonoBehaviour
{
    public List<GameObject> thingsToDisable = new List<GameObject>();
    public List<Button> DifficultyButtons = new List<Button>();
    public Button playButton;
    public GameObject mainMenu;
    public GameObject mainMenuCamera;
    private EnemyGeneration enemyGen;
    private WorldGeneration worldGen;
    private BuildManager bM;
    private GameManager gM;

    private void Awake()
    {
        mainMenu.SetActive(true);
        mainMenuCamera.SetActive(true);
        enemyGen = GameObject.FindGameObjectWithTag("GridManager").GetComponent<EnemyGeneration>();
        worldGen = GameObject.FindGameObjectWithTag("GridManager").GetComponent<WorldGeneration>();
        bM = GameObject.FindGameObjectWithTag("GridManager").GetComponent<BuildManager>();
        gM = GameObject.FindGameObjectWithTag("GridManager").GetComponent<GameManager>();

        enemyGen.currentDifficulty = 1;

        for (int i = 0; i < thingsToDisable.Count; i++)
        {
            thingsToDisable[i].SetActive(false);
        }

        for (int i = 0; i < DifficultyButtons.Count; i++)
        {
            if (enemyGen.currentDifficulty == i+1)
            {
                DifficultyButtons[i].interactable = false;
            }
            else
            {
                DifficultyButtons[i].interactable = true;
            }
        }
    }

    private void Update()
    {
        if (worldGen.canStartGame)
        {
            playButton.interactable = true;
        }else playButton.interactable = false;


        for (int i = 0; i < DifficultyButtons.Count; i++)
        {
            if (enemyGen.currentDifficulty == i + 1)
            {
                DifficultyButtons[i].interactable = false;
            }
            else
            {
                DifficultyButtons[i].interactable = true;
            }
        }
    }


    public void PlayGame()
    {
        for (int i = 0; i < thingsToDisable.Count; i++)
        {
            thingsToDisable[i].SetActive(true);         
        }
        Time.timeScale = 1;
        bM.SetStartCoins();
        mainMenuCamera.SetActive(false);
        gM.canPause = true;
        mainMenu.SetActive(false);
    }

    public void SetDifficulity1()
    {
        enemyGen.currentDifficulty = 1;
        worldGen.UpdateBaseTile();
    }

    public void SetDifficulity2()
    {
        enemyGen.currentDifficulty = 2;
        worldGen.UpdateBaseTile();
    }

    public void SetDifficulity3()
    {
        enemyGen.currentDifficulty = 3;
        worldGen.UpdateBaseTile();
    }

    public void SetDifficulity4()
    {
        enemyGen.currentDifficulty = 4;
        worldGen.UpdateBaseTile();
    }
}
