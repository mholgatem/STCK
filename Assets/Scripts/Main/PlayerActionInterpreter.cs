using UnityEngine;
using System.Collections;


/// <summary>
/// Handle specific keypresses and user events.
/// All menus should register with EscapePressed event/
/// </summary>
public class PlayerActionInterpreter : MonoBehaviour {

	public static bool menuIsOpen = false;
	public static bool editMode = true;

	public Vector3 localScaleStart = Vector3.zero;

	public delegate void UserAction();
	public static event UserAction EscapePressed;

	void Start(){
		DontDestroyOnLoad(this);
	}

	void LoadLobby(UserResponse action){
		if (action == UserResponse.OK){
			Client.currentInstance.gameObject.SetActive(false);
			Application.LoadLevel("Lobby");
		}
	}

	void QuitPrompt(UserResponse action){
		if (action == UserResponse.OK){
			Debug.Log ("User Exit");
			Application.Quit();
		}
	}

	void HandleButtons(){
		if (Input.GetKeyDown(KeyCode.Escape)){
			EscapePressed();
			if (!menuIsOpen){
				if (Application.loadedLevel > 0){
					MessageBox.Show ("Disconnect", "Do you want to disconnect from the server?", new UserResponse[] {UserResponse.OK, UserResponse.Cancel}, LoadLobby);
				}else{
					MessageBox.Show ("Quit", "Are you sure that you want to quit?", new UserResponse[] {UserResponse.OK, UserResponse.Cancel}, QuitPrompt);
				}
			}else{
				menuIsOpen = false;
			}
		}
	}

	void CheckTouchActions(){
		foreach(TouchInstance t in Input.touches.CheckRayHit2D()){
			if (t.raycastHit2D.transform != null){
				switch (t.action){
				case TouchHandler.actions.Drag:
					t.raycastHit2D.transform.position = Camera.main.ScreenToWorldPoint(t.currentPos);
					//use this to avoid accidentally swiping
					t.overrideAction = TouchHandler.actions.Drag;
					break;
				case TouchHandler.actions.Tap:
					if (t.tapCount == 2){
						Destroy(t.raycastHit2D.transform.gameObject);
					}
					break;
				case TouchHandler.actions.LongPress:
					//Debug.Log ("LongPress");
					//t.raycastHit2D.transform.Rotate(Vector3.down * Time.deltaTime * 90);
					break;
				case TouchHandler.actions.Swipe:
					break;
				}
			}
		}
	}

	void HandleTouches(){
		if (Application.loadedLevel == 1){
			if (editMode){
				// PINCHING / SCALING
				if (Input.touches.IsPinching()){
					RaycastHit2D hit;
					if (Input.touches.GetPinchRayHit2D(out hit)){
						if (Input.touches.GetPinchPhase() == TouchPhase.Began){
							localScaleStart = hit.transform.localScale;
						}
						float scaleSize = Mathf.Clamp(localScaleStart.x * Input.touches.GetPinchRatio(), 1f, 10);
						hit.transform.localScale = Vector3.one * scaleSize;
					}

				//Two-Finger Swipe / OPEN MENU
				}else if (Input.touches.GetDoubleSwipeDirection() != TouchHandler.directions.None)
				{
					if(Input.touches.GetDoubleSwipeDirection() == TouchHandler.directions.Right)
					{
						//Camera.main.transform.Rotate(Vector3.up * Time.deltaTime * 90);
					}else{
						//Camera.main.transform.Rotate(Vector3.down * Time.deltaTime * 90);
					}
					
				//All Other Actions
				}else{
					CheckTouchActions();
				}
			}

		}
	}

	void Update () {
		// HANDLE SPECIFIC KEYS
		HandleButtons();
		HandleTouches ();
	}
}
