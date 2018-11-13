using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MyHandler : MonoBehaviour {
	public SVHandler ServiceSVHandler;
	public Button BT_Certi, BT_My3_Overview, BT_My3_Back, BT_Message;
	public CanvasGroup CG_MyPage1, CG_MyPage2, CG_MyPage3, CG_Overview, CG_My3Contents, CG_LoadingPage, CG_Message;
	public GameObject GO_MyPage1, GO_MyPage2, GO_MyPage3, GO_LoadingPage;
	public RectTransform RT_Overview;
	public int myStage = 1;

	public float BarbgThreshold = 50f;

	float pagingDura = 0.3f;
	Ease pagingEase = Ease.InOutQuart;

	void Awake () {
		Application.targetFrameRate = 60;
	}

	// Use this for initialization
	void Start () {
		BT_Certi.onClick.AddListener (() => { CertiLoading (); });
		BT_My3_Overview.onClick.AddListener (() => { ShowOverview (); });
		BT_My3_Back.onClick.AddListener (() => { HideOverview (); });
		BT_Message.onClick.AddListener (() => {
			HideMeassage ();
		});
	}
	void HideMeassage () {
		CG_Message.alpha = 0;
	}

	void ShowOverview () {
		CG_Overview.gameObject.SetActive (true);
		// CG_My3Contents.DOFade (0f, 0.3f).SetEase (pagingEase);
		CG_Overview.DOFade (1f, 0.3f).SetEase (pagingEase);
		RT_Overview.DOAnchorPosX (0f, 0.3f).SetEase (pagingEase);
	}

	void HideOverview () {
		CG_Overview.DOFade (0f, 0.3f).SetEase (pagingEase);
		RT_Overview.DOAnchorPosX (1080f, 0.3f).SetEase (pagingEase).OnComplete (() => {
			CG_Overview.gameObject.SetActive (false);
		});
	}

	void CertiLoading () {
		GO_LoadingPage.SetActive (true);
		CG_LoadingPage.DOFade (1f, 0.3f);
		Invoke ("PageDown1", 1f);
	}

	void PageDown1 () {
		myStage = 2;
		CG_LoadingPage.DOFade (0f, 0.3f);
		CG_MyPage1.DOFade (0f, pagingDura).SetEase (pagingEase).OnComplete (() => {
			GO_LoadingPage.SetActive (false);
			GO_MyPage1.SetActive (false);
		});
		GO_MyPage2.SetActive (true);
		CG_MyPage2.DOFade (1f, pagingDura).SetEase (pagingEase);
		// ServiceSVHandler.isBarFixed = false;
		ShowMessage ();
	}
	bool MessageOnce = true;
	void ShowMessage () {
		if (MessageOnce) {
			CG_Message.DOFade (1f, 0.1f).SetDelay (0.8f);
			MessageOnce = false;
		}
	}

	public void PageDown2 () {
		myStage = 3;
		CG_MyPage1.DOFade (0f, 0);
		CG_MyPage2.DOFade (0f, 0).OnComplete (() => {
			GO_MyPage1.SetActive (false);
			GO_MyPage2.SetActive (false);
		});
		GO_MyPage3.SetActive (true);
		CG_MyPage3.DOFade (1f, 0);
		ServiceSVHandler.isBarFixed = false;
		// ServiceSVHandler.isBarFixed = false;
	}
}