using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Tower : MonoBehaviour
{
    
    [HideInInspector] public BuildManager bM;
    private MeshFilter mF;
    private GameObject cameraPivot;
    [HideInInspector] public WorldGeneration worldGen;
    private GameObject towerDesc;

    [Header("Evolution Stuff")]
    public int currentEvolution = 1; 
    public int numberOfEvolutions;

    public List<Mesh> evolutionLooks = new List<Mesh>(); 
    public List<Transform> shootPositions = new List<Transform>();
    public List<float> damage = new List<float>();
    public List<float> fireRate = new List<float>();
    public List<float> range = new List<float>(); 
    public List<int> evolvePrice = new List<int>();

    public List<int> sellPrice = new List<int>();


    public AnimationCurve buyPrice;
    public List<AnimationCurve> evolutionPrices = new List<AnimationCurve>();

    [Header("UI Elements")]
    public GameObject rangeVisualizer; 

    private GameObject evoMenu; 
    
    public Color rangeVisualizerColor;
    [Header("Damage Stuff")]
    public LayerMask EnemyLayer;
    public LayerMask TowerLayer;
    public float AOERange; 

    
    [Header("Slow")]
    [Range(0,1)]
    public List<float> slowMultiplier = new List<float>();
    public List<float> slowTime = new List<float>();
    
    [Header("Poison")]
    [Range(0,1)]
    public List<float> poisonMultiplier = new List<float>();
    public List<float> poisonTime = new List<float>();
    
    
    private void Awake()
    {
        mF = GetComponentInChildren<MeshFilter>();
        bM = GameObject.FindGameObjectWithTag("GridManager").GetComponent<BuildManager>();
        worldGen = GameObject.FindGameObjectWithTag("GridManager").GetComponent<WorldGeneration>();
        cameraPivot = GameObject.FindGameObjectWithTag("Pivot");
        rangeVisualizer = Instantiate(rangeVisualizer);
        towerDesc = GameObject.FindGameObjectWithTag("TowerDesc");
        currentEvolution = 1;
        evoMenu = GameObject.FindGameObjectWithTag("TowerEvo");
        mF.mesh = evolutionLooks[currentEvolution - 1];
    }

    //Arranjar um tower price manager (TowerPurchase)
    //que altera remotamente o pre�o de cada uma das torres utilizando
    //os valores definidos neste script (pela animation curve)
    //de modo a comunicar com a Shop e com o menu de evolu��o / venda



    private void Start()
    {
        rangeVisualizer.GetComponent<SpriteRenderer>().color = rangeVisualizerColor;
    }


    private void Update()
    {
        //UpdateUIElements();


        CheckRange();
        CheckEvolutionMenu();        
    }

    //private void UpdateUIElements()
    //{
    //    //evoMenu.transform.eulerAngles = new Vector3(45f, cameraPivot.transform.localEulerAngles.y, evoMenu.transform.eulerAngles.z);
       

    //    if (currentEvolution == numberOfEvolutions)
    //    {
    //        evoButton.SetActive(false);
    //        evoPriceText.text = "";
    //        //evoMenu.SetActive(false);
    //    }
    //    else
    //    {
    //        //evoPriceText.text = evolvePrice[currentEvolution-1].ToString();
    //        int price = (int)evolutionPrices[currentEvolution - 1].Evaluate(worldGen.CurrentWave - 1);
    //        evoPriceText.text = price.ToString();
    //        evoButton.SetActive(true);

    //        if (bM.CurrentCoins < (int)evolutionPrices[currentEvolution - 1].Evaluate(worldGen.CurrentWave - 1))
    //        {
    //            evoButton.GetComponent<Button>().interactable = false;
    //        }
    //        else
    //        {
    //            evoButton.GetComponent<Button>().interactable = true;
    //        }
    //    }

        
    //}


    private void CheckRange()
    {
        rangeVisualizer.transform.position = new Vector3(transform.position.x,.75f,transform.position.z);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        float rangeSize = range[currentEvolution - 1] / worldGen.chunkSize;
        
        if (Physics.Raycast(ray, out hit, Mathf.Infinity,TowerLayer))
        {
            if (hit.collider.gameObject == gameObject)
            {
                //rangeVisualizer.SetActive(true);
                rangeVisualizer.transform.localScale = new Vector3(rangeSize,rangeSize, 1);
            } 
        }
        else
        {
            rangeVisualizer.transform.localScale *= 0;
        }

    }

    private void CheckEvolutionMenu()
    {      
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    //evoMenu.SetActive(true);
                    evoMenu.GetComponent<TowerEvoMenu>().currentTower = gameObject;
                }
                //else if (hit.collider.gameObject != gameObject && !EventSystem.current.IsPointerOverGameObject())
                //{
                //    //evoMenu.GetComponent<TowerEvoMenu>().currentTower = null;
                //    //evoMenu.SetActive(false);
                //}
            }
        }


        if (Input.GetMouseButtonDown(1))
        {
            evoMenu.GetComponent<TowerEvoMenu>().currentTower = null;
            //evoMenu.SetActive(false);
        }
    }

    //Chamar quando evoluir torre
    public void EvolveTower()
    {
        if (bM.CurrentCoins >= (int)evolutionPrices[currentEvolution - 1].Evaluate(worldGen.CurrentWave - 1) && currentEvolution < numberOfEvolutions)
        {
            //bM.CurrentCoins -= evolvePrice[currentEvolution - 1];
            bM.CurrentCoins -= (int) evolutionPrices[currentEvolution - 1].Evaluate(worldGen.CurrentWave - 1);
            currentEvolution++;
            mF.mesh = evolutionLooks[currentEvolution - 1];
            gameObject.GetComponent<Outline>().Repeat();
        }

    }

    //Chamar quando vender torre
    public void SellTower()
    {
        bM.CurrentCoins += sellPrice[currentEvolution-1];
        evoMenu.GetComponent<TowerEvoMenu>().currentTower = null;
        bM.allTowers.Remove(gameObject);
        Destroy(gameObject);
    }

}
