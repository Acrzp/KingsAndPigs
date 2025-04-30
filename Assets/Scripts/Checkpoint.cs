using System;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private static readonly int IsActive = Animator.StringToHash("isActive");
    [SerializeField] private Animator animator;
    [SerializeField] private bool isActive;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isActive) return;
        if (collision.CompareTag("Player")) ActiveCheckpoint();
    }

    private void ActiveCheckpoint()
    {
        isActive = true;
        animator.SetTrigger(IsActive);
    }
}
