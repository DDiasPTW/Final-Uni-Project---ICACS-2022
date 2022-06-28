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
    public List<float> AOERange = new List<float>(); 

    
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

    private void Start()
    {
        rangeVisualizer.GetComponent<SpriteRenderer>().color = rangeVisualizerColor;
    }


    private void Update()
    {
        CheckRange();
        CheckEvolutionMenu();        
    }


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
                    evoMenu.GetComponent<TowerEvoMenu>().currentTower = gameObject;
                }

            }
        }


        if (Input.GetMouseButtonDown(1))
        {
            evoMenu.GetComponent<TowerEvoMenu>().currentTower = null;
        }
    }

    //Chamar quando evoluir torre
    public void EvolveTower()
    {
        if (bM.CurrentCoins >= (int)evolutionPrices[currentEvolution - 1].Evaluate(worldGen.CurrentWave - 1) && currentEvolution < numberOfEvolutions)
        {
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
