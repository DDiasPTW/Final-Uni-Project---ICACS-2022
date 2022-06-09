using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowSpawnPos : MonoBehaviour
{
    private WorldGeneration worldGen;
    void Awake()
    {
        worldGen = GameObject.FindGameObjectWithTag("GridManager").GetComponent<WorldGeneration>();
    }

    public void Show()
    {
        worldGen.ShowSpawnPos();
    }
}
