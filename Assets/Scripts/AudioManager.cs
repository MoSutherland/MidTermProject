using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Sound Effect")]
    [SerializeField] AudioClip magicRingClip;
    [SerializeField][Range(0f, 1f)] float magicRingClipVolume = 1f;


    private void Awake()
    {
            instance = this;
    }


    public void PlayMagicRingClip()
    {
        if (magicRingClip != null)
        {
            Debug.Log("Button Clip playing");

            AudioSource.PlayClipAtPoint(magicRingClip, Camera.main.transform.position, magicRingClipVolume);
            
        }
    }


}
