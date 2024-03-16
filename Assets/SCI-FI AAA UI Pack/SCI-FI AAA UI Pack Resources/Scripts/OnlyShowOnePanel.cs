using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnlyShowOnePanel : MonoBehaviour
{
      public List<GameObject> gos=new List<GameObject>();

 
    public  void OnClick()
    {
        foreach (GameObject item in gos)
        {
            item.SetActive(false);

        
        }
    }
}
