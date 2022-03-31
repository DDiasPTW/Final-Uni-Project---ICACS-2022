using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public float range;
    public int Price;
    public LayerMask EnemyLayer;
    public bool isAOE;
    public float AOERange;

    [Header("Abilities")]
    [Header("Slow")]
    public bool isSlow;
    [Range(0,1)]
    public float slowMultiplier;
    public float slowTime;
    [Header("Poison")]
    public bool isPoison;
    [Range(0,1)]
    public float poisonMultiplier;
    public float poisonTime;

    [SerializeField] private GameObject CurrentTarget;
    public float fireRate;
    [SerializeField]private float startFireRate;
    public float damage;

    private void Start()
    {
        startFireRate = 0;
    }


    private void Update()
    {
        if (CurrentTarget == null)
        {
            GetTarget();
            
        }
        else
        {

            startFireRate -= Time.deltaTime;

            if (isAOE && startFireRate <= 0)
            {
                AttackTargetAOE();
            }else if (!isAOE && startFireRate <= 0)
            {
                AttackTarget();
            }           
        }
    }
    private void GetTarget()
    {
        Collider[] allTargets = Physics.OverlapSphere(transform.position, range / 11, EnemyLayer);

        if (allTargets.Length != 0)
        {
            CurrentTarget = allTargets[0].gameObject;
        }
    }

    private void AttackTarget()
    {
        if (!isPoison)
        {
            CurrentTarget.GetComponent<Enemy>().LoseHealth(damage, 0, 0);
        }
        else if (isPoison)
        {
            CurrentTarget.GetComponent<Enemy>().LoseHealth(damage, poisonMultiplier, poisonTime);
        }

        if (isSlow)
        {
            CurrentTarget.GetComponent<Enemy>().ChangeSpeed(slowMultiplier,slowTime);
        }
        
        startFireRate = fireRate;
    }

    private void AttackTargetAOE()
    {
        Collider[] allTargets = Physics.OverlapSphere(CurrentTarget.transform.position, AOERange, EnemyLayer);

        if (!isPoison)
        {
            for (int i = 0; i < allTargets.Length; i++)
            {
                allTargets[i].GetComponent<Enemy>().LoseHealth(damage, 0, 0);
                Debug.Log(allTargets[i].name + "Damage");
            }
            
        }
        else if (isPoison)
        {
            for (int i = 0; i < allTargets.Length; i++)
            {
                allTargets[i].GetComponent<Enemy>().LoseHealth(damage, poisonMultiplier, poisonTime);
                Debug.Log(allTargets[i].name + " Poison");
            }
            
        }

        if (isSlow)
        {
            for (int i = 0; i < allTargets.Length; i++)
            {
                allTargets[i].GetComponent<Enemy>().ChangeSpeed(slowMultiplier, slowTime);
                Debug.Log(allTargets[i].name + " Slow");
            }
            
        }

        startFireRate = fireRate;

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(CurrentTarget.transform.position,AOERange);
    }
}
