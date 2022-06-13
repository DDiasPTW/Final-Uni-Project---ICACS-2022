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
    public GameObject itemHolderImage;

    [SerializeField] private GameObject itemDesc;
    [SerializeField] private Image itemDescImage;
    [SerializeField] private TextMeshProUGUI descriptionText;

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
                

                itemHolderImage.GetComponent<Image>().sprite = hit.collider.gameObject.GetComponent<Item>().itemImage;
                
            }
        }
        
    }

    public void ActivateItem() //chamado no event trigger do itemHolder (UI)
    {
        //Debug.Log(currentItem.gameObject.name);
        currentItem.GetComponent<Item>().activated = true;
        
        
        itemDesc.SetActive(true);
        descriptionText.text = currentItem.GetComponent<Item>().description;
        itemDescImage.sprite = currentItem.GetComponent<Item>().itemImage;
        //Debug.Log("Show item desc");
    }
}
