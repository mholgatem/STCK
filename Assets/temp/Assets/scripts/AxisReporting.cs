using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class AxisReporting : MonoBehaviour {


	public Client client;

	private string axisH, axisV;
	private Vector2 lastInput = Vector2.zero;
	private float rateTimer;
	private float rate;


	void Start () {
		client = GameObject.FindWithTag("Client").GetComponent<Client>();
		rate = client.rate;
		axisH = transform.parent.name + "-H";
		axisV = transform.parent.name + "-V";
	}


	// Update is called once per frame
	void Update () {
		if (rateTimer > rate){
			Vector2 axis = new Vector2(Mathf.Round(CrossPlatformInputManager.GetAxis(axisH)),Mathf.Round(CrossPlatformInputManager.GetAxis(axisV)));
			if (axis != Vector2.zero || lastInput != Vector2.zero)
			{
				lastInput = axis;
				rateTimer = 0f;
				client.SendAxisState((new Vector2(axis.x,-axis.y)* 32767).ToString());
			}
		}else{
			rateTimer += Time.deltaTime;
		}
		
	}
}
