using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DrawerHandler : MonoBehaviour {

	public Button BT_DrawerButton, BT_DrawerBlack;
	public RectTransform RT_Drawer;
	public CanvasGroup CG_DrawerBlack;
	float OpenDura = 0.5f;
	float CloseDura = 0.25f;
	Ease OpenEase = Ease.InOutQuart;
	bool IsOpenable = true;

	void Start () {
		BT_DrawerButton.onClick.AddListener (() => { DrawerOpen (); });
		BT_DrawerBlack.onClick.AddListener (() => { DrawerClose (); });
	}
	//열자마자 닫아버리는 걸 막는게 아니라, 진행중인 트윈을 멈추고 닫아야 한다. 
	void DrawerOpen () {
		if (IsOpenable) {
			RT_Drawer.DOAnchorPosX (0f, OpenDura).SetEase (OpenEase);
			CG_DrawerBlack.DOFade (0.5f, OpenDura).SetEase (OpenEase);
			CG_DrawerBlack.blocksRaycasts = true;
			IsOpenable = false;
		}
	}
	void DrawerClose () {
		DOTween.KillAll ();
		CG_DrawerBlack.DOFade (0f, CloseDura).SetEase (OpenEase);
		RT_Drawer.DOAnchorPosX (-700f, CloseDura).SetEase (OpenEase);
		CG_DrawerBlack.blocksRaycasts = false;
		IsOpenable = true;
	}
}