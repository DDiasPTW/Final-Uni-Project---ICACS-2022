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

    public List<string> posSpawn = new List<string>();

    public int chunkSize;
    public float checkRadius;

    private void Awake()
    {
        GameObject baseTile = GameObject.FindGameObjectWithTag("BaseTile");
        SpawnedTiles.Add(baseTile);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            SpawnTile();
        }
    }

    private void FixedUpdate()
    {
        CheckNeighbours();
    }

    //Verifica lados
    //Escolhe lado onde vai dar spawn
    //Escolhe tipo de tile para dar spawn e mete-o na posicao correta


    //Verifica entradas desse novo tile e da rotate dele conforme o lado onde deu spawn
    //volta a verificar entradas e mete ou remove spawnpoints
    private void CheckNeighbours()
    {
        bool placeN = true, placeS = true, placeE = true, placeW = true;
        //Ve todos os tiles na lista 'SpawnedTiles' e verifica se podem dar spawn de tiles adjacentes, caso nao possam remove-se esse tile
        for (int i = 0; i < SpawnedTiles.Count; i++)
        {
            Collider[] hitCollN = Physics.OverlapSphere(SpawnedTiles[i].transform.position + new Vector3(0f, 0, chunkSize), checkRadius);
            Collider[] hitCollS = Physics.OverlapSphere(SpawnedTiles[i].transform.position + new Vector3(0f, 0, -chunkSize), checkRadius);
            Collider[] hitCollE = Physics.OverlapSphere(SpawnedTiles[i].transform.position + new Vector3(chunkSize, 0, 0f), checkRadius);
            Collider[] hitCollO = Physics.OverlapSphere(SpawnedTiles[i].transform.position + new Vector3(-chunkSize, 0, 0f), checkRadius);

            //N
            if (hitCollN.Length != 0)
            {
                placeN = false;
            }
            else placeN = true;
            //S
            if (hitCollS.Length != 0)
            {
                placeS = false;
            }
            else placeS = true;
            //E
            if (hitCollE.Length != 0)
            {
                placeE = false;
            }
            else placeE = true;
            //O
            if (hitCollO.Length != 0)
            {
               placeW = false;
            }
            else placeW = true;

            //Caso todas as posicoes de spawn possiveis estejam ocupadas, remove-se esse GO da lista
            if (!placeN && !placeS && !placeE && !placeW)
            {
                SpawnedTiles.RemoveAt(i);
            }
        }
    }



    private void SpawnTile()
    {
        //keepChecking = false;
       

        int pickedTile = Random.Range(0,SpawnedTiles.Count); //de todos os tiles ja spawned escolhe um ao random
        posSpawn.Clear();
        //bool entradaN = true, entradaS = true, entradaE = true, entradaW = true;

        #region Verifica adjacentes do tile escolhido
        Collider[] hitCollN = Physics.OverlapSphere(SpawnedTiles[pickedTile].transform.position + new Vector3(0f, 0, chunkSize), checkRadius);
        Collider[] hitCollS = Physics.OverlapSphere(SpawnedTiles[pickedTile].transform.position + new Vector3(0f, 0, -chunkSize), checkRadius);
        Collider[] hitCollE = Physics.OverlapSphere(SpawnedTiles[pickedTile].transform.position + new Vector3(chunkSize, 0, 0f), checkRadius);
        Collider[] hitCollO = Physics.OverlapSphere(SpawnedTiles[pickedTile].transform.position + new Vector3(-chunkSize, 0, 0f), checkRadius);

        //N
        if (hitCollN.Length != 0)
        {
            posSpawn.Remove("placeN");
        }
        else if(hitCollN.Length == 0) posSpawn.Add("placeN");
        //S
        if (hitCollS.Length != 0)
        {
            posSpawn.Remove("placeS");
        }
        else if(hitCollS.Length == 0) posSpawn.Add("placeS");
        //E
        if (hitCollE.Length != 0)
        {
            posSpawn.Remove("placeE");
        }
        else if(hitCollE.Length == 0) posSpawn.Add("placeE");
        //O
        if (hitCollO.Length != 0)
        {
             posSpawn.Remove("placeW");
        }
        else if(hitCollO.Length == 0) posSpawn.Add("placeW");

        Debug.Log(SpawnedTiles[pickedTile].name);
        #endregion

        #region Escolhe ao random uma das coords, escolhe ao random qual o tipo de tile, e da spawn na pos correta

        int coord = Random.Range(0,posSpawn.Count);
        int whatTile = Random.Range(0,100); //0-99
        GameObject newTile;

        if (posSpawn[coord] == "placeN")
        {
            if (whatTile < 50) //2 way tile - 50%
            {
                int w = Random.Range(0,twoWayTilesPref.Count);
                newTile = Instantiate(twoWayTilesPref[w],SpawnedTiles[pickedTile].transform.position + new Vector3(0,0,chunkSize),Quaternion.identity);
                SpawnedTiles.Add(newTile);
            }
            else if (whatTile >= 50 && whatTile < 85) //3 way tile - 35%
            {
                int w = Random.Range(0, threeWayTilesPref.Count);
                newTile = Instantiate(threeWayTilesPref[w], SpawnedTiles[pickedTile].transform.position + new Vector3(0, 0, chunkSize), Quaternion.identity);
                SpawnedTiles.Add(newTile);
            }
            else //4 way tile 15%
            {
                int w = Random.Range(0, fourWayTilesPref.Count);
                newTile = Instantiate(fourWayTilesPref[w], SpawnedTiles[pickedTile].transform.position + new Vector3(0, 0, chunkSize), Quaternion.identity);
                SpawnedTiles.Add(newTile);
            }
        }
        else if (posSpawn[coord] == "placeS")
        {
            if (whatTile < 50) //2 way tile - 50%
            {
                int w = Random.Range(0, twoWayTilesPref.Count);
                newTile = Instantiate(twoWayTilesPref[w], SpawnedTiles[pickedTile].transform.position + new Vector3(0, 0, -chunkSize), Quaternion.identity);
                SpawnedTiles.Add(newTile);
            }
            else if (whatTile >= 50 && whatTile < 85) //3 way tile - 35%
            {
                int w = Random.Range(0, threeWayTilesPref.Count);
                newTile = Instantiate(threeWayTilesPref[w], SpawnedTiles[pickedTile].transform.position + new Vector3(0, 0, -chunkSize), Quaternion.identity);
                SpawnedTiles.Add(newTile);
            }
            else //4 way tile 15%
            {
                int w = Random.Range(0, fourWayTilesPref.Count);
                newTile = Instantiate(fourWayTilesPref[w], SpawnedTiles[pickedTile].transform.position + new Vector3(0, 0, -chunkSize), Quaternion.identity);
                SpawnedTiles.Add(newTile);
            }
        }
        else if (posSpawn[coord] == "placeE")
        {
            if (whatTile < 50) //2 way tile - 50%
            {
                int w = Random.Range(0, twoWayTilesPref.Count);
                newTile = Instantiate(twoWayTilesPref[w], SpawnedTiles[pickedTile].transform.position + new Vector3(chunkSize, 0, 0), Quaternion.identity);
                SpawnedTiles.Add(newTile);
            }
            else if (whatTile >= 50 && whatTile < 85) //3 way tile - 35%
            {
                int w = Random.Range(0, threeWayTilesPref.Count);
                newTile = Instantiate(threeWayTilesPref[w], SpawnedTiles[pickedTile].transform.position + new Vector3(chunkSize, 0, 0), Quaternion.identity);
                SpawnedTiles.Add(newTile);
            }
            else //4 way tile 15%
            {
                int w = Random.Range(0, fourWayTilesPref.Count);
                newTile = Instantiate(fourWayTilesPref[w], SpawnedTiles[pickedTile].transform.position + new Vector3(chunkSize, 0, 0), Quaternion.identity);
                SpawnedTiles.Add(newTile);
            }
        }
        else if (posSpawn[coord] == "placeW")
        {
            if (whatTile < 50) //2 way tile - 50%
            {
                int w = Random.Range(0, twoWayTilesPref.Count);
                newTile = Instantiate(twoWayTilesPref[w], SpawnedTiles[pickedTile].transform.position + new Vector3(-chunkSize, 0, 0), Quaternion.identity);
                SpawnedTiles.Add(newTile);
            }
            else if (whatTile >= 50 && whatTile < 85) //3 way tile - 35%
            {
                int w = Random.Range(0, threeWayTilesPref.Count);
                newTile = Instantiate(threeWayTilesPref[w], SpawnedTiles[pickedTile].transform.position + new Vector3(-chunkSize, 0, 0), Quaternion.identity);
                SpawnedTiles.Add(newTile);
            }
            else //4 way tile 15%
            {
                int w = Random.Range(0, fourWayTilesPref.Count);
                newTile = Instantiate(fourWayTilesPref[w], SpawnedTiles[pickedTile].transform.position + new Vector3(-chunkSize, 0, 0), Quaternion.identity);
                SpawnedTiles.Add(newTile);
            }
        }

        #endregion





        ////Verficar onde e que o tile escolhido tem aberturas
        //Collider[] hitCollN = Physics.OverlapSphere(SpawnedTiles[pickedTile].transform.position + new Vector3(0f, .5f, chunkSize / 2), checkRadius);
        //Collider[] hitCollS = Physics.OverlapSphere(SpawnedTiles[pickedTile].transform.position + new Vector3(0f, .5f, -chunkSize / 2), checkRadius);
        //Collider[] hitCollE = Physics.OverlapSphere(SpawnedTiles[pickedTile].transform.position + new Vector3(chunkSize / 2, .5f, 0f), checkRadius);
        //Collider[] hitCollO = Physics.OverlapSphere(SpawnedTiles[pickedTile].transform.position + new Vector3(-chunkSize / 2, .5f, 0f), checkRadius);

        ////N
        //if (hitCollN.Length != 0)
        //{
        //    Debug.Log("Nao pode N"); entradaN = false;
        //}
        //else entradaN = true;
        ////S
        //if (hitCollS.Length != 0)
        //{
        //    Debug.Log("Nao pode S"); entradaS = false;
        //}
        //else entradaS = true;
        ////E
        //if (hitCollE.Length != 0)
        //{
        //    Debug.Log("Nao pode E"); entradaE = false;
        //}
        //else entradaE = true;
        ////O
        //if (hitCollO.Length != 0)
        //{
        //    Debug.Log("Nao pode O"); entradaW = false;
        //}
        //else entradaW = true;
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < SpawnedTiles.Count; i++)
        {
            //Este
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(SpawnedTiles[i].transform.position + new Vector3(chunkSize, 0, 0f), checkRadius);

            //Oeste
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(SpawnedTiles[i].transform.position + new Vector3(-chunkSize, 0, 0f), checkRadius);

            //Norte
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(SpawnedTiles[i].transform.position + new Vector3(0f, 0, chunkSize), checkRadius);

            //Sul
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(SpawnedTiles[i].transform.position + new Vector3(0, 0, -chunkSize), checkRadius);
        }
    }

}
