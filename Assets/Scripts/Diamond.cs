using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Diamond : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Rigidbody2D mRigidbody_2D;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private DiamondType diamondType;

    //Animator ID
    private int _idPickedDiamond;
    private int _idDiamondIndex;

    private void Awake()
    {
        mRigidbody_2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
        _idPickedDiamond = Animator.StringToHash("pickedDiamond");
        _idDiamondIndex = Animator.StringToHash("diamondIndex");
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
        SetRandomDiamond();
    }

    private void SetRandomDiamond()
    {
        if(!gameManager.DiamondHaveRandomLook)
        {
            UpdateDiamondType();
            return;
        }
        var _randomDiamondIndex = Random.Range(0,5);
        animator.SetFloat(_idDiamondIndex, _randomDiamondIndex);
    }

    private void UpdateDiamondType()
    {
        animator.SetFloat(_idDiamondIndex, (int)diamondType);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(!collision.CompareTag("Player")) return; //Al estar las tags almacenadas en cache se optizima mejor 
        //spriteRenderer.enabled = false;
        mRigidbody_2D.simulated = false;
        gameManager.AddDiamond();
        animator.SetTrigger(_idPickedDiamond);
    }

}
