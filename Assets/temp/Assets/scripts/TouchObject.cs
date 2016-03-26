using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections;

public class TouchObject : MonoBehaviour {

	public Vector2 startPos;
	public Vector2 virtPos;
	public GameObject target;
	public ButtonHandler bh;

	public void setPosition(Vector2 pos){
		virtPos = pos + (pos - startPos);
		transform.position = Camera.main.ScreenToWorldPoint(virtPos);
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.name == "Button_hitbox"){
			bh = other.gameObject.GetComponent<ButtonHandler>();
			if (bh)
				bh.SetDownState();
			target = other.gameObject;
		}
	}
	
	void OnTriggerExit2D(Collider2D other) {
		if (other.gameObject.name == "Button_hitbox"){
			if (bh)
				bh.SetUpState();
			target = null;
			bh = null;
		}
	}

	public void remove(){
		if (bh)
			bh.SetUpState();
		GameObject.Destroy(gameObject);
	}

}
