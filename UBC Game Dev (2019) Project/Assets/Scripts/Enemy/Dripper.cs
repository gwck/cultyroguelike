using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dripper : EnemyController
{
    [Header("Dripper")]
    [SerializeField] private float fireRate;
    [SerializeField] private Transform firePoint;
    [SerializeField] private int dripCount;
    [SerializeField] private GameObject projectile;
    [SerializeField] private float spread;
    private float fireCooldown;

    new void Start()
    {
        base.Start();
        fireCooldown = fireRate;
    }

    private new void FixedUpdate()
    {
        base.FixedUpdate();
        DripBehaviour();
    }

    private void Drip()
    {
        Quaternion projectileAngle = Quaternion.Euler(firePoint.rotation.eulerAngles.x, firePoint.rotation.eulerAngles.y, firePoint.rotation.eulerAngles.z + Random.Range(-spread, spread));
        Instantiate(projectile, firePoint.position, projectileAngle);
    }

    private void DripBehaviour()
    {
        if (fireCooldown > 0) fireCooldown -= Time.deltaTime;

        if (fireCooldown > 0 || !CanSeePlayer()) return;
        
        fireCooldown = fireRate;

        for (int i = 0; i < dripCount; i++)
        {
            Invoke("Drip", Random.Range(0, 0.3f));
        }
    }
}
