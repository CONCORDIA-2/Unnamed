using UnityEngine;

public class ScoreCarrier : MonoBehaviour
{
    public int score;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
