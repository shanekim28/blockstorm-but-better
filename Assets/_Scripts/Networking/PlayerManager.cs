using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores client ID and username
/// </summary>
public class PlayerManager : MonoBehaviour {
    public int id; // Client ID
    public string username; // Username

    public float health;
    public float maxHealth;

    public MeshRenderer model;

    private Animator anim;

    private float nextShot;

    // TODO: Shooting and reloading

    public void Initialize(int id, string username) {
        this.id = id;
        this.username = username;
        health = maxHealth;
        anim = GetComponentInChildren<Animator>();
        nextShot = 0;
    }

	private void FixedUpdate() {
        nextShot -= Time.deltaTime;

        if (nextShot <= 0) {
            nextShot = 0;
        }
	}

	public void SetHealth (float health) {
        this.health = health;

        if (health <= 0f) {
            Die();
		}
	}

    public void Die() {
        model.enabled = false;
	}

    public void Respawn() {
        model.enabled = true;
        SetHealth(maxHealth);
	}

    public void AnimateMovement(int animationState) {
        GetComponentInChildren<Animator>().SetInteger("Movement State", animationState);
	}

    public void AnimateShoot() {
        anim.Play("Weapon.RifleShootAuto", -1, 0);
    }

    public void AnimateReload() {
        anim.Play("Weapon.RifleReload");
	}
}
