using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public int CurrentCoins;
    public int CurrentTowerCost;
    public GameObject TowerToBuild;
    private WorldGeneration worldGen;

    private Vector3 mousePos;

    private GameObject Tower;

    [SerializeField] private bool canBuild;


    private void Awake()
    {
        worldGen = GetComponent<WorldGeneration>();
    }

    //TO DO:
    //Follow cursor - adicionar gameObject pref, instanciado logo no inicio (so que vazio), da update sempre q jogador compra torre nova e segue cursor

    //Torres custarem dinheiro - apenas quando coloca a torre

    //Indicadores de onde pode colocar torre

    //FAZER COLOCAR TORRE APENAS NOS SITIOS CERTOS (NAO PODE SER EM CAMINHOS) + Colocar apenas de 2 em 2 (vector3.distance?), NAO PODE COLOCAR NO MESMO SITIO

    //CLEAN UP CODIGO

    private void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);


        if (TowerToBuild != null && CurrentCoins >= CurrentTowerCost)
        {
            canBuild = true;
        }
        else canBuild = false;

        if (canBuild)
        {
            if (Input.GetMouseButtonDown(0))
            {
                PlaceTower();
            }

            if (Input.GetMouseButtonDown(1)) //Dar reset da compra, caso queira comprar outra ou nao queira comprar de todo
            {
                ResetPurchase();
            }
        }
    }


    private void PlaceTower()
    {
        Vector3 buildPos = new Vector3();
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            //x
            if (Mathf.Round(hit.point.x) <= (-(worldGen.chunkSize / 2) + hit.transform.position.x + 1)) //min x
            {
                buildPos.x = (-(worldGen.chunkSize / 2) + hit.transform.position.x + 1) + 1f;
            }

            else if (Mathf.Round(hit.point.x) >= ((worldGen.chunkSize / 2) + hit.transform.position.x - 1)) //max x
            {
                buildPos.x = ((worldGen.chunkSize / 2) + hit.transform.position.x - 1) - 1f;
            }

            else if (Mathf.Round(hit.point.x) < 0 && hit.point.x > (-(worldGen.chunkSize / 2) + hit.transform.position.x + 1)) //x neg
            {
                buildPos.x = Mathf.Round(hit.point.x);
            }

            else if (Mathf.Round(hit.point.x) >= 0 && hit.point.x < ((worldGen.chunkSize / 2) + hit.transform.position.x - 1)) //x pos
            {
                buildPos.x = Mathf.Round(hit.point.x);
            }

            //z
            if (Mathf.Round(hit.point.z) <= (-(worldGen.chunkSize / 2) + hit.transform.position.z + 1)) //min z
            {
                buildPos.z = (-(worldGen.chunkSize / 2) + hit.transform.position.z + 1) + 1f;
            }

            else if (Mathf.Round(hit.point.z) >= ((worldGen.chunkSize / 2) + hit.transform.position.z - 1)) //max z
            {
                buildPos.z = ((worldGen.chunkSize / 2) + hit.transform.position.z - 1) - 1f;
            }

            else if (hit.point.z < 0 && hit.point.z > (-(worldGen.chunkSize / 2) + hit.transform.position.z + 1)) //z neg
            {
                buildPos.z = Mathf.Round(hit.point.z);
            }

            else if (Mathf.Round(hit.point.z) >= 0 && hit.point.z < ((worldGen.chunkSize / 2) + hit.transform.position.z - 1)) //z pos
            {
                buildPos.z = Mathf.Round(hit.point.z);
            }

            //Debug.Log("Max x: " + ((worldGen.chunkSize / 2) + hit.transform.position.x - 1));
            //Debug.Log("Min x: " + (-(worldGen.chunkSize / 2) + hit.transform.position.x + 1));
            //Debug.Log("Max z: " + ((worldGen.chunkSize / 2) + hit.transform.position.z - 1));
            //Debug.Log("Min z: " + (-(worldGen.chunkSize / 2) + hit.transform.position.z + 1));

            Debug.Log("Clicado em: " + hit.point);
            Debug.Log("Colocado em: " + new Vector3(buildPos.x, (transform.localScale.y / 2) + 2, buildPos.z));

            Tower.transform.position = new Vector3(buildPos.x, (transform.localScale.y / 2) + 2, buildPos.z);
        }
    }
    private void ResetPurchase()
    {
        TowerToBuild = null;
        CurrentTowerCost = 0;
        Destroy(Tower);
        Tower = null;
    }

}
