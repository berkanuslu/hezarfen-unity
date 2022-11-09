using UnityEngine;
using System.Collections;

using GameAnalyticsSDK;

public class PreferencesManager : MonoBehaviour
{
	static PreferencesManager _instance;
	static int instances = 0;

	public bool clearDataForTest = false;

	int coinAmmount = 1500;
	int bestDistance = 0;

	int extraSpeed = 0;
	int shield = 0;
	int abracadabra = 0;
	int revive = 0;

	int mission1 = 0;
	int mission2 = 1;
	int mission3 = 2;
	int mission1Data = 0;
	int mission2Data = 0;
	int mission3Data = 0;

	int level = 0;
	int levelStatus = 0;

	string missionData = "";

	float musicVolume = 1.0f;

	public static PreferencesManager Instance
	{
		get
		{
			if (_instance == null)
				_instance = FindObjectOfType(typeof(PreferencesManager)) as PreferencesManager;

			return _instance;
		}
	}

	void Start()
	{
		instances++;

		if (instances > 1)
		{
			Debug.LogWarning("Warning: There are more than one Save Manager at the level");
		}
		else
		{
			_instance = this;
		}

		CreateAndLoadData();
	}

	public void CreateAndLoadData()
	{
		if (clearDataForTest)
		{
			CreateData();
			return;
		}

		if (PlayerPrefs.HasKey("CoinAmmount"))
		{
			LoadData();
		}
		else
		{
			CreateData();
		}
	}

	public void CreateData()
	{
		PlayerPrefs.SetInt("CoinAmmount", coinAmmount);
		PlayerPrefs.SetInt("BestDistance", bestDistance);

		PlayerPrefs.SetInt("ExtraSpeed", extraSpeed);
		PlayerPrefs.SetInt("Shield", shield);
		PlayerPrefs.SetInt("Abracadabra", abracadabra);
		PlayerPrefs.SetInt("Revive", revive);

		PlayerPrefs.SetInt("Mission1", mission1);
		PlayerPrefs.SetInt("Mission2", mission2);
		PlayerPrefs.SetInt("Mission3", mission3);

		PlayerPrefs.SetInt("Mission1Data", mission1Data);
		PlayerPrefs.SetInt("Mission2Data", mission2Data);
		PlayerPrefs.SetInt("Mission3Data", mission3Data);

		PlayerPrefs.SetString("Missions", missionData);
		PlayerPrefs.SetFloat("MusicVolume", musicVolume);

		PlayerPrefs.SetInt("Level", level);
		PlayerPrefs.SetInt("LevelStatus", levelStatus);

		PlayerPrefs.Save();
		LoadData();
	}

	void LoadData()
	{
		coinAmmount = PlayerPrefs.GetInt("CoinAmmount", coinAmmount);
		bestDistance = PlayerPrefs.GetInt("BestDistance", bestDistance);

		extraSpeed = PlayerPrefs.GetInt("ExtraSpeed", extraSpeed);
		shield = PlayerPrefs.GetInt("Shield", shield);
		abracadabra = PlayerPrefs.GetInt("Abracadabra", abracadabra);
		revive = PlayerPrefs.GetInt("Revive", revive);

		mission1 = PlayerPrefs.GetInt("Mission1", mission1);
		mission2 = PlayerPrefs.GetInt("Mission2", mission2);
		mission3 = PlayerPrefs.GetInt("Mission3", mission3);

		mission1Data = PlayerPrefs.GetInt("Mission1Data", mission1Data);
		mission2Data = PlayerPrefs.GetInt("Mission2Data", mission2Data);
		mission3Data = PlayerPrefs.GetInt("Mission3Data", mission3Data);

		missionData = PlayerPrefs.GetString("Missions", missionData);

		musicVolume = PlayerPrefs.GetFloat("MusicVolume", musicVolume);

		level = PlayerPrefs.GetInt("Level", level);
		levelStatus = PlayerPrefs.GetInt("LevelStatus", levelStatus);
	}

	public int GetCoins()
	{
		return coinAmmount;
	}

	public int GetBestDistance()
	{
		return bestDistance;
	}

	public int GetExtraSpeed()
	{
		return extraSpeed;
	}

