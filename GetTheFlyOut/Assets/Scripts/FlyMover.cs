
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyMover : MonoBehaviour
{
    private Animator animator;
    private enum FlyState { Sitting, Flying, Evading, }
    private FlyState flyState;

    [SerializeField] private float flySpeed = 1;

    [SerializeField] private float flyTurnRange = 180;

    [SerializeField] private float sittingDuration = 3f;
    [SerializeField] private float flyingDuration = 3f;
    [SerializeField] private float evadingDuration = 3f;

    float sitTime = 0f;
    float flyTime = 0f;
    float evadeTime = 0f;

    private bool flyingInDirection = false;
    private float directionAngle = 0;
    private float zigZagTime = 0;


    void Start()
    {
        flyState = FlyState.Flying;
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        switch (flyState)
        {
            case (FlyState.Sitting):
                HandleSitting();
                if(sitTime == 0)
                    StartCoroutine(SittingCoroutine());
                break;
            case (FlyState.Flying):
                HandleFlying();
                //if (flyTime == 0)
                //    StartCoroutine(FlyingCoroutine());
                break;
            case (FlyState.Evading):
                HandleEvading();
                if (evadeTime == 0)
                    StartCoroutine(EvadingCoroutine());
                break;
            default:
                flyState = FlyState.Flying;
                break;
        }
    }
    void HandleSitting()
    {
        //When sitting, fly will wait for appropriate input to trigger Evading
        //If no input, it will simply switch to flying after a randomish duration

        animator.SetBool("IsFlying", false);
    }
    void HandleFlying()
    {
        //When flying, fly will fly around in random directions for a randomish time, then sit
        animator.SetBool("IsFlying", true);

        flyTime += Time.deltaTime;

        // Check screen bounds
        Vector3 screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        float xMax = screenBounds.x;
        float yMax = screenBounds.y;
        float xMin = -xMax;
        float yMin = -yMax;

        //pick a random direction and fly towards it
        if (!flyingInDirection) // only pick new direction if its not already flying in a direction
        {
            directionAngle = Random.Range(-180, 180);
            zigZagTime = flyTime + Random.Range(0.2f, 1.2f);
            flyingInDirection = true;
        }
        Vector2 direction = new Vector2(Mathf.Cos(directionAngle * Mathf.Deg2Rad), Mathf.Sin(directionAngle * Mathf.Deg2Rad)).normalized;

        if (flyTime < zigZagTime)
        {
            transform.position += new Vector3(direction.x, direction.y, 0) * flySpeed * Time.deltaTime;
        }
        else
        {
            flyingInDirection = false;
        }

        // Check if the fly is out of bounds and reverse direction if necessary
        if (transform.position.x >= xMax || transform.position.x <= xMin ||
            transform.position.y >= yMax || transform.position.y <= yMin)
        {
            // nudge the object a bit back into the center
            Vector3 directionToCenter = new Vector3(0, 0, 0) - transform.position;
            transform.position += directionToCenter.normalized * 0.01f;

            directionAngle += 90; // "Pong" the direction
            directionAngle = Mathf.Repeat(directionAngle, 360); // Ensure the angle is within 0 to 360 degrees
            flyingInDirection = true; // Ensure we keep flying in the new direction
        }

        if (flyTime > flyingDuration)
        {
            flyState = FlyState.Sitting;
            flyingInDirection = false;
            flyTime = 0;
        }
    }
    void HandleEvading()
    {
        //If evading, flying is more erattic. After short time evading, sit.
    }
    public void TriggerEvade()
    {
        StopAllCoroutines();  // Stop any currently running coroutines
        StartCoroutine(EvadingCoroutine());  // Start evading immediately
    }

    private IEnumerator SittingCoroutine()
    {
        flyState = FlyState.Sitting;

        transform.rotation = Quaternion.Euler(0, 0, 0);

        sitTime += Time.deltaTime;

        //maybe turn around
        float xFlipped = -transform.localScale.x;
        float y = transform.localScale.y;
        float z = transform.localScale.y;
        transform.localScale = new Vector3(xFlipped, y, z);

        yield return new WaitForSeconds(sittingDuration);
        flyState = FlyState.Flying;
        sitTime = 0;
    }

    private IEnumerator FlyingCoroutine()
    {
        flyState = FlyState.Flying;

        flyTime += Time.deltaTime;

        // turn in the air as it is moving
        float sinValue = Mathf.Sin(flyTime);
        transform.Rotate(0, 0, sinValue);
        
        yield return null;
        if (flyTime > flyingDuration) 
        {
            flyState = FlyState.Sitting;
            flyTime = 0;
        }
    }

    private IEnumerator EvadingCoroutine()
    {
        flyState = FlyState.Evading;

        sitTime += Time.deltaTime;


        yield return new WaitForSeconds(evadingDuration);
        flyState = FlyState.Evading;
    }
}
