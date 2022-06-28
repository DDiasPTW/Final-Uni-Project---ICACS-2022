using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultTower : MonoBehaviour
{
    [Header("Tower AI")]
    public float startFireRate;
    [SerializeField] private GameObject currentTarget;
    private Tower tower;

    [Header("Visuals")]
    public GameObject projectile;
    public Transform projectileShootPos;
    public float projSpeed;


    private void Awake()
    {
        tower = GetComponent<Tower>();
        startFireRate = 0;
        currentTarget = null;
    }

    private void Update()
    {
        if (currentTarget == null)
        {
            GetTarget();
        }
        else
        {
            transform.LookAt(new Vector3(currentTarget.transform.position.x, transform.position.y, currentTarget.transform.position.z));

            startFireRate -= Time.deltaTime;
            float distanceToTarget = Vector3.Distance(transform.position, currentTarget.transform.position);


            Vector3 targetDir = currentTarget.GetComponent<Enemy>().cabeca.transform.position - transform.position;

            if (startFireRate <= 0)
            {
                AttackTarget(targetDir);
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

    private void AttackTarget(Vector3 targetDir)
    {
        GameObject proj = Instantiate(projectile,tower.shootPositions[tower.currentEvolution-1].transform.position, Quaternion.identity);
        proj.GetComponent<DestroyDefProj>().damage = tower.damage[tower.currentEvolution - 1];
        proj.GetComponent<Rigidbody>().AddForce(targetDir.normalized * projSpeed, ForceMode.Impulse);

        startFireRate = tower.fireRate[tower.currentEvolution - 1];
    }
}
