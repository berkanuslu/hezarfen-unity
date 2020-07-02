using UnityEngine;
using System.Collections;

public class LevelTrigger : MonoBehaviour
{
	void OnTriggerEnter(Collider other)
	{
		if (other.name == "SpawnTriggerer")
		{
			switch (other.tag)
			{
				case "CloudLayer":
					LevelSpawnManager.Instance.SpawnCloudLayer(0);
					break;

				case "CityBackgroundLayer":
					LevelSpawnManager.Instance.SpawnCityBackgroundLayer(0);
					break;

				case "CityForegroundLayer":
					LevelSpawnManager.Instance.SpawnCityForegroundLayer(0);
					break;

				case "Obstacles":
					LevelSpawnManager.Instance.SpawnObstacles();
					break;
			}
		}
		else if (other.name == "ResetTriggerer")
		{
			switch (other.tag)
			{
				case "CloudLayer":
				case "CityBackgroundLayer":
				case "CityForegroundLayer":
					LevelSpawnManager.Instance.PoolGameObject(other.transform.parent.gameObject);
					break;

				case "Obstacles":
					other.transform.parent.GetComponent<ObstacleManager>().DeactivateChild();
					LevelSpawnManager.Instance.PoolGameObject(other.transform.parent.gameObject);
					break;
			}
		}
		else if (other.tag == "PowerUps")
		{
			other.GetComponent<PowerUp>().ResetThis();
		}
		else if (other.name == "BirdBody")
		{
			other.transform.parent.gameObject.GetComponent<BirdTraffic>().ResetThis();
		}
	}
}
