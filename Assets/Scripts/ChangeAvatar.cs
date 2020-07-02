using UnityEngine;
using System.Collections;

public class ChangeAvatar : MonoBehaviour
{
	public Sprite startSprite;

	public void change()
	{
		PlayerManager.Instance.horizontalAnimation.GetComponent<SpriteRenderer>().enabled = true;
		transform.gameObject.GetComponent<SpriteRenderer>().enabled = false;
		transform.gameObject.GetComponent<Animator>().StopPlayback();
		transform.gameObject.GetComponent<Animator>().enabled = false;
		//Start the level
		LevelManager.Instance.StartLevel();
	}
}
