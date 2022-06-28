using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TowerEvoMenu : MonoBehaviour
{
    public GameObject currentTower;
    [SerializeField] private TextMeshProUGUI evolvePrice;
    [SerializeField] private GameObject evoButton;
    public GameObject towerEvoPreviewMesh;

    private float scaleX, scaleY;
    private void Awake()
    {
        scaleX = transform.localScale.x;
        scaleY = transform.localScale.y;

    }

    private void Update()
    {
        if (currentTower != null)
        {
            transform.localScale = new Vector2(scaleX,scaleY);
            UpdateUIElements();

            if (currentTower.GetComponent<Tower>().currentEvolution != currentTower.GetComponent<Tower>().numberOfEvolutions)
            {              
                int price = (int)(currentTower.GetComponent<Tower>().evolutionPrices[currentTower.GetComponent<Tower>().currentEvolution - 1].
                Evaluate(currentTower.GetComponent<Tower>().worldGen.CurrentWave - 1));
                evolvePrice.text = "-" + price.ToString();
            }
        }
        else
        {
            transform.localScale *= 0;
        }

    }

    private void UpdateUIElements()
    {


        if (currentTower.GetComponent<Tower>().currentEvolution == currentTower.GetComponent<Tower>().numberOfEvolutions)
        {
            evoButton.SetActive(false);
            evolvePrice.text = "";
            evolvePrice.gameObject.SetActive(false);
            towerEvoPreviewMesh.GetComponent<MeshFilter>().mesh = currentTower.GetComponent<Tower>().evolutionLooks[currentTower.GetComponent<Tower>().currentEvolution - 1];
        }
        else
        {
            evolvePrice.gameObject.SetActive(true);
            towerEvoPreviewMesh.GetComponent<MeshFilter>().mesh = currentTower.GetComponent<Tower>().evolutionLooks[currentTower.GetComponent<Tower>().currentEvolution];
            int price = (int)currentTower.GetComponent<Tower>().evolutionPrices[currentTower.GetComponent<Tower>().currentEvolution - 1].Evaluate(currentTower.GetComponent<Tower>().worldGen.CurrentWave - 1);
            evolvePrice.text = price.ToString();
            evoButton.SetActive(true);

            if (currentTower.GetComponent<Tower>().bM.CurrentCoins < (int)currentTower.GetComponent<Tower>().evolutionPrices[currentTower.GetComponent<Tower>().currentEvolution - 1].Evaluate(currentTower.GetComponent<Tower>().worldGen.CurrentWave - 1))
            {
                evoButton.GetComponent<Button>().interactable = false;
            }
            else
            {
                evoButton.GetComponent<Button>().interactable = true;
            }
        }


    }

    public void EvolveTower()
    {
        currentTower.GetComponent<Tower>().EvolveTower();
    }

    public void SellTower()
    {
        currentTower.GetComponent<Tower>().SellTower();
    }

}
