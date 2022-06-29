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
    public BoxCollider enemyDetectColl;
    public List<ParticleSystem> poisonVisuals = new List<ParticleSystem>();

    private void Awake()
    {
        tower = GetComponent<Tower>();
        startFireRate = 0;
        currentTarget = null;
    }

    private void Update()
    {
        enemyDetectColl.size = new Vector3(1,.75f, tower.range[tower.currentEvolution - 1] / tower.worldGen.chunkSize);
        enemyDetectColl.center = new Vector3(0,0,enemyDetectColl.size.z / 2);


        if (currentTarget == null)
        {
            for (int i = 0; i < poisonVisuals.Count; i++)
            {
                poisonVisuals[i].Stop();
            }     
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

    private void AttackTarget()
    {      
        poisonVisuals[tower.currentEvolution - 1].Play();
        for (int i = 0; i < enemyDetectColl.GetComponent<PoisonTargets>().enemies.Count; i++)
        {
            //enemyDetectColl.GetComponent<PoisonTargets>().enemies[i].GetComponent<Enemy>().poisonTime = tower.poisonTime[tower.currentEvolution - 1];
            //enemyDetectColl.GetComponent<PoisonTargets>().enemies[i].GetComponent<Enemy>().poisonMultiplier = tower.poisonMultiplier[tower.currentEvolution - 1];
            enemyDetectColl.GetComponent<PoisonTargets>().enemies[i].GetComponent<Enemy>().SetPoison(tower.poisonMultiplier[tower.currentEvolution - 1], tower.poisonTime[tower.currentEvolution - 1]);
            enemyDetectColl.GetComponent<PoisonTargets>().enemies[i].GetComponent<Enemy>().LoseHealth(tower.damage[tower.currentEvolution - 1]);

        }
        startFireRate = tower.fireRate[tower.currentEvolution - 1];
    }

    private void OnDrawGizmos()
    {
        if (currentTarget != null)
        {
            Gizmos.DrawWireSphere(currentTarget.transform.position,tower.AOERange[tower.currentEvolution-1]);
        }
    }
}
