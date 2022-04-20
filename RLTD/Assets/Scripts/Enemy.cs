using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    [Header("Itens")]
    [Range(0,100)]
    public float itemDropChance;
    [SerializeField] private bool willDropItem;

    [Header("Resistencias")]
    public bool resSlow;
    public bool resPoison;


    public float poisonTime, poisonMultiplier;

    private float slowTimeElapsed;
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

        startScale = transform.localScale;
        startHealth = Health;
        currentColor = NormalTextColor;
    }
    void Start()
    {
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

        transform.localScale = startScale * (Health/startHealth);
        UpdateTextColor();

        if (isPoison)
        {
            //currentColor = PoisonTextColor;
            poisonTimeElapsed -= Time.deltaTime;
            if (poisonTimeElapsed <= 0)
            {
                isPoison = false;
                //currentColor = NormalTextColor;
            }
        }
        
        if (isSlow)
        {
            slowTimeElapsed -= Time.deltaTime;
            //currentColor = SlowTextColor;
            if (slowTimeElapsed <= 0)
            {
                isSlow = false;
                
                
            }
        }
        else if (!isSlow)
        {
            navAgent.speed = speed;
            //currentColor = NormalTextColor;
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
            ShowDamage(damage);
            Health -= damage;
        }
        else if(isPoison)
        {            
            currentColor = PoisonTextColor;

            ShowDamage(damage + (damage * poisonMultiplier));
            Health -= damage + (damage * poisonMultiplier);
        }    
    }

    public void ChangeSpeed(float slowMultiplier, float slowTime)
    {
        if (!resSlow)//apenas é afetado caso não tenha resistência a esse elemento
        {
            currentColor = SlowTextColor;
            navAgent.speed = speed - (speed * slowMultiplier);
            slowTimeElapsed = slowTime;
            isSlow = true;
        }         
    }
    
}
