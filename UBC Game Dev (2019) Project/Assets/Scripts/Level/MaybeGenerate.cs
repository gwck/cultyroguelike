using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaybeGenerate : MonoBehaviour
{

    [SerializeField] [Range(0, 1)] private float chance = 0.5f;
    [SerializeField] private GameObject obj;

    // Start is called before the first frame update
    void Start()
    {
        if (Random.Range(0f, 1f) < chance)
        {
            Instantiate(obj, transform.position, Quaternion.identity, transform.parent);
        }

        Destroy(gameObject);
    }
}
