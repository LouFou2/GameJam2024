using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WindowOpener : MonoBehaviour
{
    [SerializeField] private WindowTrigger windowTrigger;
    [SerializeField] private GameObject windowFrame;
    [SerializeField] private GameObject windowPane;
    [SerializeField] private Color highlightColor;
    [SerializeField] private TMP_Text text;
    [SerializeField] private float openSpeed;
    [SerializeField] private float closeSpeed;
    [SerializeField] private float openCloseDistance = 5f;

    [SerializeField] private AudioClip openAudio;
    [SerializeField] private AudioClip closeAudio;

    private PlayerControl playerControl;

    private bool canOpenWindow = false;
    private SpriteRenderer frameSpriteRenderer;
    private bool isWindowOpen = false;

    private AudioSource audioSource;

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

    private void Start()
    {
        frameSpriteRenderer = windowFrame.GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        canOpenWindow = windowTrigger.GetWindowTriggered();

        if (canOpenWindow && !isWindowOpen)
        {
            frameSpriteRenderer.color = new Color(highlightColor.r, highlightColor.g, highlightColor.b);
            text.text = ("click to open window");
        }
        else
        {
            frameSpriteRenderer.color = new Color(1f, 1f, 1f);
            text.text = string.Empty;
        }
        if (canOpenWindow && playerControl.Controls.MouseClick.IsPressed() && !isWindowOpen)
        {
            StartCoroutine(OpenWindowCoroutine());
        }
    }
    private IEnumerator OpenWindowCoroutine()
    {
        isWindowOpen = true;

        audioSource.clip = openAudio;
        audioSource.Play();

        float fullyOpenDistance = openCloseDistance; // Distance to move the window (how much it should move up)
        float yMovement = 0f; // Reset yMovement for this animation

        // Store the original Y positions of the windowPane and windowFrame
        float originalPaneYPosition = windowPane.transform.position.y;
        float originalFrameYPosition = windowFrame.transform.position.y;

        while (yMovement < fullyOpenDistance)
        {
            // Increment yMovement based on the openSpeed and deltaTime
            yMovement += Time.deltaTime * openSpeed;

            // Clamp yMovement to ensure it does not exceed fullyOpenDistance
            float actualMovement = Mathf.Min(yMovement, fullyOpenDistance);

            // Move the windowPane and windowFrame based on their original positions
            windowPane.transform.position = new Vector3(
                windowPane.transform.position.x,
                originalPaneYPosition + actualMovement,
                windowPane.transform.position.z
            );

            windowFrame.transform.position = new Vector3(
                windowFrame.transform.position.x,
                originalFrameYPosition + actualMovement,
                windowFrame.transform.position.z
            );

            // Yield until the next frame
            yield return null;
        }

        // Ensure the final position is set exactly at fullyOpenDistance relative to the original position
        windowPane.transform.position = new Vector3(
            windowPane.transform.position.x,
            originalPaneYPosition + fullyOpenDistance,
            windowPane.transform.position.z
        );

        windowFrame.transform.position = new Vector3(
            windowFrame.transform.position.x,
            originalFrameYPosition + fullyOpenDistance,
            windowFrame.transform.position.z
        );

        StartCoroutine(CloseWindowCoroutine());
    }
    private IEnumerator CloseWindowCoroutine()
    {
        audioSource.Stop();
        audioSource.clip = closeAudio;
        audioSource.Play();

        float fullyClosedDistance = openCloseDistance; // The distance to move the window to fully close it
        float yMovement = 0f; // Reset yMovement for this animation

        // Store the original Y positions of the windowPane and windowFrame (which is the fully open position now)
        float originalPaneYPosition = windowPane.transform.position.y;
        float originalFrameYPosition = windowFrame.transform.position.y;

        // Calculate the final position when the window is fully closed (where it started before opening)
        float targetPaneYPosition = originalPaneYPosition - fullyClosedDistance;
        float targetFrameYPosition = originalFrameYPosition - fullyClosedDistance;

        while (yMovement < fullyClosedDistance)
        {
            // Increment yMovement based on the openSpeed and deltaTime
            yMovement += Time.deltaTime * closeSpeed;

            // Clamp yMovement to ensure it does not exceed fullyClosedDistance
            float actualMovement = Mathf.Min(yMovement, fullyClosedDistance);

            // Move the windowPane and windowFrame based on the original positions, subtracting movement to close
            windowPane.transform.position = new Vector3(
                windowPane.transform.position.x,
                originalPaneYPosition - actualMovement, // Move downward
                windowPane.transform.position.z
            );

            windowFrame.transform.position = new Vector3(
                windowFrame.transform.position.x,
                originalFrameYPosition - actualMovement, // Move downward
                windowFrame.transform.position.z
            );

            // Yield until the next frame
            yield return null;
        }

        // Ensure the final position is set exactly at the fully closed position
        windowPane.transform.position = new Vector3(
            windowPane.transform.position.x,
            targetPaneYPosition, // Fully closed
            windowPane.transform.position.z
        );

        windowFrame.transform.position = new Vector3(
            windowFrame.transform.position.x,
            targetFrameYPosition, // Fully closed
            windowFrame.transform.position.z
        );

        isWindowOpen = false; // Reset the windowOpening flag
    }

    public bool IsWindowOpen() 
    {
        return isWindowOpen;
    }
    public float GetTopOfWindowGap() 
    {
        return 0f;
    }
    public float GetBottomOfWindowGap()
    {
        return 0f;
    }
}