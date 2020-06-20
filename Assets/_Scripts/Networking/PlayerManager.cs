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
    public GameObject laserPrefab;
    public Transform muzzle;

    public MeshRenderer model;
    private Text ammoText;
    private Text healthText;
    private Animator anim;

    public void Initialize(int id, string username) {
        this.id = id;
        this.username = username;
        health = maxHealth;

        if (Client.instance.myId != id) return;

        anim = GetComponentInChildren<Animator>();
        ammoText = GameObject.Find("Ammo Text").GetComponent<Text>();
        healthText = GameObject.Find("Health Text").GetComponent<Text>();
        ammo = 30;
    }

	private void FixedUpdate() {
        if (Client.instance.myId != id) return;

        // Update the visuals
        ammoText.text = ammo.ToString();
        healthText.text = health.ToString();
	}

    /// <summary>
    /// Sets health to a given value
    /// </summary>
    /// <param name="health">Health</param>
	public void SetHealth (float health) {
        this.health = health;

        if (health <= 0f) {
            Die();
		}
	}

    /// <summary>
    /// Disables visuals
    /// </summary>
    public void Die() {
        // TODO: Disable viewmodel visuals
        model.enabled = false;
	}

    /// <summary>
    /// Re-enables visuals and resets health
    /// </summary>
    public void Respawn() {
        model.enabled = true;
        SetHealth(maxHealth);
	}

    /// <summary>
    /// Animates movement
    /// </summary>
    /// <param name="animationState">Current movement animation state</param>
    public void AnimateMovement(int animationState) {
        GetComponentInChildren<Animator>().SetInteger("Movement State", animationState);
	}

    /// <summary>
    /// Animates a local player
    /// </summary>
    /// <param name="ammo"></param>
    public void AnimateShoot() {
        anim.Play("Weapon.RifleShootAuto", -1, 0);
    }

    public void Shoot(int id, int ammo, Vector3 direction) {
        this.ammo = ammo;
        Transform muzzle = GameManager.players[id].muzzle;

        // Raycast
        bool raycast = Physics.Raycast(muzzle.position, direction, out RaycastHit hit, 500f);

        // Summon laser
        GameObject laser = Instantiate(laserPrefab, Vector3.zero, Quaternion.identity);
        // If the raycast hit something, use that Vector3, otherwise set the direction far
        laser.GetComponentInChildren<LineRenderer>().SetPositions(new Vector3[] {muzzle.position, raycast ? hit.point : (direction * 500f)});
    }

    /// <summary>
    /// Plays a reload animation
    /// </summary>
    public void AnimateReload() {
        anim.Play("Weapon.RifleReload");
	}

    /// <summary>
    /// Reloads the current weapon
    /// </summary>
    /// <param name="ammo">Max ammo</param>
    /// <param name="reloadTime">Time it takes to reload</param>
    public void Reload(int ammo, float reloadTime) {
        StartCoroutine(WaitForReload(ammo, reloadTime));
    }

    /// <summary>
    /// Waits until reload finishes, then resets the ammo count
    /// </summary>
    /// <param name="ammo">Max ammo</param>
    /// <param name="reloadTime">Time it takes to reload</param>
    /// <returns></returns>
    private IEnumerator WaitForReload(int ammo, float reloadTime) {
        yield return new WaitForSeconds(reloadTime);
        this.ammo = ammo;
    }
}
