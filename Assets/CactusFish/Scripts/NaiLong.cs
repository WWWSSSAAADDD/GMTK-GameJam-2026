using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NaiLong : MonoBehaviour
{
    // 旋转速度，单位：度/秒
    public float rotateSpeed = 90f;
    // 旋转轴，默认绕自身Y轴旋转
    public Vector3 rotateAxis = Vector3.up;

    void Update()
    {
        // 乘以Time.deltaTime保证不同帧率下速度一致
        transform.Rotate(rotateAxis, rotateSpeed * Time.deltaTime);
    }
}
