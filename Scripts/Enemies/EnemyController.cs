using System;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    public Rigidbody2D rb2D;
    public GameObject player;
    [SerializeField] private GameObject goal;
    [SerializeField] private float speed = 3.5f;
    [SerializeField] private float nearLimit = 0.05f;
    [SerializeField] private float knockBackAmount = 15f;
    public float defeatTime = 1.0f;
    public float knockBackRecoveryTime = 0.3f;
    public bool isKnockedBack;
    public Vector2 patrolDirection;
    private Vector2 startLocation;
    private Vector2 stopLocation;
    private Vector2 directionToPlayer;

    // Start is called before the first frame update
    protected void Start()
    {
        // Initializing Components
        rb2D = this.GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        animator = this.GetComponent<Animator>();


        rb2D.freezeRotation = true;

        // Assign start and stop locations for the patrol movement.
        if(goal)
        {
            startLocation = this.transform.position;
            stopLocation = goal.transform.position;
        }
    }

    protected void Update()
    {
        directionToPlayer = (player.transform.position - this.transform.position).normalized;
    }

    protected void PatrolGround()
    {
        //Patrol behavior for ground-based enemies.
        bool bNear = false;
        
        // Check if the NPC is close to the goal
        if (Math.Abs(transform.position.x-stopLocation.x)< nearLimit)
        {
            bNear = true;
        }

        if(bNear)
        {
            ReachGoal();
        }

        UpdatePatrolDirection();

        rb2D.velocity = new Vector2(Mathf.Sign(patrolDirection.x) * speed, rb2D.velocity.y);
    }

    protected void PatrolSky()
    {
        //Patrol behavior for flying enemies.
   
        bool bNear = false;

        // Check if the NPC is close to the goal
        if((Math.Abs(transform.position.x-stopLocation.x)< nearLimit) && (Math.Abs(transform.position.y-stopLocation.y) < nearLimit))
        {
            bNear = true;
        }

        if(bNear)
        {
            ReachGoal();
        }

        UpdatePatrolDirection();

        rb2D.velocity = patrolDirection * speed;
    }

    protected void StopMoving()
    {
        rb2D.velocity = Vector2.zero;
    }

    void ReachGoal()
    {
        // Swap start and stop locations
        Vector2 oldStopLocation = stopLocation;
        stopLocation = startLocation;
        startLocation = oldStopLocation;
    }

    void UpdatePatrolDirection()
    {
        patrolDirection = new Vector2(stopLocation.x - transform.position.x, stopLocation.y - transform.position.y).normalized;
    }

    public void KnockBack()
    {
        if (!isKnockedBack)
        {
            Vector2 knockbackDirection = new Vector2(-Mathf.Sign(directionToPlayer.x), 1.0f);
            rb2D.AddForce(knockbackDirection*knockBackAmount, ForceMode2D.Impulse);
            isKnockedBack = true;

        }
    }

    public void resetKnockBack()
    {
        isKnockedBack = false;
    }



}
