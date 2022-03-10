using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckChunk : MonoBehaviour
{
    private GameGridGeneration ggg;
    //[SerializeField] private bool keepChecking = true;
    //public bool canPlace = false;
    [Header("Placement check")]
    public bool placeN; 
    public bool placeS; 
    public bool placeE; 
    public bool placeW; 
    [SerializeField] private int chunkSize;
    //[Header("What to place")]
    //public GameObject prefab; //prefab do tile default
    void Start()
    {
        ggg = GameObject.FindGameObjectWithTag("GridManager").GetComponent<GameGridGeneration>();
        chunkSize = ggg.chunkSize;
    }

    private void FixedUpdate()
    {
        CheckNeighbours(); //Verifica as boundaries do chunk para ver se tem chunks adjacentes
        //CheckPlacement();
    }

    private void CheckNeighbours()
    {
        RaycastHit hitN;
        RaycastHit hitS;
        RaycastHit hitE;
        RaycastHit hitW;

        //Norte------------------------------------------------------------------------------------------------
        if (transform.position.z < ggg.posMaxZ)
        {
            if (Physics.Raycast(transform.position, Vector3.forward, out hitN, chunkSize, ~gameObject.layer))
            {
                //Debug.Log("Chunk a norte");
                Debug.DrawRay(transform.position, Vector3.forward * chunkSize, Color.red);
                placeN = false;
            }
            else if (hitN.collider == null)
            {
                Debug.DrawRay(transform.position, Vector3.forward * chunkSize, Color.green); placeN = true;
            }
        }
        else placeN = false;


        //Sul------------------------------------------------------------------------------------------------
        if (transform.position.z > ggg.holdPosMinZ)
        {
            if (Physics.Raycast(transform.position, Vector3.back, out hitS, chunkSize, ~gameObject.layer))
            {
                //Debug.Log("Chunk a sul");
                Debug.DrawRay(transform.position, Vector3.back * chunkSize, Color.red);
                placeS = false;

            }
            else if (hitS.collider == null)
            {
                Debug.DrawRay(transform.position, Vector3.back * chunkSize, Color.green); placeS = true;
            }
        }
        else placeS = false;


        //Este------------------------------------------------------------------------------------------------
        if (transform.position.x < ggg.posMaxX)
        {
            if (Physics.Raycast(transform.position, Vector3.right, out hitE, chunkSize, ~gameObject.layer))
            {
                //Debug.Log("Chunk a este");
                Debug.DrawRay(transform.position, Vector3.right * chunkSize, Color.red);
                placeE = false;
            }
            else if (hitE.collider == null)
            {
                Debug.DrawRay(transform.position, Vector3.right * chunkSize, Color.green); placeE = true;
            }
        }else placeE = false;



        //Oeste------------------------------------------------------------------------------------------------
        if (transform.position.x > ggg.holdPosMinX)
        {
            if (Physics.Raycast(transform.position, Vector3.left, out hitW, chunkSize, ~gameObject.layer))
            {
                //Debug.Log("Chunk a oeste");
                Debug.DrawRay(transform.position, Vector3.left * chunkSize, Color.red);
                placeW = false;
            }
            else if (hitW.collider == null)
            {
                Debug.DrawRay(transform.position, Vector3.left * chunkSize, Color.green); placeW = true;
            }
        }
        else placeW = false;

        
    }

    //private void CheckPlacement()
    //{
    //    if (keepChecking)
    //    {
    //        for (int i = 0; i < ggg.placementPositions.Length; i++)
    //        {
    //            if (ggg.placementPositions[i] == transform.position)
    //            {
    //                Debug.Log("EXISTE TILE EM: " + transform.position + " - " + gameObject.name);
    //                //ggg.placedChunks.Add(gameObject);
    //                keepChecking = false;
    //            }
    //        }
    //    }

    //}
}
