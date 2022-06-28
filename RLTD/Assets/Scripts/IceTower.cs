using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceTower : MonoBehaviour
{
    [Header("Tower AI")]
    [SerializeField]private float startFireRate;
    [SerializeField] private GameObject currentTarget;
    private Tower tower;
    [Header("Visuals")]
    public GameObject iceProjectile;
    public float projSpeed;


    private void Awake()
    {
        tower = GetComponent<Tower>();
        startFireRate = 0;
        currentTarget = null;
    }

    private void Update()
    {
        startFireRate -= Time.deltaTime;
        if (currentTarget == null)
        {
            GetTarget();
        }
        else
        {
            transform.LookAt(new Vector3(currentTarget.transform.position.x, transform.position.y, currentTarget.transform.position.z));

            float distanceToTarget = Vector3.Distance(transform.position, currentTarget.transform.position);

            if (startFireRate <= 0)
            {
                AttackTarget();
            }

            //limpa o target caso saia da range
            if ((distanceToTarget * 11.1f) > tower.range[tower.currentEvolution - 1])
            {
                currentTarget = null;
            }
        }
    }


    private void GetTarget()
    {
        Collider[] allTargets = Physics.OverlapSphere(transform.position, tower.range[tower.currentEvolution - 1] / 11, tower.EnemyLayer);

        if (allTargets.Length != 0)
        {
            currentTarget = allTargets[0].gameObject;
        }
    }

    private void AttackTarget()
    {
        Shoot((currentTarget.GetComponent<Enemy>().cabeca.transform.position - transform.position).normalized);

        
        startFireRate = tower.fireRate[tower.currentEvolution - 1];
    }


    public void Shoot(Vector3 targetPos) 
    {
        GameObject projectile = Instantiate(iceProjectile,tower.shootPositions[tower.currentEvolution - 1].transform.position, Quaternion.identity);

        projectile.GetComponent<DestroyIceProj>().damage = tower.damage[tower.currentEvolution-1];
        projectile.GetComponent<DestroyIceProj>().aoeRange = tower.AOERange[tower.currentEvolution - 1];
        projectile.GetComponent<DestroyIceProj>().slowMulti = tower.slowMultiplier[tower.currentEvolution-1];
        projectile.GetComponent<DestroyIceProj>().currentEvo = tower.currentEvolution-1;
        projectile.GetComponent<DestroyIceProj>().slowTime = tower.slowTime[tower.currentEvolution-1];
        projectile.GetComponent<DestroyIceProj>().enemyLayer = tower.EnemyLayer;
        
        projectile.GetComponent<Rigidbody>().AddForce(targetPos * projSpeed,ForceMode.Impulse);
    }

    private void OnDrawGizmos()
    {
        if (currentTarget != null)
        {
            Gizmos.DrawWireSphere(currentTarget.transform.position, tower.AOERange[tower.currentEvolution - 1]);
        }
    }
}
