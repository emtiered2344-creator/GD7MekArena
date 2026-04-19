using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public UIManager uiManager;

    public GameObject player1;
    public float player1Health;
    public GameObject player2;
    public float player2Health;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;

        }
        else
        {
            Destroy(gameObject);
        }

    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        healthUpdate();
    }

    void healthUpdate()
    {
        if (player1 != null)
        {
            player1Health = player1.GetComponent<Character>().health;
            if(player1Health <= 0)
            {
                uiManager.ply2Win();
            }
        }
        else
        {
            player1Health = 0;
        }

        if (player2 != null)
        {
            player2Health = player2.GetComponent<Character>().health;
            if(player2Health <= 0)
            {
                uiManager.ply1Win();
            }
        }
        else
        {
            player2Health = 0;
        }
    }
}
