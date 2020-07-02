using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObstacleManager : MonoBehaviour
{
	public List<GameObject> elements = new List<GameObject>();

	void Start()
	{
		foreach (Transform child in transform)
		{
			if (child.name != "SpawnTriggerer" && child.name != "ResetTriggerer")
			{
				elements.Add(child.gameObject);
				child.gameObject.SetActive(false);
			}
		}
	}

	public void DeactivateChild()
	{
		foreach (GameObject child in elements)
		{
			Transform go = child.transform.Find("HideParticleEffect");

			if (go != null)
				go.GetComponent<ParticleSystem>().Stop();

			child.SetActive(false);
		}
	}

	public void ActivateChild()
	{
		foreach (GameObject child in elements)
		{
			child.SetActive(true);

			child.GetComponent<Renderer>().enabled = true;
			child.GetComponent<Collider>().enabled = true;
		}
	}
}
