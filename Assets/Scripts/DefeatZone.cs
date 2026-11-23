using UnityEngine;

public class DefeatZone : MonoBehaviour
{
    [SerializeField] private LevelManager levelManager;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Enemy>() != null)
        {
            levelManager.Lose();
        }
    }
}