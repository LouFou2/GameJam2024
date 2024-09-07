using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    private PlayerControl playerControl;

    [SerializeField] private Rigidbody2D mainControlRb;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float xLimit = 5f;
    [SerializeField] private float yCeil = 5f;
    [SerializeField] private float yFloor = 5f;

    private Vector2 mousePosition;
    private Vector2 initialPosition;

    private Vector2 moveDir;

    private float xMaxScreen;
    private float yMaxScreen;

    private void Awake(){

        playerControl = new PlayerControl();
    }

    private void OnEnable(){

        playerControl.Enable();
    }
    private void OnDisable(){

        playerControl.Disable();
    }
    private void Start(){

        initialPosition = mainControlRb.position;

        xMaxScreen = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x;
        yMaxScreen = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y;
    }

    void Update(){

        mousePosition = playerControl.Controls.MousePosition.ReadValue<Vector2>();
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        moveDir = mousePosition.normalized;
    }

    void FixedUpdate(){
        
        float xInvLerped = Mathf.InverseLerp(-xMaxScreen, xMaxScreen, mousePosition.x);
        float yInvLerped = Mathf.InverseLerp(-yMaxScreen, yMaxScreen, mousePosition.y);

        float xLerped = Mathf.Lerp(-xLimit, xLimit, xInvLerped);
        float yLerped = Mathf.Lerp(yFloor, yCeil, yInvLerped);

        Vector2 lerpPosition = new Vector2(xLerped, yLerped);

        mainControlRb.MovePosition(lerpPosition);


        //mainControlRb.MovePosition(new Vector2(clampedX, clampedY));
    }
}
