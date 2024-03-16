using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UISound : MonoBehaviour,IPointerEnterHandler,IPointerDownHandler
{
    public AudioSource ass;

    public AudioClip onhover;
    public AudioClip pressed;

    public void OnPointerDown(PointerEventData eventData)
    {
        ass.PlayOneShot(pressed,0.3f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ass.Stop();
        ass.PlayOneShot(onhover,0.1f);
    }

 
 
}
