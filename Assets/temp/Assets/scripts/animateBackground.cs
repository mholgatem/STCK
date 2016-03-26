using UnityEngine;
using System.Collections;

public class animateBackground : MonoBehaviour {

	public float rotateSpeed = 1f;

	private Color startColor;

	// Use this for initialization
	void Start () {

	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(Vector3.right * Time.deltaTime * rotateSpeed);
		transform.Rotate(Vector3.up * Time.deltaTime * rotateSpeed, Space.World);
		transform.localScale = new Vector3(27f + Mathf.PingPong(Time.time * (rotateSpeed/2), 3), transform.localScale.y, transform.localScale.z);
	}
}
