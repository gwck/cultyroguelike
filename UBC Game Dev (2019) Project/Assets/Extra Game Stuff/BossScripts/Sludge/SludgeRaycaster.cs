using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SludgeRaycaster : MonoBehaviour
{
    
    public ParticleSystem sludgeParticles;
    public GameObject sludgePrefab;

    public GameObject boss;
    // Start is called before the first frame update
    void Start()
    {
        boss = GameObject.FindGameObjectWithTag("Boss");
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void CastRay()
    {
        Ray ray = Camera.main.ScreenPointToRay(boss.GetComponent<BossAttack>().sludgePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

        if (hit.collider != null)
        {
            GameObject sludge = Instantiate(sludgePrefab, boss.GetComponent<BossAttack>().sludgePosition, Quaternion.identity);
            Sludge sludgeScript = sludge.GetComponent<Sludge>();
            sludgeParticles.transform.position = hit.point;
            sludgeParticles.Play();
        }

    }
}
