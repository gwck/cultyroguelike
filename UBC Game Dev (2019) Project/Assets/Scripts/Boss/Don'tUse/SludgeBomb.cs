using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SludgeBomb : MonoBehaviour
{
    private Transform playerTransform;
    public ParticleSystem sludgeParticles;
    public GameObject splatPrefab;

    [SerializeField]
    public int timeToExplode;


    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        timeToExplode = Random.Range(1, 3);
        ExplodingBomb();
    }

    public void ExplodingBomb()
    {

        while (timeToExplode > 0)
        {
            while (transform.position.x != playerTransform.position.x && timeToExplode > 0)
            {
                transform.position += Vector3.forward * Time.deltaTime * 1;
                transform.position += Vector3.up * Time.deltaTime * 0.5f;

                timeToExplode--;
            }
        }
        GameObject splat = Instantiate(splatPrefab, transform.position, Quaternion.identity) as GameObject;
        sludgeParticles.transform.position = transform.position;
        gameObject.GetComponent<SludgeParticles>().Explosion();
        sludgeParticles.Play();
    }
}
