using UnityEngine.Networking;

public class CharacterMsg : MessageBase
{
    public const short id = 6674;

    // 0 = rabbit, 1 = raven
    public bool isHost;
}