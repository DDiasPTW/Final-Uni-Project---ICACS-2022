using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class Tower : MonoBehaviour
{
    private BuildManager bM;
    private MeshFilter mF;
    private GameObject cameraPivot;
    private WorldGeneration worldGen;

    [Header("Evolution Stuff")]
    public int currentEvolution = 1; 
    public int numberOfEvolutions;

    public List<Mesh> evolutionLooks = new List<Mesh>(); 
    public List<float> damage = new List<float>();
    public List<float> fireRate = new List<float>();
    public List<float> range = new List<float>(); 
    public List<int> evolvePrice = new List<int>();

    public List<int> sellPrice = new List<int>();

    public AnimationCurve buyPrice;
    public List<AnimationCurve> evolutionPrices = new List<AnimationCurve>();

    [Header("UI Elements")]
    public GameObject rangeVisualizer; 
    public GameObject evoMenu;   
    public TextMeshProUGUI evoPriceText;
    public GameObject evoButton;
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
        currentEvolution = 1;
        evoMenu.SetActive(false);
    }

    //Arranjar um tower price manager (TowerPurchase)
    //que altera remotamente o preço de cada uma das torres utilizando
    //os valores definidos neste script (pela animation curve)
    //de modo a comunicar com a Shop e com o menu de evolução / venda



    private void Start()
    {
        rangeVisualizer.GetComponent<SpriteRenderer>().color = rangeVisualizerColor;
    }


    private void Update()
    {
        UpdateUIElements();


        CheckRange();

        if (currentEvolution < numberOfEvolutions)
        {
            CheckEvolutionMenu();
        }
        
    }

    private void UpdateUIElements()
    {
        evoMenu.transform.eulerAngles = new Vector3(45f, cameraPivot.transform.localEulerAngles.y, evoMenu.transform.eulerAngles.z);
       

        if (currentEvolution == numberOfEvolutions)
        {
            evoButton.SetActive(false);
            evoPriceText.text = "";
            evoMenu.SetActive(false);
        }
        else
        {
            //evoPriceText.text = evolvePrice[currentEvolution-1].ToString();
            int price = (int)evolutionPrices[currentEvolution - 1].Evaluate(worldGen.CurrentWave - 1);
            evoPriceText.text = price.ToString();
            evoButton.SetActive(true);
        }
    }


    private void CheckRange()
    {
        rangeVisualizer.transform.position = new Vector3(transform.position.x,.75f,transform.position.z);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
               
        if (Physics.Raycast(ray, out hit, Mathf.Infinity,TowerLayer))
        {
            if (hit.collider.gameObject == gameObject)
            {
                //rangeVisualizer.SetActive(true);
                rangeVisualizer.transform.localScale = new Vector3(range[currentEvolution-1],range[currentEvolution-1],1);
            } 
        }
        else
        {
            rangeVisualizer.transform.localScale *= 0;
        }

    }

    private void CheckEvolutionMenu()
    {      
        if (Input.GetMouseButtonDown(0) /*&& enemyGen.isSpawning*/)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity/*, TowerLayer*/))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    evoMenu.SetActive(true);
                    //Debug.Log("Abrir menu evolução");
                }
                else if (hit.collider.gameObject != gameObject && !EventSystem.current.IsPointerOverGameObject())
                {
                    evoMenu.SetActive(false);
                }
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            evoMenu.SetActive(false);
        }
    }

    //Chamar quando evoluir torre
    public void EvolveTower()
    {
        if (bM.CurrentCoins >= evolvePrice[currentEvolution - 1] && currentEvolution < numberOfEvolutions)
        {
            //bM.CurrentCoins -= evolvePrice[currentEvolution - 1];
            bM.CurrentCoins -= (int) evolutionPrices[currentEvolution - 1].Evaluate(worldGen.CurrentWave - 1);
            currentEvolution++;
            mF.mesh = evolutionLooks[currentEvolution - 1];
        }

    }

    //Chamar quando vender torre
    public void SellTower()
    {
        bM.CurrentCoins += sellPrice[currentEvolution-1];
        //bM.CurrentCoins += sellPrices[currentEvolution-1[]];
        bM.allTowers.Remove(gameObject);
        Destroy(gameObject);
    }

}
