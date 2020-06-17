using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class WeaponEventHandler : MonoBehaviour {
	public AudioClip[] audioClips;

	AudioSource audioSource;

	private void Awake() {
		audioSource = GetComponent<AudioSource>();
	}

	public void ShootEvent() {
		audioSource.pitch = 1 + Random.Range(-0.1f, 0.1f);
		audioSource.PlayOneShot(audioClips[0], 0.4f);
	}

	public void ReloadOutEvent() {
		audioSource.pitch = 1;
		audioSource.PlayOneShot(audioClips[1]);

	}

	public void ReloadInEvent() {
		audioSource.PlayOneShot(audioClips[2]);

	}

	public void ReloadSnapEvent() {
		audioSource.PlayOneShot(audioClips[3]);

	}
}
