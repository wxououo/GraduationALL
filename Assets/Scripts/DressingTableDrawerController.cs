using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DressingTableDrawerController : MonoBehaviour
{
    private bool isOpened = false;
    public bool requiresZoom = false; // �O�_�ݭn���Y��j�~���������
    private MouseLook cameraController; // �Ψ��ˬd���Y�O�_�w�վ�

    [Header("���ĳ]�w")]
    public AudioClip openSound;    // �}��P����
    public AudioClip closeSound;   // ����P����
    [Range(0.5f, 2f)]
    public float soundPitch = 2f;  // ���ļ���t�סA�w�]�y��
    private AudioSource audioSource;

    void Start()
    {
        cameraController = Camera.main.GetComponent<MouseLook>();
        // �T�O�� AudioSource �ե�
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        // �]�m���ļ���t��
        audioSource.pitch = soundPitch;
    }

    void OnMouseDown()
    {
        if (requiresZoom)
        {
            if (cameraController != null && cameraController.HasAdjustedCamera)
            {
                ToggleDrawer();
            }
            else
            {
                // ���Y���վ�A�L�k��������
                Debug.Log("You need to zoom in before switching scenes!");
            }
        }
        else
        {
            ToggleDrawer(); // ���P�Q�I���ɡA������P���A
        }
    }

    void ToggleDrawer()
    {
        isOpened = !isOpened; // ������P���A

        // �������������
        if (audioSource != null)
        {
            if (isOpened && openSound != null)
            {
                audioSource.PlayOneShot(openSound);
            }
            else if (!isOpened && closeSound != null)
            {
                audioSource.PlayOneShot(closeSound);
            }
        }

        // �ھک�P���A�A�]�m��P����m
        if (isOpened)
        {
            transform.Translate(0f, -200f, 0f); // �V�e���ʤ@�w�Z��
        }
        else
        {
            transform.Translate(0f, 200f, 0f); // �V�Ჾ�ʤ@�w�Z��
        }
    }
}
