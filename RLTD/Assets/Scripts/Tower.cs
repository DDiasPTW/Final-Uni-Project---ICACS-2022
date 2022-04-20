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
    public int xp = 0;
    public List<Mesh> evolutionLooks = new List<Mesh>();
    public List<float> damage = new List<float>();
    public List<float> fireRate = new List<float>();
    [SerializeField]private float startFireRate;
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
    public bool isAOE;

    
    [Header("Slow")]
    public bool isSlow;
    [Range(0,1)]
    public List<float> slowMultiplier = new List<float>();
    public List<float> slowTime = new List<float>();
    [Header("Poison")]
    public bool isPoison;
    [Range(0,1)]
    public List<float> poisonMultiplier = new List<float>();
    public List<float> poisonTime = new List<float>();
    [SerializeField] private GameObject CurrentTarget;
    
    
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
        startFireRate = 0;
        rangeVisualizer.GetComponent<SpriteRenderer>().color = rangeVisualizerColor;
    }


    private void Update()
    {
        UpdateUIElements();

        //fazer mais smooth
        if (CurrentTarget == null)
        {
            //transform.LookAt(new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z));

            GetTarget();
        }
        else
        {
            //transform.LookAt(new Vector3(CurrentTarget.transform.position.x, transform.position.y, CurrentTarget.transform.position.z));

            startFireRate -= Time.deltaTime;
            float distanceToTarget = Vector3.Distance(transform.position, CurrentTarget.transform.position);          


            if (isAOE && startFireRate <= 0)
            {
                AttackTargetAOE();
            }
            else if (!isAOE && startFireRate <= 0)
            {
                AttackTarget();
            }

            //limpa o target caso saia da range
            if ((distanceToTarget * 11.5f) > range[currentEvolution - 1])
            {
                CurrentTarget = null;
            }
        }

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
        rangeVisualizer.transform.position = new Vector3(transform.position.x,transform.position.y / 1.9f,transform.position.z);
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
        if (Input.GetMouseButtonDown(0))
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
            else
            {

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

    #region Tower AI
    private void GetTarget()
    {
        Collider[] allTargets = Physics.OverlapSphere(transform.position, range[currentEvolution-1] / 11, EnemyLayer);

        if (allTargets.Length != 0)
        {
            CurrentTarget = allTargets[0].gameObject;            
        }
    }

    private void AttackTarget()
    {
        if (!isPoison)
        {
            CurrentTarget.GetComponent<Enemy>().LoseHealth(damage[currentEvolution-1]);
        }
        else if (isPoison)
        {
            CurrentTarget.GetComponent<Enemy>().LoseHealth(damage[currentEvolution - 1]);
            CurrentTarget.GetComponent<Enemy>().currentColor = CurrentTarget.GetComponent<Enemy>().PoisonTextColor;
            CurrentTarget.GetComponent<Enemy>().poisonTime = poisonTime[currentEvolution - 1];
            CurrentTarget.GetComponent<Enemy>().poisonMultiplier = poisonMultiplier[currentEvolution - 1];
        }

        if (isSlow)
        {
            CurrentTarget.GetComponent<Enemy>().ChangeSpeed(slowMultiplier[currentEvolution - 1], slowTime[currentEvolution - 1]);
        }
        
        startFireRate = fireRate[currentEvolution-1];
    }

    private void AttackTargetAOE()
    {
        Collider[] allTargets = Physics.OverlapSphere(CurrentTarget.transform.position, AOERange, EnemyLayer);

        if (!isPoison)
        {
            for (int i = 0; i < allTargets.Length; i++)
            {
                allTargets[i].GetComponent<Enemy>().LoseHealth(damage[currentEvolution-1]);
                //Debug.Log(allTargets[i].name + "Damage");
            }
            
        }
        else if (isPoison)
        {
            for (int i = 0; i < allTargets.Length; i++)
            {
                allTargets[i].GetComponent<Enemy>().poisonTime = poisonTime[currentEvolution - 1];
                allTargets[i].GetComponent<Enemy>().currentColor = allTargets[i].GetComponent<Enemy>().PoisonTextColor;
                allTargets[i].GetComponent<Enemy>().poisonMultiplier = poisonMultiplier[currentEvolution - 1];
                allTargets[i].GetComponent<Enemy>().LoseHealth(damage[currentEvolution-1]);
                //Debug.Log(allTargets[i].name + " Poison");
            }
            
        }

        if (isSlow)
        {
            for (int i = 0; i < allTargets.Length; i++)
            {
                allTargets[i].GetComponent<Enemy>().ChangeSpeed(slowMultiplier[currentEvolution - 1], slowTime[currentEvolution - 1]);
                //Debug.Log(allTargets[i].name + " Slow");
            }
            
        }

        startFireRate = fireRate[currentEvolution-1];

    }

    #endregion
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position,range[currentEvolution-1]/11);
        Gizmos.DrawWireSphere(CurrentTarget.transform.position,AOERange);
    }
}
