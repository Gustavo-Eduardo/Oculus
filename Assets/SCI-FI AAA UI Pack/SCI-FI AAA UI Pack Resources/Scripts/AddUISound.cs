using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddUISound : MonoBehaviour
{
    public AudioSource ass;
    public AudioClip onhover;
    public AudioClip onpress;

    private void Start()
    {
       Button[] bts =  FindObjectsOfType<Button>();
        foreach (Button item in bts)
        {
            UISound u= item.gameObject.AddComponent<UISound>();
            u.ass = ass; u.onhover = onhover; u.pressed = onpress;
        }
    }
}
