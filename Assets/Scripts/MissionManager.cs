using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MissionManager : MonoBehaviour
{
	public Mission[] missions;

	static MissionManager _instance;
	static int instances = 0;

	int[] activeMissionIDs = new int[3];
	bool[] activeMissionComplete = new bool[3];
	string data = "";

	public static MissionManager Instance
	{
		get
		{
			if (_instance == null)
				_instance = FindObjectOfType(typeof(MissionManager)) as MissionManager;

			return _instance;
		}
	}

	void Start()
	{
		instances++;

		if (instances > 1)
			Debug.LogWarning("Warning: There are more than one Mission Manager at the level");
		else
			_instance = this;
	}

	public void LoadStatus()
	{
		data = PreferencesManager.Instance.GetMissionData();

		if (data.Length > missions.Length)
			ResetDataString();
		else if (data.Length < missions.Length)
			UpdateDataString();

		activeMissionIDs[0] = PreferencesManager.Instance.GetMission1();
		activeMissionIDs[1] = PreferencesManager.Instance.GetMission2();
		activeMissionIDs[2] = PreferencesManager.Instance.GetMission3();

		for (int i = 0; i < 3; i++)
		{
			if (activeMissionIDs[i] == -1)
				GetNewMission(i);
		}

		for (int i = 0; i < 3; i++)
		{
			if (activeMissionIDs[i] != -1)
				activeMissionComplete[i] = false;
			else
				activeMissionComplete[i] = true;
		}

		if (!activeMissionComplete[0] && missions[activeMissionIDs[0]].goalType != Mission.GoalType.InOneRun)
			missions[activeMissionIDs[0]].SetStoredValue(PreferencesManager.Instance.GetMission1Data());

		if (!activeMissionComplete[1] && missions[activeMissionIDs[1]].goalType != Mission.GoalType.InOneRun)
			missions[activeMissionIDs[1]].SetStoredValue(PreferencesManager.Instance.GetMission2Data());

		if (!activeMissionComplete[2] && missions[activeMissionIDs[2]].goalType != Mission.GoalType.InOneRun)
			missions[activeMissionIDs[2]].SetStoredValue(PreferencesManager.Instance.GetMission3Data());

		UpdateGUITexts();
	}

	public void Save()
	{
		for (int i = 0; i < 3; i++)
		{
			if (activeMissionComplete[i] && activeMissionIDs[i] != -1)
			{
				char[] newData = data.ToCharArray();
				newData[activeMissionIDs[i]] = '1';
				data = new string(newData);

				if (i == 0)
					PreferencesManager.Instance.SetMission1Data(0);
				else if (i == 1)
					PreferencesManager.Instance.SetMission2Data(0);
				else
					PreferencesManager.Instance.SetMission3Data(0);

				PreferencesManager.Instance.SetMissionData(data);

				GetNewMission(i);
			}
			else if (!activeMissionComplete[i])
			{
				Mission mission = missions[activeMissionIDs[i]];

				if (mission.goalType == Mission.GoalType.InMultipleRun || mission.goalType == Mission.GoalType.InShop)
				{
					if (i == 0)
						PreferencesManager.Instance.SetMission1Data(mission.StoredValue());
					else if (i == 1)
						PreferencesManager.Instance.SetMission2Data(mission.StoredValue());
					else
						PreferencesManager.Instance.SetMission3Data(mission.StoredValue());
				}
			}
		}

		UpdateGUITexts();
	}

	public void DistanceEvent(int number)
	{
		Mission mission;

		for (int i = 0; i < 3; i++)
		{
			if (!activeMissionComplete[i])
			{
				mission = missions[activeMissionIDs[i]];

				if (mission.missionType == Mission.MissionType.Distance)
					CheckDistanceIn(mission, number, i);
				else if (mission.missionType == Mission.MissionType.DistanceWithNoCoins)
					CheckDistanceNoCoin(mission, number, i);
				else if (mission.missionType == Mission.MissionType.DistanceWithNoPowerUps)
					CheckDistanceNoPowerUp(mission, number, i);
			}
		}
	}

	public void FailEvent(int number)
	{
		Mission mission;

		for (int i = 0; i < 3; i++)
		{
			if (!activeMissionComplete[i])
			{
				mission = missions[activeMissionIDs[i]];

				if (mission.missionType == Mission.MissionType.FailBetween)
					CheckFailBetween(mission, number, i);
			}
		}
	}

	public void CoinEvent(int number)
	{
		Mission mission;

		for (int i = 0; i < 3; i++)
		{
			if (!activeMissionComplete[i])
			{
				mission = missions[activeMissionIDs[i]];

				if (mission.missionType == Mission.MissionType.Coin)
					CheckCoinIn(mission, number, i);
			}
		}
	}

	public void PowerUpEvent(string name)
	{
		Mission mission;

		for (int i = 0; i < 3; i++)
		{
			if (!activeMissionComplete[i])
			{
				mission = missions[activeMissionIDs[i]];

				if (mission.missionType == Mission.MissionType.PowerUps)
					CheckPowerUpBased(mission, i);
				else if (mission.missionType == Mission.MissionType.ExtraSpeed && name == "ExtraSpeed")
					CheckPowerUpBased(mission, i);
				else if (mission.missionType == Mission.MissionType.Shield && name == "Shield")
					CheckPowerUpBased(mission, i);
				else if (mission.missionType == Mission.MissionType.Abracadabra && name == "Abracadabra")
					CheckPowerUpBased(mission, i);
				else if (mission.missionType == Mission.MissionType.Revive && name == "Revive")
					CheckPowerUpBased(mission, i);
			}
		}
	}

	public void ObstacleEvent(string name)
	{
		Mission mission;

		for (int i = 0; i < 3; i++)
		{
			if (!activeMissionComplete[i])
			{
				mission = missions[activeMissionIDs[i]];

				if (mission.missionType == Mission.MissionType.Obstacles)
					CheckObstacleBased(mission, i);
				else if (mission.missionType == Mission.MissionType.BirdBrown && name == "BirdBrown")
					CheckObstacleBased(mission, i);
				else if (mission.missionType == Mission.MissionType.BirdWhite && name == "BirdWhite")
					CheckObstacleBased(mission, i);
				else if (mission.missionType == Mission.MissionType.BirdBody && name == "BirdBody")
					CheckObstacleBased(mission, i);
				else if (mission.missionType == Mission.MissionType.StorkTall && name == "StorkTall")
					CheckObstacleBased(mission, i);
			}
		}
	}

	public void ShopEvent(string name)
	{
		Mission mission;

		for (int i = 0; i < 3; i++)
		{
			if (!activeMissionComplete[i])
			{
				mission = missions[activeMissionIDs[i]];

				if (mission.goalType == Mission.GoalType.InShop)
				{
					if (mission.missionType == Mission.MissionType.PowerUps)
						CheckShopBased(mission, i);
					else if (mission.missionType == Mission.MissionType.ExtraSpeed && name == "ExtraSpeed")
						CheckShopBased(mission, i);
					else if (mission.missionType == Mission.MissionType.Shield && name == "Shield")
						CheckShopBased(mission, i);
					else if (mission.missionType == Mission.MissionType.Abracadabra && name == "Abracadabra")
						CheckShopBased(mission, i);
					else if (mission.missionType == Mission.MissionType.Revive && name == "Revive")
						CheckShopBased(mission, i);
				}
			}
		}
	}

	void CheckDistanceIn(Mission mission, int number, int i)
	{
		if (mission.goalType == Mission.GoalType.InOneRun)
		{
			GameMenuManager.Instance.UpdateMissionStatus(i, number, mission.valueA);

			if (mission.valueA <= number)
				MissionCompleted(mission, i);
		}
		else if (mission.goalType == Mission.GoalType.InMultipleRun)
		{
			mission.ModifyStoredValue(false, number);
			GameMenuManager.Instance.UpdateMissionStatus(i, mission.StoredValue(), mission.valueA);

			if (mission.valueA <= mission.StoredValue())
				MissionCompleted(mission, i);
		}
	}

	void CheckDistanceNoCoin(Mission mission, int number, int i)
	{
		if (mission.goalType == Mission.GoalType.InOneRun)
		{
			GameMenuManager.Instance.UpdateMissionStatus(i, number, mission.valueA);

			if (mission.valueA <= number && LevelManager.Instance.Coins() == 0)
				MissionCompleted(mission, i);
		}
	}

	void CheckDistanceNoPowerUp(Mission mission, int number, int i)
	{
		if (mission.goalType == Mission.GoalType.InOneRun)
		{
			GameMenuManager.Instance.UpdateMissionStatus(i, number, mission.valueA);

			if (mission.valueA <= number && !PlayerManager.Instance.PowerUpUsed())
				MissionCompleted(mission, i);
		}
	}

	void CheckFailBetween(Mission mission, int number, int i)
	{
		if (mission.goalType == Mission.GoalType.InOneRun)
		{
			GameMenuManager.Instance.UpdateMissionStatus(i, number, mission.valueA);

			if (mission.valueA <= number && number <= mission.valueB)
				MissionCompleted(mission, i);
		}
	}

	void CheckCoinIn(Mission mission, int number, int i)
	{
		if (mission.goalType == Mission.GoalType.InOneRun)
		{
			GameMenuManager.Instance.UpdateMissionStatus(i, number, mission.valueA);

			if (mission.valueA <= number)
				MissionCompleted(mission, i);
		}
		else if (mission.goalType == Mission.GoalType.InMultipleRun)
		{
			mission.ModifyStoredValue(false, number);
			GameMenuManager.Instance.UpdateMissionStatus(i, mission.StoredValue(), mission.valueA);

			if (mission.valueA <= mission.StoredValue())
				MissionCompleted(mission, i);
		}
	}

	void CheckPowerUpBased(Mission mission, int i)
	{
		if (mission.goalType == Mission.GoalType.InOneRun || mission.goalType == Mission.GoalType.InMultipleRun)
		{
			mission.ModifyStoredValue(true, 1);
			GameMenuManager.Instance.UpdateMissionStatus(i, mission.StoredValue(), mission.valueA);

			if (mission.valueA == mission.StoredValue())
				MissionCompleted(mission, i);
		}
	}

	void CheckObstacleBased(Mission mission, int i)
	{
		if (mission.goalType == Mission.GoalType.InOneRun || mission.goalType == Mission.GoalType.InMultipleRun)
		{
			mission.ModifyStoredValue(true, 1);
			GameMenuManager.Instance.UpdateMissionStatus(i, mission.StoredValue(), mission.valueA);

			if (mission.valueA == mission.StoredValue())
				MissionCompleted(mission, i);
		}
	}

	void CheckShopBased(Mission mission, int i)
	{
		mission.ModifyStoredValue(true, 1);
		GameMenuManager.Instance.UpdateMissionStatus(i, mission.StoredValue(), mission.valueA);

		if (mission.valueA == mission.StoredValue())
		{
			MissionCompleted(mission, i);
			Save();
		}
	}

	void MissionCompleted(Mission mission, int missionID)
	{
		activeMissionComplete[missionID] = true;
		EventManager.Instance.SendLevelUp(missionID+1);

		StartCoroutine(GameMenuManager.Instance.ShowMissionComplete(mission.description));
	}

	void GetNewMission(int i)
	{
		for (int j = 0; j < data.Length; j++)
		{
			if (data[j] == '0')
			{
				if (i == 0)
				{
					if (activeMissionIDs[1] != j && activeMissionIDs[2] != j)
					{
						activeMissionIDs[i] = j;
						activeMissionComplete[i] = false;
						PreferencesManager.Instance.SetMission1(j);
						return;
					}
				}
				else if (i == 1)
				{
					if (activeMissionIDs[0] != j && activeMissionIDs[2] != j)
					{
						activeMissionIDs[i] = j;
						activeMissionComplete[i] = false;
						PreferencesManager.Instance.SetMission2(j);
						return;
					}
				}
				else
				{
					if (activeMissionIDs[0] != j && activeMissionIDs[1] != j)
					{
						activeMissionIDs[i] = j;
						activeMissionComplete[i] = false;
						PreferencesManager.Instance.SetMission3(j);
						return;
					}
				}
			}
		}

		activeMissionIDs[i] = -1;

		if (i == 0)
			PreferencesManager.Instance.SetMission1(-1);
		else if (i == 1)
			PreferencesManager.Instance.SetMission2(-1);
		else
			PreferencesManager.Instance.SetMission3(-1);
	}

	void GetNextMissions()
	{
		for (int i = 0; i < 3; i++)
		{
			if (activeMissionComplete[i])
			{
				bool found = false;

				for (int j = 0; j < data.Length; j++)
				{
					if (data[j] == '0')
					{
						if (i == 0)
						{
							if (activeMissionIDs[1] != j && activeMissionIDs[2] != j)
							{
								activeMissionIDs[i] = j;
								PreferencesManager.Instance.SetMission1(j);
							}
						}
						else if (i == 1)
						{
							if (activeMissionIDs[0] != j && activeMissionIDs[2] != j)
							{
								activeMissionIDs[i] = j;
								PreferencesManager.Instance.SetMission2(j);
							}
						}
						else
						{
							if (activeMissionIDs[0] != j && activeMissionIDs[1] != j)
							{
								activeMissionIDs[i] = j;
								PreferencesManager.Instance.SetMission3(j);
							}
						}

						found = true;
					}
				}

				if (!found)
				{
					if (i == 0)
						PreferencesManager.Instance.SetMission1(-1);
					else if (i == 1)
						PreferencesManager.Instance.SetMission2(-1);
					else
						PreferencesManager.Instance.SetMission3(-1);
				}
			}
		}
	}

	void UpdateGUITexts()
	{
		string text1;
		string text2;
		string text3;

		if (!activeMissionComplete[0])
		{
			text1 = missions[activeMissionIDs[0]].description;
			GameMenuManager.Instance.UpdateMissionStatus(0, missions[activeMissionIDs[0]].StoredValue(), missions[activeMissionIDs[0]].valueA);
		}
		else
		{
			text1 = "Completed";
			GameMenuManager.Instance.UpdateMissionStatus(0, 0, 0);
		}

		if (!activeMissionComplete[1])
		{
			text2 = missions[activeMissionIDs[1]].description;
			GameMenuManager.Instance.UpdateMissionStatus(1, missions[activeMissionIDs[1]].StoredValue(), missions[activeMissionIDs[1]].valueA);
		}
		else
		{
			text2 = "Completed";
			GameMenuManager.Instance.UpdateMissionStatus(0, 0, 0);
		}

		if (!activeMissionComplete[2])
		{
			text3 = missions[activeMissionIDs[2]].description;
			GameMenuManager.Instance.UpdateMissionStatus(2, missions[activeMissionIDs[2]].StoredValue(), missions[activeMissionIDs[2]].valueA);
		}
		else
		{
			text3 = "Completed";
			GameMenuManager.Instance.UpdateMissionStatus(0, 0, 0);
		}

		GameMenuManager.Instance.UpdateMissionTexts(text1, text2, text3);
	}

	void UpdateDataString()
	{
		for (int i = data.Length; i < missions.Length; i++)
			data += "0";

		PreferencesManager.Instance.SetMissionData(data);
	}

	public void ResetDataString()
	{
		string s = "";
		for (int i = 0; i < missions.Length; i++)
			s += "0";

		data = s;
		PreferencesManager.Instance.SetMissionData(data);

		ResetMissions();
	}

	void ResetMissions()
	{
		PreferencesManager.Instance.SetMission1(0);
		PreferencesManager.Instance.SetMission2(1);
		PreferencesManager.Instance.SetMission3(2);

		PreferencesManager.Instance.SetMission1Data(0);
		PreferencesManager.Instance.SetMission2Data(0);
		PreferencesManager.Instance.SetMission3Data(0);
	}
}
