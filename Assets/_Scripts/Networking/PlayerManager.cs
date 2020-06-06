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

    public void Initialize(int id, string username) {
        this.id = id;
        this.username = username;
        health = maxHealth;
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
}
