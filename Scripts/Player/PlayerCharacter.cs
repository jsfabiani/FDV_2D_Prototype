using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCharacter : MonoBehaviour
{
    Health playerHealth;
    AudioSource audioSource;
    
    [SerializeField] Slider healthBar;
    [SerializeField] TextMeshProUGUI scoreText;


    [SerializeField] private float playerScore;
    [SerializeField] private float doubleJumpUnlockCost = 50;
    [SerializeField] private float fightUnlockCost = 100;

    [SerializeField] private float iFrames = 0.2f;
    [SerializeField] private bool isInvulnerable;
    private bool doubleJumpBought;
    private bool fightBought;
    
    private bool destroyInvoked;

    [SerializeField] private AudioClip audioHurt, audioHeal, audioScoreIncrease, audioBuy, audioDefeated;

    // Event for the player being defeated
    public delegate void DestroyPlayer();
    public static event DestroyPlayer destroyPlayer;

    public delegate void UnlockAbility();
    public static event UnlockAbility unlockDoubleJump, unlockFight;

    // Start is called before the first frame update
    void Start()
    {
        playerHealth = GetComponent<Health>();
        audioSource = GetComponent<AudioSource>();
        Enemy.onHitPlayer += HitByEnemy;
        Enemy.destroyEnemy += EnemyScore;
    }

    // Update is called once per frame
    void Update()
    {
        if(playerHealth){
            healthBar.value = playerHealth.GetHealth()/100;
            if (playerHealth.GetHealth() == 0)
            {
                if(!destroyInvoked)
                {
                    destroyPlayer?.Invoke();
                    destroyInvoked = true;
                    audioSource.PlayOneShot(audioDefeated);
                }
            }
        }

        scoreText.text = playerScore + " $"; 
    }

    private void OnCollisionEnter2D(Collision2D other)
    {

        // Increase health or score if the player picks up an item.
        if (other.gameObject.CompareTag("PickUp"))
        {
            PickUpItem item = other.gameObject.GetComponent<PickUpItem>();
            float healing = item.healingAmount;
            if (healing != 0)
            {
                if (playerHealth)
                {
                    playerHealth.ModifyHealth(healing);
                }
                if (audioSource)
                {
                    if (playerHealth.GetHealth() != 0)
                    {
                        audioSource.PlayOneShot(audioHeal);
                    }
                }
            }
            
            float scoreIncrease = item.scoreAmount;
            if(scoreIncrease != 0 )
            {
                playerScore += scoreIncrease;
                if (audioSource)
                    audioSource.PlayOneShot(audioScoreIncrease);
            }

            other.gameObject.SetActive(false);
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("DoubleJumpUnlock") && Input.GetKeyDown(KeyCode.F))
        {
            // UI 
            if (playerScore >= doubleJumpUnlockCost && Input.GetKey(KeyCode.F))
            {
                if (!doubleJumpBought)
                {
                    playerScore -= doubleJumpUnlockCost;
                    unlockDoubleJump?.Invoke();
                    doubleJumpBought = true;
                    Destroy(other.gameObject);
                    audioSource.PlayOneShot(audioBuy);
                }
            }
        }

        if(other.gameObject.CompareTag("FightUnlock"))
        {
            // UI 
            if (playerScore >= fightUnlockCost && Input.GetKey(KeyCode.F))
            {
                if (!fightBought)
                {
                    playerScore -= fightUnlockCost;
                    unlockFight?.Invoke();
                    fightBought = true;
                    Destroy(other.gameObject);
                    audioSource.PlayOneShot(audioBuy);
                }
            }
        }
    }


    void HitByEnemy(float damage)
    {
        if(!isInvulnerable)
        {
            if (playerHealth)
                playerHealth.ModifyHealth(-damage);
            if (audioSource)
                audioSource.PlayOneShot(audioHurt);
            isInvulnerable = true;
            Invoke("RemoveInvulnerability", iFrames);
        }
    }

    void FinishGame()
    {
        Application.Quit();
        UnityEditor.EditorApplication.isPlaying = false;
    }

    void EnemyScore(GameObject enemy)
    {
        playerScore += enemy.GetComponent<Enemy>().enemyPoints;
    }

    void RemoveInvulnerability()
    {
        isInvulnerable = false;
    }
}
