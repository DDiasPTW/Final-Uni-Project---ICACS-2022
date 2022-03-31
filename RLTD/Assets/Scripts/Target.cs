using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    private int rotY;
    private float startPosY;

    private void Awake()
    {
        startPosY = transform.position.y;
        rotY = (int)startPosY - 6;
    }

    void Update()
    {
        transform.position = new Vector3(0, (Mathf.Sin(Time.time) * rotY) + startPosY,0);
    }
}
