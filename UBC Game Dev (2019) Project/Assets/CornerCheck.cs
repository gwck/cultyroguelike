using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CornerCheck : MonoBehaviour
{
    [SerializeField] private Transform checkPoint;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("hello");
        CheckForNeighbor();
    }

    void CheckForNeighbor()
    {
        Collider2D[] colliders = Physics2D.OverlapPointAll(checkPoint.position);
        Debug.Log("checked " + colliders.Length);
        foreach (Collider2D collider in colliders)
        {
            Debug.Log(collider.gameObject.name);
            if (collider.tag == "RaisedArea")
                Destroy(gameObject);
        }
    }
}
