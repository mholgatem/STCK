using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class WheelReporting : MonoBehaviour {


	public clientControl client;
	public Toggle freezeTiltInput;
	public GameObject controlRig;

	private Vector2 lastInput = Vector2.zero;
	private float rateTimer;
	private float rate;
	
	void Start () {
		client = GameObject.FindWithTag("Client").GetComponent<clientControl>();
		rate = client.rate;
	}

	public void toggleUpdate(bool toggleValue){
		controlRig.SetActive(!toggleValue);
	}


	// Update is called once per frame
	void Update () {
		if (!freezeTiltInput.isOn)
		{
			if (rateTimer > rate){
				Vector2 axis = new Vector2(Mathf.Round(CrossPlatformInputManager.GetAxis("Wheel-H")),Mathf.Round(CrossPlatformInputManager.GetAxis("Wheel-V")));
				if (axis != Vector2.zero || lastInput != Vector2.zero)
				{
					lastInput = axis;
					rateTimer = 0f;
					client.SendAxisState((axis * 32767).ToString());
				}
			}else{
				rateTimer += Time.deltaTime;
			}

		}
	}
}
