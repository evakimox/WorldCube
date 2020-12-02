using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;
using Cinemachine;

public class CinemachineCtrlWithTouch : MonoBehaviour
{
    public float speed = 1f;
    public float swipeMaxSpeed = 50f;
    CinemachineFreeLook freeLookComponent;

    // Start is called before the first frame update
    void Start()
    {
        freeLookComponent = GetComponent<CinemachineFreeLook>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 avgScreenDelta = LeanGesture.GetScreenDelta();       
        if (avgScreenDelta.magnitude > Mathf.Epsilon)
        {
            if(avgScreenDelta.magnitude > 50f)
            {
                avgScreenDelta = avgScreenDelta.normalized * swipeMaxSpeed;
            }
            float thetax = avgScreenDelta.x * 0.2f * speed; // 横向旋转(角度)
            float thetay = -avgScreenDelta.y * 0.001f * speed; // 纵向旋转(比例)
            freeLookComponent.m_XAxis.Value = freeLookComponent.m_XAxis.Value + thetax;
            freeLookComponent.m_YAxis.Value = Mathf.Clamp(freeLookComponent.m_YAxis.Value + thetay, -1f, 1f);
        }
    }
}
