using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Countdown : MonoBehaviour
{
    public CountdownUI ui;
    public float time;
    // Start is called before the first frame update
    void Start()
    {
        ui.StartCountdown(time);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
