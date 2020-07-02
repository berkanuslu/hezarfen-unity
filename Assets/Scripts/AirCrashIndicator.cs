using UnityEngine;
using System.Collections;

public class AirCrashIndicator : MonoBehaviour
{
	public float speed = 5.0f;
	public float distance = 1.0f;

	float offset = 0.0f;

	float originalPos = 0;
	Vector3 nextPos = new Vector3();

	bool paused = false;

	void OnEnable()
	{
		originalPos = this.transform.position.y;
		paused = false;
	}

	void Update()
	{
		if (!paused)
		{
			offset = (1 + Mathf.Sin(Time.time * speed)) * distance / 2.0f;

			nextPos = this.transform.position;
			nextPos.y = originalPos + offset;

			this.transform.position = nextPos;
		}
	}

	public void Pause()
	{
		paused = true;
	}

	public void Resume()
	{
		paused = false;
	}
}
