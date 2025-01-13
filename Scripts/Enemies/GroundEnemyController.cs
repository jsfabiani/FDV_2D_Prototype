using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public enum GroundEnemyState
{
    statePatrol,
    stateKnockback,
    stateDefeated
}


public class GroundEnemyController : EnemyController
{
    public GroundEnemyState state;
    private bool isDefeated;
    void Start()
    {
        base.Start();
        state = GroundEnemyState.statePatrol;

        
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
            case GroundEnemyState.statePatrol :
                PatrolGround();
                break;

            case GroundEnemyState.stateKnockback :
                if (!isKnockedBack)
                {
                    StopMoving();
                    KnockBack();
                    Invoke("KnockBackRecovery", knockBackRecoveryTime);
                }
                break;
            case GroundEnemyState.stateDefeated :
                CancelInvoke("KnockBackRecovery");
                StopMoving();
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
                state = GroundEnemyState.stateKnockback;
            }
        }
    }

    void KnockBackRecovery()
    {
        if(!isDefeated)
        {
            resetKnockBack();
            state = GroundEnemyState.statePatrol;
        }
    }

    void EnemyDefeated(GameObject enemy)
    {
        if (enemy == this.gameObject)
        {
            animator.SetBool("isDefeated", true);
            GetComponent<Collider2D>().enabled = false;
            rb2D.isKinematic = true;
            state = GroundEnemyState.stateDefeated;
            isDefeated = true;
        }
    }
}
