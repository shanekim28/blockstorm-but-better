using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

[Serializable]
public class AudioClipDictionary : SerializableDictionary<string, AudioClip> {
}

public class AudioManager : MonoBehaviour {
	public static AudioManager instance;
	public GameObject tempAudioSource;

	private void Awake() {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Debug.LogError("Audio Manager instance already exists. Destroying instance");
			Destroy(this);
		}
	}

	public AudioClipDictionary audioClips;

	public void PlaySound(string clipName, Vector3 position, float volume, float pitch) {
		// Create game object
		GameObject tempGameObject = Instantiate(tempAudioSource, position, Quaternion.identity);

		// Audio source setup
		var audioSource = tempGameObject.GetComponent<AudioSource>();
		audioSource.clip = audioClips[clipName];
		audioSource.volume = volume;
		audioSource.pitch = pitch;

		audioSource.Play();
		Destroy(tempGameObject, audioSource.clip.length);
	}

	public void PlaySound(string clipName, int playerID, float volume, float pitch) {
		var audioSource = GameManager.players[playerID].GetComponent<AudioSource>();
		audioSource.volume = volume;
		audioSource.pitch = pitch;

		audioSource.PlayOneShot(audioClips[clipName]);
	}
}
