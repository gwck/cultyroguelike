using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{   //Class that has all the info we need about a SINGLE dialogue

    public string name;
    [TextArea(3, 10)] //Amount of lines text area will use
    public string[] sentences;
}
