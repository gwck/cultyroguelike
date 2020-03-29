using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public void Win()
    {
        VictoryMenu menu = GameObject.FindObjectOfType<VictoryMenu>();
        menu.Win();
    }
}
