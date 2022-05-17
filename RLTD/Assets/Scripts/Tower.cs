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

    [Header("Evolution Stuff")]
    public int currentEvolution = 1; 
    public int numberOfEvolutions;

    public List<Mesh> evolutionLooks = new List<Mesh>(); 
    public List<float> damage = new List<float>();
    public List<float> fireRate = new List<float>();
    public List<float> range = new List<float>(); 
    public List<int> evolvePrice = new List<int>();
    public List<int> sellPrice = new List<int>();

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
        cameraPivot = GameObject.FindGameObjectWithTag("Pivot");
        rangeVisualizer = Instantiate(rangeVisualizer);
        currentEvolution = 1;
        evoMenu.SetActive(false);
    }


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
            evoPriceText.text = evolvePrice[currentEvolution-1].ToString();
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

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, TowerLayer))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    evoMenu.SetActive(true);
                    //Debug.Log("Abrir menu evolução");
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
            bM.CurrentCoins -= evolvePrice[currentEvolution - 1];
            currentEvolution++;
            mF.mesh = evolutionLooks[currentEvolution - 1];
        }

    }

}
