using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonTower : MonoBehaviour
{
    [Header("Tower AI")]
    public float startFireRate;
    [SerializeField] private GameObject currentTarget;
    private Tower tower;
    [Header("Visuals")]
    public GameObject projectile;
    public Transform projectileShootPos;
    public float speed;

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
                Shoot(currentTarget.transform.position);
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
            allTargets[i].GetComponent<Enemy>().poisonTime = tower.poisonTime[tower.currentEvolution - 1];
            allTargets[i].GetComponent<Enemy>().currentColor = allTargets[i].GetComponent<Enemy>().PoisonTextColor;
            allTargets[i].GetComponent<Enemy>().poisonMultiplier = tower.poisonMultiplier[tower.currentEvolution - 1];
            allTargets[i].GetComponent<Enemy>().LoseHealth(tower.damage[tower.currentEvolution - 1]);
        }

        startFireRate = tower.fireRate[tower.currentEvolution - 1];
    }

    public void Shoot(Vector3 position) //Projetil da spawn na posicao do inimigo e "explode"
    {
        GameObject proj;

        proj = Instantiate(projectile, position, Quaternion.identity);
        //VFX

        //Debug.Log("spawned proj");
    }
}
