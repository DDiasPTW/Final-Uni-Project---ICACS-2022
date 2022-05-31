using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BomberTower : MonoBehaviour
{
    [Header("Tower AI")]
    private float startFireRate;
    [SerializeField] private GameObject currentTarget;
    private Tower tower;

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
        Collider[] allTargets = Physics.OverlapSphere(currentTarget.transform.position, tower.AOERange, tower.EnemyLayer);
        for (int i = 0; i < allTargets.Length; i++)
        {
            allTargets[i].GetComponent<Enemy>().LoseHealth(tower.damage[tower.currentEvolution - 1]);
        }
        //Shoot(currentTarget.transform.position);
        startFireRate = tower.fireRate[tower.currentEvolution - 1];
    }
}
