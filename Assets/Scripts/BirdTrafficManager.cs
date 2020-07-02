using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BirdTrafficManager : MonoBehaviour
{
	public float movementSpeed = 0;

	List<BirdTraffic> inactive = new List<BirdTraffic>();
	List<BirdTraffic> activated = new List<BirdTraffic>();

	void Start()
	{
		foreach (Transform child in transform)
		{
			inactive.Add(child.GetComponent<BirdTraffic>());
		}
	}

	public void LaunchBird()
	{
		BirdTraffic t = inactive[0];

		inactive.Remove(t);
		activated.Add(t);

		t.Launch(movementSpeed);
	}

	public void ResetBird(BirdTraffic sender)
	{
		activated.Remove(sender);
		inactive.Add(sender);
	}

	public void ResetAll()
	{
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
