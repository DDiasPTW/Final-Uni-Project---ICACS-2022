using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.AI.Navigation;

public class WorldGeneration : MonoBehaviour
{
    public int CurrentWave = 0; //wave atual
    [Range(1,50)]
    public int MaxWave; //wave maxima do jogo

    [SerializeField] private bool showAdjacentGizmo = true; //apenas serve para debug, para ver os tiles adjacentes dos tiles que podem dar spawn
    [SerializeField] private bool showPathGizmo = true; //apenas serve para debug, para ver os entradas dos tiles que podem dar spawn

    [SerializeField] private bool CheckRotation = false;

    private string coordenada;

    public GameObject baseTile;
    [SerializeField] private List<GameObject> baseTiles = new List<GameObject>(); //prefabs dos tiles base- TÊM QUE ESTAR POR ORDEM
    [SerializeField] private List<GameObject> adjacentToBase = new List<GameObject>(); //prefabs dos tiles que podem aparecer adjacentes à base
    [SerializeField] private List<GameObject> twoWayTilesPref = new List<GameObject>(); //prefabs dos tiles com 2 entradas 
    [SerializeField] private List<GameObject> threeWayTilesPref = new List<GameObject>(); //prefabs dos tiles com 3 entradas
    [SerializeField] private List<GameObject> fourWayTilesPref = new List<GameObject>(); //prefabs dos tiles com 4 entradas
    [SerializeField] private GameObject spawnPointPref; //prefab dos spawnPoints de inimigos


    [SerializeField] private List<GameObject> allTiles = new List<GameObject>(); //lista de todos os tiles em jogo
    [SerializeField] private List<GameObject> spawnableTiles = new List<GameObject>(); //lista dos tiles que podem dar spawn de novos tiles
    public List<GameObject> spawnPoints = new List<GameObject>(); //lista de todos os spawnPoints no mapa


    [SerializeField] private List<GameObject> spawnedTile = new List<GameObject>(); //ultimo tile que foi spawnado

    private List<string> posSpawn = new List<string>(); //direcao onde o tile escolhido pode dar spawn

    public int chunkSize; //tamanho de cada tile
    [SerializeField] private float checkRadius; //raio de verificacao de tiles adjacentes
    [SerializeField ]private float checkRadiusEntradas;  //raio de verificacao de entradas


    private NavMeshSurface nav;

    private EnemyGeneration enemyGen;
    
    private void Awake()
    {
        nav = GetComponent<NavMeshSurface>();
        enemyGen = GetComponent<EnemyGeneration>();

        baseTile = Instantiate(baseTiles[enemyGen.currentDifficulty - 1], Vector3.zero, Quaternion.identity);
        spawnableTiles.Add(baseTile); allTiles.Add(baseTile);
       
    }

