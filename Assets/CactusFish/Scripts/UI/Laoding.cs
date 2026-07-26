using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Laoding : MonoBehaviour
{
    public void one(string name)
    {
        SceneManager.LoadScene(name);
        GameManager.Instance.ChangeState(GameState.Playing);
    }
}
