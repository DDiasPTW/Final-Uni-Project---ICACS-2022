using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    //private int rotY;
    //private float startPosY;
    [SerializeField] private Transform spawnPos;
    private GameManager gM;
    [SerializeField]private int control = 0;
    [SerializeField] private GameObject moneyPref;
    public float spawnDelay;
    private float delay;
    public List<GameObject> moneys = new List<GameObject>();

    private void Awake()
    {
        gM = GameObject.FindGameObjectWithTag("GridManager").GetComponent<GameManager>();
        delay = spawnDelay;
        //startPosY = transform.position.y;
        //rotY = (int)startPosY - 6;
    }

    void Update()
    {
        spawnDelay -= Time.deltaTime;

        if (spawnDelay <= 0)
        {
            if (control < gM.Health)
            {
                SpawnMoney();
            }
            else
            {
                if (moneys.Count > gM.Health)
                {
                    int difference = moneys.Count - gM.Health;
                    UpdateMoney(difference);
                }
            }
        }

        
        
        
        //transform.position = new Vector3(0, (Mathf.Sin(Time.time) * rotY) + startPosY,0);
    }

    private void SpawnMoney()
    {
        GameObject money = Instantiate(moneyPref, spawnPos);
        moneys.Add(money);
        control++;
        spawnDelay = delay;
    }

    private void UpdateMoney(int difference)
    {
        for (int i = 0; i < difference - 1; i++)
        {
            Destroy(moneys[i]);
            moneys.RemoveAt(i);
        }
    }
}
