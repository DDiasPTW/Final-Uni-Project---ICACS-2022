using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyDefProj : MonoBehaviour
{
    public float lifeTime;
    public float damage;
    private void Update()
    {
        lifeTime-= Time.deltaTime;

        if (lifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Inimigo"))
        {
            other.gameObject.GetComponent<Enemy>().LoseHealth(damage);
            Destroy(gameObject);
        }
        Destroy(gameObject);
    }
}
