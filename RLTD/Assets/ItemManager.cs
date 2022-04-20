using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
    public List<GameObject> allItems = new List<GameObject>();

    public GameObject currentItem;
    public GameObject itemHolderUI;
    public GameObject itemHolderImage;

    public LayerMask itemLayer;
    public LayerMask UILayer;

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
        }
        else itemHolderUI.SetActive(true);

        //if (currentItem != null)
        //{
        //    ActivateItem();
        //}
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
                
                currentItem = hit.collider.gameObject;
                currentItem.GetComponent<Item>().pickedUp = true;

                itemHolderImage.GetComponent<Image>().sprite = hit.collider.gameObject.GetComponent<Item>().itemImage;
                
            }
        }
        
    }

    public void ActivateItem() //chamado no event trigger do itemHolder (UI)
    {
        Debug.Log(currentItem.gameObject.name);
    }
}
