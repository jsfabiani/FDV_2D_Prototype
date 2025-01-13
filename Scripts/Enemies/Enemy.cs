using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Health enemyHealth;
    Animator animator;
    AudioSource audioSource;
    [SerializeField] private AudioClip audioHurt, audioDefeated;
    public float enemyDamage;
    public float enemyPoints;
    public bool isInvulnerable;
    private bool destroyInvoked;

    public delegate void OnHitPlayer(float damage);
    public static event OnHitPlayer onHitPlayer;

    public delegate void DestroyEnemy(GameObject enemy);
    public static event DestroyEnemy destroyEnemy;

    // Start is called before the first frame update
    void Start()
    {
        enemyHealth = GetComponent<Health>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        HitDetector.onHitEnemy += DamageEnemy;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (enemyHealth)
        {
            // Destroy enemy upon reaching 0 health
            if (enemyHealth.GetHealth() == 0)
            {
                if(!destroyInvoked)
                {
                    destroyEnemy?.Invoke(gameObject);
                    destroyInvoked = true;
                    audioSource.PlayOneShot(audioDefeated);
                }
            }
        }

        if (animator)
        {
            // Deactivate hurt animation
            animator.SetBool("isHurt", false);
        }
    }

    public void DamageEnemy(GameObject enemy, float damage)
    {
        if (enemy == this.gameObject)
        {
            if(!isInvulnerable)
            {
                // Damage the enemy when not invulnerable.
                if(animator)
                {
                    animator.SetBool("isHurt", true);
                }
                if(enemyHealth)
                {
                    enemyHealth.ModifyHealth(-damage);
                }
                if(audioSource)
                {
                    
                audioSource.PlayOneShot(audioHurt);
                
                }
            }
        }
    }

    public void OnCollisionEnter2D(Collision2D other)
    {
        // Damage the player when colliding with it.
        if (other.gameObject.CompareTag("Player") && enemyDamage != 0)
        {
            onHitPlayer?.Invoke(enemyDamage);
        }
    }

}
