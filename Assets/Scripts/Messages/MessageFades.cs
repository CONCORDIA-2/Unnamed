using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageFades : MonoBehaviour
{
    private Text messageText;
    public float fadeSpeed = 4f;
    public float visibleTimer;
    public bool isVisible;  //To track if the text is currently visible
    public bool displayed = false;  //To track if the text has been displayed since the camera view began to include it
    public bool singleUse;  //Creepy messages are true, tutorial messages are false
    private GameObject localPlayerManager;
    private LocalPlayerManager localPlayerManagerScript;
    private Camera cameraObject;

    void Start()
    {
        // grab reference to the local player manager if not assigned
        localPlayerManager = GameObject.FindGameObjectWithTag("PlayerManager");
        if (localPlayerManager)
            localPlayerManagerScript = localPlayerManager.GetComponent<LocalPlayerManager>();

        cameraObject = localPlayerManagerScript.GetPlayerCameraObject().GetComponent<Camera>();
        messageText = GetComponent<Text>();
        Color startColor = new Color(messageText.color.r, messageText.color.g, messageText.color.b, 0);
        messageText.color = startColor;
    }


    void Update()
    {
    	if (cameraObject)
    	{
	        if (IsVisibleFrom(this.gameObject, cameraObject) && messageText.color.a <= 0.001 && !displayed)
	        {
	            visibleTimer = 6;
	            StartCoroutine(FadeTextToFullAlpha(fadeSpeed, messageText));
	            displayed = true;
	            isVisible = true;

	        }

	        else if (isVisible && visibleTimer > 0)
	            visibleTimer -= Time.deltaTime;

	        else if (visibleTimer <= 0 && messageText.color.a >= 0.99)  //As a result of this check, the coroutine is called several times(~3) so the fading out is faster than in
	            StartCoroutine(FadeTextToZeroAlpha(fadeSpeed, messageText));

	        else if (displayed && messageText.color.a <= 0)
	            if (singleUse)
	            {
	                UnityEngine.Object.Destroy(this.transform.parent.gameObject);   //Destroys the canvas, and thus also its child (the text itself)
	            }
	            else
	            {
	                isVisible = false;
	            }
	        if (!IsVisibleFrom(this.gameObject, cameraObject))
	            displayed = false;
	    } else
	    {
	    	cameraObject = localPlayerManagerScript.GetPlayerCameraObject().GetComponent<Camera>();
	    }

    }

    public static bool IsVisibleFrom(GameObject ob, Camera camera)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(planes, ob.GetComponent<Collider>().bounds);
    }

    public static IEnumerator FadeTextToFullAlpha(float t, Text i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
        //Debug.Log("Fading in!");
        while (i.color.a < 1.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
            yield return null;
        }
    }

    public static IEnumerator FadeTextToZeroAlpha(float t, Text i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        //Debug.Log("Fading out...");
        while (i.color.a > 0.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
            yield return null;
        }
    }
}
