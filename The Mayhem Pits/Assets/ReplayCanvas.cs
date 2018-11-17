using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class ReplayCanvas : MonoBehaviour {

    private ReplayPlayer player;
    private Animator animator;

    private bool isBeginning = false;
    private bool isEnding = false;

    private float currentBeginCounter = 0.0f;
    private float currentEndCounter = 0.0f;

    [SerializeField] private VideoPlayer video;

	// Use this for initialization
	void Start () {
        animator = GetComponentInChildren<Animator>();
        player = GetComponent<ReplayPlayer>();
	}

    private void Update()
    {
        CheckStates();
    }

    private void CheckStates()
    {
        if (isBeginning)
        {
            currentBeginCounter += Time.deltaTime;
            if (currentBeginCounter >= 3.0f)
            {
                OnAnimationEnd_Begin();
                currentBeginCounter = 0.0f;
            }
        }

        if (isEnding)
        {
            currentEndCounter += Time.deltaTime;
            if (currentEndCounter >= 0.5f)
            {
                OnAnimationEnd_End();
                currentEndCounter = 0.0f;
            }
        }
    }

    public void Begin()
    {
        if (isBeginning) return;

        isBeginning = true;
        animator.SetBool("Begin", true);
        video.Play ();
        //animator.ResetTrigger("Begin");
    }

    public void End()
    {
        if (isEnding) return;

        Debug.Log("End");

        isEnding = true;
        animator.SetBool("End", true);
    }    

    private void OnAnimationEnd_Begin()
    {
        isBeginning = false;
        player.introIsPlaying = false;
        animator.SetBool("Begin", false);
    }

    private void OnAnimationEnd_End()
    {
        isEnding = false;
        player.Finish();

        animator.SetBool("End", false);
    }
}
