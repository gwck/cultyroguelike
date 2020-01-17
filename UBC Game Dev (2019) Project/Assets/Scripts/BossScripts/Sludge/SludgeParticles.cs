using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SludgeParticles : MonoBehaviour
{
    public ParticleSystem sludgeParticles;
    public GameObject sludgePrefab;
    public int sludgeDamage;
    public SludgeBomb sludgeBomb;

    public void Explosion() { 
       GameObject sludge = Instantiate(sludgePrefab, sludgeBomb.gameObject.transform.position, Quaternion.identity); //Quaternion.identity means NO ROTATION
       Sludge sludgeScript = sludge.GetComponent<Sludge>();
       sludgeScript.Intialize();
        }

    }
