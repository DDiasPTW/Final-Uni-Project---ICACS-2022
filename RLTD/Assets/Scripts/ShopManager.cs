using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{ 
    //----
    public bool canOpen = false;
    private Animator anim;
    private string closeShopAnimation = "CloseShop_Anim";
    private string openShopAnimation = "OpenShop_Anim";
    //----
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }




    #region Abrir UI
    public void ShopButton()
    {
        if (!canOpen)
        {
            anim.Play(closeShopAnimation);
            canOpen = true;
        }
        if (canOpen)
        {
            anim.Play(openShopAnimation);
            canOpen = false;
        }
    }
    #endregion
}
