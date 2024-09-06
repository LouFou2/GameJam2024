using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    private PlayerControl playerControl;

    [SerializeField] private Rigidbody2D mainControlRb;
    [SerializeField] private float moveSpeed = 5f;

    private Vector2 movementInput;
    private Vector2 direction;

    private bool isFirstFrame = false;


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

    void Start()
    {
        // Initialize lastMousePosition with the current mouse position
        movementInput = playerControl.Controls.Move.ReadValue<Vector2>();
    }

    void Update()
    {
        //float moveInputX = Input.GetAxis("Horizontal");
        //float moveInputY = Input.GetAxis("Vertical");

        movementInput = playerControl.Controls.Move.ReadValue<Vector2>();

        // Convert mouse position to world position
        //Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //mouseWorldPosition.z = 0; // Ensure it's on the same plane as the object

        // Calculate direction from the object to the mouse position
        //direction = (mouseWorldPosition - transform.position).normalized;


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
