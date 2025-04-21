using System; //Para la prueba con eventos
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; 
   
    [Header("Player Settings")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform playerRespawnPoint;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private float respawnPlayerDelay;
   
    public PlayerController PlayerController => playerController;

    public static event Action<Transform> OnPlayerRespawned; //Evento de prueba para el seguimiento de la cámara

    [Header("Diamond Manager")]
    [SerializeField] private int diamondCollected;
    [SerializeField] private bool diamondHaveRandomLook;

    public int DiamondCollected => diamondCollected;
    public bool DiamondHaveRandomLook => diamondHaveRandomLook;


    private void Awake()
    {
        if(Instance == null) Instance = this; //Instanciar a si mismo el GameManager
        else Destroy(gameObject); //Destruir el resto de GameManagers en una escena
    }
    
    public void RespawnPlayer() => StartCoroutine(RespawnPlayerCoroutine());

    private IEnumerator RespawnPlayerCoroutine()
    {
        yield return new WaitForSeconds(respawnPlayerDelay);
        GameObject _newPlayer = Instantiate(playerPrefab, playerRespawnPoint.position, Quaternion.identity);
        _newPlayer.name = "Player";
        playerController = _newPlayer.GetComponent<PlayerController>();
        OnPlayerRespawned?.Invoke(_newPlayer.transform); //Disparo de evento de Prueba
    }

    public void AddDiamond() => diamondCollected++;
    
}
