using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TowerPurchase : MonoBehaviour
{
    [SerializeField] private GameObject towerToPurchase;
    private int price;
    private BuildManager bM;
    public TextMeshProUGUI textoPreco;

    private void Awake()
    {
        bM = GameObject.FindGameObjectWithTag("GridManager").GetComponent<BuildManager>();
        price = towerToPurchase.GetComponent<Tower>().sellPrice[towerToPurchase.GetComponent<Tower>().currentEvolution - 1];
        textoPreco.text = price.ToString();
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
