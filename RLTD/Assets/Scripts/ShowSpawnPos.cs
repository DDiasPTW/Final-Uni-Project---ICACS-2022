using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowSpawnPos : MonoBehaviour
{
    private WorldGeneration worldGen;
    public GameObject tileAppearVFX;
    void Awake()
    {
        worldGen = GameObject.FindGameObjectWithTag("GridManager").GetComponent<WorldGeneration>();
    }

    public void Show()
    {
        worldGen.ShowSpawnPos();
        Instantiate(tileAppearVFX,transform);
        worldGen.canStartGame = true;
    }

    private void StopPlay()
    {
        worldGen.canStartGame = false;
    }

}
