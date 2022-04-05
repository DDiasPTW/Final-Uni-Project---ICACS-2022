using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    private GameObject cameraPivot;
    public int currentEvolution = 1;
    public int numberOfEvolutions;
    public List<float> damage = new List<float>();
    public List<float> fireRate = new List<float>();
    [SerializeField]private float startFireRate;
    public List<float> range = new List<float>();
    public List<int> Price = new List<int>();

    public GameObject rangeVisualizer;
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
    private GameObject CurrentTarget;
    
    
    private void Awake()
    {
        cameraPivot = GameObject.FindGameObjectWithTag("Pivot");
        rangeVisualizer = Instantiate(rangeVisualizer);
        currentEvolution = 1;
    }


    private void Start()
    {
        startFireRate = 0;
        rangeVisualizer.GetComponent<SpriteRenderer>().color = rangeVisualizerColor;
    }


    private void Update()
    {
        //fazer mais smooth
        if (CurrentTarget == null)
        {
            transform.LookAt(new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z));

            GetTarget();
        }
        else
        {
            transform.LookAt(new Vector3(CurrentTarget.transform.position.x, transform.position.y, CurrentTarget.transform.position.z));

            startFireRate -= Time.deltaTime;
            if (isAOE && startFireRate <= 0)
            {
                AttackTargetAOE();
            }
            else if (!isAOE && startFireRate <= 0)
            {
                AttackTarget();
            }
        }

        CheckRange();
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
            CurrentTarget.GetComponent<Enemy>().LoseHealth(damage[currentEvolution-1], 0, 0);
        }
        else if (isPoison)
        {
            CurrentTarget.GetComponent<Enemy>().LoseHealth(damage[currentEvolution - 1], poisonMultiplier[currentEvolution-1], poisonTime[currentEvolution - 1]);
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
                allTargets[i].GetComponent<Enemy>().LoseHealth(damage[currentEvolution-1], 0, 0);
                //Debug.Log(allTargets[i].name + "Damage");
            }
            
        }
        else if (isPoison)
        {
            for (int i = 0; i < allTargets.Length; i++)
            {
                allTargets[i].GetComponent<Enemy>().LoseHealth(damage[currentEvolution-1], poisonMultiplier[currentEvolution - 1], poisonTime[currentEvolution - 1]);
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

    //Chamar quando evoluir torre
    public void EvolveTower()
    {
        currentEvolution++;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position,range[currentEvolution-1]/11);
        Gizmos.DrawWireSphere(CurrentTarget.transform.position,AOERange);
    }
}
