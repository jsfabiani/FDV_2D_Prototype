using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpDetector : MonoBehaviour
{
    public delegate void OnTouchObject();
    public static event OnTouchObject onTouchFloor, onTouchEnemy, onLeaveFloor;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Floor"))
        {
            onTouchFloor?.Invoke();
        }
        if (other.gameObject.CompareTag("Enemy"))
        {
            onTouchEnemy?.Invoke();
        }
    }

    private void OnTriggerExit2D (Collider2D trigger)
    {
        if (trigger.gameObject.CompareTag("Floor") | trigger.gameObject.CompareTag("Enemy"))
        {          
            onLeaveFloor();
        }
    }
}
