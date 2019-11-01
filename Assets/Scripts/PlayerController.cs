﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class PlayerController : MonoBehaviour {
    public static PlayerController sng { get; private set; } //singletone
    public int health;
    public float gravityCoeff;
    public float speed;
    public float recoveryTime;
    public bool topOriented;
    
    private Rigidbody2D rb;
    private bool isRecovering = false;
    
    private void Awake() {
        if (sng == null) sng = this;
        else {
            Destroy(sng);
            sng = this;
        }
    }

    void Start() {
        Initialize();
    }
    
    void Update() {
        if (health > 0 && (Input.GetKeyDown(KeyCode.DownArrow) && rb.gravityScale < 0 && topOriented ||
                           Input.GetKeyDown(KeyCode.UpArrow) && rb.gravityScale > 0 && !topOriented))
            rb.gravityScale *= -1;
        UserInterface.sng.UpdateScore(Mathf.FloorToInt(transform.position.x));
    }

    public void OnGravityDirectionChange(int k) {
        if (health > 0 && (k == 1 && rb.gravityScale < 0 && topOriented) ||
            k == -1 && rb.gravityScale > 0 && !topOriented)
            rb.gravityScale *= -1;
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (health > 0) {
            if (other.collider.CompareTag("Obstacle")) {
                Vector2 pointDifference = other.GetContact(0).point-(Vector2)transform.position;
                if(Mathf.Abs(pointDifference.y)>0.2f) topOriented = pointDifference.y > 0;
                if (Mathf.Abs(pointDifference.x) < 0.25f) rb.velocity = Vector2.right * speed;
            }
            else if (other.collider.CompareTag("Obstahurt")) {
                Health -= other.gameObject.GetComponentInParent<Obstahurt>().damage;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) { //Vracanje prepreka u pool!
        if (other.gameObject.CompareTag("ObstaclePack") && other.isTrigger) {
            other.GetComponent<ObstaclePack>().ActiveObstaclePack = false;
        }
    }

    public void Initialize() {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravityCoeff;
        topOriented = true;
        rb.velocity = Vector2.right * speed;
        UserInterface.sng.UpdateHealth(health);
    }
    
    private IEnumerator DamageRecovery(float tickTime) {
        isRecovering = true;
        WaitForSeconds tick = new WaitForSeconds(tickTime);
        float tRecovery = recoveryTime;
        SpriteRenderer spr = GetComponent<SpriteRenderer>();
        while (tRecovery > 0) {
            spr.enabled = !spr.enabled;
            UserInterface.sng.damageSplash.SetActive(!UserInterface.sng.damageSplash.activeSelf);
            tRecovery -= tickTime;
            yield return tick;
        }
        spr.enabled = true;
        isRecovering = false;
        UserInterface.sng.damageSplash.SetActive(false);
    }
    
    public int Health {
        get => health;
        set {
            if (!isRecovering) {
                Debug.Log("IT HITS YOU");
                health = value;
                UserInterface.sng.UpdateHealth(health);
                if (health <= 0) {
                    UserInterface.sng.ripSplash.SetActive(true);
                }
                else StartCoroutine(DamageRecovery(0.1f));
            }
        }
    }
}