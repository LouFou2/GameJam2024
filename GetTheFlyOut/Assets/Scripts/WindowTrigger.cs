using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowTrigger : MonoBehaviour
{
    [SerializeField] private Collider2D handCollider;
    private bool windowTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collider that entered is the handCollider
        if (other == handCollider)
        {
            windowTriggered = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other == handCollider)
        {
            windowTriggered = false;
        }
    }

    public bool GetWindowTriggered() 
    {
        return windowTriggered;
    }
}
