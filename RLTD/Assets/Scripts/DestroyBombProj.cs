using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBombProj : MonoBehaviour
{
    //public float lifeTime;

    public float damage;
    public float aoeRange;
    public LayerMask enemyLayer;

    private bool canExplode = true;

    public GameObject explodeVFX;

    private void Update()
    {
        //lifeTime -= Time.deltaTime;

        //if (lifeTime <= 0)
        //{
        //    Destroy(gameObject);
        //}

    }

    private void OnCollisionEnter(Collision other)
    {
        Collider[] allTargets = Physics.OverlapSphere(other.gameObject.transform.position, aoeRange, enemyLayer);
        for (int i = 0; i < allTargets.Length; i++)
        {
            allTargets[i].GetComponent<Enemy>().LoseHealth(damage);
        }
        if (canExplode)
        {
            Instantiate(explodeVFX, transform);
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            gameObject.GetComponent<TrailRenderer>().enabled = false;
            canExplode = false;
        }
        
        StartCoroutine(DestroyGO());
    }
    IEnumerator DestroyGO()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
