using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class Enemy : MonoBehaviour
{
    private GameObject cameraPivot;
    private NavMeshAgent navAgent;
    private BuildManager bM;
    private EnemyGeneration enemyGen;
    private GameManager gM;
    private ItemManager iM;
    public float Health;
    public int Value;
    public int Damage;
    public float speed;

    public GameObject damageIndicatorText;
    public Color NormalTextColor, PoisonTextColor, SlowTextColor;
    public Color currentColor;

    [SerializeField]private bool isPoison = false;
    [SerializeField]private bool isSlow = false;

    [Header("Aparencia")]
    public GameObject acessorio;
    private MeshFilter mF_acess;
    public GameObject cabeca;
    private MeshFilter mF_cabeca;
    public GameObject corpo;
    private MeshFilter mF_corpo;
    public GameObject pes;
    private MeshFilter mF_pes;

    [Header("Itens")]
    [Range(0,100)]
    public float itemDropChance;
    [SerializeField] private bool willDropItem;

    [Header("Resistencias")]
    public bool resSlow;
    public bool resPoison;


    public float poisonTime, poisonMultiplier;
    [SerializeField] private float slowMulti;

    [SerializeField] private float slowTimeElapsed;
    private float poisonTimeElapsed;

    private Vector3 startScale;
    private float startHealth;


    private void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.speed = speed;
        bM = GameObject.FindGameObjectWithTag("GridManager").GetComponent<BuildManager>();
        enemyGen = GameObject.FindGameObjectWithTag("GridManager").GetComponent<EnemyGeneration>();
        gM = GameObject.FindGameObjectWithTag("GridManager").GetComponent<GameManager>();
        iM = GameObject.FindGameObjectWithTag("GridManager").GetComponent<ItemManager>();
        cameraPivot = GameObject.FindGameObjectWithTag("Pivot");

        mF_acess = transform.GetChild(0).GetComponent<MeshFilter>();
        mF_cabeca = transform.GetChild(1).GetComponent<MeshFilter>();
        mF_corpo = transform.GetChild(2).GetComponent<MeshFilter>();
        mF_pes = transform.GetChild(3).GetComponent<MeshFilter>();
        
        

        //startScale = transform.localScale;
        startHealth = Health;
        currentColor = NormalTextColor;

    }

    private void SetLooks()
    {
        //mF_acess.mesh = enemyGen.chosen_acessorio.GetComponent<MeshFilter>().sharedMesh;
        //acessorio.GetComponent<MeshRenderer>().material = enemyGen.chosen_acessorio.GetComponent<MeshRenderer>().material;

        mF_cabeca.mesh = enemyGen.chosen_cabeca.GetComponent<MeshFilter>().sharedMesh;
        //cabeca.GetComponent<MeshRenderer>().material = enemyGen.chosen_cabeca.GetComponent<MeshRenderer>().material;

        mF_corpo.mesh = enemyGen.chosen_corpo.GetComponent<MeshFilter>().sharedMesh;
        //corpo.GetComponent<MeshRenderer>().material = enemyGen.chosen_corpo.GetComponent<MeshRenderer>().material;

        mF_pes.mesh = enemyGen.chosen_pes.GetComponent<MeshFilter>().sharedMesh;
        //pes.GetComponent<MeshRenderer>().material = enemyGen.chosen_pes.GetComponent<MeshRenderer>().material;
    }
    void Start()
    {
        SetLooks();
        navAgent.SetDestination(GameObject.FindGameObjectWithTag("TARGET").transform.position);

        float itemDrop = Random.Range(0f,100f);
        if (itemDrop <= itemDropChance)
        {
            willDropItem = true;
        }
        else willDropItem = false;
    }

    private void Update()
    {
        if (Vector3.Distance(gameObject.transform.position, navAgent.destination) < .8f)
        {
            gM.LoseHealth(Damage);
            enemyGen.spawnedEnemies.Remove(gameObject);
            Destroy(gameObject);
        }
        if (Health <= 0)
        {
            if (willDropItem) //DROP DE ITENS
            {
                DropItem();
            }


            bM.CurrentCoins += Value;
            enemyGen.spawnedEnemies.Remove(gameObject);
            Destroy(gameObject);
        }

        //transform.localScale = startScale * (Health/startHealth);
        UpdateTextColor();

        if (isPoison)
        {
            poisonTimeElapsed -= Time.deltaTime;
            if (poisonTimeElapsed <= 0)
            {
                isPoison = false;
            }
        }
        
        //Slow effect
        if (isSlow)
        {          
            if (slowTimeElapsed <= 0)
            {
                isSlow = false;
            }
            else
            {
                navAgent.speed = speed - (speed * slowMulti);
                slowTimeElapsed -= Time.deltaTime;
            }
        }
        else
        {
            navAgent.speed = speed;
        }

    }

    private void UpdateTextColor()
    {
        if (isPoison)
        {
            currentColor = PoisonTextColor;
        }
        else if (isSlow)
        {
            currentColor = SlowTextColor;
        }
        else currentColor = NormalTextColor;
    }

    private void DropItem()
    {
        int whatItem = Random.Range(0,iM.allItems.Count);
        Instantiate(iM.allItems[whatItem],transform.position + Vector3.up,Quaternion.identity);
        //Debug.Log("Item chosen is: " + iM.allItems[whatItem].name);
    }

    private void ShowDamage(float value)
    {
        GameObject text;
        text = Instantiate(damageIndicatorText, transform.position + new Vector3(0, 2f, 0), Quaternion.identity);
        text.transform.localEulerAngles = new Vector3(45f, cameraPivot.transform.localEulerAngles.y, text.transform.localEulerAngles.z);
        text.GetComponent<TextMeshPro>().text = value.ToString();   
        text.GetComponent<TextMeshPro>().color = currentColor;
    }

    public void LoseHealth(float damage)
    {     
        if (!resPoison) //apenas é afetado caso não tenha resistência a esse elemento
        {
            poisonTimeElapsed = poisonTime;
            if (poisonTime > 0)
            {
                isPoison = true;
            }
        }
        
        if (!isPoison)
        {            
            Health -= damage;
            ShowDamage(damage);
        }
        else if(isPoison)
        {            
            currentColor = PoisonTextColor;

            Health -= damage + (damage * poisonMultiplier);
            ShowDamage(damage + (damage * poisonMultiplier));
        }    
    }

    public void ChangeSpeed(float slowMultiplier, float slowTime)
    {
        if (!resSlow)//apenas é afetado caso não tenha resistência a esse elemento
        {
            isSlow = true;
            currentColor = SlowTextColor;           
            slowMulti = slowMultiplier;
            slowTimeElapsed = slowTime;
        }
    }
    
}
