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


            Vector3 targetDir = currentTarget.transform.position - transform.position;

            if (startFireRate <= 0)
            {
                AttackTarget();
                //Shoot(targetDir);
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

        currentTarget.GetComponent<Enemy>().LoseHealth(tower.damage[tower.currentEvolution - 1]);
        startFireRate = tower.fireRate[tower.currentEvolution - 1];
    }

    //public void Shoot(Vector3 direction) //Projetil e disparado diretamente ao inimigo
    //{
    //    GameObject proj;

    //    proj = Instantiate(projectile,projectileShootPos.position,Quaternion.identity);
    //    proj.GetComponent<Rigidbody>().AddForce(direction * speed,ForceMode.Impulse);
    //}
}
