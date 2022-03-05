using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGridGeneration : MonoBehaviour
{
    [Header("Full Grid Dimensions - MUST BE ODD")]
    public int GridX;
    public int GridZ;
    [SerializeField] private int chunkSize;
    public GameObject prefab;
    [Header("Placement check")]
    public bool placeN; //0
    public bool placeS; //1
    public bool placeE; //2
    public bool placeW; //3
    [Header("Create the grid")]
    public int currentWave = 0;

    private void Start()
    {
    }

    private void Update()
    {
        CheckNeighbours();

        if (Input.GetKeyDown(KeyCode.G))
        {
            AddChunk();
            currentWave++;
        }
    }

    private void CheckNeighbours()
    {
        RaycastHit hitN;
        RaycastHit hitS;
        RaycastHit hitE;
        RaycastHit hitW;

        //Norte------------------------------------------------------------------------------------------------
        if (Physics.Raycast(transform.position, Vector3.forward, out hitN, chunkSize,~gameObject.layer))
        {
            Debug.Log("Chunk a norte");
            Debug.DrawRay(transform.position, Vector3.forward * chunkSize, Color.red);
            placeN = false;
        }
        else if(hitN.collider == null)
        {
            Debug.DrawRay(transform.position, Vector3.forward * chunkSize, Color.green); placeN = true;
        }

        //Sul------------------------------------------------------------------------------------------------
        if (Physics.Raycast(transform.position, Vector3.back, out hitS, chunkSize, ~gameObject.layer))
        {
            Debug.Log("Chunk a sul");
            Debug.DrawRay(transform.position, Vector3.back * chunkSize, Color.red);
            placeS = false;

        }
        else if (hitS.collider == null)
        {
            Debug.DrawRay(transform.position, Vector3.back * chunkSize, Color.green); placeS = true;
        }

        //Este------------------------------------------------------------------------------------------------
        if (Physics.Raycast(transform.position, Vector3.right, out hitE, chunkSize, ~gameObject.layer))
        {
            Debug.Log("Chunk a este");
            Debug.DrawRay(transform.position, Vector3.right * chunkSize, Color.red);
            placeE = false;
        }
        else if (hitE.collider == null)
        {
            Debug.DrawRay(transform.position, Vector3.right * chunkSize, Color.green); placeE = true;
        }

        //Oeste------------------------------------------------------------------------------------------------
        if (Physics.Raycast(transform.position, Vector3.left, out hitW, chunkSize, ~gameObject.layer))
        {
            Debug.Log("Chunk a oeste");
            Debug.DrawRay(transform.position, Vector3.left * chunkSize, Color.red);
            placeW = false;
        }
        else if (hitW.collider == null)
        {
            Debug.DrawRay(transform.position, Vector3.left * chunkSize, Color.green); placeW = true;
        }
    }

    private void AddChunk()
    {
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(gameObject.transform.position, new Vector3(GridX * chunkSize,1,GridZ * chunkSize));
    }
}
