using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class SubElementChangeColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public UnityEvent enterEvents;
    public UnityEvent exitEvents;

 

    public void OnPointerEnter(PointerEventData eventData)
    {
        enterEvents.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        exitEvents.Invoke();
    }
}


 