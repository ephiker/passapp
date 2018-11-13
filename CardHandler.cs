using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardHandler : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler, IBeginDragHandler {

    public SVHandler SV;
    public MyHandler MY;
    public RectTransform RT_Maintext, RT_Subtext, RT_Subsbtn, RT_Graphic, RT_Redbg, RT_Shadow, RT_Card1, RT_Contents, RT_Card1Body, RT_Mask, RT_CardHead, RT_CardTail, RT_TargetContents;
    public CanvasGroup CG_Menubar, CG_Shadow, CG_Card1Body, CG_SubsText1, CG_SubsText2;
    public Button BT_Card1, BT_Xbutton, BT_Sub;
    public Image IMG_SubsButton;
    Vector2 OrigPos_Maintext, OrigPos_Subtext, OrigPos_Subsbtn, OrigPos_Graphic, OrigPos_Redbg, OrigPos_Shadow;
    float TargetPosX_Redbg = 0, TargetPosX_Graphic = 201f, TargetPosX_Subsbtn = 679f, TargetPosX_Subtext = 72f, TargetPosX_Maintext = 72f;
    float TargetPosY_Redbg = 0, TargetPosY_Graphic = -421f, TargetPosY_Subsbtn = -1010f, TargetPosY_Subtext = -926f, TargetPosY_Maintext = -76f;

    float OpenDura = 0.5f;
    float CloseDura = 0.1f;
    Ease OpenEase = Ease.OutQuart;
    public GameObject Card1Detail;
    GameObject TargetContents;
    Color Col_Subs;
    public bool IsCardOpenable = true;

    void Awake () {
        Application.targetFrameRate = 60;
    }
    bool pulldown = true;
    void Update () {
        if (pulldown) {
            if (RT_TargetContents.anchoredPosition.y < -300f) {
                Invoke ("CloseCard1", 0.15f);
                pulldown = false;
            }
        }

        if (SV.PG.IsTweening) {
            BT_Card1.enabled = false;
            BT_Xbutton.enabled = false;
        }
    }

    void Start () {
        Col_Subs = new Color (152f / 255f, 3f / 255f, 20f / 255f);
        BT_Card1.onClick.AddListener (() => {
            if (IsCardOpenable) {
                OpenCard1 ();
            }
        });
        BT_Xbutton.onClick.AddListener (() => {
            if (!IsCardOpenable) {
                CloseCard1 ();
            }
        });
        BT_Sub.onClick.AddListener (() => { Subscribe (); });
        OrigPos_Maintext = new Vector2 (RT_Maintext.anchoredPosition.x, RT_Maintext.anchoredPosition.y);
        OrigPos_Subtext = new Vector2 (RT_Subtext.anchoredPosition.x, RT_Subtext.anchoredPosition.y);
        OrigPos_Subsbtn = new Vector2 (RT_Subsbtn.anchoredPosition.x, RT_Subsbtn.anchoredPosition.y);
        OrigPos_Graphic = new Vector2 (RT_Graphic.anchoredPosition.x, RT_Graphic.anchoredPosition.y);
        OrigPos_Redbg = new Vector2 (RT_Redbg.anchoredPosition.x, RT_Redbg.anchoredPosition.y);
        OrigPos_Shadow = new Vector2 (RT_Shadow.anchoredPosition.x, RT_Shadow.anchoredPosition.y);
    }
    float SubsDura = 0.5f;
    public void Subscribe () {
        IMG_SubsButton.DOColor (Col_Subs, SubsDura);
        MY.PageDown2 ();
        CG_SubsText1.DOFade (0f, SubsDura - 0.3f);
        CG_SubsText2.DOFade (1f, SubsDura - 0.3f);
    }

    public void OpenCard1 () {
        SV.BarHide ();
        pulldown = true;
        IsCardOpenable = false;
        RT_CardHead.DOAnchorPosY (1000f, OpenDura);
        RT_CardTail.DOAnchorPosY (-6500f, OpenDura);

        RT_Mask.DOSizeDelta (new Vector2 (1080f, 1200f + 4382f), OpenDura).SetEase (OpenEase);
        RT_Card1Body.DOScale (Vector3.one, OpenDura).SetEase (OpenEase);
        RT_Card1Body.DOAnchorPosY (-1200f, OpenDura).SetEase (OpenEase);
        CG_Card1Body.DOFade (1f, OpenDura).SetEase (OpenEase);

        RT_Redbg.DOScaleX (1.3f, OpenDura).SetEase (OpenEase);
        RT_Shadow.DOScaleX (1.3f, OpenDura).SetEase (OpenEase);
        CG_Shadow.DOFade (0f, OpenDura).SetEase (OpenEase);
        // RT_Redbg.DOAnchorPos (new Vector2 ((TargetPosX_Redbg), (-RT_Contents.anchoredPosition.y - RT_Card1.anchoredPosition.y - OrigPos_Redbg.y - TargetPosY_Redbg)), OpenDura);
        //다 튀어나와서 TargetContents의 자식으로 들어간다.
        Card1Detail.SetActive (true);
        TargetContents = GameObject.Find ("TargetContents");

        RT_Redbg.transform.SetParent (TargetContents.transform);
        RT_Redbg.DOAnchorPos (new Vector2 (TargetPosX_Redbg, TargetPosY_Redbg), OpenDura).SetEase (OpenEase);
        RT_Maintext.transform.SetParent (TargetContents.transform);
        RT_Maintext.DOAnchorPos (new Vector2 (TargetPosX_Maintext, TargetPosY_Maintext), OpenDura).SetEase (OpenEase);
        RT_Subtext.transform.SetParent (TargetContents.transform);
        RT_Subtext.DOAnchorPos (new Vector2 (TargetPosX_Subtext, TargetPosY_Subtext), OpenDura).SetEase (OpenEase);
        RT_Subsbtn.transform.SetParent (TargetContents.transform);
        RT_Subsbtn.DOAnchorPos (new Vector2 (TargetPosX_Subsbtn, TargetPosY_Subsbtn), OpenDura).SetEase (OpenEase);
        RT_Graphic.transform.SetParent (TargetContents.transform);
        RT_Graphic.DOAnchorPos (new Vector2 (TargetPosX_Graphic, TargetPosY_Graphic), OpenDura).SetEase (OpenEase).OnComplete (() => {
            //X버튼 등장
            BT_Xbutton.gameObject.SetActive (true);
            RT_Card1Body.DOAnchorPosY (-1200f, OpenDura).SetEase (OpenEase);
            RT_Card1Body.transform.SetParent (TargetContents.transform);
            CG_Menubar.alpha = 0f;
        });
        print ("CARD1 OPEN !!!!!");
    }

    public void CloseCard1 () {
        CG_Menubar.alpha = 1f;
        BT_Xbutton.gameObject.SetActive (true);
        CG_Card1Body.DOFade (0f, 0.15f).SetEase (OpenEase).OnComplete (() => {
            // RT_Card1Body.DOAnchorPosY (-500f, 0.05f).SetEase (OpenEase);
            RT_Card1Body.DOScale (new Vector3 (0.86f, 0.86f, 0.86f), 0f).SetEase (OpenEase);
            Card1Detail.SetActive (false);
            SV.BarShow ();
        });

        TargetContents = GameObject.Find ("Card4");
        RT_Redbg.transform.SetParent (TargetContents.transform);
        RT_Redbg.DOAnchorPos (OrigPos_Redbg, OpenDura).SetEase (OpenEase);
        RT_Maintext.transform.SetParent (TargetContents.transform);
        RT_Maintext.DOAnchorPos (OrigPos_Maintext, OpenDura).SetEase (OpenEase);
        RT_Subtext.transform.SetParent (TargetContents.transform);
        RT_Subtext.DOAnchorPos (OrigPos_Subtext, OpenDura).SetEase (OpenEase);
        RT_Subsbtn.transform.SetParent (TargetContents.transform);
        RT_Subsbtn.DOAnchorPos (OrigPos_Subsbtn, OpenDura).SetEase (OpenEase);
        RT_Graphic.transform.SetParent (TargetContents.transform);
        RT_Graphic.DOAnchorPos (OrigPos_Graphic, OpenDura).SetEase (OpenEase);
        RT_Redbg.DOScaleX (1f, OpenDura).SetEase (OpenEase).OnComplete (() => {
            RT_Shadow.DOScaleX (1f, OpenDura).SetEase (OpenEase);
            CG_Shadow.DOFade (1f, OpenDura).SetEase (OpenEase);
            RT_TargetContents.DOAnchorPosY (0f, 0);

        });
        //
        RT_Mask.DOSizeDelta (new Vector2 (931f, 1200f), OpenDura).SetEase (OpenEase);

        //

        RT_CardHead.DOAnchorPosY (-275f, 0.2f);
        RT_CardTail.DOAnchorPosY (-5230f, 0.2f);
        IsCardOpenable = true;

    }

    public void OnBeginDrag (PointerEventData e) {
        SV.OnBeginDrag (e);

    }

    public void OnDrag (PointerEventData e) {
        SV.OnDrag (e);
        if (Mathf.Abs (e.pressPosition.x - e.position.x) > 5) {
            // Debug.Log ("Disable Button");
            BT_Card1.enabled = false;
        }
        if (Mathf.Abs (e.pressPosition.y - e.position.y) > 5) {
            // Debug.Log ("Disable Button");
            BT_Card1.enabled = false;
        }
    }

    public void OnPointerDown (PointerEventData e) {
        SV.OnPointerDown (e);

    }

    public void OnPointerUp (PointerEventData e) {
        SV.OnPointerUp (e);
        Invoke ("ButtonReset", 0.5f);
    }
    void ButtonReset () {
        BT_Card1.enabled = true;
    }

}