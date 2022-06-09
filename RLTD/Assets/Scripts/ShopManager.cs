using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{ 
    //----
    public bool isOpen = false;
    public Animator anim;
    private string closeShopAnimation = "CloseShop_Anim";
    private string openShopAnimation = "OpenShop_Anim";
    private EnemyGeneration enemyGen;
    private WorldGeneration worldGen;
    //----
    private void Awake()
    {
        isOpen = false;
        anim = GetComponent<Animator>();
        enemyGen = GameObject.FindGameObjectWithTag("GridManager").GetComponent<EnemyGeneration>();
        worldGen = GameObject.FindGameObjectWithTag("GridManager").GetComponent<WorldGeneration>();
    }

    #region Abrir/Fechar UI
    public void ShopButton()
    {
        if (isOpen)
        {
            isOpen = false;
            anim.Play(closeShopAnimation);
        }
        if (!isOpen)
        {
            isOpen = true;
            anim.Play(openShopAnimation);
        }

    }
    #endregion
}
