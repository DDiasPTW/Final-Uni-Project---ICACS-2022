using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Target : MonoBehaviour
{
    //private int rotY;
    //private float startPosY;
    private GameManager gM;
    //[SerializeField] private Transform spawnPos;
    //[SerializeField]private int control = 0;
    //[SerializeField] private GameObject moneyPref;
    //public float spawnDelay;
    //private float delay;
    //public List<GameObject> moneys = new List<GameObject>();

    public List<TextMeshProUGUI> healthText;

    private void Awake()
    {
        gM = GameObject.FindGameObjectWithTag("GridManager").GetComponent<GameManager>();
        //delay = spawnDelay;
        //startPosY = transform.position.y;
        //rotY = (int)startPosY - 6;
    }

    void Update()
    {
        for (int i = 0; i < healthText.Count; i++)
        {
            healthText[i].text = gM.Health.ToString();
        }

        //spawnDelay -= Time.deltaTime;

        //if (spawnDelay <= 0)
        //{
        //    if (control < gM.Health)
        //    {
        //        SpawnMoney();
        //    }
        //    else
        //    {
        //        if (moneys.Count > gM.Health)
        //        {
        //            int difference = moneys.Count - gM.Health;
        //            UpdateMoney(difference);
        //        }
        //    }
        //}
        //transform.position = new Vector3(0, (Mathf.Sin(Time.time) * rotY) + startPosY,0);
    }

    //private void SpawnMoney()
    //{
    //    GameObject money = Instantiate(moneyPref, spawnPos);
    //    moneys.Add(money);
    //    control++;
    //    spawnDelay = delay;
    //}

    //private void UpdateMoney(int difference)
    //{
    //    for (int i = 0; i < difference - 1; i++)
    //    {
    //        Destroy(moneys[i]);
    //        moneys.RemoveAt(i);
    //    }
    //}
}
