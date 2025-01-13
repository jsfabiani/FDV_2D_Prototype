using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaDetector : MonoBehaviour
{

    public delegate void OnAreaChange(GameObject AreaLimits);

    public static event OnAreaChange onEnterArea, onExitArea;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("AreaLimits"))
        {
            onEnterArea?.Invoke(other.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("AreaLimits"))
        {
            onExitArea?.Invoke(other.gameObject);
        }
    }
}
