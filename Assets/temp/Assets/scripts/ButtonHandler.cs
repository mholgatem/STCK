using System;
using UnityEngine;

namespace UnityStandardAssets.CrossPlatformInput
{
    public class ButtonHandler : MonoBehaviour
    {

		public string command = "KEY_A";
		public clientControl client;

        private string Name;
		private bool isPressed = false;
		private Transform buttonMesh;
		private float pressAmt = 4f;
		private CanvasRenderer canvas;
		private Color originalColor;
		private int buttonState = 0;
		private float repeatTimer = 0;
		private string commandType = "EV_KEY";

        void Start()
        {
			Name = transform.parent.parent.parent.name + "_" + transform.parent.name;
			buttonMesh = transform.parent.transform;
			canvas = GetComponent<CanvasRenderer>();
			originalColor = canvas.GetColor ();
			client = GameObject.FindWithTag("Client").GetComponent<clientControl>();
			if (!command.StartsWith("KEY_") && !command.StartsWith("BTN_"))
				commandType = "runCommand";
        }

        public void SetDownState()
        {

            CrossPlatformInputManager.SetButtonDown(Name);

			if (!isPressed)
			{
				originalColor = canvas.GetColor ();
				if (canvas.tag != "UIcontrol" && originalColor.a < .5)
				{
					canvas.SetColor(Color.white);
				}
				else
				{
					canvas.SetColor(originalColor/2);
					canvas.SetAlpha(1f);
				}
				Vector3 temp = buttonMesh.localPosition;
				temp.y -= pressAmt;
				buttonMesh.localPosition = temp;
				isPressed = true;
			}
			buttonState = 1;
			client.SendButtonState(command, commandType, buttonState);
			//client.SendCommand(command);
        }

        public void SetUpState()
        {
            CrossPlatformInputManager.SetButtonUp(Name);
			canvas.SetColor(originalColor);

			if (isPressed)
			{
				Vector3 temp = buttonMesh.localPosition;
				temp.y += pressAmt;
				buttonMesh.localPosition = temp;
				isPressed = false;
			}
			buttonState = 0;
			client.SendButtonState(command, commandType, buttonState);
        }
		/*
		void OnTriggerEnter2D(Collider2D other) {
			if (other.gameObject.tag == "virtualTouch")
				SetDownState();
		}
		
		void OnTriggerExit2D(Collider2D other) {
			if (other.gameObject.tag == "virtualTouch")
				SetUpState();
		}
*/
        public void SetAxisPositiveState()
        {
            CrossPlatformInputManager.SetAxisPositive(Name);
        }


        public void SetAxisNeutralState()
        {
            CrossPlatformInputManager.SetAxisZero(Name);
        }


        public void SetAxisNegativeState()
        {
            CrossPlatformInputManager.SetAxisNegative(Name);
        }

        public void Update()
        {
			if (buttonState == 1)
			{
				repeatTimer += Time.deltaTime;
				if (repeatTimer > .3f)
				{
					repeatTimer = 0f;
					buttonState = 2;
				}
			}
			if (buttonState == 2)
				client.SendButtonState(command, commandType,buttonState);

        }
    }
}
