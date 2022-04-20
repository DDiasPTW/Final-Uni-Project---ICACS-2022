using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public Sprite itemImage;
    public float aliveTime;
    public bool pickedUp = false;
    private void Update()
    {
        aliveTime -= Time.deltaTime;

        if (aliveTime <= 0 && !pickedUp)
        {
            Destroy(gameObject);
        }

        if (pickedUp)
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            gameObject.GetComponent<SphereCollider>().enabled = false;
        }


    }
}
