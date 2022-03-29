using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPurchase : MonoBehaviour
{
    [SerializeField] private GameObject towerToPurchase;
    [SerializeField] private int price;
    private BuildManager bM;

    private void Awake()
    {
        bM = GameObject.FindGameObjectWithTag("GridManager").GetComponent<BuildManager>();
    }

    public void PurchaseTower()
    {
        if (bM.CurrentCoins >= price)
        {
            //Debug.Log("Comprou");
            bM.CurrentTowerCost = price;
            bM.TowerToBuild = towerToPurchase;
        }
    }
}
