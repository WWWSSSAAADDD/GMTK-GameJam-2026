using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRegister : MonoBehaviour
{
    public List<GameObject> objects = new List<GameObject>();
    void Start()
    {
        foreach (GameObject obj in objects)
        {
            GameManager.Instance.UI.Register(obj.name, obj, UILayer.Popup);
        }
    }
}
