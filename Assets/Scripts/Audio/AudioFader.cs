/*
 * Script taken from AI_Rogues, copyright 2019
 * http://cinderscript.azurewebsites.net/AI_Rogues
 * 
 * The AudioFader class provides a simple way to fade in and out audio sound effects.
 */

using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioFader : MonoBehaviour
{
	private AudioSource audioSource;

	[Tooltip("Minmum volume during fade out.  " +
		"MaxVolume is set by AudioSource starting volume.")]
	public float MinVolume = 0;

	// maxVolume is controlled by AudioSource
	private float maxVolume;
	public bool IsFading { get; private set; }
	public float TimeRemaining { get; private set; }

	private Coroutine fading;

	void Awake()
	{
		IsFading = false;

		if (!audioSource)
		{
			audioSource = GetComponent<AudioSource>();
		}

		if (audioSource)
		{
			maxVolume = audioSource.volume;
		}
	}

	public void FadeIn(float duration, bool stopAudioSource = false)
	{
		FadeTo(maxVolume, duration, stopAudioSource);
	}
	public void FadeOut(float duration, bool stopAudioSource = false)
	{
		FadeTo(MinVolume, duration, stopAudioSource);
	}

	/// <summary>
	/// Fades the audio source to the volume level indicated.  
	/// </summary>
	/// <param name="targetVolume"></param>
	/// <param name="duration"></param>
	public void FadeTo(float targetVolume, float duration, bool stopAudioSource = false)
	{
		if (IsFading)
		{
			StopCoroutine(fading);
		}

		fading = StartCoroutine(Fade(targetVolume, duration, stopAudioSource));
	}
	IEnumerator Fade(float targetVolume, float duration, bool stopAudioSource = false)
	{
		IsFading = true;

		if (audioSource)
		{
			// Start audio if not already playing
			if (!audioSource.isPlaying)
			{
				audioSource.Play();
				audioSource.volume = 0;
			}

			float timer = 0;
			float startVolume = audioSource.volume;

			while (timer < duration)
			{
				timer += Time.deltaTime;
				TimeRemaining = duration - timer;
				Mathf.Lerp(startVolume, targetVolume, timer / duration);
				float volume = Mathf.Lerp(startVolume, targetVolume, timer / duration);
				audioSource.volume = volume;

				yield return null;
			}

			audioSource.volume = targetVolume;

			if (stopAudioSource)
			{
				audioSource.Stop();
			}
		}

		IsFading = false;
	}
}
