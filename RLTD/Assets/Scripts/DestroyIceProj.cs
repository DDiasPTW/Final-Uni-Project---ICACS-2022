using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyIceProj : MonoBehaviour
{
    public float lifeTime;

    public float damage;
    public float slowTime;
    public float slowMulti;
    public float aoeRange;
    public LayerMask enemyLayer;

    private void Update()
    {
        lifeTime -= Time.deltaTime;

        if (lifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        Collider[] allTargets = Physics.OverlapSphere(other.gameObject.transform.position, aoeRange, enemyLayer);

        for (int i = 0; i < allTargets.Length; i++)
        {
            allTargets[i].GetComponent<Enemy>().LoseHealth(damage);
            allTargets[i].GetComponent<Enemy>().ChangeSpeed(slowMulti, slowTime);
        }

        Destroy(gameObject);
    }
}
