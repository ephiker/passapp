using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PageHandler : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler, IEndDragHandler {
	public SVHandler ServiceSVHandler;
	public CardHandler CD;
	public RectTransform RT_Page1, RT_Page2, RT_Page3;
	public List<RectTransform> RT_Page;
	public CanvasGroup CG_Page1, CG_Page2, CG_Page3;
	public List<CanvasGroup> CG_Page;
	public Button BT_Page1, BT_Page2, BT_Page3, BT_Reset;
	public GameObject GO_Underline;
	public Sprite SP_Underline_1, SP_Underline_2, SP_Underline_3;
	float Underline_width_1, Underline_width_2, Underline_width_3;
	float Underline_posx_1 = 64f, Underline_posx_2 = 256f, Underline_posx_3 = 522f;

	public float prevOrg = -1080f, curOrg = 0f, nextOrg = 1080f;
	float pagingDura = 0.3f;
	Ease pagingEase = Ease.InOutQuart;
	float curX_page1, curX_page2, curX_page3, curY;
	public int listSize = 3;
	public float threshold;
	public int cur, prev, next;
	public float PosTweenDuration = 0.3f;
	public Ease PosTweenEase = Ease.OutQuart;
	public bool IsPagingable = true;
	bool IsIndexPlus = true;

	public float UnderliningDura = 0.3f;
	public Ease UnderliningEase = Ease.InOutQuart;

	void Awake () {
		Application.targetFrameRate = 60;
	}

	void Start () {
		foreach (Transform child in transform) {
			RT_Page.Add (child.gameObject.GetComponent<RectTransform> ());
			CG_Page.Add (child.gameObject.GetComponent<CanvasGroup> ());
		}

		BT_Reset.onClick.AddListener (ResetScene);

		//add btn
		BT_Page1.onClick.AddListener (() => { LoadPage1 (); });
		BT_Page2.onClick.AddListener (() => { LoadPage2 (); });
		BT_Page3.onClick.AddListener (() => { LoadPage3 (); });
		// position
		curX_page1 = RT_Page1.anchoredPosition.x;
		curX_page2 = RT_Page2.anchoredPosition.x;
		curX_page3 = RT_Page3.anchoredPosition.x;
		curY = RT_Page1.anchoredPosition.y;

		//
		Underline_width_1 = SP_Underline_1.rect.width;
		Underline_width_2 = SP_Underline_2.rect.width;
		Underline_width_3 = SP_Underline_3.rect.width;

		prev = 2;
		cur = 0;
		next = 1;
	}

	float pressedX;
	float movedX;
	float currentX;

	public void OnPointerDown (PointerEventData e) {
		DOTween.KillAll ();
		CancelInvoke ("AlphaOff");
		AlphaOn ();
		//
		pressedX = e.pressPosition.x; //처음 클릭한 포지션 추출
	}

	public void OnDrag (PointerEventData e) {
		//움직임 중에 또 다른 입력이 있을 때는, 그 드래그를 안되게 막는게 아니라 새로운 드래그로 자연스럽게 전환되어야 한다.
		currentX = e.position.x; //현재 포지션 추출
		movedX = currentX - pressedX; //총 이동거리 추출
		// print ("movedX : " + movedX);

		if (IsPagingable) {
			RT_Page[prev].anchoredPosition = new Vector2 (prevOrg + movedX, curY);
			RT_Page[cur].anchoredPosition = new Vector2 (curOrg + movedX, curY);
			RT_Page[next].anchoredPosition = new Vector2 (nextOrg + movedX, curY);
		}

	}

	public void OnPointerUp (PointerEventData e) {
		threshold = 540f;
		float snapThreshold = 3.5f;

		if (IsPagingable) {
			if (movedX < -threshold || (e.delta.x) < -snapThreshold) {
				//정방향, 넘어감
				prev = IndexPlus (prev);
				cur = IndexPlus (cur);
				next = IndexPlus (next);
				IsIndexPlus = true;
			} else if (movedX > threshold || (e.delta.x) > snapThreshold) {
				//역방향, 넘어감
				prev = IndexMinus (prev);
				cur = IndexMinus (cur);
				next = IndexMinus (next);
				IsIndexPlus = false;
			} else {
				// 넘어가지 못함, 인덱스 체인지 필요 없음.
			}

			//나머지 알파 끄기 
			AlphaOff ();
			PosTween ();
			//알파 켜기
			movedX = 0;

			if (cur == 1) {
				if (ServiceSVHandler.MY.myStage == 3 || ServiceSVHandler.MY.myStage == 2) {
					ServiceSVHandler.isBarFixed = false;
				} else {
					ServiceSVHandler.isBarFixed = true;
				}
			}

			//3번에서만 바 왔다갔다 할 수 있음
			if (cur == 2) {
				ServiceSVHandler.isBarFixed = false;
			} else {
				ServiceSVHandler.isBarFixed = true;
			}
		}
		// ServiceSVHandler.dragUp = true;
	}

	public void OnEndDrag (PointerEventData e) { }

	public int IndexPlus (int target) {
		//정방향일 때, 마지막 카드에서 넘길때 0번이 나옴
		if (target + 1 == listSize) {
			target = 0;
		} else {
			target += 1;
		}
		return target;
	}
	public int IndexMinus (int target) {
		//역방향일 때, 첫번째 카드에서 오른쪽으로 넘길 때
		if (target - 1 == -1) {
			target = listSize - 1;
		} else {
			target -= 1;
		}
		return target;
	}
	public bool IsTweening = false;
	public void PosTween () {
		//버튼막기
		// CD.BT_Card1.enabled = false;
		// CD.BT_Xbutton.enabled = false;
		//드래그 막기
		IsTweening = true;

		// IsDragable = false;
		// 트윈 중에는 드래그가 되지 않도록 raycast을 막고, 트윈종료 후에 다시 켜준다.
		RT_Page[prev].DOAnchorPosX (prevOrg, PosTweenDuration).SetEase (PosTweenEase);
		RT_Page[cur].DOAnchorPosX (curOrg, PosTweenDuration).SetEase (PosTweenEase);
		RT_Page[next].DOAnchorPosX (nextOrg, PosTweenDuration).SetEase (PosTweenEase).OnComplete (() => {
			IsTweening = false;
			CD.BT_Card1.enabled = true;
			CD.BT_Xbutton.enabled = true;
		});
		//한번 넣어본다.
		ServiceSVHandler.BarShow ();
		// 언더라인!
		DoUnderline ();

	}

	public void AlphaOff () {
		CG_Page[cur].alpha = 1f;
		if (IsIndexPlus) {
			CG_Page[next].alpha = 0f;
		} else if (!IsIndexPlus) {
			CG_Page[prev].alpha = 0f;
		}

	}
	public void AlphaOn () {
		CG_Page[prev].alpha = 1f;
		CG_Page[cur].alpha = 1f;
		CG_Page[next].alpha = 1f;
	}

	//============================= 탭하여 전환 ================================//

	void LoadPage1 () {
		if (cur != 0) {
			//빠르게 클릭 했을 때 오류를 막기 위한 코딩 필요
			DOTween.KillAll ();

			//2와 3 dofade 0.3 & 2,3 move to  thier each position
			CG_Page2.DOFade (0f, pagingDura).SetEase (pagingEase).OnComplete (() => {
				RT_Page2.DOAnchorPosX (nextOrg, 0);
				CG_Page2.DOFade (1f, 0f).SetDelay (0.1f);
			});
			CG_Page3.DOFade (0f, pagingDura).SetEase (pagingEase).OnComplete (() => {
				RT_Page3.DOAnchorPosX (prevOrg, 0);
				CG_Page3.DOFade (1f, 0f).SetDelay (0.1f);
			});

			//1 alpha = 0 and move to position 0 & 1 do fade to 1 0.2
			CG_Page1.alpha = 0;
			RT_Page1.DOAnchorPosX (0, 0).SetDelay (0.2f).OnComplete (() => {
				CG_Page1.DOFade (1f, 0).SetEase (pagingEase);
			});

			prev = 2;
			cur = 0;
			next = 1;
			//언더라인
			DoUnderline ();
			ServiceSVHandler.isBarFixed = true;
		}

	}

	void LoadPage2 () {
		if (cur != 1) {
			//빠르게 클릭 했을 때 오류를 막기 위한 코딩
			DOTween.KillAll ();

			//1와 3 dofade 0.3 & 1,3 move to  thier each position
			CG_Page1.DOFade (0f, pagingDura).SetEase (pagingEase).OnComplete (() => {
				RT_Page1.DOAnchorPosX (prevOrg, 0);
				CG_Page1.DOFade (1f, 0f).SetDelay (0.1f);
			});
			CG_Page3.DOFade (0f, pagingDura).SetEase (pagingEase).OnComplete (() => {
				RT_Page3.DOAnchorPosX (nextOrg, 0);
				CG_Page3.DOFade (1f, 0f).SetDelay (0.1f);
			});

			//1 alpha = 0 and move to position 0 & 1 do fade to 1 0.2
			CG_Page2.alpha = 0;
			RT_Page2.DOAnchorPosX (0, 0).SetDelay (0.2f).OnComplete (() => {
				CG_Page2.DOFade (1f, 0).SetEase (pagingEase);
			});

			prev = 0;
			cur = 1;
			next = 2;
			//언더라인
			DoUnderline ();

			if (ServiceSVHandler.MY.myStage == 3 || ServiceSVHandler.MY.myStage == 2) {
				ServiceSVHandler.isBarFixed = false;
			} else {
				ServiceSVHandler.isBarFixed = true;
			}
		}
	}

	void LoadPage3 () {
		if (cur != 2) {
			//빠르게 클릭 했을 때 오류를 막기 위한 코딩
			DOTween.KillAll ();

			//1와 3 dofade 0.3 & 1,3 move to  thier each position
			CG_Page1.DOFade (0f, pagingDura).SetEase (pagingEase).OnComplete (() => {
				RT_Page1.DOAnchorPosX (nextOrg, 0);
				CG_Page1.DOFade (1f, 0f).SetDelay (0.1f);
			});
			CG_Page2.DOFade (0f, pagingDura).SetEase (pagingEase).OnComplete (() => {
				RT_Page2.DOAnchorPosX (prevOrg, 0);
				CG_Page2.DOFade (1f, 0f).SetDelay (0.1f);
			});

			//1 alpha = 0 and move to position 0 & 1 do fade to 1 0.2
			CG_Page3.alpha = 0;
			RT_Page3.DOAnchorPosX (0, 0).SetDelay (0.2f).OnComplete (() => {
				CG_Page3.DOFade (1f, 0).SetEase (pagingEase);
			});

			prev = 1;
			cur = 2;
			next = 0;
			//언더라인
			DoUnderline ();
			ServiceSVHandler.isBarFixed = false;
		}
	}

	void DoUnderline () {
		if (cur == 0) {
			Underlining1 ();
		} else if (cur == 1) {
			Underlining2 ();
		} else if (cur == 2) {
			Underlining3 ();
		}
	}

	void Underlining1 () {
		// //타겟 찾아가기
		// GameObject _place = GameObject.Find ("text1");
		// //자식으로 들어가기
		// GO_Underline.transform.SetParent (_place.transform);
		//리소스 변경 
		GO_Underline.GetComponent<Image> ().sprite = SP_Underline_1;
		//포지션 트윈
		GO_Underline.GetComponent<RectTransform> ().DOAnchorPosX (Underline_posx_1, UnderliningDura).SetEase (UnderliningEase);
		//width 변경
		GO_Underline.GetComponent<RectTransform> ().DOSizeDelta (new Vector2 (Underline_width_1, 15f), UnderliningDura - 0.1f).SetEase (UnderliningEase);
		// //타겟 찾아가기
		// 	GameObject _place2 = GameObject.Find ("button1");
		// 	//첫자식으로 들어가기
		// 	GO_Underline.transform.SetParent (_place2.transform);
		// 	GO_Underline.transform.SetAsFirstSibling ();	

	}

	void Underlining2 () {
		// //타겟 찾아가기
		// GameObject _place = GameObject.Find ("text2");
		// //자식으로 들어가기
		// GO_Underline.transform.SetParent (_place.transform);
		//리소스 변경 
		GO_Underline.GetComponent<Image> ().sprite = SP_Underline_2;
		//포지션 트윈
		GO_Underline.GetComponent<RectTransform> ().DOAnchorPosX (Underline_posx_2, UnderliningDura).SetEase (UnderliningEase);
		//width 변경
		GO_Underline.GetComponent<RectTransform> ().DOSizeDelta (new Vector2 (Underline_width_2, 15f), UnderliningDura).SetEase (UnderliningEase);
		//타겟 찾아가기
		// _place = GameObject.Find ("button2");
		// //자식으로 들어가기
		// GO_Underline.transform.SetParent (_place.transform);
	}

	void Underlining3 () {
		// //타겟 찾아가기
		// GameObject _place = GameObject.Find ("text3");
		// //자식으로 들어가기
		// GO_Underline.transform.SetParent (_place.transform);
		//리소스 변경 
		GO_Underline.GetComponent<Image> ().sprite = SP_Underline_3;
		//포지션 트윈
		GO_Underline.GetComponent<RectTransform> ().DOAnchorPosX (Underline_posx_3, UnderliningDura).SetEase (UnderliningEase);
		//width 변경
		GO_Underline.GetComponent<RectTransform> ().DOSizeDelta (new Vector2 (Underline_width_3, 15f), UnderliningDura).SetEase (UnderliningEase);
		//타겟 찾아가기
		// _place = GameObject.Find ("button3");
		// //자식으로 들어가기
		// GO_Underline.transform.SetParent (_place.transform);
		// GO_Underline.transform.SetAsLastSibling ();
	}

	void ResetScene () {
		Application.LoadLevel (Application.loadedLevel);
	}
}