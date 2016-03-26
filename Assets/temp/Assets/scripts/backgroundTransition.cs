using UnityEngine;
using System.Collections;

public class backgroundTransition : MonoBehaviour {
	
	public float speed = 0f;
	private float currentSpeed = 0f;

	void Update () {
		//parallax moving
		if (currentSpeed < speed)
			currentSpeed = Mathf.Lerp(currentSpeed, speed, Time.deltaTime);
		GetComponent<Renderer>().material.mainTextureOffset -= (Vector2.down * ((Time.deltaTime * currentSpeed) % 1)); //new Vector2 (0f, (Time.deltaTime * speed)%1); 
		
	}
	
	
}
