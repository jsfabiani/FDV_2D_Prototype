using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    private Rigidbody2D rb2D;
    public GameObject goal;
    public float platformSpeed = 1.0f;
    public Vector2 startLocation;
    public Vector2 stopLocation;

    // Start is called before the first frame update
    void Start()
    {
        // Assign start and stop locations for the looping movement.
        startLocation = this.transform.position;
        stopLocation = goal.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the position is close enough to the stop location.
        if(Math.Abs(transform.position.y-stopLocation.y) < 0.01 && Math.Abs(transform.position.x-stopLocation.x)< 0.01)
        {
            ReachLocation();
        }

        // Establish a direction pointing towards the stop location.
        Vector2 dir = new Vector2(stopLocation.x - this.transform.position.x, stopLocation.y - this.transform.position.y);
        this.transform.position +=  new Vector3 (dir.normalized.x, dir.normalized.y, 0) * platformSpeed * Time.deltaTime;
    }


    // Function to switch the stop and start locations upon reaching the stop location.
    private void ReachLocation()
    {
        Vector2 oldStopLocation = stopLocation;
        stopLocation = startLocation;
        startLocation = oldStopLocation;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Make the player's transform a child of the platform.
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.transform.SetParent(this.gameObject.transform, true);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // Detach the transform from the platform when leaving it.
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.transform.SetParent(null);
        }
    }

}
