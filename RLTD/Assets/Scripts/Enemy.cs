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
    private WorldGeneration worldGen;

    public float Health;
    public int Value;
    public int Damage;
    public float speed;
    

    public GameObject damageIndicatorText;

    public GameObject slowIndicator, poisonIndicator;

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


    private void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
        
        bM = GameObject.FindGameObjectWithTag("GridManager").GetComponent<BuildManager>();
        enemyGen = GameObject.FindGameObjectWithTag("GridManager").GetComponent<EnemyGeneration>();
        gM = GameObject.FindGameObjectWithTag("GridManager").GetComponent<GameManager>();
        iM = GameObject.FindGameObjectWithTag("GridManager").GetComponent<ItemManager>();
        worldGen = GameObject.FindGameObjectWithTag("GridManager").GetComponent<WorldGeneration>();
        cameraPivot = GameObject.FindGameObjectWithTag("Pivot");

        mF_acess = transform.GetChild(0).GetChild(0).GetComponent<MeshFilter>();
        mF_cabeca = transform.GetChild(0).GetChild(1).GetComponent<MeshFilter>();
        mF_corpo = transform.GetChild(0).GetChild(2).GetComponent<MeshFilter>();
        mF_pes = transform.GetChild(0).GetChild(3).GetComponent<MeshFilter>();

        SetLooks();

        slowIndicator.SetActive(false);
        poisonIndicator.SetActive(false);
        //currentColor = NormalTextColor;
    }

    private void SetLooks()
    {
        BoxCollider pesColl = enemyGen.chosen_pes.GetComponent<BoxCollider>();
        BoxCollider corpoColl = enemyGen.chosen_corpo.GetComponent<BoxCollider>();
        BoxCollider cabecaColl = enemyGen.chosen_cabeca.GetComponent<BoxCollider>();
        BoxCollider acessColl = enemyGen.chosen_acessorio.GetComponent<BoxCollider>();

        pes.transform.localPosition = new Vector3(0, pesColl.size.y / 2, 0);

        corpo.transform.localPosition = new Vector3(0, corpoColl.size.y / 2 + pesColl.size.y, 0);

        cabeca.transform.localPosition = new Vector3(0, cabecaColl.size.y / 2 + corpoColl.size.y + pesColl.size.y, 0);

        acessorio.transform.localPosition = new Vector3(0,acessColl.size.y / 2 + cabecaColl.size.y + corpoColl.size.y + pesColl.size.y,0);

        //Pes - muda a velocidade que inimigo ira ter
        mF_pes.mesh = enemyGen.chosen_pes.GetComponent<MeshFilter>().sharedMesh;
        speed = enemyGen.chosen_pes.GetComponent<Pes>().speed;
        navAgent.speed = speed;

        //Corpo - muda quanta vida inimigo ira ter
        mF_corpo.mesh = enemyGen.chosen_corpo.GetComponent<MeshFilter>().sharedMesh;
        Health = enemyGen.chosen_corpo.GetComponent<Corpo>().health;
        Value = enemyGen.chosen_corpo.GetComponent<Corpo>().value;
        
        //Cabeca - muda quanto dano inimigo ira dar na base e muda tambem quanto dinheiro esse inimigo da quando morre
        mF_cabeca.mesh = enemyGen.chosen_cabeca.GetComponent<MeshFilter>().sharedMesh;
        Damage = enemyGen.chosen_cabeca.GetComponent<Head>().damage;

        //Acessorio - muda que resistencia inimigo vai ter
        mF_acess.mesh = enemyGen.chosen_acessorio.GetComponent<MeshFilter>().sharedMesh;
        resSlow = enemyGen.chosen_acessorio.GetComponent<Acessorio>().resSlow;
        resPoison = enemyGen.chosen_acessorio.GetComponent<Acessorio>().resPoison;
    }
    void Start()
    {
        float distance = 10000000;
        int t = 0;

        for (int i = 0; i < worldGen.allTargets.Count; i++)
        {
            if (Vector3.Distance(transform.position, worldGen.allTargets[i].transform.position) < distance)
            {
                distance = Vector3.Distance(transform.position, worldGen.allTargets[i].transform.position);
                t = i;
            }
        }
        navAgent.SetDestination(worldGen.allTargets[t].transform.position);

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
            //currentColor = PoisonTextColor;
            poisonIndicator.SetActive(true);
            poisonIndicator.transform.eulerAngles = new Vector3(45f, cameraPivot.transform.localEulerAngles.y, transform.eulerAngles.z);
        }
        else poisonIndicator.SetActive(false);

        if (isSlow)
        {
            //currentColor = SlowTextColor;
            slowIndicator.SetActive(true);
            slowIndicator.transform.eulerAngles = new Vector3(45f, cameraPivot.transform.localEulerAngles.y, transform.eulerAngles.z);
        }
        else slowIndicator.SetActive(false);
    }

    private void DropItem()
    {
        int whatItem = Random.Range(0,iM.allItems.Count);
        Instantiate(iM.allItems[whatItem],transform.position + new Vector3(0,3f,0),Quaternion.identity);
        //Debug.Log("Item chosen is: " + iM.allItems[whatItem].name);
    }

    private void ShowDamage(float value)
    {
        int valueToShow = (int) value;
        GameObject text;
        text = Instantiate(damageIndicatorText, transform.position + new Vector3(0, 2f, 0), Quaternion.identity);
        text.transform.localEulerAngles = new Vector3(45f, cameraPivot.transform.localEulerAngles.y, text.transform.localEulerAngles.z);
        text.GetComponent<TextMeshPro>().text = valueToShow.ToString();   
    }

    public void LoseHealth(float damage)
    {     
        if (!resPoison) //apenas é afetado caso não tenha resistência a esse elemento
        {
            poisonTimeElapsed = poisonTime;
            if (poisonTimeElapsed > 0)
            {
                isPoison = true;
            }
        }
        
        if (!isPoison)
        {            
            Health -= damage;
        }
        else if(isPoison)
        {            
            Health -= damage + (damage * poisonMultiplier);            
        }
        ShowDamage(damage + (damage * poisonMultiplier));
    }

    public void ChangeSpeed(float slowMultiplier, float slowTime)
    {
        if (!resSlow)//apenas é afetado caso não tenha resistência a esse elemento
        {
            isSlow = true;         
            slowMulti = slowMultiplier;
            slowTimeElapsed = slowTime;
        }
    }

    public void ItemChangeSpeed(float slowMultiplier, float slowTime)
    {
        isSlow = true;
        slowMulti = slowMultiplier;
        slowTimeElapsed = slowTime;
    }
    
}
