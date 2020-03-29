using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private PlayerController player;

    // used by playerController during the Attack method
    public float range; // radius of the attack's effects from the attackCheck point
    public float delay; // amount of time after an attack before the player can attack again
    public float invulnDuration; // duration of invulnerability during attack animation
    public GameObject effect; // visual effect of an attack

    // used in the Hit method
    [HideInInspector] public int damage; // damage of the attack
    [SerializeField] private Vector2 knockbackForce; // force to be applied to enemies that are hit with attacks
    [SerializeField] private float stunDuration; // amount of time stun effect that accompanies knockback lasts

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    // apply damage and any additional effects to the enemy
    public void Hit(EnemyController enemy, bool isFacingRight)
    {
        float adjustedDamage = damage;
        foreach(Visage item in player.items)
        {
            adjustedDamage = item.ModifyWeaponDamage(adjustedDamage);
        }
        enemy.TakeDamage((int)adjustedDamage);

        Vector2 adjustedKnockback = knockbackForce;
        foreach(Visage item in player.items)
        {
            adjustedKnockback = item.ModifyKnockbackForce(adjustedKnockback);
        }
        adjustedKnockback *= (isFacingRight ? Vector2.one : new Vector2(-1, 1));

        enemy.TakeKnockback(adjustedKnockback, stunDuration);
    }
}