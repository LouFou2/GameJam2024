using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    private PlayerControl playerControl;

    [SerializeField] private Rigidbody2D mainControlRb;
    [SerializeField] private float moveSpeed = 5f;

    private Vector2 movementInput;


    private void Awake()
    {
        playerControl = new PlayerControl();
    }

    private void OnEnable()
    {
        playerControl.Enable();
    }
    private void OnDisable()
    {
        playerControl.Disable();
    }

    void Update()
    {
        movementInput = playerControl.Controls.Move.ReadValue<Vector2>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Apply movement to the Rigidbody2D
        mainControlRb.velocity = movementInput * moveSpeed;

        // Apply movement input to the Rigidbody
        //mainControlRb.velocity = movementInput * moveSpeed * Time.fixedDeltaTime;
    }
}
