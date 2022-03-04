using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGridGeneration : MonoBehaviour
{
    [Header("Full Grid Dimensions - MUST BE ODD")]
    public int GridX;
    public int GridZ;
    [SerializeField] private int chunkSize;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(gameObject.transform.position, new Vector3(GridX * chunkSize,1,GridZ * chunkSize));
    }
}
