using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEffect : MonoBehaviour
{
    public void Destroy()
    {
        Destroy(gameObject, 0.5f);
    }
}
