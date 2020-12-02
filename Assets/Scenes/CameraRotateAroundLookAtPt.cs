using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;

public class CameraRotateAroundLookAtPt : MonoBehaviour
{
    public GameObject cameraObj;
    public GameObject lookAtPoint;
    public float speed = 10f;

    Vector3 initialForward;
    void Start()
    {
        UpdateCameraLook();
    }

    void Update()
    {
        Vector2 avgScreenDelta = LeanGesture.GetScreenDelta();
        Vector3 forward = lookAtPoint.transform.forward;
        Vector3 right = lookAtPoint.transform.right;
        Vector3 up = lookAtPoint.transform.up;
        if (avgScreenDelta.magnitude > Mathf.Epsilon)
        {
            float thetax = avgScreenDelta.x * 0.001f * speed; // 横向旋转
            Vector3 dstForward = Mathf.Cos(thetax) * forward + Mathf.Sin(thetax) * right;
            dstForward = dstForward.normalized;

            float thetay = avgScreenDelta.y * 0.001f * speed; // 纵向旋转
            Vector3 tempDstForward = Mathf.Cos(thetay) * dstForward + Mathf.Sin(thetay) * up;
            tempDstForward = tempDstForward.normalized;
            //if (Mathf.Abs(tempDstForward.x) < 0.5f)
            //{
            //    float sign = Mathf.Sign(tempDstForward.x);
            //    tempDstForward.x = 0.5f * sign;
            //}
            dstForward = tempDstForward;
            dstForward = dstForward.normalized;

            lookAtPoint.transform.forward = dstForward;

            //Debug.Log(lookAtPoint.gameObject.name + " forward: " + lookAtPoint.transform.forward + " Eulers: " + lookAtPoint.transform.rotation.eulerAngles);
        }
    }

    public void UpdateCameraLook()
    {
        if (cameraObj.transform.parent.gameObject != lookAtPoint)
        {
            // 使相机“看”向LookAtPoint的位置
            Vector3 forward = lookAtPoint.transform.position - cameraObj.transform.position;
            Vector3 forwardNormalized = forward.normalized;
            cameraObj.transform.forward = forwardNormalized;
            lookAtPoint.transform.forward = forwardNormalized;
            initialForward = forwardNormalized;


            // 把相机塞进去，保证相对位置
            cameraObj.transform.parent = lookAtPoint.transform;
        }
    }
}
