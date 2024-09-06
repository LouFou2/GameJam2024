using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WindowOpener : MonoBehaviour
{
    [SerializeField] private WindowTrigger windowTrigger;
    [SerializeField] private GameObject windowFrame;
    [SerializeField] private GameObject windowPane;
    [SerializeField] private TMP_Text text;
    [SerializeField] private float openSpeed;

    private bool canOpenWindow = false;
    private SpriteRenderer frameSpriteRenderer;
    private bool windowOpening = false;
    private float yMovement = 0;

    private void Start()
    {
        frameSpriteRenderer = windowFrame.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        canOpenWindow = windowTrigger.GetWindowTriggered();

        if (canOpenWindow && !windowOpening)
        {
            frameSpriteRenderer.color = new Color(0.2f, 1f, 0f);
            text.text = ("spacebar to open window");
        }
        else 
        {
            frameSpriteRenderer.color = new Color(1f, 1f, 1f);
            text.text = string.Empty;
        }
        if (canOpenWindow && Input.GetKeyDown(KeyCode.Space) && !windowOpening) 
        {
            StartCoroutine(OpenWindowCoroutine());
        }
    }
    private IEnumerator OpenWindowCoroutine() 
    {
        windowOpening = true;

        float fullyOpen = 5f; // Set this to the distance you want the window to move
        yMovement = 0; // Reset yMovement to 0 for this animation

        while (yMovement < fullyOpen)
        {
            // Increment yMovement based on the openSpeed and deltaTime
            yMovement += Time.deltaTime * openSpeed;

            // Clamp yMovement to ensure it does not exceed fullyOpen
            yMovement = Mathf.Min(yMovement, fullyOpen);

            // Move the windowPane and windowFrame based on yMovement
            windowPane.transform.position += new Vector3(0, Time.deltaTime * openSpeed, 0);
            windowFrame.transform.position += new Vector3(0, Time.deltaTime * openSpeed, 0);

            // Yield until the next frame
            yield return null;
        }

        // Ensure the final position is set exactly at the fullyOpen value
        windowPane.transform.position = new Vector3(windowPane.transform.position.x, fullyOpen, windowPane.transform.position.z);
        windowFrame.transform.position = new Vector3(windowFrame.transform.position.x, fullyOpen, windowFrame.transform.position.z);

        windowOpening = false; // Reset the windowOpening flag
    }
}
