using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WorldGeneration : MonoBehaviour
{
    [SerializeField] private int CurrentWave = 0;
    [SerializeField] private int MaxWave;

    public List<GameObject> twoWayTilesPref = new List<GameObject>();
    public List<GameObject> threeWayTilesPref = new List<GameObject>();
    public List<GameObject> fourWayTilesPref = new List<GameObject>();

    public List<GameObject> SpawnedTiles = new List<GameObject>();

    public List<string> posSpawn = new List<string>();

    public int chunkSize;
    public float checkRadius;
    public float checkRadiusEntradas;

    private void Awake()
    {
        GameObject baseTile = GameObject.FindGameObjectWithTag("BaseTile");
        SpawnedTiles.Add(baseTile); CurrentWave++;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) && CurrentWave < MaxWave)
        {
            SpawnTile();
        }
    }

    private void FixedUpdate()
    {
        if (CurrentWave < MaxWave)
        {
            CheckNeighbours();
        }
    }


    //Verifica lados e entradas
    private void CheckNeighbours()
    {
        bool placeN = true, placeS = true, placeE = true, placeW = true;
        //Ve todos os tiles na lista 'SpawnedTiles' e verifica se podem dar spawn de tiles adjacentes, caso nao possam remove-se esse tile
        for (int i = 0; i < SpawnedTiles.Count; i++)
        {
            //verifica adjacencia de todos os tiles que podem dar spawn de outros
            Collider[] hitCollN = Physics.OverlapSphere(SpawnedTiles[i].transform.position + new Vector3(0f, 0, chunkSize), checkRadius);
            Collider[] hitCollS = Physics.OverlapSphere(SpawnedTiles[i].transform.position + new Vector3(0f, 0, -chunkSize), checkRadius);
            Collider[] hitCollE = Physics.OverlapSphere(SpawnedTiles[i].transform.position + new Vector3(chunkSize, 0, 0f), checkRadius);
            Collider[] hitCollO = Physics.OverlapSphere(SpawnedTiles[i].transform.position + new Vector3(-chunkSize, 0, 0f), checkRadius);

            //verifica entradas de todos os tiles que podem dar spawn de outros
            Collider[] entradaN = Physics.OverlapSphere(SpawnedTiles[i].transform.position + new Vector3(0f, .5f, chunkSize / 2), checkRadiusEntradas);
            Collider[] entradaS = Physics.OverlapSphere(SpawnedTiles[i].transform.position + new Vector3(0f, .5f, -chunkSize / 2), checkRadiusEntradas);
            Collider[] entradaE = Physics.OverlapSphere(SpawnedTiles[i].transform.position + new Vector3(chunkSize / 2, .5f, 0f), checkRadiusEntradas);
            Collider[] entradaO = Physics.OverlapSphere(SpawnedTiles[i].transform.position + new Vector3(-chunkSize / 2, .5f, 0f), checkRadiusEntradas);
            //N
            if (hitCollN.Length != 0 || entradaN.Length != 0)
            {
                placeN = false;
            }
            else if(hitCollN.Length == 0 && entradaN.Length == 0) placeN = true;
            //S
            if (hitCollS.Length != 0 || entradaS.Length != 0)
            {
                placeS = false;
            }
            else if (hitCollS.Length == 0 && entradaS.Length == 0) placeS = true;
            //E
            if (hitCollE.Length != 0 || entradaE.Length != 0)
            {
                placeE = false;
            }
            else if(hitCollE.Length == 0 && entradaE.Length == 0) placeE = true;
            //O
            if (hitCollO.Length != 0 || entradaO.Length != 0)
            {
               placeW = false;
            }
            else if(hitCollO.Length == 0 && entradaO.Length == 0) placeW = true;

            //Caso todas as posicoes de spawn possiveis estejam ocupadas, remove-se esse GO da lista
            if (!placeN && !placeS && !placeE && !placeW)
            {
                SpawnedTiles.RemoveAt(i);
            }
        }
    }


    //Escolhe lado onde vai dar spawn
    //Escolhe tipo de tile para dar spawn e mete-o na posicao correta
    
    private void SpawnTile()
    {
        CurrentWave++;
        int pickedTile = Random.Range(0,SpawnedTiles.Count); //de todos os tiles ja spawned escolhe um ao random
        posSpawn.Clear();

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
        
        #endregion

        # region Verficar onde tile escolhido tem aberturas
        Collider[] entradaN = Physics.OverlapSphere(SpawnedTiles[pickedTile].transform.position + new Vector3(0f, .5f, chunkSize / 2), checkRadiusEntradas);
        Collider[] entradaS = Physics.OverlapSphere(SpawnedTiles[pickedTile].transform.position + new Vector3(0f, .5f, -chunkSize / 2), checkRadiusEntradas);
        Collider[] entradaE = Physics.OverlapSphere(SpawnedTiles[pickedTile].transform.position + new Vector3(chunkSize / 2, .5f, 0f), checkRadiusEntradas);
        Collider[] entradaO = Physics.OverlapSphere(SpawnedTiles[pickedTile].transform.position + new Vector3(-chunkSize / 2, .5f, 0f), checkRadiusEntradas);

        //N
        if (entradaN.Length != 0)
        {
            Debug.Log("Nao pode N"); posSpawn.Remove("placeN");
        }
        //S
        if (entradaS.Length != 0)
        {
            Debug.Log("Nao pode S"); posSpawn.Remove("placeS");
        }
        //E
        if (entradaE.Length != 0)
        {
            Debug.Log("Nao pode E"); posSpawn.Remove("placeE");
        }
        //O
        if (entradaO.Length != 0)
        {
            Debug.Log("Nao pode O"); posSpawn.Remove("placeW");
        }

        #endregion

        #region Escolhe ao random uma das coords, escolhe ao random qual o tipo de tile, e da spawn na pos correta

        int coord = Random.Range(0,posSpawn.Count);
        int whatTile = Random.Range(0,100); //0-99
        GameObject newTile;

        if (posSpawn[coord] == "placeN")
        {
            if (whatTile < 85) //2 way tile
            {
                int w = Random.Range(0,twoWayTilesPref.Count);
                newTile = Instantiate(twoWayTilesPref[w],SpawnedTiles[pickedTile].transform.position + new Vector3(0,0,chunkSize),Quaternion.identity);
                SpawnedTiles.Add(newTile);
                SetRotation(newTile, posSpawn[coord]);
            }
            else if (whatTile >= 85 && whatTile < 95) //3 way tile
            {
                int w = Random.Range(0, threeWayTilesPref.Count);
                newTile = Instantiate(threeWayTilesPref[w], SpawnedTiles[pickedTile].transform.position + new Vector3(0, 0, chunkSize), Quaternion.identity);
                SpawnedTiles.Add(newTile);
                SetRotation(newTile, posSpawn[coord]);
            }
            else //4 way tile
            {
                int w = Random.Range(0, fourWayTilesPref.Count);
                newTile = Instantiate(fourWayTilesPref[w], SpawnedTiles[pickedTile].transform.position + new Vector3(0, 0, chunkSize), Quaternion.identity);
                SpawnedTiles.Add(newTile);
                SetRotation(newTile, posSpawn[coord]);
            }
            
        }
        else if (posSpawn[coord] == "placeS")
        {
            if (whatTile < 85) //2 way tile
            {
                int w = Random.Range(0, twoWayTilesPref.Count);
                newTile = Instantiate(twoWayTilesPref[w], SpawnedTiles[pickedTile].transform.position + new Vector3(0, 0, -chunkSize), Quaternion.identity);
                SpawnedTiles.Add(newTile);
                SetRotation(newTile, posSpawn[coord]);
            }
            else if (whatTile >= 85 && whatTile < 95) //3 way tile 
            {
                int w = Random.Range(0, threeWayTilesPref.Count);
                newTile = Instantiate(threeWayTilesPref[w], SpawnedTiles[pickedTile].transform.position + new Vector3(0, 0, -chunkSize), Quaternion.identity);
                SpawnedTiles.Add(newTile);
                SetRotation(newTile, posSpawn[coord]);
            }
            else //4 way tile
            {
                int w = Random.Range(0, fourWayTilesPref.Count);
                newTile = Instantiate(fourWayTilesPref[w], SpawnedTiles[pickedTile].transform.position + new Vector3(0, 0, -chunkSize), Quaternion.identity);
                SpawnedTiles.Add(newTile);
                SetRotation(newTile, posSpawn[coord]);
            }
        }
        else if (posSpawn[coord] == "placeE")
        {
            if (whatTile < 85) //2 way tile
            {
                int w = Random.Range(0, twoWayTilesPref.Count);
                newTile = Instantiate(twoWayTilesPref[w], SpawnedTiles[pickedTile].transform.position + new Vector3(chunkSize, 0, 0), Quaternion.identity);
                SpawnedTiles.Add(newTile);
                SetRotation(newTile, posSpawn[coord]);
            }
            else if (whatTile >= 85 && whatTile < 95) //3 way tile
            {
                int w = Random.Range(0, threeWayTilesPref.Count);
                newTile = Instantiate(threeWayTilesPref[w], SpawnedTiles[pickedTile].transform.position + new Vector3(chunkSize, 0, 0), Quaternion.identity);
                SpawnedTiles.Add(newTile);
                SetRotation(newTile, posSpawn[coord]);
            }
            else //4 way tile
            {
                int w = Random.Range(0, fourWayTilesPref.Count);
                newTile = Instantiate(fourWayTilesPref[w], SpawnedTiles[pickedTile].transform.position + new Vector3(chunkSize, 0, 0), Quaternion.identity);
                SpawnedTiles.Add(newTile);
                SetRotation(newTile, posSpawn[coord]);
            }
        }
        else if (posSpawn[coord] == "placeW")
        {
            if (whatTile < 85) //2 way tile
            {
                int w = Random.Range(0, twoWayTilesPref.Count);
                newTile = Instantiate(twoWayTilesPref[w], SpawnedTiles[pickedTile].transform.position + new Vector3(-chunkSize, 0, 0), Quaternion.identity);
                SpawnedTiles.Add(newTile);
            }
            else if (whatTile >= 85 && whatTile < 95) //3 way tile
            {
                int w = Random.Range(0, threeWayTilesPref.Count);
                newTile = Instantiate(threeWayTilesPref[w], SpawnedTiles[pickedTile].transform.position + new Vector3(-chunkSize, 0, 0), Quaternion.identity);
                SpawnedTiles.Add(newTile);
            }
            else //4 way tile
            {
                int w = Random.Range(0, fourWayTilesPref.Count);
                newTile = Instantiate(fourWayTilesPref[w], SpawnedTiles[pickedTile].transform.position + new Vector3(-chunkSize, 0, 0), Quaternion.identity);
                SpawnedTiles.Add(newTile);
            }
            SetRotation(newTile, posSpawn[coord]);
        }

        #endregion

        
        
        //TO DO: MELHORAR GERA��O PARA SER MENOS RANDOM, METER SPAWN POINT, DAR RE-BAKE DA NAVMESH


    }


    //Verifica entradas desse novo tile e da rotate dele conforme o lado onde deu spawn
    //volta a verificar entradas e mete ou remove spawnpoints
    //volta a dar build da navMesh
    private void SetRotation(GameObject tileToRotate, string placement)
    {
        //TO DO: Verficiar se os restantes caminhos ficam sem block, ao fim de testar as 4 rotacoes caso nao haja substituir o tile spawnado por um tile 'portal'

        if (placement == "placeN") //novo tile foi spawnado a norte por isso tem q ter uma entrada a sul
        {
            Collider[] adjacentCheck = Physics.OverlapSphere(tileToRotate.transform.position + new Vector3(0f, 0, -chunkSize), checkRadius);
            Collider[] entranceCheck = Physics.OverlapSphere(tileToRotate.transform.position + new Vector3(0f, .5f, -chunkSize / 2), checkRadiusEntradas);
            if ((entranceCheck.Length != 0 && adjacentCheck.Length != 0))
            {
                tileToRotate.transform.rotation *= Quaternion.Euler(0, 90, 0);
                SetRotation(tileToRotate, placement);
            }

        }
        else if (placement == "placeS") //novo tile foi spawnado a sul por isso tem q ter uma entrada a norte
        {
            Collider[] adjacentCheck = Physics.OverlapSphere(tileToRotate.transform.position + new Vector3(0f, 0, chunkSize), checkRadius);
            Collider[] entranceCheck = Physics.OverlapSphere(tileToRotate.transform.position + new Vector3(0f, .5f, chunkSize / 2), checkRadiusEntradas);
            if ((entranceCheck.Length != 0 && adjacentCheck.Length != 0))
            {
                tileToRotate.transform.rotation *= Quaternion.Euler(0, 90, 0);
                SetRotation(tileToRotate, placement);
            }

        }
        else if (placement == "placeE") //novo tile foi spawnado a este por isso tem q ter uma entrada a oeste
        {
            Collider[] adjacentCheck = Physics.OverlapSphere(tileToRotate.transform.position + new Vector3(-chunkSize, 0, 0f), checkRadius);
            Collider[] entranceCheck = Physics.OverlapSphere(tileToRotate.transform.position + new Vector3(-chunkSize / 2, .5f, 0f), checkRadiusEntradas);
            if ((entranceCheck.Length != 0 && adjacentCheck.Length != 0))
            {
                tileToRotate.transform.rotation *= Quaternion.Euler(0, 90, 0);
                SetRotation(tileToRotate, placement);
            }

        }
        else if (placement == "placeW") //novo tile foi spawnado a oeste por isso tem q ter uma entrada a este
        {
            Collider[] adjacentCheck = Physics.OverlapSphere(tileToRotate.transform.position + new Vector3(chunkSize, 0, 0f), checkRadius);
            Collider[] entranceCheck = Physics.OverlapSphere(tileToRotate.transform.position + new Vector3(chunkSize / 2, .5f, 0f), checkRadiusEntradas);
            if ((entranceCheck.Length != 0 && adjacentCheck.Length != 0))
            {
                tileToRotate.transform.rotation *= Quaternion.Euler(0, 90, 0);
                SetRotation(tileToRotate, placement);
            }
        }
        else return;
        

    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < SpawnedTiles.Count; i++)
        {
            //Este
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(SpawnedTiles[i].transform.position + new Vector3(chunkSize, 0, 0f), checkRadius);
            Gizmos.DrawWireSphere(SpawnedTiles[i].transform.position + new Vector3(chunkSize/2, .5f, 0f), checkRadiusEntradas);

            //Oeste
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(SpawnedTiles[i].transform.position + new Vector3(-chunkSize, 0, 0f), checkRadius);
            Gizmos.DrawWireSphere(SpawnedTiles[i].transform.position + new Vector3(-chunkSize / 2, .5f, 0f), checkRadiusEntradas);
            //Norte
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(SpawnedTiles[i].transform.position + new Vector3(0f, 0, chunkSize), checkRadius);
            Gizmos.DrawWireSphere(SpawnedTiles[i].transform.position + new Vector3(0f, .5f, chunkSize/2), checkRadiusEntradas);

            //Sul
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(SpawnedTiles[i].transform.position + new Vector3(0, 0, -chunkSize), checkRadius);
            Gizmos.DrawWireSphere(SpawnedTiles[i].transform.position + new Vector3(0f, .5f, -chunkSize/2), checkRadiusEntradas);
        }
    }

}
