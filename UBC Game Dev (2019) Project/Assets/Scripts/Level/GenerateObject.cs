using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateObject : MonoBehaviour
{
    [SerializeField] GameObject[] objs;
    
    void Start()
    {
        int rand = Random.Range(0, objs.Length);
        var obj = Instantiate(objs[rand], transform.position, Quaternion.identity, transform.parent);

        Destroy(gameObject);
    }
}
