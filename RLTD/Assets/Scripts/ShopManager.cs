using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{ 
    //----
    public bool canOpen = false;
    public Animator anim;
    private string closeShopAnimation = "CloseShop_Anim";
    private string openShopAnimation = "OpenShop_Anim";
    private EnemyGeneration enemyGen;
    //----
    private void Awake()
    {
        anim = GetComponent<Animator>();
        enemyGen = GameObject.FindGameObjectWithTag("GridManager").GetComponent<EnemyGeneration>();
    }

    #region Abrir/Fechar UI
    public void ShopButton()
    {
        if (!enemyGen.isSpawning)
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
      
    }
    #endregion
}
