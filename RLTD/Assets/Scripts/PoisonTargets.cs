using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonTargets : MonoBehaviour
{
    public List<GameObject> enemies = new List<GameObject>();

    private void Update()
    {
        CheckEnemies();
    }

    private void CheckEnemies()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i] == null)
            {
                enemies.RemoveAt(i);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Inimigo"))
        {
            enemies.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Inimigo"))
        {
            enemies.Remove(other.gameObject);
        }
    }
}
