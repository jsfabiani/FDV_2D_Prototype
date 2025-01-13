using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public enum PlayerState
{
    stateIdle,
    stateRunning,
    stateJump,
    stateDoubleJump,
    stateFight,
    stateDefeated
}

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb2D;

    // Animation
    SpriteRenderer spriteRenderer;
    Animator animator;

    // Audio
    AudioSource audioSource;
    [SerializeField] private AudioClip audioJump, audioLand, audioFight;
    private bool audioPlaying = false;

    // State
    public PlayerState state;

    // Hit detector for attacking
    GameObject hitDetector;

    // UI
    [SerializeField] GameObject doubleJumpIcon, fightIcon;

    //Variables
    [SerializeField] private float attackDelay = 0.5f;
    [SerializeField] private float attackDamage = 20f;
    [SerializeField] private float hitWindow = 0.1f;
    [SerializeField] private float attackRange = 0.3f;
    private bool attackReady = true;
    [SerializeField] private float speed = 25.0f;
    [SerializeField] private float jumpHeight = 2.5f;
    private float jumpImpulse;
    private float horizontalInput;
    private bool jumpInput = false;
    private bool fightInput = false;
    private bool isJumping = false;
    private bool doubleJumpSpent = false;
    [SerializeField] private bool doubleJumpEnabled = false;
    [SerializeField] private bool fightEnabled = false;
    private bool isHurt;


    // Start is called before the first frame update
    void Start()
    {
        // Initializing Components
        rb2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        hitDetector = gameObject.transform.GetChild(0).gameObject;

        // Initialize hit detector
        hitDetector.transform.position = transform.position + new Vector3(attackRange, 0f, 0f);
        hitDetector.GetComponent<HitDetector>().attackDamage = attackDamage;
        hitDetector.SetActive(false);

        // Lock the rigidbody2D rotation to avoid bugs when touching the edges of platforms.
        rb2D.freezeRotation = true;

        // UI
        doubleJumpIcon.SetActive(false);
        fightIcon.SetActive(false);

        // Event bindings for jumping on objects
        JumpDetector.onTouchFloor += PlayAudioLand;
        JumpDetector.onTouchFloor += RechargeJumps;
        JumpDetector.onTouchEnemy += RechargeJumps;
        JumpDetector.onLeaveFloor += LoseJump;

        // Event bindings
        Enemy.onHitPlayer += HurtAnimation;
        PlayerCharacter.destroyPlayer += PlayerDefeated;
        PlayerCharacter.unlockDoubleJump += EnableDoubleJump;
        PlayerCharacter.unlockFight += EnableFight;
    }

    // Update is called once per frame
    void Update()
    {
        // Setting Jump Impulse for the player.
        jumpImpulse = rb2D.mass * Mathf.Sqrt(2.0f * Mathf.Abs(Physics2D.gravity.y) * rb2D.gravityScale * jumpHeight);

        // Inputs
        if(state != PlayerState.stateDefeated)
        {
            horizontalInput = Input.GetAxis("Horizontal");
            jumpInput = Input.GetButtonDown("Jump");
            fightInput = Input.GetButtonDown("Fire1");
        }

        
        // Flip the sprite based on the direction of movement
        if (horizontalInput < 0)
        {
            if (spriteRenderer)
                spriteRenderer.flipX = true;

            // Change the position of the hit detector for attacking.
            hitDetector.transform.position = transform.position - new Vector3(attackRange, 0f, 0f);
        }
        else if (horizontalInput > 0)
        {
            if (spriteRenderer)
                spriteRenderer.flipX = false;

            // Change the position of the hit detector for attacking.
            hitDetector.transform.position = transform.position + new Vector3(attackRange, 0f, 0f);
        }


        // State machine for the player
        switch(state)
        {
            case PlayerState.stateIdle :
                if (animator)
                {
                    animator.SetBool("isWalking", false);
                    animator.SetBool("firstJump", false);
                    animator.SetBool("secondJump", false);
                }
                if (horizontalInput != 0)
                    state = PlayerState.stateRunning;
                if (jumpInput)
                {
                    if (!isJumping)
                    {
                        isJumping = true;
                        Jump();
                        state = PlayerState.stateJump;
                    }
                    else if (doubleJumpEnabled && !doubleJumpSpent)
                    {
                        doubleJumpSpent = true;
                        Jump();
                        state = PlayerState.stateDoubleJump;
                    }
                }
                if (fightInput && fightEnabled && attackReady)
                {
                    if(animator)
                    {
                        animator.SetBool("fightIdle", true);
                    }
                    state = PlayerState.stateFight;
                }
                break;

            case PlayerState.stateRunning :
                if (animator)
                {
                    animator.SetBool("isWalking", true);
                    animator.SetBool("firstJump", false);
                    animator.SetBool("secondJump", false);
                }
                if (jumpInput)
                {
                    if (!isJumping)
                    {
                        isJumping = true;
                        Jump();
                        state = PlayerState.stateJump;
                    }
                    else if (doubleJumpEnabled && !doubleJumpSpent)
                    {
                        doubleJumpSpent = true;
                        Jump();
                        state = PlayerState.stateDoubleJump;
                    }
                }
                if (fightInput && fightEnabled && attackReady)
                {
                    if(animator)
                    {
                        animator.SetBool("fightRunning", true);
                    }
                    state = PlayerState.stateFight;
                }
                if (horizontalInput == 0)
                    state = PlayerState.stateIdle;
                break;

            case PlayerState.stateJump :
                if (animator)
                {
                    animator.SetBool("firstJump", true);
                    animator.SetBool("secondJump", false);
                }
                if (!isJumping)
                    state = PlayerState.stateIdle;
                if (jumpInput)
                {
                    if (doubleJumpEnabled && !doubleJumpSpent)
                    {
                        doubleJumpSpent = true;
                        Jump();
                        state = PlayerState.stateDoubleJump;
                    }
                }
                break;

            case PlayerState.stateDoubleJump:
                if (animator)
                {
                    animator.SetBool("firstJump", false);
                    animator.SetBool("secondJump", true);
                }
                if (!isJumping)
                    state = PlayerState.stateIdle;
                break;    

            case PlayerState.stateFight :
                if(animator)
                {
                    animator.SetBool("fightIdle", false);
                    animator.SetBool("fightRunning", false);
                }
                Attack();
                state = PlayerState.stateIdle;
                break;

            case PlayerState.stateDefeated :
                horizontalInput = 0f;
                jumpInput = false;
                fightInput = false;
                break;
        }

        // Horizontal Movement
        rb2D.velocity = new Vector2(horizontalInput * speed, rb2D.velocity.y);

    }

    private void LateUpdate()
    {
        // Deactivate hurt animation
        if(isHurt)
        {
            isHurt = false;
            animator.SetBool("isHurt", false);
        }

        // Activate PowerUp UI elements
        if(state != PlayerState.stateDefeated)
        {
            if (doubleJumpEnabled)
            {
                doubleJumpIcon.SetActive(true);
            }
            else
            {
                doubleJumpIcon.SetActive(false);
            }

            if (fightEnabled)
            {
                fightIcon.SetActive(true);
            }
            else
            {
                fightIcon.SetActive(false);
            }
        }
    }

    void HurtAnimation(float damage)
    {
        isHurt = true;
        if (animator)
            animator.SetBool("isHurt", true);
    }

    private void Jump()
    {
        // Reset the vertical velocity before jumping.
        rb2D.velocity = new Vector2(rb2D.velocity.x, 0);
        rb2D.AddForce(Vector2.up * jumpImpulse, ForceMode2D.Impulse);
        if (audioSource)
            audioSource.PlayOneShot(audioJump);
        isJumping = true;
    }

    private void RechargeJumps()
    {
        // Recharge Jumping and Double Jump.
        isJumping = false;
        doubleJumpSpent = false;

        rb2D.velocity = new Vector2(rb2D.velocity.x, 0);
    }

    private void LoseJump()
    {
        isJumping = true;
    }

    private void PlayAudioLand()
    {
        // Play landing audio if the player comes from a jump.    
        if (isJumping == true)
        {
            audioSource.PlayOneShot(audioLand);
        }
    }


    private void Attack()
    {
        // Attacking works by enabling the hit detector child object
        audioSource.PlayOneShot(audioFight);
        hitDetector.SetActive(true);
        attackReady = false;
        StartCoroutine("HitWindow");
        StartCoroutine("AttackRecharge");
    }

    IEnumerator AttackRecharge()
    {
        // Time for the attack to recharge
        yield return new WaitForSeconds(attackDelay);
        attackReady = true;
    }

    IEnumerator HitWindow()
    {
        // Time that the hit detector is active
        yield return new WaitForSeconds(hitWindow);
        hitDetector.SetActive(false);
    }

    private void PlayerDefeated()
    {
        animator.SetBool("isDefeated", true);
        // Deactivate all children when defeated
        foreach(Transform child in transform)
        {
            if (child != this.transform)
            {
                child.gameObject.SetActive(false);
            }
        }
        state = PlayerState.stateDefeated;
        
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            // Disable collider and rigidbody upon touching floor when defeated
            if(state == PlayerState.stateDefeated)
            {
                GetComponent<Collider2D>().enabled = false;
                rb2D.isKinematic = true;
                rb2D.velocity = Vector2.zero;
            }
        }
    }

    void EnableDoubleJump()
    {
        doubleJumpEnabled = true;
    }

    void EnableFight()
    {
        fightEnabled = true;
    }
}