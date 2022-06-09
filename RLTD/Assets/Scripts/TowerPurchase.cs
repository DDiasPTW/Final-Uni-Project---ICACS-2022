using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TowerPurchase : MonoBehaviour
{
    [SerializeField] private GameObject towerToPurchase;
    private int price;
    private BuildManager bM;
    private ShopManager shopM;
    public TextMeshProUGUI textoPreco;

    [SerializeField] private GameObject towerDesc;
    [SerializeField] private Image towerDescImage;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [Multiline(2)]
    [SerializeField] private string towerDescription;

    private void Awake()
    {
        bM = GameObject.FindGameObjectWithTag("GridManager").GetComponent<BuildManager>();
        shopM = GameObject.FindGameObjectWithTag("ShopM").GetComponent<ShopManager>();
        towerDesc.SetActive(false);
        price = towerToPurchase.GetComponent<Tower>().sellPrice[towerToPurchase.GetComponent<Tower>().currentEvolution - 1];
        textoPreco.text = price.ToString();
    }

    private void Update()
    {
        if (bM.CurrentCoins < price)
        {
            gameObject.GetComponent<Button>().interactable = false;
        }else
        {
            gameObject.GetComponent<Button>().interactable = true;
        }

        if (bM.TowerToBuild == null)
        {
            towerDesc.SetActive(false);
        }
    }

    public void PurchaseTower()
    {
        towerDesc.SetActive(true);
        descriptionText.text = towerDescription;
        towerDescImage.sprite = gameObject.GetComponent<Image>().sprite;

        if (bM.CurrentCoins >= price)
        {
            bM.CurrentTowerCost = price;
            bM.TowerToBuild = towerToPurchase;
        }
    }
}
