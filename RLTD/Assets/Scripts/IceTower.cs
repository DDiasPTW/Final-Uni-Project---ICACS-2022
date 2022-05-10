using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceTower : MonoBehaviour
{
    //FAZER MOVIMENTO DE PROJETIL (PARABOLA, USANDO VETOR FRENTE E UP)
    [Header("Tower AI")]
    private float startFireRate;
    [SerializeField] private GameObject currentTarget;
    private Tower tower;
    [Header("Visuals")]
    public GameObject iceProjectile;
    public Transform projExit;
    public float multiplier;
    public float timeOfFlight;


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
                //Shoot(currentTarget.transform.position);
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
            allTargets[i].GetComponent<Enemy>().ChangeSpeed(tower.slowMultiplier[tower.currentEvolution - 1], tower.slowTime[tower.currentEvolution - 1]);
        }
        //Shoot(currentTarget.transform.position);
        startFireRate = tower.fireRate[tower.currentEvolution - 1];
    }


    public void Shoot(Vector3 targetPos) //Projetil e lancado para cima, deve ter SEMPRE o mesmo tempo de voo. tem uma trajetoria 'parabolica'
    {
        GameObject projectile = Instantiate(iceProjectile,projExit.position, Quaternion.identity);
        Vector3 distance = targetPos - transform.position;

        projectile.GetComponent<Rigidbody>().velocity = (((transform.forward + Vector3.up) * distance.magnitude) / timeOfFlight) * multiplier;
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawWireSphere(transform.position, tower.range[tower.currentEvolution - 1] / 11);
        Gizmos.DrawWireSphere(currentTarget.transform.position, tower.AOERange);
    }
}
