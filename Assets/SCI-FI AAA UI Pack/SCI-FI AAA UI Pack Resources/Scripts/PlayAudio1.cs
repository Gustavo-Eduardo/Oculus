using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudio1 : MonoBehaviour
{

    public AudioSource ass;

    public AudioClip clip;
    
    
    public void Play()
    {
        ass.PlayOneShot((clip));
    }

  
}
