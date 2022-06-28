using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TowerPurchase : MonoBehaviour
{
    [SerializeField] private GameObject towerToPurchase;
    private int price;
    private WorldGeneration worldGen;
    private BuildManager bM;
    private ShopManager shopM;
    private ItemManager iM;
    public TextMeshProUGUI textoPreco;

    [SerializeField] private Mesh towerMesh;
    [SerializeField] private GameObject towerPreviewMesh;
    [SerializeField] private GameObject itemPreviewMesh;

    [SerializeField] private GameObject towerDesc;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [Multiline(2)]
    [SerializeField] private string towerDescription;

    private void Awake()
    {
        worldGen = GameObject.FindGameObjectWithTag("GridManager").GetComponent<WorldGeneration>();
        bM = GameObject.FindGameObjectWithTag("GridManager").GetComponent<BuildManager>();
        iM = GameObject.FindGameObjectWithTag("GridManager").GetComponent<ItemManager>();
        shopM = GameObject.FindGameObjectWithTag("ShopM").GetComponent<ShopManager>();
        towerDesc.SetActive(false);
        GetPrice();
    }

    private void GetPrice()
    {
        price = (int)towerToPurchase.GetComponent<Tower>().buyPrice.Evaluate(worldGen.CurrentWave - 1);
        textoPreco.text = price.ToString();
    }

    private void Update()
    {
        price = (int)towerToPurchase.GetComponent<Tower>().buyPrice.Evaluate(worldGen.CurrentWave - 1);
        textoPreco.text = price.ToString();

        if (bM.CurrentCoins < price)
        {
            gameObject.GetComponent<Button>().interactable = false;
        }else
        {
            gameObject.GetComponent<Button>().interactable = true;
        }

        if (bM.TowerToBuild == null)
        {
            if (iM.currentItem != null && iM.currentItem.GetComponent<Item>().activated == true) 
            {
                //Debug.Log("");
            }
            else
            {
                towerDesc.SetActive(false);
            }
        }
    }

    public void PurchaseTower()
    {
        towerDesc.SetActive(true);
        descriptionText.text = towerDescription;
        towerPreviewMesh.GetComponent<MeshFilter>().mesh = towerMesh;
        itemPreviewMesh.GetComponent<MeshFilter>().sharedMesh = null;

        if (bM.CurrentCoins >= price)
        {
            bM.CurrentTowerCost = price;
            bM.TowerToBuild = towerToPurchase;
        }
    }
}
