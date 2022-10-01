using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class InspirationObject : Interactable
{
    public GameObject interactUI;
    public GameObject videoPanel;
    public VideoClip videoClip;
    public GameObject firstPersonController;
    public static float maxVideoAlpha = 0.85f; 

    private VideoPlayer videoPlayer;
    private GameObject idleFX;
    private GameObject focusFX;
    private Camera cutsceneCamera;
    private bool isFocused = false;
    private FirstPersonMovement movementController;
    private FirstPersonLook lookController;
    private float videoAlpha = 0f;
    private float increment = 0.01f;

    public virtual void Awake() 
    {
        gameObject.layer = 6;

        interactUI.SetActive(false);

        videoPlayer = videoPanel.GetComponent<VideoPlayer>();
        videoPlayer.playOnAwake = false;
        videoPanel.SetActive(false);

        idleFX = GameObject.Find("Idle fx");
        focusFX = GameObject.Find("Focus fx");
        cutsceneCamera = GetComponentInChildren<Camera>();
        
        movementController = firstPersonController.GetComponent<FirstPersonMovement>();
        lookController = firstPersonController.GetComponentInChildren<FirstPersonLook>();
    }

    public virtual void Start() 
    {
        videoPlayer.loopPointReached += OnVideoEnd;
        //StopFocusFX();
        focusFX.SetActive(false);
    }

    public override void OnInteract() 
    {
        print("INTERACTED WITH " + gameObject.name);

        movementController.canMove = false;
        videoPlayer.clip = videoClip;
        lookController.EnterCutscene(cutsceneCamera);
        interactUI.SetActive(false);

        StartCoroutine(PlayVideoRoutine());
    }
    
    public override void OnFocus()
    {
        if (!isFocused) 
        {
            print("LOOKING AT " + gameObject.name);

            isFocused = true;
            interactUI.SetActive(true);
            StartFocusFX();
        }
    }

    public override void OnLoseFocus() 
    {
        if (isFocused)
        {
            print("STOPPED LOOKING AT " + gameObject.name);

            isFocused = false;
            interactUI.SetActive(false);
            StopFocusFX();
        }
    }

    void OnVideoEnd(UnityEngine.Video.VideoPlayer vp)
    {
        print("VIDEO ENDED");

        StartCoroutine(ExitVideoRoutine());
    }

    public void StartFocusFX() 
    {
        focusFX.SetActive(true);
        ParticleSystem particleSystem = focusFX.GetComponent<ParticleSystem>(); 
        print("START FOCUS FX " + particleSystem);
        particleSystem.Play(true);
        //focusFX.GetComponentInChildren<Light>().range = 6;
    }

    public void StopFocusFX() 
    {
        ParticleSystem particleSystem = focusFX.GetComponent<ParticleSystem>(); 
        print("STOP FOCUS FX " + particleSystem);
        particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        //focusFX.GetComponentInChildren<Light>().range = 0;
    }

    IEnumerator PlayVideoRoutine()
    {
        RawImage videoTexture = videoPanel.GetComponent<RawImage>();
        videoTexture.color = Color.clear;
        
        yield return new WaitForSeconds(2);

        videoPanel.SetActive(true);
        videoPlayer.Play();
        videoAlpha = 0f;

        while(videoAlpha < maxVideoAlpha)
        {
            videoTexture.color = new Color(1, 1, 1, videoAlpha);
            videoAlpha += increment;
            yield return new WaitForSeconds(increment);
        }
    }

    IEnumerator ExitVideoRoutine()
    {
        RawImage videoTexture = videoPanel.GetComponent<RawImage>();
        
        while(videoAlpha > 0)
        {
            videoTexture.color = new Color(1, 1, 1, videoAlpha);
            videoAlpha -= increment;
            yield return new WaitForSeconds(increment);
        }
        
        yield return new WaitForSeconds(2);

        videoPanel.gameObject.SetActive(false);
        movementController.canMove = true;
        lookController.ExitCutscene();
    }
}
