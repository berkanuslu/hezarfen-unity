using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{

	static SoundManager _instance;
	static int instances = 0;

	public Transform[] audioButtons = null;

	public static SoundManager Instance
	{
		get
		{
			if (_instance == null)
				_instance = FindObjectOfType(typeof(SoundManager)) as SoundManager;

			return _instance;
		}
	}

	void Start()
	{
		instances++;
		if (instances > 1)
			Debug.LogWarning("Warning: There are more than one SoundManager at the level");
		else
			_instance = this;

		float storedVolumeValue = PreferencesManager.Instance.GetMusicVolume();
		for (int i = 0; i < audioButtons.Length; i++)
		{
			if (storedVolumeValue > 0)
			{
				audioButtons[i].GetComponent<Renderer>().material.mainTexture = GameMenuManager.Instance.menuTextures[2];
				GameMenuManager.Instance.audioEnabled = true;
			}
			else
			{
				audioButtons[i].GetComponent<Renderer>().material.mainTexture = GameMenuManager.Instance.menuTextures[3];
				GameMenuManager.Instance.audioEnabled = false;
			}
		}
		if (storedVolumeValue <= 0)
		{
			this.StopMusic();
		}
	}

	public void SetMusicVolume(float value)
	{
		if (gameObject.GetComponent<AudioSource>() != null)
		{
			gameObject.GetComponent<AudioSource>().volume = value;
			PreferencesManager.Instance.SetMusicVolume(value);
		}
	}

	public void PauseMusic()
	{
		if (gameObject.GetComponent<AudioSource>() != null)
		{
			gameObject.GetComponent<AudioSource>().Pause();
		}
	}

	public void StopMusic()
	{
		if (gameObject.GetComponent<AudioSource>() != null)
		{
			gameObject.GetComponent<AudioSource>().Stop();
		}
	}

	public void StartMusic()
	{
		if (PreferencesManager.Instance.GetMusicVolume() > 0)
		{
			if (gameObject.GetComponent<AudioSource>() != null)
			{
				gameObject.GetComponent<AudioSource>().Stop();
				gameObject.GetComponent<AudioSource>().Play();
			}
		}
	}

	public void ResumeMusic()
	{
		if (PreferencesManager.Instance.GetMusicVolume() > 0)
		{
			if (gameObject.GetComponent<AudioSource>() != null)
			{
				gameObject.GetComponent<AudioSource>().Play();
			}
		}
	}

	public void CheckVolumeButtons()
	{
		float storedVolumeValue = gameObject.GetComponent<AudioSource>().volume;
		for (int i = 0; i < audioButtons.Length; i++)
		{
			if (storedVolumeValue > 0)
			{
				audioButtons[i].GetComponent<Renderer>().material.mainTexture = GameMenuManager.Instance.menuTextures[2];
				GameMenuManager.Instance.audioEnabled = true;
			}
			else
			{
				audioButtons[i].GetComponent<Renderer>().material.mainTexture = GameMenuManager.Instance.menuTextures[3];
				GameMenuManager.Instance.audioEnabled = false;
			}
		}
	}
}
