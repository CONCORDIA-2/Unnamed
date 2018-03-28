using UnityEngine;

public class ScoreCarrier : MonoBehaviour
{
    public bool isHost;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
