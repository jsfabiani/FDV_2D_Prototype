using System;
using UnityEngine;

public enum SkyEnemyState
{
    statePatrol,
    stateKnockback,
    stateDefeated
}

public class SkyEnemyController : EnemyController
{
    public SkyEnemyState state;
    bool isDefeated;
    void Start()
    {
        base.Start();
        state = SkyEnemyState.statePatrol;
        
        HitDetector.onHitEnemy += EnemyHitKnockback;  
        Enemy.destroyEnemy += EnemyDefeated;      
    }

    void Update()
    {
        base.Update();
        
        //Animation
        if (patrolDirection.x < 0)
        {
            animator.SetBool("isWalking", true);
            spriteRenderer.flipX = true;
        }
        else if (patrolDirection.x > 0)
        {
            animator.SetBool("isWalking", true);
            spriteRenderer.flipX = false;
        }
        else if (patrolDirection.x == 0)
        {
            animator.SetBool("isWalking", false);
        }

        switch(state)
        {
            case SkyEnemyState.statePatrol :
                PatrolSky();
                break;

            case SkyEnemyState.stateKnockback :
                if (!isKnockedBack)
                {
                    StopMoving();
                    KnockBack();
                    Invoke("KnockBackRecovery", knockBackRecoveryTime);
                }
                break;
            case SkyEnemyState.stateDefeated :
                CancelInvoke("KnockBackRecovery");
                if(isDefeated)
                {
                    StopMoving();
                }
                break;
        }

    }

    


    void EnemyHitKnockback(GameObject enemy, float damage)
    {
        if (enemy == this.gameObject)
        {
            if(!isDefeated)
            {
                resetKnockBack();
                state = SkyEnemyState.stateKnockback;
            }
        }
    }

    void KnockBackRecovery()
    {
        if(!isDefeated)
        {
            resetKnockBack();
            state = SkyEnemyState.statePatrol;
        }
    }

    void EnemyDefeated(GameObject enemy)
    {
        if (enemy == this.gameObject)
        {
            animator.SetBool("isDefeated", true);
            state = SkyEnemyState.stateDefeated;
            StopMoving();
        }
    }

    void DisableCollider()
    {
        isDefeated = true;
        rb2D.isKinematic = true;
        GetComponent<Collider2D>().enabled = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Disable the collider and rigidbody on collision with the floor, to avoid the enemy staying defeated in the air.
        if (state == SkyEnemyState.stateDefeated && collision.gameObject.CompareTag("Floor"))
            DisableCollider();
    }
}
