using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Money : MonoBehaviour
{
    private Item item;
    private BuildManager bM;
    [SerializeField] private int value;
    [SerializeField] private float delay;
    private float timer;
    private EnemyGeneration enGen;

    private void Awake()
    {
        item = GetComponent<Item>();
        bM = GameObject.FindGameObjectWithTag("GridManager").GetComponent<BuildManager>();
        enGen = GameObject.FindGameObjectWithTag("GridManager").GetComponent<EnemyGeneration>();
        timer = delay;
    }

    private void Update()
    {
        if (item.pickedUp)
        {
            timer -= Time.deltaTime;
        }

        if (enGen.isSpawning)
        {
            if (timer <= 0)
            {
                bM.CurrentCoins += value;
                timer = delay;
            }
        }       
    }
}
