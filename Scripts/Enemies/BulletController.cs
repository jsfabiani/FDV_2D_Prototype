using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject player;
    public Transform boss;
    [SerializeField] private float bulletSpeed = 5;
    private Vector3 delta;




    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        delta = player.transform.position - this.transform.position;  

    }  



    // Update is called once per frame
    void Update()
    {
        if (delta.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (delta.x > 0)
        {
            spriteRenderer.flipX = false;
        }      
        
        transform.position += delta.normalized * bulletSpeed * Time.deltaTime;

        if ((boss.position - transform.position).magnitude >= 50)
        {
            gameObject.SetActive(false);
        }

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        gameObject.SetActive(false);     
    }

}
