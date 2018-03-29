using UnityEngine;

// much help from this thread:
// https://stackoverflow.com/questions/22015697/how-to-keep-2-objects-in-view-at-all-time-by-scaling-the-field-of-view-or-zy

public class CameraControl : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform otherPlayer;
    [SerializeField] private float frontFactor = 0.5f;
    [SerializeField] private float smoothingFactor = 2.0f;
    [SerializeField] private float playerBias = 0.75f;

    private Vector3 camToPlayer;
    private Vector3 targetCamPos;

    public void SetCameraTarget(Transform target)
    {
        player = target;

        // lock camera on player's x-axis
        transform.position = new Vector3(player.position.x, transform.position.y, transform.position.z);

        // record distance between the camera and the player
        camToPlayer = transform.position - player.position;
    }

    void Update()
    {
        if (player)
        {
            // if the other player is in game, get the middle point between the two players

            // position the camera at the local player's position
            targetCamPos = player.position + camToPlayer;

            // place the camera slightly ahead of the player
            Vector3 playerFront = player.forward.normalized;
            targetCamPos = new Vector3(targetCamPos.x + frontFactor * playerFront.x,
                targetCamPos.y + 2f,
                targetCamPos.z + frontFactor * playerFront.z);

            // if the other player is in game, move the camera to include their character
            // TODO

            // move the camera to its new position
            transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothingFactor * Time.deltaTime);
        }
    }
}
