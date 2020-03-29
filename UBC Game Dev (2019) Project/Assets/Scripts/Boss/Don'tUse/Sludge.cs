using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sludge : MonoBehaviour
{
    public float minSize;
    public float maxSize;

    public Sprite sprite;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    //EFFECTS: Intializes the sprite
    public void Intialize()
    {
        SetSize();
        SetRotation();

        //  spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask; //sprite is not visible beyond ground!
        spriteRenderer.sortingOrder = 3; //done so sprite does not overlap with things
    }

    //EFFECTS: Set size of sprite
    private void SetSize()
    {
        float sizeModifier = Random.Range(minSize, maxSize);
        transform.localScale *= sizeModifier;

    }

    //EFFECTS: Set rotation of sprite
    private void SetRotation()
    {
        float randRotation = Random.Range(-360, 360);
        transform.rotation *= Quaternion.Euler(0f, 0f, randRotation);
    }


}
