using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosetController : MonoBehaviour
{
    private bool isOpened = false;
    public Transform pivot; // Reference to the pivot GameObject
    private Quaternion closedRotation; // Store as a Quaternion
    private Quaternion openedRotation; // Opened rotation as a Quaternion
    public float rotationSpeed = 100f; // Speed for rotation
    public bool isLeftDoor = false; // Set this to true for the left door, false for the right door
    private float precisionThreshold = 1f; // Precision tolerance for stopping rotation
    public bool requiresZoom = false;
    private MouseLook cameraController;
    public AudioSource audioSource; // AudioSource for the door sound

    void Start()
    {
        cameraController = Camera.main.GetComponent<MouseLook>();
        // Record the pivot's initial rotation
        closedRotation = pivot.localRotation; // Using Quaternion for smoother rotation

        // Set the target opened rotation based on the pivot's initial rotation
        if (isLeftDoor)
        {
            openedRotation = Quaternion.Euler(closedRotation.eulerAngles.x, closedRotation.eulerAngles.y - 90f, closedRotation.eulerAngles.z); // Left door opens outward
        }
        else
        {
            openedRotation = Quaternion.Euler(closedRotation.eulerAngles.x, closedRotation.eulerAngles.y + 90f, closedRotation.eulerAngles.z); // Right door opens outward
        }
    }
    void Update()
    {
        if (!cameraController.HasAdjustedCamera)
        {
            StartCoroutine(CloseDoor());
        }
    }
    void OnMouseDown()
    {
        StopAllCoroutines(); // Stop any ongoing movement
        if (cameraController.HasAdjustedCamera)
        {
            if (isOpened)
            {
                PlayAudioSegment(4f, 2f);
                StartCoroutine(CloseDoor());
            }
            else
            {
                PlayAudioSegment(1f, 3f);
                StartCoroutine(OpenDoor());
            }
        }
    }

    IEnumerator OpenDoor()
    {
        isOpened = true;


        // Smoothly rotate the door using the pivot
        while (Quaternion.Angle(pivot.localRotation, openedRotation) > precisionThreshold)
        {
            pivot.localRotation = Quaternion.RotateTowards(pivot.localRotation, openedRotation, Time.deltaTime * rotationSpeed);
            yield return null;
        }

        // Snap to the final rotation to avoid overshooting
        pivot.localRotation = openedRotation;


    }

    IEnumerator CloseDoor()
    {
        isOpened = false;


        // Smoothly rotate the door back to the closed state
        while (Quaternion.Angle(pivot.localRotation, closedRotation) > precisionThreshold)
        {
            pivot.localRotation = Quaternion.RotateTowards(pivot.localRotation, closedRotation, Time.deltaTime * rotationSpeed);
            yield return null;
        }

        // Snap to the final closed rotation
        pivot.localRotation = closedRotation;

    }
    void PlayAudioSegment(float startTime, float duration)
    {
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.time = startTime; // 設置播放開始時間
            audioSource.Play(); // 播放音效
            StartCoroutine(StopAudioAfterDuration(duration));
        }
    }

    IEnumerator StopAudioAfterDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        if (audioSource.isPlaying)
        {
            audioSource.Stop(); // 停止音效播放
        }
    }
}
