using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    public GameObject message;
    public void Start(){
        message.SetActive(false);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player") && !other.isTrigger)
        {
            
            message.SetActive(true);
        }
            
    }
    public void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player") && !other.isTrigger)
        {
            
            message.SetActive(false);
        }
    }
}
