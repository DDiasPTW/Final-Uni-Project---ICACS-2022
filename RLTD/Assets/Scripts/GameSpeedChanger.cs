using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSpeedChanger : MonoBehaviour
{
    public List<Button> buttons = new List<Button>();
    public void SpeedHalf()
    {
        Time.timeScale = .5f;
        buttons[0].interactable = false;
        buttons[1].interactable = true;
        buttons[2].interactable = true;
        buttons[3].interactable = true;
    }

    public void SpeedNormal()
    {
        Time.timeScale = 1;
        buttons[0].interactable = true;
        buttons[1].interactable = false;
        buttons[2].interactable = true;
        buttons[3].interactable = true;
    }

    public void SpeedDouble()
    {
        Time.timeScale = 2;
        buttons[0].interactable = true;
        buttons[1].interactable = true;
        buttons[2].interactable = false;
        buttons[3].interactable = true;
    }

    public void SpeedTriple()
    {
        Time.timeScale = 3;
        buttons[0].interactable = true;
        buttons[1].interactable = true;
        buttons[2].interactable = true;
        buttons[3].interactable = false;
    }
}
