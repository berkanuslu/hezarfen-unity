using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PowerUpManager : MonoBehaviour
{
	public float verticalSpeed = 5.0f;
	public float verticalDistance = 1.0f;

	public float horizontalSpeed = 0;

	List<PowerUp> inactive = new List<PowerUp>();
	List<PowerUp> activated = new List<PowerUp>();

	bool canSpawnRevive = true;

	void Start()
	{
		foreach (Transform child in transform)
		{
			inactive.Add(child.GetComponent<PowerUp>());
		}
	}

	PowerUp FindCompatiblePowerUp()
	{
		int n = 0;
		if (!canSpawnRevive)
		{
			PowerUp powerUp = null;

			do
			{
				n = Random.Range(0, inactive.Count);
				powerUp = inactive[n];
			} while (powerUp.name == "Revive");

			return powerUp;
		}
		else
		{
			n = Random.Range(0, inactive.Count);
			return inactive[n];
		}
	}

	public void SpawnPowerUp(float multiplyValue)
	{
		PowerUp powerUp = FindCompatiblePowerUp();
		inactive.Remove(powerUp);
		Vector3 newPos = powerUp.transform.position;
		newPos.y = Random.Range(-5, 11);
		powerUp.transform.position = newPos;
		activated.Add(powerUp);
		powerUp.Setup(verticalSpeed, verticalDistance, horizontalSpeed * multiplyValue);
	}

	public void ResetPowerUp(PowerUp sender)
	{
		sender.DisableTrail();
		activated.Remove(sender);
		inactive.Add(sender);
	}

	public void DisableReviveGeneration()
	{
		canSpawnRevive = false;
	}

	public void ResetAll()
	{
		canSpawnRevive = true;

		gameObject.BroadcastMessage("ResetThis");
	}

	public void PauseAll()
	{
		this.gameObject.BroadcastMessage("Pause");
	}

	public void ResumeAll()
	{
		this.gameObject.BroadcastMessage("Resume");
	}
}
