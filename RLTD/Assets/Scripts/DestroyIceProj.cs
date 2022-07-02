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
    public int currentEvo;

    private bool canExplode = true;
    public List<GameObject> explodeVFX = new List<GameObject>();
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
        

        foreach (Collider target in allTargets)
        {
            
            target.GetComponent<Enemy>().LoseHealth(damage);
            target.GetComponent<Enemy>().ChangeSpeed(slowMulti, slowTime);
        }
        //for (int i = 0; i < allTargets.Length; i++)
        //{
        //    allTargets[i].GetComponent<Enemy>().LoseHealth(damage);
        //    allTargets[i].GetComponent<Enemy>().ChangeSpeed(slowMulti, slowTime);
        //}
        if (canExplode)
        {
            Instantiate(explodeVFX[currentEvo], transform);
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            gameObject.GetComponent<TrailRenderer>().enabled = false;
            canExplode = false;
        }
        transform.GetComponent<SphereCollider>().enabled = false;
        StartCoroutine(DestroyGO());
    }

    IEnumerator DestroyGO()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
