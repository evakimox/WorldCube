using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;

public class Room_PotChangeColor : MonoBehaviour
{
    public Camera cubeCamera;
    public Camera worldCamera;

    public GameObject colorObj;
    public List<Color> colors = new List<Color>();
    int currentColorIndex = 0;

    GameObject LeanController;

    ParticleSystem myParticleSystem;
    ParticleSystem.ColorOverLifetimeModule colorModule;
    
    // Start is called before the first frame update
    void Start()
    {
        // 魔药锅的参数缓存
        myParticleSystem = colorObj.GetComponent<ParticleSystem>();
        colorModule = myParticleSystem.colorOverLifetime;

        // 把响应加到场景的Lean控制器
        LeanController = GameObject.Find("LeanTouch");
        LeanFingerTap tapCtrl = LeanController.GetComponent<LeanFingerTap>();
        tapCtrl.OnFinger.AddListener(OnTap);

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTap(LeanFinger finger)
    {
        bool shouldChange = CubeRaycastHitTest.Instance.isHit(cubeCamera, worldCamera, finger.ScreenPosition, GetComponent<Collider>(),"Room");
        if (shouldChange)
        {
            ChangeColor();
        }
    }

    void ChangeColor()
    {
        currentColorIndex = currentColorIndex + 1;
        if (currentColorIndex >= colors.Count)
        {
            currentColorIndex = 0;
        }
        Color color = colors[currentColorIndex];
        Color gradientStart = color;
        Color gradientEnd = color;
        gradientEnd.a = 0;
        colorModule.color = new ParticleSystem.MinMaxGradient(gradientStart, gradientEnd);
    }
}
