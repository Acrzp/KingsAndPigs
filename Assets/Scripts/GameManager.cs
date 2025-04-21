using UnityEngine;

public class GameManager : MonoBehaviour
{
   public static GameManager instance; 

   [SerializeField] private PlayerController playerController;
    public PlayerController PlayerController => playerController;

    [SerializeField] private int diamondCollected;
    public int DiamondCollected => diamondCollected;

    [SerializeField] private bool diamondHaveRandomLook;
    public bool DiamondHaveRandomLook => diamondHaveRandomLook; 

    private void Awake()
    {
        if(instance == null) instance = this; //Instanciar a si mismo el GameManager
        else Destroy(gameObject); //Destruir el resto de GameManagers en una escena
    }
    
    public void AddDiamond() => diamondCollected++;
    
}
