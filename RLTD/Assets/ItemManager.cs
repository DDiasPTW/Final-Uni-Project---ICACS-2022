using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public List<GameObject> allItems = new List<GameObject>();

    public GameObject currentItem;
    public GameObject itemHolderUI;

    private void Awake()
    {
        itemHolderUI.SetActive(false);
        currentItem = null;
    }

    private void Update()
    {
        if (currentItem == null)
        {
            itemHolderUI.SetActive(false);
        }
        else itemHolderUI.SetActive(true);
    }
}
