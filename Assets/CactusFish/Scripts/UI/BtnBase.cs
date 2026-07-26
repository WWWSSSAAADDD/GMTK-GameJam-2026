using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtnBase : MonoBehaviour
{
    public void OpenPanel(string Uiname)
    {
        GameManager.Instance.UI.Open(Uiname);
    }
    public void ClosePanel(string Uiname)
    {
        GameManager.Instance.UI.Close(Uiname);
    }
}
