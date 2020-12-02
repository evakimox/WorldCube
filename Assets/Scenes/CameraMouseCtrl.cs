using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMouseCtrl : MonoBehaviour
{
    Camera mainCam;
    Vector3 dragOrigin;
    private void Start()
    {
        mainCam = GetComponent<Camera>();
    }
    private void Update()
    {
        if(Input.mouseScrollDelta.y > 0.05f)
        {
            transform.position = transform.position + 0.5f * mainCam.transform.forward;
        }
        if (Input.mouseScrollDelta.y < -0.05f)
        {
            transform.position = transform.position - 0.5f * mainCam.transform.forward;
        }

        if (Input.GetMouseButtonDown(2) || Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        float rotSpd = 0.1f;

        if (Input.GetMouseButton(0))
        {
            Vector3 pos = mainCam.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
            if(pos.x > Mathf.Epsilon)
            {
                Vector3 angles = transform.rotation.eulerAngles;
                angles.y = angles.y + rotSpd;
                transform.eulerAngles = angles;
            }
            if (pos.x < -Mathf.Epsilon)
            {
                Vector3 angles = transform.rotation.eulerAngles;
                angles.y = angles.y - rotSpd;
                transform.eulerAngles = angles;
            }
            if (pos.y > Mathf.Epsilon)
            {
                Vector3 angles = transform.rotation.eulerAngles;
                angles.x = angles.x - rotSpd;
                transform.eulerAngles = angles;
            }
            if (pos.y < - Mathf.Epsilon)
            {
                Vector3 angles = transform.rotation.eulerAngles;
                angles.x = angles.x + rotSpd;
                transform.eulerAngles = angles;
            }
        }

        if (Input.GetMouseButton(2))
        {
            Vector3 pos = Input.mousePosition - dragOrigin;
            pos = pos * -0.1f;
            transform.Translate(pos.x,pos.y,0);
            dragOrigin = Input.mousePosition;
        }
    }
}
