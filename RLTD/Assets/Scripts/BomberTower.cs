using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BomberTower : MonoBehaviour
{
    [Header("Tower AI")]
    [SerializeField] private float startFireRate;
    [SerializeField] private GameObject currentTarget;
    private Tower tower;
    [Header("Visuals")]
    public GameObject bomberProjectile;
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

            Vector3 lookPos = currentTarget.GetComponent<Enemy>().cabeca.transform.position - transform.position;

            if (startFireRate <= 0)
            {
                AttackTarget(lookPos);
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
        float distance = 1000000;

        if (allTargets.Length != 0)
        {
            for (int i = 0; i < allTargets.Length; i++)
            {
                if (Vector3.Distance(transform.position, allTargets[i].transform.position) < distance)
                {
                    currentTarget = allTargets[i].gameObject;
                    distance = Vector3.Distance(transform.position, allTargets[i].transform.position);
                }

            }
            //currentTarget = allTargets[0].gameObject;

        }
    }

    private void AttackTarget(Vector3 lookPos)
    {
        GameObject proj = Instantiate(bomberProjectile,tower.shootPositions[tower.currentEvolution - 1].transform.position, Quaternion.identity);

        proj.GetComponent<DestroyBombProj>().damage = tower.damage[tower.currentEvolution -1];
        proj.GetComponent<DestroyBombProj>().aoeRange = tower.AOERange[tower.currentEvolution - 1];
        proj.GetComponent<DestroyBombProj>().enemyLayer = tower.EnemyLayer;
        proj.GetComponent<DestroyBombProj>().currentEvo = tower.currentEvolution-1;
        proj.GetComponent<Rigidbody>().AddForce(lookPos * projSpeed, ForceMode.Impulse);

        startFireRate = tower.fireRate[tower.currentEvolution - 1];
    }

    private void OnDrawGizmos()
    {
        if (currentTarget != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(currentTarget.transform.position, tower.AOERange[tower.currentEvolution - 1]);
        }
    }
}
