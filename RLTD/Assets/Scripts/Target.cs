using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Target : MonoBehaviour
{
    private GameManager gM;
    public List<TextMeshProUGUI> healthText;

    private void Awake()
    {
        gM = GameObject.FindGameObjectWithTag("GridManager").GetComponent<GameManager>();
    }

    void Update()
    {
        for (int i = 0; i < healthText.Count; i++)
        {
            healthText[i].text = gM.Health.ToString();
        }
    }
}
