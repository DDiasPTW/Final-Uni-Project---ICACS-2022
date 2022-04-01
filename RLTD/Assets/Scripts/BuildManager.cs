using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class BuildManager : MonoBehaviour
{
    private WorldGeneration worldGen;
    private EnemyGeneration enemyGen;
    public LayerMask layerToBuild;
    public LayerMask towerLayer;
    [Header("Building stats")]
    public int startCoins;
    public int CurrentCoins;
    public int CurrentTowerCost;
    public GameObject TowerToBuild;
    private Vector3 mousePos;

    [Header("Tower placement visualizer")]
    public GameObject rangeSprite;
    public GameObject towerVisualizer;
    public Color canPlaceColor;
    public Color cannotPlaceColor;

    [Header("UI Stuff")]
    public TextMeshProUGUI moneyText;
    public GameObject cameraPivot;
    public float movementTime;

    private GameObject placedTower;

    [SerializeField] private bool canBuild;
    [SerializeField] private bool canPlace;

    private void Awake()
    {
        worldGen = GetComponent<WorldGeneration>();
        enemyGen = GetComponent<EnemyGeneration>();
        rangeSprite = Instantiate(rangeSprite);
        towerVisualizer = Instantiate(towerVisualizer);

        
    }

    public void SetStartCoins() //chamado no mainMenu
    {
        CurrentCoins = startCoins * enemyGen.currentDifficulty;
    }

    private void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        UpdateUI();
        

        if (TowerToBuild != null)
        {
            canBuild = true;
        }
        else canBuild = false; 

        if (canBuild)
        {
            FollowMouse();

            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) //apenas pode colocar torre se nao estiver sobre elemento UI
            {
                PlaceTower();
            }

            if (Input.GetMouseButtonDown(1)) //Dar reset da compra, caso queira comprar outra ou nao queira comprar de todo
            {
                ResetPurchase();
            }
        }
        else
        {
            rangeSprite.transform.localScale *= 0;
        }
    }

    private void UpdateUI()
    {
        moneyText.text = CurrentCoins.ToString();
    }

    private void FollowMouse()
    {
        Vector3 seePos = new Vector3();            
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        

        //Apenas mostra os sitios onde se pode colocar a torre
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerToBuild))
        {
            

            if (Mathf.Round(hit.point.x) <= (-(worldGen.chunkSize / 2) + hit.transform.position.x + 1f)) //min x
            {
                seePos.x = Mathf.Round(hit.point.x);
            }

            else if (Mathf.Round(hit.point.x) >= ((worldGen.chunkSize / 2) + hit.transform.position.x - 1f)) //max x
            {
                seePos.x = Mathf.Round(hit.point.x);
            }

            else if (Mathf.Round(hit.point.x) < 0 && Mathf.Round(hit.point.x) > (-(worldGen.chunkSize / 2) + hit.transform.position.x + 1f)) //x neg
            {
                seePos.x = Mathf.Round(hit.point.x);
            }

            else if (Mathf.Round(hit.point.x) >= 0 && Mathf.Round(hit.point.x) < ((worldGen.chunkSize / 2) + hit.transform.position.x - 1f)) //x pos
            {
                seePos.x = Mathf.Round(hit.point.x);
            }

            //z
            if (Mathf.Round(hit.point.z) <= (-(worldGen.chunkSize / 2) + hit.transform.position.z + 1f)) //min z
            {
                seePos.z = Mathf.Round(hit.point.z);
            }

            else if (Mathf.Round(hit.point.z) >= ((worldGen.chunkSize / 2) + hit.transform.position.z - 1f)) //max z
            {
                seePos.z = Mathf.Round(hit.point.z);
            }

            else if (Mathf.Round(hit.point.z) < 0 && Mathf.Round(hit.point.z) > (-(worldGen.chunkSize / 2) + hit.transform.position.z + 1f)) //z neg
            {
                seePos.z = Mathf.Round(hit.point.z);
            }

            else if (Mathf.Round(hit.point.z) >= 0 && Mathf.Round(hit.point.z) < ((worldGen.chunkSize / 2) + hit.transform.position.z - 1f)) //z pos
            {
                seePos.z = Mathf.Round(hit.point.z);
            }

            //garantir que n�o coloca por cima de outras torres
            Collider[] checkTower = Physics.OverlapSphere(new Vector3(seePos.x,hit.point.y,seePos.z), .3f, towerLayer);
            
            
            //Verificar onde pode colocar
            if (hit.point.y < .49f || hit.point.y > 1.1f || CurrentCoins < CurrentTowerCost || checkTower.Length != 0)
            {
                canPlace = false;
            }
            else canPlace = true;



            //Seguir cursor
            towerVisualizer.transform.localScale = TowerToBuild.transform.localScale;
            towerVisualizer.transform.position = new Vector3(seePos.x, hit.point.y + (TowerToBuild.transform.localScale.y / 2), seePos.z);

            rangeSprite.transform.localScale = new Vector3(TowerToBuild.GetComponent<Tower>().range, TowerToBuild.GetComponent<Tower>().range, 0);
            rangeSprite.transform.position = new Vector3(seePos.x, hit.point.y + .1f, seePos.z);
        }
        else
        {
            rangeSprite.transform.position = mousePos;
            rangeSprite.transform.localScale *= 0;
            towerVisualizer.transform.localScale *= 0;
        }

        //Muda cor da range como indicador
        if (canPlace)
        {
            rangeSprite.GetComponent<SpriteRenderer>().color = canPlaceColor;
        }else if (!canPlace)
        {
            rangeSprite.GetComponent<SpriteRenderer>().color = cannotPlaceColor;
        }
    }

    private void PlaceTower()
    {
        Vector3 buildPos = new Vector3();
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerToBuild))
        {
            //x
            buildPos.y = hit.point.y;

            if (canPlace)
            {
                if (Mathf.Round(hit.point.x) <= (-(worldGen.chunkSize / 2) + hit.transform.position.x + 1f)) //min x
                {
                    buildPos.x = Mathf.Round(hit.point.x);
                }

                else if (Mathf.Round(hit.point.x) >= ((worldGen.chunkSize / 2) + hit.transform.position.x - 1f)) //max x
                {
                    buildPos.x = Mathf.Round(hit.point.x);
                }

                else if (Mathf.Round(hit.point.x) < 0 && Mathf.Round(hit.point.x) > (-(worldGen.chunkSize / 2) + hit.transform.position.x + 1f)) //x neg
                {
                    buildPos.x = Mathf.Round(hit.point.x);
                }

                else if (Mathf.Round(hit.point.x) >= 0 && Mathf.Round(hit.point.x) < ((worldGen.chunkSize / 2) + hit.transform.position.x - 1f)) //x pos
                {
                    buildPos.x = Mathf.Round(hit.point.x);
                }

                //z
                if (Mathf.Round(hit.point.z) <= (-(worldGen.chunkSize / 2) + hit.transform.position.z + 1f)) //min z
                {
                    buildPos.z = Mathf.Round(hit.point.z);
                }

                else if (Mathf.Round(hit.point.z) >= ((worldGen.chunkSize / 2) + hit.transform.position.z - 1f)) //max z
                {
                    buildPos.z = Mathf.Round(hit.point.z);
                }

                else if (Mathf.Round(hit.point.z) < 0 && Mathf.Round(hit.point.z) > (-(worldGen.chunkSize / 2) + hit.transform.position.z + 1f)) //z neg
                {
                    buildPos.z = Mathf.Round(hit.point.z);
                }

                else if (Mathf.Round(hit.point.z) >= 0 && Mathf.Round(hit.point.z) < ((worldGen.chunkSize / 2) + hit.transform.position.z - 1f)) //z pos
                {
                    buildPos.z = Mathf.Round(hit.point.z);
                }

                //Debug.Log("Max x: " + ((worldGen.chunkSize / 2) + hit.transform.position.x - 1));
                //Debug.Log("Max z: " + ((worldGen.chunkSize / 2) + hit.transform.position.z - 1));            

                placedTower = Instantiate(TowerToBuild, mousePos, Quaternion.identity);
                placedTower.transform.position = new Vector3(buildPos.x, buildPos.y + (placedTower.transform.localScale.y / 2), buildPos.z);
                CurrentCoins -= CurrentTowerCost;


                //Debug.Log(worldGen.chunkSize + " | " + Mathf.Round(hit.point.x) + " | " + buildPos.x);
                //Debug.Log("Clicado em: " + hit.point);
                //Debug.Log("Colocado em: " + new Vector3(buildPos.x, buildPos.y + (placedTower.transform.localScale.y / 2), buildPos.z));
            }
            else Debug.Log("CANNOT PLACE HERE");  
        }
    }
    private void ResetPurchase()
    {
        TowerToBuild = null;
        CurrentTowerCost = 0;
        towerVisualizer.transform.localScale *= 0;
        placedTower = null;
    }


}