using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

public class FollowAI : MonoBehaviour
{
    [SerializeField] private SendTextToGPT gpt;
    [SerializeField] private NavMeshAgent aiAgent;
    [SerializeField] private Transform player;
    [SerializeField] private Animator aiAnimator;

    private bool isTalking = false;

    private Vector3 destination;

    private void Start()
    {
        gpt.OnTextChange += FollowAI_Talk;
    }

    private void FollowAI_Talk(string text)
    {
        aiAnimator.ResetTrigger("walking");
        aiAnimator.ResetTrigger("idle");

        aiAnimator.SetTrigger("talking");
        isTalking = true;

        StartCoroutine(TalkCoroutine());
    }

    private IEnumerator TalkCoroutine()
    {
        yield return new WaitForSeconds(3f);
        aiAnimator.ResetTrigger("talking");
        isTalking = false;
    }

    private void Update()
    {
        destination = player.position;
        aiAgent.destination = destination;

        if (isTalking)
            return;

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
