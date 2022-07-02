using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBombProj : MonoBehaviour
{
    public float damage;
    public float aoeRange;
    public LayerMask enemyLayer;
    public int currentEvo;
    private bool canExplode = true;

    public List<GameObject> explodeVFX = new List<GameObject>();

    private void OnCollisionEnter(Collision other)
    {
        Collider[] allTargets = Physics.OverlapSphere(other.gameObject.transform.position, aoeRange, enemyLayer);

        foreach (Collider target in allTargets)
        {
            Debug.Log(target.name);
            target.GetComponent<Enemy>().LoseHealth(damage);
        }
        //for (int i = 0; i < allTargets.Length; i++)
        //{
        //    allTargets[i].GetComponent<Enemy>().LoseHealth(damage);
        //}
        if (canExplode)
        {
            Instantiate(explodeVFX[currentEvo], transform);
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            gameObject.GetComponent<TrailRenderer>().enabled = false;
            canExplode = false;
        }
        
        StartCoroutine(DestroyGO());
        transform.GetComponent<SphereCollider>().enabled = false;
    }
    IEnumerator DestroyGO()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
