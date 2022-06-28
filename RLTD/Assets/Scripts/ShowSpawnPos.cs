using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowSpawnPos : MonoBehaviour
{
    private GameManager gM;
    private WorldGeneration worldGen;
    public GameObject tileAppearVFX;
    void Awake()
    {
        worldGen = GameObject.FindGameObjectWithTag("GridManager").GetComponent<WorldGeneration>();
        gM = GameObject.FindGameObjectWithTag("GridManager").GetComponent<GameManager>();
    }

    public void Show()
    {
        worldGen.ShowSpawnPos();
        Instantiate(tileAppearVFX,transform);
        worldGen.canStartGame = true;
        gM.spawnButton.SetActive(true);
    }

    private void StopPlay()
    {
        worldGen.canStartGame = false;
        gM.spawnButton.SetActive(false);
    }

}
