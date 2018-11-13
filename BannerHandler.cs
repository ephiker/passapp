using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BannerHandler : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler {

	public bbHandler bb;
	public List<RectTransform> Banner;
	public List<CanvasGroup> BannerCG;
	public List<Button> BannerBTN;
	public Image IMG_Banner;
	public int listSize;

	public float pressedX, movedX, currentX;
	public float threshold;
	public int cur, prev, next;
	public float prevOrg = -1080f, curOrg = 0f, nextOrg = 1080f;
	public float PosTweenDuration = 0.3f;
	public Ease PosTweenEase = Ease.OutQuart;

	void Start () {
		//본 오브젝트 아래에 종속되어 있는 자식들을 요소로 가지는 리스트 만들기
		foreach (Transform child in transform) {
			Banner.Add (child.gameObject.GetComponent<RectTransform> ());
			BannerCG.Add (child.gameObject.GetComponent<CanvasGroup> ());
			BannerBTN.Add (child.gameObject.GetComponent<Button> ());
		}

		for (int i = 0; i < listSize; i++) {
			BannerBTN[i].onClick.AddListener (() => { Debug.Log ("clicked + " + i); });
		}

		BannerBTN[0].onClick.AddListener (() => { Debug.Log ("clicked 0"); });
		BannerBTN[1].onClick.AddListener (() => { Debug.Log ("clicked 1"); });
		BannerBTN[2].onClick.AddListener (() => { Debug.Log ("clicked 2"); });
		BannerBTN[3].onClick.AddListener (() => { Debug.Log ("clicked 3"); });

		listSize = Banner.Count;

		prev = listSize - 1;
		cur = 0;
		next = 1;
	}

	public void OnPointerDown (PointerEventData e) {
		//트윈 중에 다시 터치/드래그가 일어나면 이전 트윈 없애기
		DOTween.KillAll ();
		CancelInvoke ("AlphaOff");
		AlphaOn ();
		//처음 클릭한 포지션 추출
		pressedX = e.pressPosition.x;

	}
	public void OnDrag (PointerEventData e) {
		//움직임 중에는 드래그 안되게 막는게 아니라 새로운 드래그로 자연스럽게 전환되어야 한다.
		currentX = e.position.x; //현재 포지션 추출
		movedX = currentX - pressedX; //총 이동거리 추출
		Banner[prev].anchoredPosition = new Vector2 (prevOrg + movedX, 0);
		Banner[cur].anchoredPosition = new Vector2 (curOrg + movedX, 0);
		Banner[next].anchoredPosition = new Vector2 (nextOrg + movedX, 0);
	}

	public void OnPointerUp (PointerEventData e) {
		// increse, decrease, retain 으로 index 설정 후에 트윈 시켜준다!
		print ("movedX = " + movedX);
		threshold = 300;

		if (movedX < -threshold) {
			//정방향, 넘어감
			prev = IndexPlus (prev);
			cur = IndexPlus (cur);
			next = IndexPlus (next);
		} else if (movedX > threshold) {
			//역방향, 넘어감
			prev = IndexMinus (prev);
			cur = IndexMinus (cur);
			next = IndexMinus (next);
		} else {
			// 넘어가지 못함, 인덱스 체인지 필요 없음.
		}
		//나머지 알파 끄기 
		Invoke ("AlphaOff", 0.1f);
		PosTween ();
		//알파 켜기
		// Invoke ("AlphaOn", PosTweenDuration);
		movedX = 0;
	}

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

	public void PosTween () {
		// IsDragable = false;
		// 트윈 중에는 드래그가 되지 않도록 raycast을 막고, 트윈종료 후에 다시 켜준다.
		Banner[prev].DOAnchorPosX (prevOrg, PosTweenDuration).SetEase (PosTweenEase);
		Banner[cur].DOAnchorPosX (curOrg, PosTweenDuration).SetEase (PosTweenEase);
		Banner[next].DOAnchorPosX (nextOrg, PosTweenDuration).SetEase (PosTweenEase).OnComplete (() => { });
	}

	public void AlphaOff () {
		for (int i = 0; i < listSize; i++) {
			// if (i != prev && i != cur && i != next) {
			if (i != cur) {
				BannerCG[i].alpha = 0;
			} else {
				BannerCG[i].alpha = 1;
			}
		}
	}
	public void AlphaOn () {
		BannerCG[prev].alpha = 1f;
		BannerCG[cur].alpha = 1f;
		BannerCG[next].alpha = 1f;
	}

	void Update () {

	}
}