using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCreator : MonoBehaviour
{
    [SerializeField] private int ChunkX, ChunkZ;
    [SerializeField] private GameObject[,] gameGrid;
    public GameObject cube;
    void Start()
    {
        gameGrid = new GameObject[ChunkX,ChunkZ];
    }

 
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            GenerateGrid(ChunkX,ChunkZ);
        }
    }

    private void GenerateGrid(int chunkX, int chunkZ)
    {
        //coloca cubos 1 a 1, caso a grid comece a ficar demasiado grande a performance fica terrivel, tem que have um numero maximo de chunks
        //int newlarg = chunkX / 2;
        //int newcomp = chunkZ / 2;

        for (int i = 0; i < chunkX; i++)
        {
            for (int j = 0; j < chunkZ; j++)
            {
                gameGrid[i, j] = Instantiate(cube, new Vector3(i * chunkX /*- (newlarg)*/, 0, j * chunkZ /*- (newcomp)*/), Quaternion.identity);
            }
        }

        //mete cubo do tamanho de um chunk
        //GameObject grid = Instantiate(cube, new Vector3 (0,0,0), Quaternion.identity);
        //grid.transform.localScale = new Vector3(largura,transform.localScale.y,comprimento); 
    }
}
