using UnityEngine.UI;
using UnityEngine;
using System.Collections;

// TODO: Add ammo to PlayerManager and network it in a similar fashion to health
/// <summary>
/// Stores client ID and username
/// </summary>
public class PlayerManager : MonoBehaviour {
    public int id; // Client ID
    public string username; // Username

    public float health;
    public float maxHealth;

    public int ammo;

    public MeshRenderer model;
    private Text ammoText;
    private Text healthText;
    private Animator anim;

    private float nextShot;

    public void Initialize(int id, string username) {
        this.id = id;
        this.username = username;
        health = maxHealth;

        if (Client.instance.myId != id) return;

        anim = GetComponentInChildren<Animator>();
        ammoText = GameObject.Find("Ammo Text").GetComponent<Text>();
        healthText = GameObject.Find("Health Text").GetComponent<Text>();
        nextShot = 0;
        ammo = 30;
    }

	private void FixedUpdate() {
        if (Client.instance.myId != id) return;

        nextShot -= Time.deltaTime;

        if (nextShot <= 0) {
            nextShot = 0;
        }

        ammoText.text = ammo.ToString();
        healthText.text = health.ToString();
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

    public void AnimateShoot(int ammo) {
        anim.Play("Weapon.RifleShootAuto", -1, 0);
        this.ammo = ammo;
    }

    public void AnimateReload(int ammo, float reloadTime) {
        anim.Play("Weapon.RifleReload");
        StartCoroutine(WaitForReload(ammo, reloadTime));
	}

    private IEnumerator WaitForReload(int ammo, float reloadTime) {
        yield return new WaitForSeconds(reloadTime);
        this.ammo = ammo;
    }
}
