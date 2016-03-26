using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DropDownModifier : MonoBehaviour {


	public InputField InputFieldTarget;
	public Text label;

	public void setInputFieldValue(int selectionNumber){
		if (selectionNumber == 1){
			InputFieldTarget.gameObject.SetActive(true);
			InputFieldTarget.ActivateInputField();
			InputFieldTarget.text = GetComponent<Dropdown>().options[0].text;
			label.enabled = false;
			//gameObject.SetActive(false);
		}
		else if(selectionNumber > 1){
			label.enabled = true;
			string selectionText = GetComponent<Dropdown>().options[selectionNumber].text;
			InputFieldTarget.gameObject.SetActive(false);
			gameObject.SetActive(true);
			InputFieldTarget.text = selectionText;
			GetComponentInChildren<Text>().text = GetComponent<Dropdown>().options[0].text + selectionText;

		}
	}
}
