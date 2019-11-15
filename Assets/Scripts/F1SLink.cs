﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class F1SLink : MonoBehaviour {
    private Player p;
    
    void Start() {
        Initialize();
        //AndroidJavaClass jc = new AndroidJavaClass("com.lelo.f1s.OverrideUnityActivity");
        //AndroidJavaObject overrideActivity = jc.GetStatic<AndroidJavaObject>("instance");
        //jc.CallStatic("UnitySendMessage", "F1SLink", "ChangeColor", "Yellow");
        //overrideActivity.Call("showMainActivity", lastStringColor);
    }

    public void UpdateDepthFromSensor(String s) {
        p.UpdateDepth(Int32.Parse(s));
    }

    public void AttackSignal(String s) {
        p.Attack();
    }

    void Update() {
    }

    private void Initialize() {
        p=Player.sng;
    }
}
