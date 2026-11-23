using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game Instance { get; private set; }

    public int userId;
    public string username;
    public int coins;
    public string token;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}