using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : EnemyController
{
    [Header("Shooter")]
    [SerializeField] private float fireRate;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject projectile;
    private float fireCooldown;

    new void Start()
    {
        base.Start();
        fireCooldown = fireRate;
    }

    private new void FixedUpdate()
    {
        base.FixedUpdate();
        FacePlayer();
        ShootBehaviour();
    }

    private void ShootBehaviour()
    {
        if (fireCooldown > 0) fireCooldown -= Time.deltaTime;

        if (fireCooldown > 0 || !CanSeePlayer()) return;

        SoundManager.Instance.Play(attack, transform);
        fireCooldown = fireRate;
        Instantiate(projectile, firePoint.position, firePoint.rotation);
    }
}