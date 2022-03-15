using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WorldGeneration : MonoBehaviour
{
    [SerializeField] private int CurrentWave = 0;
    [SerializeField] private int MaxWave;

    [SerializeField] private bool showAdjacentGizmo = true;
    [SerializeField] private bool showPathGizmo = true;

    [SerializeField] private bool CheckRotation = false;
    private string coordenada;

    public List<GameObject> twoWayTilesPref = new List<GameObject>();
    public List<GameObject> threeWayTilesPref = new List<GameObject>();
    public List<GameObject> fourWayTilesPref = new List<GameObject>();

    public List<GameObject> spawnableTiles = new List<GameObject>();
    
    
    public List<GameObject> spawnedTile = new List<GameObject>();

    private List<string> posSpawn = new List<string>();

    public int chunkSize;
    public float checkRadius;
    public float checkRadiusEntradas;

    private void Awake()
    {
        GameObject baseTile = GameObject.FindGameObjectWithTag("BaseTile");
        spawnableTiles.Add(baseTile); CurrentWave++;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) && CurrentWave < MaxWave)
        {
            SpawnTile();
            CurrentWave++;
        }

        if (CheckRotation)
        {
            SetRotation(coordenada);
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
        for (int i = 0; i < spawnableTiles.Count; i++)
        {
            //verifica adjacencia de todos os tiles que podem dar spawn de outros
            Collider[] hitCollN = Physics.OverlapSphere(spawnableTiles[i].transform.position + new Vector3(0f, 0, chunkSize), checkRadius);
            Collider[] hitCollS = Physics.OverlapSphere(spawnableTiles[i].transform.position + new Vector3(0f, 0, -chunkSize), checkRadius);
            Collider[] hitCollE = Physics.OverlapSphere(spawnableTiles[i].transform.position + new Vector3(chunkSize, 0, 0f), checkRadius);
            Collider[] hitCollO = Physics.OverlapSphere(spawnableTiles[i].transform.position + new Vector3(-chunkSize, 0, 0f), checkRadius);

            //verifica entradas de todos os tiles que podem dar spawn de outros
            Collider[] entradaN = Physics.OverlapSphere(spawnableTiles[i].transform.position + new Vector3(0f, .5f, chunkSize / 2), checkRadiusEntradas);
            Collider[] entradaS = Physics.OverlapSphere(spawnableTiles[i].transform.position + new Vector3(0f, .5f, -chunkSize / 2), checkRadiusEntradas);
            Collider[] entradaE = Physics.OverlapSphere(spawnableTiles[i].transform.position + new Vector3(chunkSize / 2, .5f, 0f), checkRadiusEntradas);
            Collider[] entradaO = Physics.OverlapSphere(spawnableTiles[i].transform.position + new Vector3(-chunkSize / 2, .5f, 0f), checkRadiusEntradas);
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
                spawnableTiles.RemoveAt(i);
            }
        }
    }


    //Escolhe lado onde vai dar spawn
    //Escolhe tipo de tile para dar spawn e mete-o na posicao correta
    
    private void SpawnTile()
    {
        int pickedTile = Random.Range(0,spawnableTiles.Count); //de todos os tiles ja spawned escolhe um ao random
        posSpawn.Clear();
        spawnedTile.Clear();

        #region Verifica adjacentes do tile escolhido
        Collider[] hitCollN = Physics.OverlapSphere(spawnableTiles[pickedTile].transform.position + new Vector3(0f, 0, chunkSize), checkRadius);
        Collider[] hitCollS = Physics.OverlapSphere(spawnableTiles[pickedTile].transform.position + new Vector3(0f, 0, -chunkSize), checkRadius);
        Collider[] hitCollE = Physics.OverlapSphere(spawnableTiles[pickedTile].transform.position + new Vector3(chunkSize, 0, 0f), checkRadius);
        Collider[] hitCollO = Physics.OverlapSphere(spawnableTiles[pickedTile].transform.position + new Vector3(-chunkSize, 0, 0f), checkRadius);

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
        Collider[] entradaN = Physics.OverlapSphere(spawnableTiles[pickedTile].transform.position + new Vector3(0f, .5f, chunkSize / 2), checkRadiusEntradas);
        Collider[] entradaS = Physics.OverlapSphere(spawnableTiles[pickedTile].transform.position + new Vector3(0f, .5f, -chunkSize / 2), checkRadiusEntradas);
        Collider[] entradaE = Physics.OverlapSphere(spawnableTiles[pickedTile].transform.position + new Vector3(chunkSize / 2, .5f, 0f), checkRadiusEntradas);
        Collider[] entradaO = Physics.OverlapSphere(spawnableTiles[pickedTile].transform.position + new Vector3(-chunkSize / 2, .5f, 0f), checkRadiusEntradas);

        //N
        if (entradaN.Length != 0)
        {
            posSpawn.Remove("placeN");
        }
        //S
        if (entradaS.Length != 0)
        {
            posSpawn.Remove("placeS");
        }
        //E
        if (entradaE.Length != 0)
        {
            posSpawn.Remove("placeE");
        }
        //O
        if (entradaO.Length != 0)
        {
            posSpawn.Remove("placeW");
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
                newTile = Instantiate(twoWayTilesPref[w],spawnableTiles[pickedTile].transform.position + new Vector3(0,0,chunkSize),Quaternion.identity);
                spawnableTiles.Add(newTile);
            }
            else if (whatTile >= 85 && whatTile < 95) //3 way tile
            {
                int w = Random.Range(0, threeWayTilesPref.Count);
                newTile = Instantiate(threeWayTilesPref[w], spawnableTiles[pickedTile].transform.position + new Vector3(0, 0, chunkSize), Quaternion.identity);
                spawnableTiles.Add(newTile);
            }
            else //4 way tile
            {
                int w = Random.Range(0, fourWayTilesPref.Count);
                newTile = Instantiate(fourWayTilesPref[w], spawnableTiles[pickedTile].transform.position + new Vector3(0, 0, chunkSize), Quaternion.identity);
                spawnableTiles.Add(newTile);
            }
            spawnedTile.Add(newTile);
        }
        else if (posSpawn[coord] == "placeS")
        {
            if (whatTile < 85) //2 way tile
            {
                int w = Random.Range(0, twoWayTilesPref.Count);
                newTile = Instantiate(twoWayTilesPref[w], spawnableTiles[pickedTile].transform.position + new Vector3(0, 0, -chunkSize), Quaternion.identity);
                spawnableTiles.Add(newTile);
            }
            else if (whatTile >= 85 && whatTile < 95) //3 way tile 
            {
                int w = Random.Range(0, threeWayTilesPref.Count);
                newTile = Instantiate(threeWayTilesPref[w], spawnableTiles[pickedTile].transform.position + new Vector3(0, 0, -chunkSize), Quaternion.identity);
                spawnableTiles.Add(newTile);             
            }
            else //4 way tile
            {
                int w = Random.Range(0, fourWayTilesPref.Count);
                newTile = Instantiate(fourWayTilesPref[w], spawnableTiles[pickedTile].transform.position + new Vector3(0, 0, -chunkSize), Quaternion.identity);
                spawnableTiles.Add(newTile);
            }
            spawnedTile.Add(newTile);
        }
        else if (posSpawn[coord] == "placeE")
        {
            if (whatTile < 85) //2 way tile
            {
                int w = Random.Range(0, twoWayTilesPref.Count);
                newTile = Instantiate(twoWayTilesPref[w], spawnableTiles[pickedTile].transform.position + new Vector3(chunkSize, 0, 0), Quaternion.identity);
                spawnableTiles.Add(newTile);
            }
            else if (whatTile >= 85 && whatTile < 95) //3 way tile
            {
                int w = Random.Range(0, threeWayTilesPref.Count);
                newTile = Instantiate(threeWayTilesPref[w], spawnableTiles[pickedTile].transform.position + new Vector3(chunkSize, 0, 0), Quaternion.identity);
                spawnableTiles.Add(newTile);
            }
            else //4 way tile
            {
                int w = Random.Range(0, fourWayTilesPref.Count);
                newTile = Instantiate(fourWayTilesPref[w], spawnableTiles[pickedTile].transform.position + new Vector3(chunkSize, 0, 0), Quaternion.identity);
                spawnableTiles.Add(newTile);
                
            }
            spawnedTile.Add(newTile);
        }
        else if (posSpawn[coord] == "placeW")
        {
            if (whatTile < 85) //2 way tile
            {
                int w = Random.Range(0, twoWayTilesPref.Count);
                newTile = Instantiate(twoWayTilesPref[w], spawnableTiles[pickedTile].transform.position + new Vector3(-chunkSize, 0, 0), Quaternion.identity);
                spawnableTiles.Add(newTile);
            }
            else if (whatTile >= 85 && whatTile < 95) //3 way tile
            {
                int w = Random.Range(0, threeWayTilesPref.Count);
                newTile = Instantiate(threeWayTilesPref[w], spawnableTiles[pickedTile].transform.position + new Vector3(-chunkSize, 0, 0), Quaternion.identity);
                spawnableTiles.Add(newTile);
            }
            else //4 way tile
            {
                int w = Random.Range(0, fourWayTilesPref.Count);
                newTile = Instantiate(fourWayTilesPref[w], spawnableTiles[pickedTile].transform.position + new Vector3(-chunkSize, 0, 0), Quaternion.identity);
                spawnableTiles.Add(newTile);
            }
            spawnedTile.Add(newTile);
        }


        #endregion

        coordenada = posSpawn[coord];
        CheckRotation = true;
        
        
        //TO DO: MELHORAR GERAÇÃO PARA SER MENOS RANDOM, METER SPAWN POINT, DAR RE-BAKE DA NAVMESH
    }


    //Verifica entradas desse novo tile e da rotate dele conforme o lado onde deu spawn

    private void SetRotation(string placement)
    {
        GameObject tileToRotate = spawnedTile[0];

        if (placement == "placeN") //foi colocado a norte por isso tem que ter entrada a sul
        {
            Collider[] entrada = Physics.OverlapSphere(tileToRotate.transform.position + new Vector3(0f, .5f, -chunkSize / 2), checkRadiusEntradas);
            if (entrada.Length != 0)
            {
                tileToRotate.transform.rotation *= Quaternion.Euler(0,90,0);
            }
            else CheckRotation = false;
        }
        else if (placement == "placeS") //foi colocado a sul por isso tem que ter entrada a norte
        {
            Collider[] entrada = Physics.OverlapSphere(tileToRotate.transform.position + new Vector3(0f, .5f, chunkSize / 2), checkRadiusEntradas);
            if (entrada.Length != 0)
            {
                tileToRotate.transform.rotation *= Quaternion.Euler(0, 90, 0);
            }
            else CheckRotation = false;
        }
        else if (placement == "placeE") //foi colocado a este por isso tem que ter entrada a oeste
        {
            Collider[] entrada = Physics.OverlapSphere(tileToRotate.transform.position + new Vector3(-chunkSize / 2, .5f, 0f), checkRadiusEntradas);
            if (entrada.Length != 0)
            {
                tileToRotate.transform.rotation *= Quaternion.Euler(0, 90, 0);
            }
            else CheckRotation = false;
        }
        else if (placement == "placeW") //foi colocado a noeste por isso tem que ter entrada a este
        {
            Collider[] entrada = Physics.OverlapSphere(tileToRotate.transform.position + new Vector3(chunkSize / 2, .5f, 0f), checkRadiusEntradas);
            if (entrada.Length != 0)
            {
                tileToRotate.transform.rotation *= Quaternion.Euler(0, 90, 0);
            }
            else CheckRotation = false; 
        }

        
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < spawnableTiles.Count; i++)
        {
            if (showAdjacentGizmo)
            {
                //Este
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(spawnableTiles[i].transform.position + new Vector3(chunkSize, 0, 0f), checkRadius);
                //Oeste
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(spawnableTiles[i].transform.position + new Vector3(-chunkSize, 0, 0f), checkRadius);
                //Norte
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(spawnableTiles[i].transform.position + new Vector3(0f, 0, chunkSize), checkRadius);
                //Sul
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(spawnableTiles[i].transform.position + new Vector3(0, 0, -chunkSize), checkRadius);
            }

            if (showPathGizmo)
            {
                //Este
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(spawnableTiles[i].transform.position + new Vector3(chunkSize / 2, .5f, 0f), checkRadiusEntradas);
                //Oeste
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(spawnableTiles[i].transform.position + new Vector3(-chunkSize / 2, .5f, 0f), checkRadiusEntradas);
                //Norte
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(spawnableTiles[i].transform.position + new Vector3(0f, .5f, chunkSize / 2), checkRadiusEntradas);
                //Sul
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(spawnableTiles[i].transform.position + new Vector3(0f, .5f, -chunkSize / 2), checkRadiusEntradas);
            }
            
        }
    }

}
