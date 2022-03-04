using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCreator : MonoBehaviour
{
    [SerializeField] private int largura, comprimento;
    //[SerializeField] private GameObject[,] gameGrid;
    public GameObject cube;
    void Start()
    {
        //gameGrid = new GameObject[largura,comprimento];
    }

 
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            GenerateGrid(largura,comprimento);
        }
    }

    private void GenerateGrid(int largura, int comprimento)
    {
        //coloca cubos 1 a 1, caso a grid comece a ficar demasiado grande a performance fica terrivel
        //int newlarg = largura / 2;
        //int newcomp = comprimento / 2;

        //for (int i = 0; i < largura; i++)
        //{
        //    for (int j = 0; j < comprimento; j++)
        //    {
        //        gameGrid[i, j] = Instantiate(cube, new Vector3(i - (newlarg), 0, j - (newcomp)), Quaternion.identity);
        //    }
        //}
        GameObject grid = Instantiate(cube, new Vector3 (0,0,0), Quaternion.identity);
        grid.transform.localScale = new Vector3(largura,transform.localScale.y,comprimento); 
    }
}
