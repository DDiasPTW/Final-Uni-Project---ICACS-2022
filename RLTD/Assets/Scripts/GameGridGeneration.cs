using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGridGeneration : MonoBehaviour
{
    [Header("Full Grid Dimensions")] //As dimensoes finais da grid devem ser impar e equilaterais para que a base fique exatamente no meio (ex: 15*15 ou 13*13 ou 29*29)
                                     //Caso nao sejam, os chunks devem ter dimensoes impar. Ex: grid impar (15*15) -> chunk (10*10), grid par (10*10) -> chunk (5*5)
    public int GridX;
    public int GridZ;
    public int chunkSize; //tamanho de cada chunk

    [Header("Check grid coords")]
    public int posMaxX;
    public int posMaxZ;
    public int holdPosMinX;
    public int holdPosMinZ;

    [Header("Check Placements")]
    public Vector3[] placementPositions;
    public bool[] availablePositions;
    public float checkRadius;



    private void Awake()
    {
        placementPositions = new Vector3[GridX * GridZ];
        availablePositions = new bool[placementPositions.Length];
        DefinePositions();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            PlaceChunk(); //TO DO
        }
    }

    private void FixedUpdate()
    {
        DetectPlacedChunks();
    }

    private void DefinePositions()
    {
        //Define qual a coordenada positiva maxima que o chunk pode ter
        posMaxX = ((chunkSize * GridX) / 2) - (chunkSize / 2);
        posMaxZ = ((chunkSize * GridZ) / 2) - (chunkSize / 2);
        //Define qual a coordenada negativa minima que o chunk pode ter
        int posMinX = -(((chunkSize * GridX) / 2) - (chunkSize / 2));
        int posMinZ = -(((chunkSize * GridZ) / 2) - (chunkSize / 2));
        //serve para segurar a coordenada minima
        holdPosMinX = posMinX;
        holdPosMinZ = posMinZ;

        //percorre todas as posicoes possiveis
        for (int i = 0; i < placementPositions.Length; i++)
        {
            //a primeira posicao definida sera a posicao minima
            placementPositions[i] = new Vector3(posMinX, 0, posMinZ);

            //a cada loop aumenta a coordenada X de acordo com o tamanho de cada chunk
            posMinX += chunkSize;

            //quando a posicao do X atinge um valor maior que o valor maximo mete-se o valor novamente minimo e aumenta-se o Z por um valor de chunk
            if (posMinX > posMaxX)
            {
                posMinX = holdPosMinX;
                posMinZ += chunkSize;
            }

            if (posMinZ > posMaxZ)
            {
                posMinZ = holdPosMinZ;
            }
        }


        //Debug.Log("Min X: " + holdPosMinX + " Min Z: " + holdPosMinZ);
        //Debug.Log("Pos max = " + new Vector3(posMaxX,0,posMaxZ));
        //Debug.Log("Pos min = " + new Vector3(posMinX,0,posMinZ));
    }

    private void DetectPlacedChunks()
    {
        //Em cada posicao no array mete uma pequena esfera, se detetar alguma colisao
        //mete essa posicao das availablePositions[] como falsa

        
        //Detetar colisao
        for (int i = 0; i < placementPositions.Length; i++)
        {
            Collider[] hitColl = Physics.OverlapSphere(placementPositions[i],checkRadius);
            if (hitColl.Length != 0)
            {
                availablePositions[i] = false;
            }
            else if (hitColl.Length == 0) availablePositions[i] = true; //else Debug.Log("Encontrados mais do que 1 collider");
        }
    }

    private void PlaceChunk()
    {
               
    }

    private void OnDrawGizmos()
    {
        //serve para observar as dimensoes maximas da grid final
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(gameObject.transform.position, new Vector3(GridX * chunkSize,1,GridZ * chunkSize));

        //serve para observar as posicoes onde se pode dar spawn de um chunk
        
        for (int i = 0; i < placementPositions.Length; i++)
        {
            if (availablePositions[i] == true)
            {
                Gizmos.color = Color.blue;

            }
            else Gizmos.color = Color.red;

            Gizmos.DrawWireSphere(placementPositions[i], checkRadius);
        }


    }
}