    private void Start()
    {
        CheckNeighbours();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) && CurrentWave < MaxWave && !enemyGen.isSpawning)
        {
            SpawnTile();
            CurrentWave++;


            //MUDA NUMERO DE INIMIGOS PARA SPAWNAR NESTA RONDA
            if (CurrentWave < (MaxWave / 6))
            {
                enemyGen.howManyEnemies = enemyGen.enemiesPerWave * CurrentWave * enemyGen.currentDifficulty;
            }else if (CurrentWave >= (MaxWave/6) && CurrentWave <= (MaxWave/2))
            {
                enemyGen.howManyEnemies = enemyGen.enemiesPerWave * CurrentWave * enemyGen.currentDifficulty * 2;
            }else if(CurrentWave > (MaxWave/2) && CurrentWave < (MaxWave / 1.2f))
            {
                enemyGen.spawnCooldown = enemyGen.spawnCooldown - .02f;
                enemyGen.howManyEnemies = enemyGen.enemiesPerWave * CurrentWave * enemyGen.currentDifficulty * 3;
            }
            else
            {
                enemyGen.spawnCooldown = enemyGen.spawnCooldown - .025f;
                enemyGen.howManyEnemies = enemyGen.enemiesPerWave * CurrentWave * enemyGen.currentDifficulty * 4;
            }
            
            //------------------------------------------------------------------------------------------------------------------
        }

        if (spawnableTiles.Count > 0)
        {
            CheckNeighbours();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void FixedUpdate()
    {
        if (CheckRotation)
        {
            SetRotation(coordenada);
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

            //ver 2 tiles a frente para evitar dead-ends
            Collider[] hitCollNN = Physics.OverlapSphere(spawnableTiles[i].transform.position + new Vector3(0f, 0, chunkSize * 2), checkRadius);
            Collider[] hitCollSS = Physics.OverlapSphere(spawnableTiles[i].transform.position + new Vector3(0f, 0, -chunkSize * 2), checkRadius);
            Collider[] hitCollEE = Physics.OverlapSphere(spawnableTiles[i].transform.position + new Vector3(chunkSize * 2, 0, 0f), checkRadius);
            Collider[] hitCollOO = Physics.OverlapSphere(spawnableTiles[i].transform.position + new Vector3(-chunkSize * 2, 0, 0f), checkRadius);

            //ver tiles diagonal
            Collider[] hitCollNO = Physics.OverlapSphere(spawnableTiles[i].transform.position + new Vector3(-chunkSize, 0, chunkSize), checkRadius);
            Collider[] hitCollNE = Physics.OverlapSphere(spawnableTiles[i].transform.position + new Vector3(chunkSize, 0, chunkSize), checkRadius);
            Collider[] hitCollSO = Physics.OverlapSphere(spawnableTiles[i].transform.position + new Vector3(-chunkSize , 0, -chunkSize), checkRadius);
            Collider[] hitCollSE = Physics.OverlapSphere(spawnableTiles[i].transform.position + new Vector3(chunkSize, 0, -chunkSize), checkRadius);

            //verifica entradas de todos os tiles que podem dar spawn de outros
            Collider[] entradaN = Physics.OverlapSphere(spawnableTiles[i].transform.position + new Vector3(0f, .5f, chunkSize / 2), checkRadiusEntradas);
            Collider[] entradaS = Physics.OverlapSphere(spawnableTiles[i].transform.position + new Vector3(0f, .5f, -chunkSize / 2), checkRadiusEntradas);
            Collider[] entradaE = Physics.OverlapSphere(spawnableTiles[i].transform.position + new Vector3(chunkSize / 2, .5f, 0f), checkRadiusEntradas);
            Collider[] entradaO = Physics.OverlapSphere(spawnableTiles[i].transform.position + new Vector3(-chunkSize / 2, .5f, 0f), checkRadiusEntradas);
            
            
            //N
            if (hitCollN.Length != 0 || entradaN.Length != 0 || hitCollNN.Length != 0 || hitCollNO.Length != 0 || hitCollNE.Length != 0)
            {
                placeN = false;
            }
            else if(hitCollN.Length == 0 && entradaN.Length == 0 && hitCollNN.Length == 0 && hitCollNO.Length == 0 && hitCollNE.Length == 0) placeN = true;


            //S
            if (hitCollS.Length != 0 || entradaS.Length != 0 || hitCollSS.Length != 0 || hitCollSO.Length != 0 || hitCollSE.Length != 0)
            {
                placeS = false;
            }
            else if (hitCollS.Length == 0 && entradaS.Length == 0 && hitCollSS.Length == 0 && hitCollSO.Length == 0 && hitCollSE.Length == 0) placeS = true; 


            //E
            if (hitCollE.Length != 0 || entradaE.Length != 0 || hitCollEE.Length != 0 || hitCollNE.Length != 0 || hitCollSE.Length != 0)
            {
                placeE = false;
            }
            else if(hitCollE.Length == 0 && entradaE.Length == 0 && hitCollEE.Length == 0 && hitCollNE.Length == 0 && hitCollSE.Length == 0) placeE = true;


            //O
            if (hitCollO.Length != 0 || entradaO.Length != 0 || hitCollOO.Length != 0 || hitCollNO.Length != 0 || hitCollSO.Length != 0)
            {
               placeW = false;
            }
            else if(hitCollO.Length == 0 && entradaO.Length == 0 && hitCollOO.Length == 0 && hitCollNO.Length == 0 && hitCollSO.Length == 0) placeW = true;

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
        if (spawnableTiles.Count > 0)
        {
            int pickedTile;

            if (spawnableTiles[0].gameObject.tag == "BaseTile") //Se o tileBase ainda tiver nos tiles, então deve sempre preencher primeiro esse tile
            {
                pickedTile = 0;
                posSpawn.Clear();
                spawnedTile.Clear();
            }
            else
            {
                pickedTile = Random.Range(0, spawnableTiles.Count); //de todos os tiles ja spawned escolhe um ao random
                posSpawn.Clear();
                spawnedTile.Clear();
            }

            #region Verifica adjacentes do tile escolhido
            Collider[] hitCollN = Physics.OverlapSphere(spawnableTiles[pickedTile].transform.position + new Vector3(0f, 0, chunkSize), checkRadius);
            Collider[] hitCollS = Physics.OverlapSphere(spawnableTiles[pickedTile].transform.position + new Vector3(0f, 0, -chunkSize), checkRadius);
            Collider[] hitCollE = Physics.OverlapSphere(spawnableTiles[pickedTile].transform.position + new Vector3(chunkSize, 0, 0f), checkRadius);
            Collider[] hitCollO = Physics.OverlapSphere(spawnableTiles[pickedTile].transform.position + new Vector3(-chunkSize, 0, 0f), checkRadius);

            //ver tiles diagonal
            Collider[] hitCollNO = Physics.OverlapSphere(spawnableTiles[pickedTile].transform.position + new Vector3(-chunkSize, 0, chunkSize), checkRadius);
            Collider[] hitCollNE = Physics.OverlapSphere(spawnableTiles[pickedTile].transform.position + new Vector3(chunkSize, 0, chunkSize), checkRadius);
            Collider[] hitCollSO = Physics.OverlapSphere(spawnableTiles[pickedTile].transform.position + new Vector3(-chunkSize, 0, -chunkSize), checkRadius);
            Collider[] hitCollSE = Physics.OverlapSphere(spawnableTiles[pickedTile].transform.position + new Vector3(chunkSize, 0, -chunkSize), checkRadius);

            //ver 2 tiles a frente para evitar dead-ends
            Collider[] hitCollNN = Physics.OverlapSphere(spawnableTiles[pickedTile].transform.position + new Vector3(0f, 0, chunkSize * 2), checkRadius);
            Collider[] hitCollSS = Physics.OverlapSphere(spawnableTiles[pickedTile].transform.position + new Vector3(0f, 0, -chunkSize * 2), checkRadius);
            Collider[] hitCollEE = Physics.OverlapSphere(spawnableTiles[pickedTile].transform.position + new Vector3(chunkSize * 2, 0, 0f), checkRadius);
            Collider[] hitCollOO = Physics.OverlapSphere(spawnableTiles[pickedTile].transform.position + new Vector3(-chunkSize * 2, 0, 0f), checkRadius);

            //N
            if (hitCollN.Length != 0 || hitCollNN.Length != 0 || hitCollNO.Length != 0 || hitCollNE.Length != 0)
            {
                posSpawn.Remove("placeN");
            }
            else if (hitCollN.Length == 0 && hitCollNN.Length == 0 && hitCollNO.Length == 0 && hitCollNE.Length == 0) posSpawn.Add("placeN");
            
            
            //S
            if (hitCollS.Length != 0 || hitCollSS.Length != 0 || hitCollSO.Length != 0 || hitCollSE.Length != 0)
            {
                posSpawn.Remove("placeS");
            }
            else if (hitCollS.Length == 0 && hitCollSS.Length == 0 && hitCollSO.Length == 0 && hitCollSE.Length == 0) posSpawn.Add("placeS");
            
            
            //E
            if (hitCollE.Length != 0 || hitCollEE.Length != 0 || hitCollNE.Length != 0 || hitCollSE.Length != 0)
            {
                posSpawn.Remove("placeE");
            }
            else if (hitCollE.Length == 0 && hitCollEE.Length == 0 && hitCollNE.Length == 0 && hitCollSE.Length == 0) posSpawn.Add("placeE");
            
            
            //O
            if (hitCollO.Length != 0 || hitCollOO.Length != 0 || hitCollNO.Length != 0 || hitCollSO.Length != 0)
            {
                posSpawn.Remove("placeW");
            }
            else if (hitCollO.Length == 0 && hitCollOO.Length == 0 && hitCollNO.Length == 0 && hitCollSO.Length == 0) posSpawn.Add("placeW");

            #endregion

            #region Verficar onde tile escolhido tem aberturas
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

            int coord = Random.Range(0, posSpawn.Count);
            int whatTile = Random.Range(0, 100); //0-99
            GameObject newTile;


            if (spawnableTiles[pickedTile].gameObject.tag != "BaseTile")
            {
                if (posSpawn[coord] == "placeN")
                {
                    if (whatTile < 65) //2 way tile
                    {
                        int w = Random.Range(0, twoWayTilesPref.Count);
                        newTile = Instantiate(twoWayTilesPref[w], spawnableTiles[pickedTile].transform.position + new Vector3(0, 0, chunkSize), Quaternion.identity);
                        spawnableTiles.Add(newTile); allTiles.Add(newTile);
                    }
                    else if (whatTile >= 65 && whatTile < 90) //3 way tile
                    {
                        int w = Random.Range(0, threeWayTilesPref.Count);
                        newTile = Instantiate(threeWayTilesPref[w], spawnableTiles[pickedTile].transform.position + new Vector3(0, 0, chunkSize), Quaternion.identity);
                        spawnableTiles.Add(newTile); allTiles.Add(newTile);
                    }
                    else //4 way tile
                    {
                        int w = Random.Range(0, fourWayTilesPref.Count);
                        newTile = Instantiate(fourWayTilesPref[w], spawnableTiles[pickedTile].transform.position + new Vector3(0, 0, chunkSize), Quaternion.identity);
                        spawnableTiles.Add(newTile); allTiles.Add(newTile);
                    }
                    spawnedTile.Add(newTile);
                }
                else if (posSpawn[coord] == "placeS")
                {
                    if (whatTile < 65) //2 way tile
                    {
                        int w = Random.Range(0, twoWayTilesPref.Count);
                        newTile = Instantiate(twoWayTilesPref[w], spawnableTiles[pickedTile].transform.position + new Vector3(0, 0, -chunkSize), Quaternion.identity);
                        spawnableTiles.Add(newTile); allTiles.Add(newTile);
                    }
                    else if (whatTile >= 65 && whatTile < 90) //3 way tile 
                    {
                        int w = Random.Range(0, threeWayTilesPref.Count);
                        newTile = Instantiate(threeWayTilesPref[w], spawnableTiles[pickedTile].transform.position + new Vector3(0, 0, -chunkSize), Quaternion.identity);
                        spawnableTiles.Add(newTile); allTiles.Add(newTile);
                    }
                    else //4 way tile
                    {
                        int w = Random.Range(0, fourWayTilesPref.Count);
                        newTile = Instantiate(fourWayTilesPref[w], spawnableTiles[pickedTile].transform.position + new Vector3(0, 0, -chunkSize), Quaternion.identity);
                        spawnableTiles.Add(newTile); allTiles.Add(newTile);
                    }
                    spawnedTile.Add(newTile);
                }
                else if (posSpawn[coord] == "placeE")
                {
                    if (whatTile < 65) //2 way tile
                    {
                        int w = Random.Range(0, twoWayTilesPref.Count);
                        newTile = Instantiate(twoWayTilesPref[w], spawnableTiles[pickedTile].transform.position + new Vector3(chunkSize, 0, 0), Quaternion.identity);
                        spawnableTiles.Add(newTile); allTiles.Add(newTile);
                    }
                    else if (whatTile >= 65 && whatTile < 90) //3 way tile
                    {
                        int w = Random.Range(0, threeWayTilesPref.Count);
                        newTile = Instantiate(threeWayTilesPref[w], spawnableTiles[pickedTile].transform.position + new Vector3(chunkSize, 0, 0), Quaternion.identity);
                        spawnableTiles.Add(newTile); allTiles.Add(newTile);
                    }
                    else //4 way tile
                    {
                        int w = Random.Range(0, fourWayTilesPref.Count);
                        newTile = Instantiate(fourWayTilesPref[w], spawnableTiles[pickedTile].transform.position + new Vector3(chunkSize, 0, 0), Quaternion.identity);
                        spawnableTiles.Add(newTile); allTiles.Add(newTile);
                    }
                    spawnedTile.Add(newTile);
                }
                else if (posSpawn[coord] == "placeW")
                {
                    if (whatTile < 65) //2 way tile
                    {
                        int w = Random.Range(0, twoWayTilesPref.Count);
                        newTile = Instantiate(twoWayTilesPref[w], spawnableTiles[pickedTile].transform.position + new Vector3(-chunkSize, 0, 0), Quaternion.identity);
                        spawnableTiles.Add(newTile); allTiles.Add(newTile);
                    }
                    else if (whatTile >= 65 && whatTile < 90) //3 way tile
                    {
                        int w = Random.Range(0, threeWayTilesPref.Count);
                        newTile = Instantiate(threeWayTilesPref[w], spawnableTiles[pickedTile].transform.position + new Vector3(-chunkSize, 0, 0), Quaternion.identity);
                        spawnableTiles.Add(newTile); allTiles.Add(newTile);
                    }
                    else //4 way tile
                    {
                        int w = Random.Range(0, fourWayTilesPref.Count);
                        newTile = Instantiate(fourWayTilesPref[w], spawnableTiles[pickedTile].transform.position + new Vector3(-chunkSize, 0, 0), Quaternion.identity);
                        spawnableTiles.Add(newTile); allTiles.Add(newTile);
                    }
                    spawnedTile.Add(newTile);
                }
            }
            else
            {
                if (posSpawn[coord] == "placeN")
                {
                    int w = Random.Range(0, adjacentToBase.Count);
                    newTile = Instantiate(adjacentToBase[w], spawnableTiles[pickedTile].transform.position + new Vector3(0, 0, chunkSize), Quaternion.identity);
                    spawnableTiles.Add(newTile); allTiles.Add(newTile);
                    spawnedTile.Add(newTile);
                    spawnedTile.Add(newTile);
                }
                else if (posSpawn[coord] == "placeS")
                {
                    int w = Random.Range(0, adjacentToBase.Count);
                    newTile = Instantiate(adjacentToBase[w], spawnableTiles[pickedTile].transform.position + new Vector3(0, 0, -chunkSize), Quaternion.identity);
                    spawnableTiles.Add(newTile); allTiles.Add(newTile);
                    spawnedTile.Add(newTile);
                }
                else if (posSpawn[coord] == "placeE")
                {
                    int w = Random.Range(0, adjacentToBase.Count);
                    newTile = Instantiate(adjacentToBase[w], spawnableTiles[pickedTile].transform.position + new Vector3(chunkSize, 0, 0), Quaternion.identity);
                    spawnableTiles.Add(newTile); allTiles.Add(newTile);
                    spawnedTile.Add(newTile);
                }
                else if (posSpawn[coord] == "placeW")
                {
                    int w = Random.Range(0, adjacentToBase.Count);
                    newTile = Instantiate(adjacentToBase[w], spawnableTiles[pickedTile].transform.position + new Vector3(-chunkSize, 0, 0), Quaternion.identity);
                    spawnableTiles.Add(newTile); allTiles.Add(newTile);
                    spawnedTile.Add(newTile);
                }
            }

            coordenada = posSpawn[coord]; //serve para SetRotation()
            CheckRotation = true;
            #endregion
        }
        else Debug.Log("No available spawn positions");
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
                tileToRotate.transform.rotation *= Quaternion.Euler(0, -90, 0);
            }
            else CheckRotation = false;
        }
        else if (placement == "placeE") //foi colocado a este por isso tem que ter entrada a oeste
        {
            Collider[] entrada = Physics.OverlapSphere(tileToRotate.transform.position + new Vector3(-chunkSize / 2, .5f, 0f), checkRadiusEntradas);
            if (entrada.Length != 0)
            {
                tileToRotate.transform.rotation *= Quaternion.Euler(0, -90, 0);
            }
            else CheckRotation = false;
        }
        else if (placement == "placeW") //foi colocado a oeste por isso tem que ter entrada a este
        {
            Collider[] entrada = Physics.OverlapSphere(tileToRotate.transform.position + new Vector3(chunkSize / 2, .5f, 0f), checkRadiusEntradas);
            if (entrada.Length != 0)
            {
                tileToRotate.transform.rotation *= Quaternion.Euler(0, 90, 0);
            }
            else CheckRotation = false; 
        }
        //UpdateNavMesh();
        SetSpawnPos();
    }

    //coloca os spawnPoints nos sitios corretos
    private void SetSpawnPos()
    {
        if (spawnPoints.Count > 0)
        {
            //limpa os spawnpoints a cada wave
            for (int s = 0; s < spawnPoints.Count; s++)
            {
                Destroy(spawnPoints[s]);
            }
            spawnPoints.Clear();
        }

        //verifica todos os tiles onde pode dar spawn e verifica as entradas, apenas coloca spawnPoints em tiles diferentes do baseTile
        for (int i = 0; i < allTiles.Count; i++)
        {
            if (allTiles[i].gameObject.tag != "BaseTile")
            {
                Collider[] hitCollN = Physics.OverlapSphere(allTiles[i].transform.position + new Vector3(0f, 0, chunkSize), checkRadius);
                Collider[] hitCollS = Physics.OverlapSphere(allTiles[i].transform.position + new Vector3(0f, 0, -chunkSize), checkRadius);
                Collider[] hitCollE = Physics.OverlapSphere(allTiles[i].transform.position + new Vector3(chunkSize, 0, 0f), checkRadius);
                Collider[] hitCollO = Physics.OverlapSphere(allTiles[i].transform.position + new Vector3(-chunkSize, 0, 0f), checkRadius);

                Collider[] entradaN = Physics.OverlapSphere(allTiles[i].transform.position + new Vector3(0f, .5f, chunkSize / 2), checkRadiusEntradas);
                Collider[] entradaS = Physics.OverlapSphere(allTiles[i].transform.position + new Vector3(0f, .5f, -chunkSize / 2), checkRadiusEntradas);
                Collider[] entradaE = Physics.OverlapSphere(allTiles[i].transform.position + new Vector3(chunkSize / 2, .5f, 0f), checkRadiusEntradas);
                Collider[] entradaO = Physics.OverlapSphere(allTiles[i].transform.position + new Vector3(-chunkSize / 2, .5f, 0f), checkRadiusEntradas);

                if (hitCollN.Length == 0 && entradaN.Length == 0)
                {
                    GameObject spawnPos = Instantiate(spawnPointPref, allTiles[i].transform.position + new Vector3(0f, spawnPointPref.transform.localScale.y / 2, chunkSize / 2.1f), Quaternion.identity);
                    spawnPoints.Add(spawnPos);
                }

                if (hitCollS.Length == 0 && entradaS.Length == 0)
                {
                    GameObject spawnPos = Instantiate(spawnPointPref, allTiles[i].transform.position + new Vector3(0f, spawnPointPref.transform.localScale.y / 2, -chunkSize / 2.1f), Quaternion.identity);
                    spawnPoints.Add(spawnPos);
                }

                if (hitCollE.Length == 0 && entradaE.Length == 0)
                {
                    GameObject spawnPos = Instantiate(spawnPointPref, allTiles[i].transform.position + new Vector3(chunkSize / 2.1f, spawnPointPref.transform.localScale.y / 2, 0f), Quaternion.identity);
                    spawnPoints.Add(spawnPos);
                }

                if (hitCollO.Length == 0 && entradaO.Length == 0)
                {
                    GameObject spawnPos = Instantiate(spawnPointPref, allTiles[i].transform.position + new Vector3(-chunkSize / 2.1f, spawnPointPref.transform.localScale.y / 2, 0f), Quaternion.identity);
                    spawnPoints.Add(spawnPos);
                }
            }
            
        }
        //UpdateNavMesh();
    }

    public void UpdateNavMesh()
    {
        nav.RemoveData();
        nav.BuildNavMesh(); //atualiza navMesh
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
                Gizmos.DrawWireSphere(spawnableTiles[i].transform.position + new Vector3(chunkSize * 2, 0, 0f), checkRadius);
                
                //Oeste
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(spawnableTiles[i].transform.position + new Vector3(-chunkSize, 0, 0f), checkRadius);
                Gizmos.DrawWireSphere(spawnableTiles[i].transform.position + new Vector3(-chunkSize * 2, 0, 0f), checkRadius);
                
                //Norte
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(spawnableTiles[i].transform.position + new Vector3(0f, 0, chunkSize), checkRadius);
                Gizmos.DrawWireSphere(spawnableTiles[i].transform.position + new Vector3(0f, 0, chunkSize * 2), checkRadius);
                
                //Sul
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(spawnableTiles[i].transform.position + new Vector3(0, 0, -chunkSize), checkRadius);
                Gizmos.DrawWireSphere(spawnableTiles[i].transform.position + new Vector3(0, 0, -chunkSize * 2), checkRadius);
            }

            if (showPathGizmo)
            {
                //Este
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(spawnableTiles[i].transform.position + new Vector3(chunkSize / 2, .5f, 0f), checkRadiusEntradas);
                //Oeste
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(spawnableTiles[i].transform.position + new Vector3(-chunkSize / 2, .5f, 0f), checkRadiusEntradas);
                //Norte
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(spawnableTiles[i].transform.position + new Vector3(0f, .5f, chunkSize / 2), checkRadiusEntradas);
                //Sul
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(spawnableTiles[i].transform.position + new Vector3(0f, .5f, -chunkSize / 2), checkRadiusEntradas);
            }
            
        }
    }

}
