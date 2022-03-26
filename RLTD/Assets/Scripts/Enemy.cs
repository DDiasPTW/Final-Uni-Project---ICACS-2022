using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private NavMeshAgent navAgent;
    public float Health;
    public float speed;

    private void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.speed = speed;
    }
    void Start()
    {
        navAgent.SetDestination(GameObject.FindGameObjectWithTag("TARGET").transform.position);
    }

    
}
