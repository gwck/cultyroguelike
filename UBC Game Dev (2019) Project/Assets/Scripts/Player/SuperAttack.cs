using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Experimental.Rendering.LWRP;
using Cinemachine;

public class SuperAttack : MonoBehaviour
{
    private bool isPreparingSuperAttack;
    private bool isUsingSuperAttack;

    // private float meterFill;  //How much the meter has already filled
    private float globalLightOriginalIntensity; //original intensity of global light when game starts
    private float playerLightOriginalIntensity; //original intensity of player light when game starts
    private float chargeTimePassed; //amount of time passed during super attack charge
    [SerializeField] private float minGlobalLightIntensity;
    [SerializeField] private float minPlayerLightIntensity;

    [SerializeField] private CinemachineImpulseSource impulseSource;

    [SerializeField] private float lightChangeAmount; //Amount of how much light intensity changes each frame
    [SerializeField] private float timeToChargeSuperAttack; // how much time it takes to charge the super attack
    [SerializeField] private float superAttackRange; //range of the super attack
    [SerializeField] private float superAttackCooldown; //temporary variable, cooldown for super  attack
    [SerializeField] private int superAttackDamage; //damage of the super attack


    [SerializeField] private Transform superAttackCheck; // starting point from which the attack's range is measured

    [SerializeField] private LayerMask whatIsEnemies; // layers that count as enemies

    public Image superAttackbarImage;

    public Light2D globalLight;
    public Light2D playerLight;

    public PlayerController playerController;
    public GameObject attackEffectPrefab;
    public Transform attackEffectLocation;

    // Start is called before the first frame update
    void Start()
    {
        superAttackbarImage.fillAmount = 1;
        globalLightOriginalIntensity = globalLight.intensity;
        playerLightOriginalIntensity = playerLight.intensity;
    }

    // Update is called once per frame
    void Update()
    {
        if (superAttackbarImage.fillAmount >= 1)
        {
            if (chargeTimePassed >= timeToChargeSuperAttack)
            {
                StartCoroutine(UseSuperAttack());
            }
            if (!isUsingSuperAttack)
            {
                FadeSuperAttack();
                PreparingSuperAttack();
            }
        }
    }

    //Light fades when player is using super attack, and increases if player has stopped super attack after some charging
    public void FadeSuperAttack()
    {
        if (isPreparingSuperAttack && (globalLight.intensity > minGlobalLightIntensity || playerLight.intensity < minPlayerLightIntensity))
        {

            playerLight.intensity = playerLight.intensity - lightChangeAmount;
            globalLight.intensity = globalLight.intensity - lightChangeAmount;
        }
        else if (globalLightOriginalIntensity > globalLight.intensity || playerLightOriginalIntensity > playerLight.intensity)
        {
            playerLight.intensity = playerLight.intensity + lightChangeAmount;
            globalLight.intensity = globalLight.intensity + lightChangeAmount;
        }
    }


    public void PreparingSuperAttack()
    {
        //Only charge super attack if player holds down b while not moving and not attacking, otherwise do nothing!
        if (Input.GetButton("SuperAttack") && !playerController.isRunning && !playerController.isAttacking && chargeTimePassed < timeToChargeSuperAttack)
        {
            isPreparingSuperAttack = true;
            chargeTimePassed++;
        }
        else
        {
            isPreparingSuperAttack = false;
            chargeTimePassed = 0;
        }
    }


    //Using super attack causes lights to immediately go back to normal with a slash as well
    public IEnumerator UseSuperAttack()
    {
        chargeTimePassed = 0;
        isUsingSuperAttack = true;
        GameObject attackEffect = Instantiate(attackEffectPrefab, attackEffectLocation.position, transform.rotation, transform);
        attackEffect.transform.localScale = new Vector3(superAttackRange, attackEffect.transform.localScale.x, attackEffect.transform.localScale.z);

        yield return new WaitForSeconds(0.1f);

        playerLight.intensity = playerLightOriginalIntensity;
        globalLight.intensity = globalLightOriginalIntensity;

        superAttackbarImage.fillAmount = superAttackbarImage.fillAmount - 1;

        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(superAttackCheck.position, superAttackRange, whatIsEnemies); //enemies hit by attack
        //hit all enemies in range!
        for (int i = 0; i < enemiesToDamage.Length; i++)
        {
            if (enemiesToDamage[i].transform.parent)
            {
                if (enemiesToDamage[i].transform.parent.name == "Boss")
                {
                    Debug.Log(enemiesToDamage[i].transform.parent.name);
                    EnemyStats controller = enemiesToDamage[i].transform.parent.GetComponent<EnemyStats>();
                    controller.TakeDamage(superAttackDamage);
                }
                else
                {
                    Debug.Log(enemiesToDamage[i].transform.parent.name);
                    EnemyController controller = enemiesToDamage[i].transform.parent.GetComponent<EnemyController>();
                    controller.TakeDamage(superAttackDamage);
                }
            }
        }
        isUsingSuperAttack = false;

    }

    public void FillBar()
    {
        if (!isUsingSuperAttack)
        {
            superAttackbarImage.fillAmount = superAttackbarImage.fillAmount + 0.5f;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(superAttackCheck.position, superAttackRange);

    }
}
