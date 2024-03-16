 

using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Image))]
public class ImagechangeMyColor : MonoBehaviour
{
    public Color normalColor;
    public Color highlightColor;

    public void Start()
    {
        normalColor = GetComponent<Image>().color;
    }

    public void OnEnter()
    {
        GetComponent<Image>().color = highlightColor;
    }

    public void OnExit()
    {
        GetComponent<Image>().color = normalColor;
    }
}