	public int GetShield()
	{
		return shield;
	}

	public int GetAbracadabra()
	{
		return abracadabra;
	}

	public int GetRevive()
	{
		return revive;
	}

	public int GetMission1()
	{
		return mission1;
	}

	public int GetMission2()
	{
		return mission2;
	}

	public int GetMission3()
	{
		return mission3;
	}

	public int GetMission1Data()
	{
		return mission1Data;
	}

	public int GetMission2Data()
	{
		return mission2Data;
	}

	public int GetMission3Data()
	{
		return mission3Data;
	}

	public string GetMissionData()
	{
		return missionData;
	}

	public float GetMusicVolume()
	{
		return musicVolume;
	}

	public bool GetFirtsOpen()
	{
		if (PlayerPrefs.HasKey("first_open"))
		{
			return true;
		}
		return false;
	}

	public int GetLevel()
	{
		return level;
	}

	public int GetLevelStatus()
	{
		return levelStatus;
	}

	public void SetCoins(int ammount)
	{
		coinAmmount = ammount;
		PlayerPrefs.SetInt("CoinAmmount", ammount);
		PlayerPrefs.Save();
	}

	public void SetBestDistance(int distance)
	{
		bestDistance = distance;
		PlayerPrefs.SetInt("BestDistance", distance);
		PlayerPrefs.Save();
	}

	public void ModifyExtraSpeedBy(int modifyBy)
	{
		extraSpeed += modifyBy;
		PlayerPrefs.SetInt("ExtraSpeed", extraSpeed);
		PlayerPrefs.Save();
	}

	public void ModifyShieldBy(int modifyBy)
	{
		shield += modifyBy;
		PlayerPrefs.SetInt("Shield", shield);
		PlayerPrefs.Save();
	}

	public void ModifyAbracadabraBy(int modifyBy)
	{
		abracadabra += modifyBy;
		PlayerPrefs.SetInt("Abracadabra", abracadabra);
		PlayerPrefs.Save();
	}

	public void ModifyReviveBy(int modifyBy)
	{
		revive += modifyBy;
		PlayerPrefs.SetInt("Revive", revive);
		PlayerPrefs.Save();
	}

	public void SetMission1(int id)
	{
		mission1 = id;
		PlayerPrefs.SetInt("Mission1", id);
		PlayerPrefs.Save();
	}

	public void SetMission2(int id)
	{
		mission2 = id;
		PlayerPrefs.SetInt("Mission2", id);
		PlayerPrefs.Save();
	}

	public void SetMission3(int id)
	{
		mission3 = id;
		PlayerPrefs.SetInt("Mission3", id);
		PlayerPrefs.Save();
	}

	public void SetMission1Data(int id)
	{
		mission1Data = id;
		PlayerPrefs.SetInt("Mission1Data", id);
		PlayerPrefs.Save();
	}

	public void SetMission2Data(int id)
	{
		mission2Data = id;
		PlayerPrefs.SetInt("Mission2Data", id);
		PlayerPrefs.Save();
	}

	public void SetMission3Data(int id)
	{
		mission3Data = id;
		PlayerPrefs.SetInt("Mission3Data", id);
		PlayerPrefs.Save();
	}

	public void SetMissionData(string s)
	{
		missionData = s;
		PlayerPrefs.SetString("Missions", s);
		PlayerPrefs.Save();
	}

	public void SetMusicVolume(float value)
	{
		musicVolume = value;
		PlayerPrefs.SetFloat("MusicVolume", value);
		PlayerPrefs.Save();
	}

	public void SetFirtsOpen()
	{
		if (!PlayerPrefs.HasKey("first_open"))
		{
			EventManager.Instance.SendFirstOpenEvent();
			PlayerPrefs.SetInt("first_open", 1);
			PlayerPrefs.Save();
		}
	}

	public void SetLevel(int newLevel)
	{
		level = newLevel;
		PlayerPrefs.SetInt("Level", newLevel);
		PlayerPrefs.Save();
	}

	public void SetLevelStatus(int newStatus)
	{
		levelStatus = newStatus;
		PlayerPrefs.SetInt("LevelStatus", newStatus);
		PlayerPrefs.Save();
	}
}