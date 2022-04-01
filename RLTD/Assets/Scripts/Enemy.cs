using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private NavMeshAgent navAgent;
    private BuildManager bM;
    private EnemyGeneration enemyGen;
    private GameManager gM;
    public float Health;
    public int Value;
    public int Damage;
    public float speed;
    [SerializeField]private bool isPoison = false;
    [SerializeField]private bool isSlow = false;


    [Header("Resistencias")]
    public bool resSlow;
    public bool resPoison;


    private float slowTimeElapsed;
    private float poisonTimeElapsed;

    private void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.speed = speed;
        bM = GameObject.FindGameObjectWithTag("GridManager").GetComponent<BuildManager>();
        enemyGen = GameObject.FindGameObjectWithTag("GridManager").GetComponent<EnemyGeneration>();
        gM = GameObject.FindGameObjectWithTag("GridManager").GetComponent<GameManager>();
    }
    void Start()
    {
        navAgent.SetDestination(GameObject.FindGameObjectWithTag("TARGET").transform.position);
    }

    private void Update()
    {
        //Debug.Log(Vector3.Distance(gameObject.transform.position,navAgent.destination));


        if (Vector3.Distance(gameObject.transform.position, navAgent.destination) < .8f)
        {
            gM.LoseHealth(Damage);
            enemyGen.spawnedEnemies.Remove(gameObject);
            Destroy(gameObject);
        }
        if (Health <= 0)
        {
            bM.CurrentCoins += Value;
            enemyGen.spawnedEnemies.Remove(gameObject);
            Destroy(gameObject);
        }


        if (isPoison)
        {
            poisonTimeElapsed -= Time.deltaTime;
            if (poisonTimeElapsed <= 0)
            {
                isPoison = false;
                
            }
        }

        if (isSlow)
        {
            slowTimeElapsed -= Time.deltaTime;
            if (slowTimeElapsed <= 0)
            {
                isSlow = false;
                
            }
        }
        else if (!isSlow)
        {
            navAgent.speed = speed;
        }

    }

    public void LoseHealth(float damage, float multiplier, float poisonTime)
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
        }
        else if(isPoison)
        {
            Health -= damage + (damage * multiplier);
        }    
    }

    public void ChangeSpeed(float slowMultiplier, float slowTime)
    {
        if (!resSlow)//apenas é afetado caso não tenha resistência a esse elemento
        {
            navAgent.speed = speed - (speed * slowMultiplier);
            slowTimeElapsed = slowTime;
            isSlow = true;
        }         
    }
    
}
