using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Trap : MonoBehaviour
{
    [Header("Classes necessarias")]
    private Item item;
    private ItemManager iM;
    private WorldGeneration worldGen;
    [Header("Building")]
    public LayerMask layerToBuild;
    private bool canPlace;
    [Header("Visualizadores")]
    public GameObject rangeSprite;
    public Sprite thisSprite;
    public Color canPlaceColor;
    public Color cannotPlaceColor;
    private Vector3 mousePos;
    private bool spawnedRange = false;
    [Header("Ataque")]
    public LayerMask enemyLayer;
    public LayerMask itemLayer;
    public float range; //range do ataque AOE
    public float activationRange; //range minima que os inimigos precisam de estar para o item ser ativado
    [Range(0,1)]
    public float multiplier;
    public float duration;
    private bool canAttack = false;
    public float attackDelay;

    private void Awake()
    {
        worldGen = GameObject.FindGameObjectWithTag("GridManager").GetComponent<WorldGeneration>();
        iM = GameObject.FindGameObjectWithTag("GridManager").GetComponent<ItemManager>();
        item = GetComponent<Item>();
        item.itemImage = thisSprite;
    }


    void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (item.activated)
        {
            //Do stuff
            gameObject.GetComponentInChildren<MeshRenderer>().enabled = true;
            FollowMouse();

            if (!spawnedRange)
            {
                rangeSprite = Instantiate(rangeSprite);
                spawnedRange = true;
            }

            if (Input.GetMouseButtonDown(1))
            {
                item.activated = false;
            }
        }
        else if (!item.activated)
        {
            rangeSprite.transform.localScale *= 0;
        }

        if (item.placed)
        {
            CheckEnemies();
        }

        if (canAttack)
        {
            attackDelay -= Time.deltaTime;

            if (attackDelay <= 0)
            {
                Attack();
            }
        }
    }

    private void FollowMouse()
    {
        Vector3 seePos = new Vector3();
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        //Apenas mostra os sitios onde se pode colocar a bomba
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerToBuild))
        {
            if (Mathf.Round(hit.point.x) <= (-(worldGen.chunkSize / 2) + hit.transform.position.x + 1f)) //min x
            {
                seePos.x = Mathf.Round(hit.point.x);
            }

            else if (Mathf.Round(hit.point.x) >= ((worldGen.chunkSize / 2) + hit.transform.position.x - 1f)) //max x
            {
                seePos.x = Mathf.Round(hit.point.x);
            }

            else if (Mathf.Round(hit.point.x) < 0 && Mathf.Round(hit.point.x) > (-(worldGen.chunkSize / 2) + hit.transform.position.x + 1f)) //x neg
            {
                seePos.x = Mathf.Round(hit.point.x);
            }

            else if (Mathf.Round(hit.point.x) >= 0 && Mathf.Round(hit.point.x) < ((worldGen.chunkSize / 2) + hit.transform.position.x - 1f)) //x pos
            {
                seePos.x = Mathf.Round(hit.point.x);
            }

            //z
            if (Mathf.Round(hit.point.z) <= (-(worldGen.chunkSize / 2) + hit.transform.position.z + 1f)) //min z
            {
                seePos.z = Mathf.Round(hit.point.z);
            }

            else if (Mathf.Round(hit.point.z) >= ((worldGen.chunkSize / 2) + hit.transform.position.z - 1f)) //max z
            {
                seePos.z = Mathf.Round(hit.point.z);
            }

            else if (Mathf.Round(hit.point.z) < 0 && Mathf.Round(hit.point.z) > (-(worldGen.chunkSize / 2) + hit.transform.position.z + 1f)) //z neg
            {
                seePos.z = Mathf.Round(hit.point.z);
            }

            else if (Mathf.Round(hit.point.z) >= 0 && Mathf.Round(hit.point.z) < ((worldGen.chunkSize / 2) + hit.transform.position.z - 1f)) //z pos
            {
                seePos.z = Mathf.Round(hit.point.z);
            }

            Collider[] checkItens = Physics.OverlapSphere(new Vector3(seePos.x, hit.point.y, seePos.z), .3f, itemLayer);

            if (hit.point.y <= .05f && checkItens.Length == 0)
            {
                canPlace = true;
            }
            else canPlace = false;


            transform.position = new Vector3(seePos.x, hit.point.y + (transform.localScale.y / 2), seePos.z);

            rangeSprite.transform.localScale = new Vector3(range, range, 0);
            rangeSprite.transform.position = new Vector3(seePos.x, hit.point.y + .1f, seePos.z);
            //Debug.Log(hit.point.y) ;
        }
        else
        {
            //visualizador de range
            rangeSprite.transform.localScale *= 0;
            rangeSprite.transform.position = mousePos;
            transform.position = mousePos;
        }

        //Visualizador de colocar itens
        if (canPlace)
        {
            rangeSprite.GetComponent<SpriteRenderer>().color = canPlaceColor;
        }
        else if (!canPlace)
        {
            rangeSprite.GetComponent<SpriteRenderer>().color = cannotPlaceColor;
        }

        //Place the bomb
        if (Input.GetMouseButtonDown(0) && canPlace)
        {
            gameObject.transform.position = new Vector3(seePos.x, hit.point.y + (transform.localScale.y / 2), seePos.z);

            item.placed = true;
            item.activated = false;
            iM.currentItem = null;
        }
    }

    private void CheckEnemies()
    {
        Collider[] allTargets = Physics.OverlapSphere(transform.position, activationRange / 11, enemyLayer);
        if (allTargets.Length != 0)
        {
            canAttack = true;
        }
    }

    private void Attack()
    {
        Collider[] allTargets = Physics.OverlapSphere(transform.position, range / 11, enemyLayer);

        for (int i = 0; i < allTargets.Length; i++)
        {
            allTargets[i].GetComponent<Enemy>().ChangeSpeed(multiplier,duration);
        }
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, activationRange / 11);
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, range / 11);
    }
}
