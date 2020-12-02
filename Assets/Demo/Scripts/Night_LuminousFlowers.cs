using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;

public class Night_LuminousFlowers : MonoBehaviour
{
    public Camera cubeCamera;
    public Camera worldCamera;

    GameObject LeanController;

    ParticleSystem myParticleSystem;

    void Start()
    {
        myParticleSystem = GetComponent<ParticleSystem>();

        LeanController = GameObject.Find("LeanTouch");
        LeanFingerTap tapCtrl = LeanController.GetComponent<LeanFingerTap>();
        tapCtrl.OnFinger.AddListener(OnTap);
    }

    void OnTap(LeanFinger finger)
    {
        bool shouldChange = CubeRaycastHitTest.Instance.isHit(cubeCamera, worldCamera, finger.ScreenPosition, GetComponent<Collider>(), "Night");
        if (shouldChange)
        {
            EmitParticles();
        }
    }

    void EmitParticles()
    {
        myParticleSystem.Play();
    }
}
