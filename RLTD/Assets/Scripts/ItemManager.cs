using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ItemManager : MonoBehaviour
{
    public List<GameObject> allItems = new List<GameObject>();

    public GameObject currentItem;
    public GameObject itemHolderUI;
    public GameObject itemHolderPreview;

    [SerializeField] private GameObject itemDesc;
    [SerializeField] private Image itemDescImage;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private GameObject itemPrev;
    [SerializeField] private GameObject towerPrev;

    public GameObject itemPickupVFX;
    public LayerMask itemLayer;

    private void Awake()
    {
        itemHolderUI.SetActive(false);
        currentItem = null;
    }

    private void Update()
    {
        PickupItem();

        if (currentItem == null)
        {
            itemHolderUI.SetActive(false);
            itemPrev.GetComponent<MeshFilter>().mesh = null;
        }
        else itemHolderUI.SetActive(true);

        if (Input.GetMouseButton(1))
        {
            if (currentItem != null)
            {
                if (currentItem.GetComponent<Item>().activated == true)
                {
                    currentItem.GetComponent<Item>().activated = false;
                }
            }
            
        }
    }

    void PickupItem()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, itemLayer))
            {
                if (currentItem != null)
                {
                    currentItem.GetComponent<Item>().pickedUp = false;
                    currentItem = null;
                }

                if (!hit.collider.gameObject.GetComponent<Item>().placed)
                {
                    currentItem = hit.collider.gameObject;
                    currentItem.GetComponent<Item>().pickedUp = true;
                }
                Instantiate(itemPickupVFX,hit.collider.transform);
                itemHolderPreview.GetComponent<MeshFilter>().mesh = hit.collider.gameObject.GetComponent<Item>().thisMesh;
                
            }
        }
        
    }

    public void ActivateItem() //chamado no event trigger do itemHolder (UI)
    {
        currentItem.GetComponent<Item>().activated = true;

        itemDesc.SetActive(true);
        descriptionText.text = currentItem.GetComponent<Item>().description;
        itemPrev.GetComponent<MeshFilter>().mesh = currentItem.GetComponentInChildren<MeshFilter>().sharedMesh;
        towerPrev.GetComponent<MeshFilter>().mesh = null;
    }
}
