using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrimTower : MonoBehaviour
{
    private Tower tower;
    public List<float> brimValues = new List<float>();
    private void Awake()
    {
        tower = GetComponent<Tower>();
    }

    private void Update()
    {
        BrimTowers();
    }

    private void BrimTowers()
    {
        Collider[] allTargets = Physics.OverlapSphere(transform.position, tower.range[tower.currentEvolution - 1] / 11, tower.TowerLayer);

        for (int i = 0; i < allTargets.Length; i++)
        {
            float normalFireRate = allTargets[i].GetComponent<Tower>().fireRate[allTargets[i].GetComponent<Tower>().currentEvolution - 1];

            allTargets[i].GetComponent<Tower>().fireRate[allTargets[i].GetComponent<Tower>().currentEvolution - 1] = normalFireRate + (normalFireRate * brimValues[tower.currentEvolution - 1]);
        }
    }
}
