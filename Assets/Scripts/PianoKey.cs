using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PianoKey : MonoBehaviour
{
   public AudioClip correctSound;
    public AudioClip incorrectSound;
    public bool isCorrectKey = false;

    private AudioSource audioSource;
    private Vector3 originalPosition;
    public float pressDepth = 0.01f;   // 下壓深度
    public float pressSpeed = 0.5f;    // 彈回速度

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        originalPosition = transform.localPosition;
    }

    void OnMouseDown()
    {
        PlaySound();
        PressKey();
    }

    void OnMouseUp()
    {
        ReleaseKey();
    }

    void PlaySound()
    {
        AudioClip clip = isCorrectKey ? correctSound : incorrectSound;
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    void PressKey()
    {
        transform.localPosition = originalPosition - new Vector3(0, 0, pressDepth);
    }

    void ReleaseKey()
    {
        transform.localPosition = originalPosition;
    }
}
