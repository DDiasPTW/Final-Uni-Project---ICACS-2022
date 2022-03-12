using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGeneration : MonoBehaviour
{
    public bool keepChecking = true;

    public List<GameObject> twoWayTilesPref = new List<GameObject>();
    public List<GameObject> threeWayTilesPref = new List<GameObject>();
    public List<GameObject> fourWayTilesPref = new List<GameObject>();

    public List<GameObject> SpawnedTiles = new List<GameObject>();

    public int chunkSize;

    private void Awake()
    {
        GameObject baseTile = GameObject.FindGameObjectWithTag("BaseTile");
        SpawnedTiles.Add(baseTile);
    }

    private void Update()
    {

    }

    private void FixedUpdate()
    {
        if (keepChecking)
        {
            CheckNeighbours();
        }
    }


    private void CheckNeighbours()
    {
        //Ve todos os tiles na lista 'SpawnedTiles' e verifica se podem dar spawn de tiles adjacentes, caso nao possam remove-se esse tile
        for (int i = 0; i < SpawnedTiles.Count; i++)
        {
            RaycastHit hitN;
            RaycastHit hitS;
            RaycastHit hitE;
            RaycastHit hitW;
            bool placeN = true, placeS = true, placeE = true, placeW = true;

            //Norte
            if (Physics.Raycast(SpawnedTiles[i].transform.position, Vector3.forward, out hitN, chunkSize, ~SpawnedTiles[i].gameObject.layer))
            {
                Debug.DrawRay(SpawnedTiles[i].transform.position, Vector3.forward * chunkSize, Color.red);
                placeN = false;
            }
            else if (hitN.collider == null)
            {
                Debug.DrawRay(SpawnedTiles[i].transform.position, Vector3.forward * chunkSize, Color.green);
                placeN = true;
            }

            //Sul
            if (Physics.Raycast(SpawnedTiles[i].transform.position, Vector3.back, out hitS, chunkSize, ~gameObject.layer))
            {
                Debug.DrawRay(SpawnedTiles[i].transform.position, Vector3.back * chunkSize, Color.red);
                placeS = false;

            }
            else if (hitS.collider == null)
            {
                Debug.DrawRay(SpawnedTiles[i].transform.position, Vector3.back * chunkSize, Color.green);
                placeS = true;
            }

            //Este
            if (Physics.Raycast(SpawnedTiles[i].transform.position, Vector3.right, out hitE, chunkSize, ~gameObject.layer))
            {
                Debug.DrawRay(SpawnedTiles[i].transform.position, Vector3.right * chunkSize, Color.red); 
                placeE = false;
            }
            else if (hitE.collider == null)
            {
                Debug.DrawRay(SpawnedTiles[i].transform.position, Vector3.right * chunkSize, Color.green);
                placeE = true;
            }

            //Oeste
            if (Physics.Raycast(SpawnedTiles[i].transform.position, Vector3.left, out hitW, chunkSize, ~gameObject.layer))
            {
                Debug.DrawRay(SpawnedTiles[i].transform.position, Vector3.left * chunkSize, Color.red); 
                placeW = false;
            }
            else if (hitW.collider == null)
            {
                Debug.DrawRay(SpawnedTiles[i].transform.position, Vector3.left * chunkSize, Color.green); 
                placeW = true;
            }

            //Caso todas as posicoes de spawn possiveis estejam ocupadas, remove-se esse GO da lista
            if (!placeN && !placeS && !placeE && !placeW)
            {
                SpawnedTiles.RemoveAt(i);
            }
        }
        

    }

    private void SpawnNext()
    {
        //Escolhe ao random um dos tiles que esteja na lista 'SpawnedTiles' e verifica as direcoes onde pode dar spawn do prox tile
        //Escolhe uma dessas direcoes ao random, depois escolhe random qual tipo de tile da spawn (2 way, 3 way ou 4 way) e depois da spawn desse novo tile
        //com a rotacao correta, depois da reset das posicoes de spawn de inimigos e mete-as no sitio correto
        int dir = Random.Range(0,4); //0 = N, 1 = S, 2 = E, 3 = O

    }
}
