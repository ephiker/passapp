using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SVHandler : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler, IBeginDragHandler {
	public CardHandler CD;
	public PageHandler PG;
	public ScrollRect SR;
	public MyHandler MY;
	public GameObject GO_Overview, GO_Card1Detail, GO_Loading;
	public RectTransform RT_Menubar, RT_SVContents, RT_My2Contents, RT_My3Contents;
	public CanvasGroup CG_MenubarBG, CG_MenubarBG2;
	float currentY, movedY, pressedY;
	public bool dragUp;
	Ease BarEase = Ease.OutQuart;
	bool IsChosen = false;
	public bool isBarFixed = true;
	public float BarThreshold = 10f;
	public float BarbgThreshold = 50f;

	void Awake () {
		Application.targetFrameRate = 60;
	}

	public void OnPointerDown (PointerEventData e) {
		if (!GO_Overview.activeSelf && !GO_Card1Detail.activeSelf && !GO_Loading.activeSelf) {
			if (!PG.IsTweening) {
				SR.OnBeginDrag (e);
				PG.OnPointerDown (e);
			}
		}

		// CD.OnPointerDown (e);
	}

	// 컨텐츠 페이지를 정방향으로 드래그 시킬 때는 사라지고, 역방향으로 드래그 시키면 나타나는 하단바 구현
	// 역방향이라고 바로 나타나도록 하면 안되고, 델타를 계산하여 드래그에 정도가 일정 이상이면 나타나도록 함.

	public void OnBeginDrag (PointerEventData e) {

		if (!GO_Overview.activeSelf && !GO_Card1Detail.activeSelf && !GO_Loading.activeSelf) {
			//델타 체크 해서 세로가 가로보다 크면 가로 드래그를 막는다, 가로가 세로보다 크면 
			if (!PG.IsTweening) {
				if (!IsChosen) {
					if (Mathf.Abs (e.delta.y) > Mathf.Abs (e.delta.x)) {
						//세로가 우세할 때 
						PG.IsPagingable = false;
					} else {
						//가로가 우세할 때 
						isBarFixed = true;
						SR.vertical = false;
					}
					IsChosen = true;
				}
			}
		}
	}

	//팝업이 켜져있을 때에는 아래 레이어로 연결되지 않도록 한다.
	public void OnDrag (PointerEventData e) {
		if (!GO_Overview.activeSelf && !GO_Card1Detail.activeSelf && !GO_Loading.activeSelf) {
			if (!PG.IsTweening) {
				SR.OnDrag (e);
				PG.OnDrag (e);

				// CD.OnDrag (e);
				currentY = e.position.y; //현재 포지션 추출
				movedY = currentY - pressedY; //총 이동거리 추출
				//print (e.delta.y);

				if (!isBarFixed) {
					//dragUp
					if (e.delta.y > BarThreshold) {
						if (!dragUp) { dragUp = true; BarHide (); }
					}
					//dragDown
					if (e.delta.y < -BarThreshold) {
						if (dragUp) { dragUp = false; BarShow (); }
					}
				}
			}
		}

	}

	public void OnPointerUp (PointerEventData e) {
		if (!GO_Overview.activeSelf && !GO_Card1Detail.activeSelf && !GO_Loading.activeSelf) {
			if (!PG.IsTweening) {
				SR.OnEndDrag (e);
				PG.OnPointerUp (e);
				PG.OnEndDrag (e);

				// CD.OnPointerDown (e);
				PG.IsPagingable = true;
				SR.vertical = true;
				IsChosen = false;
			}
		}
	}

	// Use this for initialization
	void Start () {

	}

	// 특정 양수 이하일 때 
	void Update () {
		if (PG.cur == 0) {
			CG_MenubarBG.alpha = 0f;
			CG_MenubarBG2.alpha = 1f;
		}

		if (PG.cur == 1 && MY.myStage == 1) {
			CG_MenubarBG.alpha = 0f;
			CG_MenubarBG2.alpha = 1f;
		}

		if (PG.cur == 2) {
			if (!isBarFixed) {
				if (RT_SVContents.anchoredPosition.y < BarbgThreshold) {
					CG_MenubarBG.alpha = 0f;
					CG_MenubarBG2.alpha = 1f;
				} else if (RT_SVContents.anchoredPosition.y > BarbgThreshold) {
					CG_MenubarBG.alpha = 1f;
					CG_MenubarBG2.alpha = 0f;
				}
			}
		}

		if (PG.cur == 1 && MY.myStage == 2) {
			if (RT_My2Contents.anchoredPosition.y < BarbgThreshold) {
				CG_MenubarBG.alpha = 0f;
				CG_MenubarBG2.alpha = 1f;
			} else if (RT_My2Contents.anchoredPosition.y > BarbgThreshold) {
				CG_MenubarBG.alpha = 1f;
				CG_MenubarBG2.alpha = 0f;
			}
		}

		if (PG.cur == 1 && MY.myStage == 3) {
			if (RT_My3Contents.anchoredPosition.y < BarbgThreshold) {
				CG_MenubarBG.alpha = 0f;
				CG_MenubarBG2.alpha = 1f;
			} else if (RT_My3Contents.anchoredPosition.y > BarbgThreshold) {
				CG_MenubarBG.alpha = 1f;
				CG_MenubarBG2.alpha = 0f;
			}
		}
	}

	public void BarHide () {
		RT_Menubar.DOKill ();
		RT_Menubar.DOAnchorPos (new Vector2 (0, RT_Menubar.rect.height), 0.2f).SetEase (BarEase);
	}

	public void BarShow () {
		RT_Menubar.DOKill ();
		RT_Menubar.DOAnchorPos (new Vector2 (0, 0), 0.2f).SetEase (BarEase);
	}

}