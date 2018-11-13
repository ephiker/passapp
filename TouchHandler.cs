using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchHandler : MonoBehaviour
{
	private const float inchToCm = 2.54f;

	[SerializeField]
	private EventSystem eventSystem = null;

	[SerializeField]
	private float dragThresholdCM = 0.5f;
	// private float dragThresholdCM;
	//For drag Threshold

	private void SetDragThreshold ()
	{
		if (eventSystem != null) {
			eventSystem.pixelDragThreshold = (int)(dragThresholdCM * Screen.dpi / inchToCm);
		}
	}


	void Awake ()
	{
//		dragThresholdCM = 1f;
		SetDragThreshold ();
	}
}
