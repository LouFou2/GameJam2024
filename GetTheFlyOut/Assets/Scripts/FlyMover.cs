
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyMover : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private Collider2D handCollider;
    private enum FlyState { Sitting, Flying, Evading, }
    private FlyState flyState;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private WindowOpener windowOpener;

    [SerializeField] private float flySpeed = 1;

    [SerializeField] private float sittingDuration = 3f;
    [SerializeField] private float flyingDuration = 3f;
    [SerializeField] private float evadingDuration = 3f;
    [SerializeField] [Range(0, 1)] private float buzzVolume = 0.7f;


    float sitTime = 0f;
    float flyTime = 0f;

    private bool flyingInDirection = false;
    private float directionAngle = 0;
    private float zigZagTime = 0;

    private bool didHandCollide = false;

    private bool isEvading = false;

    


    void Start()
    {
        flyState = FlyState.Flying;
        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider == handCollider) 
        {
            didHandCollide = true;
        }
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
                break;
            default:
                flyState = FlyState.Flying;
                break;
        }
    }
    void HandleSitting()
    {
        bool isWindowOpen = windowOpener.IsWindowOpen();
        if (isWindowOpen || didHandCollide) 
        {
            flyState = FlyState.Flying;
        }
        animator.SetBool("IsFlying", false);
        audioSource.volume = 0f;

        if (didHandCollide)
            HandleEvading();

        didHandCollide = false;
    }
    void HandleEvading() 
    {
        if (!isEvading)
            StartCoroutine(EvadingCoroutine());
    }
    void HandleFlying()
    {
        audioSource.volume = buzzVolume;
        // shift the pitch a little
        if(!isEvading) //evading does a different pitch
            audioSource.pitch = Mathf.Clamp(audioSource.pitch + Random.Range(-0.1f, 0.1f), 0.9f, 1.1f);


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

        if (didHandCollide)
            HandleEvading();

        if (flyTime < zigZagTime && !didHandCollide)
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

        bool isWindowOpen = windowOpener.IsWindowOpen();

        if (flyTime > flyingDuration && isWindowOpen)
        {
            flyTime = 0;
        }
        if (flyTime > flyingDuration && !isWindowOpen)
        {
            flyState = FlyState.Sitting;
            flyingInDirection = false;
            flyTime = 0;
        }

        didHandCollide = false;

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
    private IEnumerator EvadingCoroutine() 
    {
        float evadingTime = 0;
        isEvading = true;
        float ogFlyspeed = flySpeed;

        flySpeed *= 3;
        audioSource.pitch = 1.3f;

        while (evadingTime < evadingDuration) 
        {
            evadingTime += Time.deltaTime;

            audioSource.pitch += Random.Range(-0.1f, 0.1f);
            audioSource.pitch = Mathf.Clamp(audioSource.pitch, 1.1f, 1.5f);

            yield return null;
        }
        //yield return new WaitForSeconds(evadingDuration);

        flySpeed = ogFlyspeed;
        audioSource.pitch = 1f;

        isEvading = false;
    }
}
