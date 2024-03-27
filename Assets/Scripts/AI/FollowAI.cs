using System.Collections;
using System.Collections.Generic;
using LeastSquares.Overtone;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

public class FollowAI : MonoBehaviour
{
    [SerializeField] private TTSPlayer ttsPlayer;
    [SerializeField] private NavMeshAgent aiAgent;
    [SerializeField] private Transform player;
    [SerializeField] private Animator aiAnimator;
    [SerializeField] private TextMeshPro chatText;

    private bool isTalking = false;

    private Vector3 destination;

    private void Start()
    {
        ttsPlayer.OnTTSPlayerStart += FollowAI_Talk;
    }

    private void FollowAI_Talk(string text, float phraseLength)
    {
        Debug.Log("Triggering AI talk");

        aiAnimator.ResetTrigger("walking");
        aiAnimator.ResetTrigger("idle");

        aiAnimator.SetTrigger("talking");
        isTalking = true;

        chatText.text = text;

        StartCoroutine(TalkCoroutine(phraseLength));
    }

    private IEnumerator TalkCoroutine(float phraseLength)
    {
        Debug.Log("Waiting on phrase finish");
        yield return new WaitForSeconds(phraseLength);
        isTalking = false;
        chatText.text = "";
        aiAnimator.ResetTrigger("talking");
        aiAnimator.SetTrigger("idle");
        Debug.Log("phrase finished, isTalking: " + isTalking.ToString());
    }

    private void Update()
    {
        if (isTalking)
            return;

        destination = player.position;
        aiAgent.destination = destination;

        if (aiAgent.remainingDistance <= aiAgent.stoppingDistance)
        {
            aiAnimator.ResetTrigger("walking");
            aiAnimator.SetTrigger("idle");
        }
        else
        {
            aiAnimator.ResetTrigger("idle");
            aiAnimator.SetTrigger("walking");
        }
    }
}
