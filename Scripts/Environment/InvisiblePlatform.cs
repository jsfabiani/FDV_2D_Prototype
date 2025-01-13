using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisiblePlatform : MonoBehaviour
{
    Material material;
    // Start is called before the first frame update
    void Start()
    {
        material = GetComponent<SpriteRenderer>().material;
        material.color = new Color(material.color.r, material.color.g, material.color.b, 0.0f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") | collision.gameObject.layer==LayerMask.NameToLayer("NPCs"))
        {  
            material.color = new Color(material.color.r, material.color.g, material.color.b, 1.0f);
        }
    } 

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") | collision.gameObject.layer==LayerMask.NameToLayer("NPCs"))
        {  
            material.color = new Color(material.color.r, material.color.g, material.color.b, 0.0f);
        }
    }   
}
