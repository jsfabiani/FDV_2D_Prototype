using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : EnemyController
{
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        Enemy.destroyEnemy += EnemyDefeated;      
    }


    void EnemyDefeated(GameObject enemy)
    {
        if (enemy == this.gameObject)
        {
            animator.SetBool("isDefeated", true);
            GetComponent<Collider2D>().enabled = false;
        }
    }
}
