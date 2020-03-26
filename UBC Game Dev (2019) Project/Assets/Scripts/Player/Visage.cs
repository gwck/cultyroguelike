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
    public float duration = 3f;
    [HideInInspector] public float time = 0f;

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
