using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGridGeneration : MonoBehaviour
{
    [Header("Full Grid Dimensions")] //As dimensoes finais da grid devem ser impar e equilaterais para que a base fique exatamente no meio (ex: 15 x 15 ou 13*13 ou 29 *29)
    public int GridX;
    public int GridZ;
    public int chunkSize; //tamanho de cada chunk
    public GameObject prefab; //prefab do tile default

    public int posMaxX;
    public int posMaxZ;

    public int holdPosMinX;
    public int holdPosMinZ;

    public Vector3[] placementPositions;
    public List<GameObject> placedChunks = new List<GameObject>();


    private void Awake()
    {
        placementPositions = new Vector3[GridX * GridZ];
        DefinePositions();
    }
    private void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            DefinePositions();
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            int chunk = Random.Range(0,placedChunks.Count-1);
            for (int i = 0; i < placedChunks.Count; i++)
            {
                placedChunks[chunk].GetComponent<PlaceChunk>().canPlace = true;
            }
        }
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

    private void OnDrawGizmos()
    {
        //serve para observar as dimnesoes maximas da grid final
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(gameObject.transform.position, new Vector3(GridX * chunkSize,1,GridZ * chunkSize));

        //serve para observar as posicoes onde se pode dar spawn de um chunk
        Gizmos.color = Color.blue;
        for (int i = 0; i < placementPositions.Length; i++)
        {
            Gizmos.DrawWireSphere(placementPositions[i],1f);
        }
    }
}
