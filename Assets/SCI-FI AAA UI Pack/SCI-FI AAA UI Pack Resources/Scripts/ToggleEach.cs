using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleEach : MonoBehaviour
{
    public Toggle toggle;
   public void ValueChange(bool b)
    {
        toggle.isOn = !b;
    }
}
