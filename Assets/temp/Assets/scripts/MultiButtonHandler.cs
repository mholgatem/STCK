using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;


public class trackingData {

	public GameObject touchPrefab;
	public GameObject startTouchPrefab;
	public TouchObject script;
	public TouchObject startScript;

	public Vector2 startPos;
	public Vector2 virtPos;
	public float distance;
	public GameObject _virt;
	public Transform _parent;
	public GameObject virt;
	public GameObject target;

	public trackingData(Vector2 pos, GameObject v, Transform p){
		_parent = p;
		_virt = v;
		reset (pos);
	}

	public void reset(Vector2 pos){
		touchPrefab = GameObject.Instantiate(_virt);
		startTouchPrefab = GameObject.Instantiate(_virt);
		
		touchPrefab.transform.SetParent(_parent, false);
		startTouchPrefab.transform.SetParent(_parent, false);
		
		script = touchPrefab.GetComponent<TouchObject>();
		startScript = startTouchPrefab.GetComponent<TouchObject>();
		
		script.startPos = pos;
		startScript.startPos = pos;
		
		script.setPosition(pos);
		startScript.setPosition (pos);
	}

	public void setPosition(Vector2 pos){
		script.setPosition(pos);
	}

	public void remove(){
		script.remove();
		startScript.remove();
	}
	
}

public class MultiButtonHandler : MonoBehaviour {


	public List<trackingData> touchList = new List<trackingData>();
	public GameObject virt;

	void Start(){
		//touchList = new List<trackingData>(){new trackingData(Vector2.zero, virt, transform)};
		Input.multiTouchEnabled = true;
	}

	void Update(){
		foreach(Touch t in Input.touches){
			if (touchList.Count > t.fingerId){
				if (t.phase == TouchPhase.Began){
					touchList[t.fingerId].reset (t.position);
				}else if (t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary){
					touchList[t.fingerId].setPosition(t.position);
				}else if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled){
					touchList[t.fingerId].remove();
				}
			}else{
				touchList.Add(new trackingData(t.position, virt, transform));
			}
		}
	}
	void onMouseDown(){

	}

}
