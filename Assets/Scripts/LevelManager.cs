using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour
{
	int collectedCoins = 0;

	static LevelManager _instance;
	static int instances = 0;

	public static LevelManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType(typeof(LevelManager)) as LevelManager;
			}

			return _instance;
		}
	}

	void Start()
	{
		instances++;

		if (instances > 1)
		{
			Debug.LogWarning("Warning: There are more than one Level Manager at the level");
		}
		else
		{
			_instance = this;
		}

        GameAnalytics.Initialize();
		GameMenuManager.Instance.UpdateBestDistance();
		GameMenuManager.Instance.SetLevelResolution();
		MissionManager.Instance.LoadStatus();
	}

	public void StartLevel()
	{
		StartCoroutine(LevelSpawnManager.Instance.StartToSpawn(1.25f, 3));
		PlayerManager.Instance.ResetStatus(true);
		GameMenuManager.Instance.ShowStartPowerUps();
		GameMenuManager.Instance.ActivateMainGUI();
	}

	public void PauseGame()
	{
		PlayerManager.Instance.DisableControls();
		LevelSpawnManager.Instance.Pause();
	}

	public void ResumeGame()
	{
		PlayerManager.Instance.EnableControls();
		LevelSpawnManager.Instance.Resume();
	}

	public void Revive()
	{
		StartCoroutine(PlayerManager.Instance.Revive());
	}

	public void CoinGathered()
	{
		collectedCoins++;
		MissionManager.Instance.CoinEvent(collectedCoins);
	}

	public int Coins()
	{
		return collectedCoins;
	}

	public void Restart()
	{
		collectedCoins = 0;

		LevelSpawnManager.Instance.Restart(true);
		PlayerManager.Instance.ResetStatus(true);
		MissionManager.Instance.Save();

		GameMenuManager.Instance.ShowStartPowerUps();
		GameMenuManager.Instance.ActivateMainGUI();
		GameMenuManager.Instance.UpdateBestDistance();
	}

	public void QuitToMain()
	{
		LevelSpawnManager.Instance.Restart(false);
		PlayerManager.Instance.ResetStatus(false);
		MissionManager.Instance.Save();

		GameMenuManager.Instance.DeactivateMainGUI();
		GameMenuManager.Instance.ActivateMainMenu();
		GameMenuManager.Instance.UpdateBestDistance();
	}
}
