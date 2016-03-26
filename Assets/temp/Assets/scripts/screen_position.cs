using UnityEngine;
using System.Collections;

public class screen_position : MonoBehaviour {


	public Vector3 position = new Vector3(-1,-1,3);
	// Use this for initialization
	void Start () {
	
	}

	void OnEnable()
	{

		Vector3 screenPoint = new Vector3 (Screen.width / position.x, Screen.height / position.y, position.z);
		
		if ( 0 < position.x && position.x <= 1)
			screenPoint = new Vector3 (Screen.width * position.x, Screen.height * position.y, position.z);
		
		
		Vector3 worldPos = Camera.main.ScreenToWorldPoint ( screenPoint );
		if (screenPoint.x > 0 && screenPoint.y > 0)
			transform.position = worldPos;
	}


	// Update is called once per frame
	void Update () {
	
	}
}
