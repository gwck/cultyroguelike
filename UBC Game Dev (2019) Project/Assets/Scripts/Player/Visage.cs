using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Visage : MonoBehaviour
{
    [SerializeField] private float incomingDamageMultiplier = 1;
    [SerializeField] private float speedMultiplier = 1;
    [SerializeField] private float jumpVelocityMultiplier = 1;
    [SerializeField] private float attackDelayMultiplier = 1;
    [SerializeField] private float attackRangeMultiplier = 1;
    [SerializeField] private float weaponDamageMultiplier = 1;
    [SerializeField] private float knockbackForceMultiplier = 1;
    public string text = "Item";
    public float duration = 10f;
    [HideInInspector] public float time = 0f;

    private void Start()
    {
        // allow the item to drop for half a second before it can be picked up
        Invoke("EnableCollider", 0.5f);
    }

    private void EnableCollider()
    {
        GetComponent<Collider2D>().enabled = true;
    }

    public float ModifyDamageReceived(float damage)
    {
        return damage * incomingDamageMultiplier;
    }
    public float ModifySpeed(float speed)
    {
        return speed * speedMultiplier;
    }
    public float ModifyJumpVelocity(float velocity)
    {
        return velocity * jumpVelocityMultiplier;
    }
    public float ModifyAttackDelay(float delay)
    {
        return delay * attackDelayMultiplier;
    }

    public float ModifyAttackRange(float range)
    {
        return range * attackRangeMultiplier;
    }

    public float ModifyWeaponDamage(float damage)
    {
        return damage * weaponDamageMultiplier;
    }

    public Vector2 ModifyKnockbackForce(Vector2 force)
    {
        return force * knockbackForceMultiplier;
    }
}
