using UnityEngine;
using System.Collections;

public class TrailEffect : MonoBehaviour
{
	void Update()
	{
		Vector3 targetPos = this.transform.position;
		targetPos.y = PlayerManager.Instance.gameObject.transform.position.y;
		targetPos.x = PlayerManager.Instance.gameObject.transform.position.x - 10;

		this.transform.position = targetPos;
	}
}
