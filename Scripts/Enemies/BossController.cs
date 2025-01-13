using System;
using System.Collections;
using UnityEngine;

public enum BossState
{
    statePatrol,
    stateReloading,
    stateBarrage,
    stateDefeated
}

public class BossController : EnemyController
{
    private GameObject missile;
    private Enemy bossEnemy;
    public BossState state;
    AudioSource audioSource;
    [SerializeField] AudioClip audioFire;

    private float horizDistanceToPlayer;
    private bool isFiring;
    private bool isDefeated;
    private bool coroutineStarted;
    [SerializeField] private float walkingFireRate = 2.0f;
    [SerializeField] private float barrageRate = 0.2f;
    [SerializeField] private float barrageDelay = 2.0f;
    [SerializeField] private int barrageAmount = 8;
    [SerializeField] private float reloadTime = 5.0f;
    [SerializeField] private float patrolTime = 20.0f;
    private float damage;
    private int missilesFired;
    ObjectPooling missilePool;

    public delegate void BossDefeated();
    public static event BossDefeated bossDefeated;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        state = BossState.statePatrol; 
        bossEnemy = GetComponent<Enemy>();
        Enemy.destroyEnemy += EnemyDefeated;
        missilePool = GetComponent<ObjectPooling>();    
        audioSource = GetComponent<AudioSource>();  

        damage = bossEnemy.enemyDamage;
    }

    void Update()
    {
        base.Update();
        horizDistanceToPlayer = player.transform.position.x - this.transform.position.x;
        
        // Animation
        if (!isDefeated)
        {
            if (horizDistanceToPlayer < 0)
            {
                spriteRenderer.flipX = true;
            }
            else if (horizDistanceToPlayer > 0)
            {
                spriteRenderer.flipX = false;
            }
        }

        switch(state)
        {
            case BossState.statePatrol :
                animator.SetBool("isWalking", true);
                animator.SetBool("isFiring", false);
                bossEnemy.isInvulnerable = true;
                bossEnemy.enemyDamage = damage;
                PatrolGround();
                if(!isFiring)
                {
                    isFiring = true;
                    InvokeRepeating("Fire", walkingFireRate, walkingFireRate);
                }
                if(!coroutineStarted)
                {
                    StartCoroutine("PatrolTimer");
                    coroutineStarted = true;
                }
                break;

            case BossState.stateBarrage :
                StopMoving();
                animator.SetBool("isWalking", false);
                if (missilesFired < barrageAmount)
                {
                    if(!isFiring)
                    {
                        isFiring = true;
                        InvokeRepeating("Fire", barrageDelay, barrageRate);
                    }
                }
                else
                    state = BossState.stateReloading;
                break;

            case BossState.stateReloading :
                ResetFiring();
                animator.SetBool("isFiring", false);
                bossEnemy.isInvulnerable = false;
                bossEnemy.enemyDamage = 0;
                if(!coroutineStarted)
                {
                    StartCoroutine("Reload");
                    coroutineStarted = true;
                }
                break;

            case BossState.stateDefeated :
                StopMoving();
                CancelInvoke("Fire");
                StopAllCoroutines();
                break;
        }
    }


    void Fire()
    {
        audioSource.PlayOneShot(audioFire);
        missile = missilePool.GetPooledObject();
        if (missile != null)
        {
            animator.SetBool("isFiring", true);
            missile.transform.position = new Vector3 (transform.position.x, transform.position.y + 0.7f, transform.position.z);
            missile.GetComponent<BulletController>().boss = this.transform;
            missile.SetActive(true);
        }
        missilesFired ++;
    }
    
    void ResetFiring()
    {
        CancelInvoke("Fire");
        isFiring = false;
        missilesFired = 0;
    }

    void ResetCoroutines()
    {
        StopAllCoroutines();
        coroutineStarted = false;
    }

    IEnumerator Reload()
    {
        yield return new WaitForSeconds(reloadTime);
        state = BossState.statePatrol;
        ResetCoroutines();
    }

    IEnumerator PatrolTimer()
    {
        yield return new WaitForSeconds(patrolTime);
        ResetFiring();
        state = BossState.stateBarrage;
        ResetCoroutines();
    }

    void EnemyDefeated(GameObject enemy)
    {
        if (enemy == this.gameObject)
        {
            animator.SetBool("isDefeated", true);
            GetComponent<Collider2D>().enabled = false;
            rb2D.isKinematic = true;
            isDefeated = true;
            state = BossState.stateDefeated;
            bossDefeated?.Invoke();
        }
    }


    void OnDisable()
    {
        CancelInvoke("Fire");
    }
}
