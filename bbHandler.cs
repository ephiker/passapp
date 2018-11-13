using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class bbHandler : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler {

	public BannerHandler ban;

	public void OnPointerDown (PointerEventData e) {
		ban.OnPointerDown (e);
	}

	public void OnDrag (PointerEventData e) {
		if (Mathf.Abs (e.pressPosition.x - e.position.x) > 5) {
			Debug.Log ("Disable Button");
			for (int i = 0; i < ban.listSize; i++) {
				ban.BannerBTN[i].enabled = false;
			}
		}
		ban.OnDrag (e);
	}
	public void OnPointerUp (PointerEventData e) {
		ban.OnPointerUp (e);
		Invoke ("ButtonReset", 0.5f);
		// print ("Is Button Enabled? : " + ban.BannerBTN[0].enabled);
	}

	void ButtonReset () {
		for (int i = 0; i < ban.listSize; i++) {
			ban.BannerBTN[i].enabled = true;
		}
	}

}