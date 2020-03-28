using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CornerCheck : MonoBehaviour
{
    [SerializeField] private Transform checkPoint;

    // Start is called before the first frame update
    void Start()
    {
        CheckForNeighbor();
    }

    void CheckForNeighbor()
    {
        Collider2D[] colliders = Physics2D.OverlapPointAll(checkPoint.position);
        foreach (Collider2D collider in colliders)
        {
            if (collider.tag == "RaisedArea")
                Destroy(gameObject);
        }
    }
}
