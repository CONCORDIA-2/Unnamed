using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLighting : MonoBehaviour {

	public const float outlineTransitionSpeed = 0.01f;
	public Vector3 ravenPartnerOutlineColor = new Vector3(0.0f, 0.0f, 0.0f);
	public Vector3 rabbitPartnerOutlineColor = new Vector3(1.0f, 1.0f, 1.0f);
	public const float maxWidth = 0.2f;
	public bool specialLighting = false;

	public bool isRaven = true;

	private float outlineWidth = 0.0f;
	private Material material;

	void Start () {
		material = GetComponent<Renderer>().material;
	}
	
	void Update () {
		//if gameobject is under special lighting conditions
		//render outline, and increase width, else decrease width
		if (specialLighting) {
			if (outlineWidth < 0.7f) outlineWidth += outlineTransitionSpeed;
		} else if (outlineWidth > 0.0f) outlineWidth -= outlineTransitionSpeed;

		//get client, and render outline color
		if (isRaven) {
			material.SetVector("_OutlineColor", new Vector4(ravenPartnerOutlineColor.x, ravenPartnerOutlineColor.y, ravenPartnerOutlineColor.z, 1.0f));
		} else {
			material.SetVector("_OutlineColor", new Vector4(rabbitPartnerOutlineColor.x, rabbitPartnerOutlineColor.y, rabbitPartnerOutlineColor.z, 1.0f));
		}

		//map outline width to fraction of outline opacity
		float offset = 1.0f / maxWidth;

		//set outline width
		material.SetFloat("_OutlineWidth", outlineWidth / offset);
	}

	//on collision enter with collider specific to client, toggle specialLighting flag
	void OnTriggerEnter(Collider collision) {
		if (isRaven) {
			if (collision.gameObject.tag == "LitArea") specialLighting = true;
		} else {
			if (collision.gameObject.tag == "DarkArea") specialLighting = true;
		}
    }

    //on collision exit with collider specific to client, toggle specialLighting flag
    void OnTriggerExit(Collider collision) {
    	if (isRaven) {
    		if (collision.gameObject.tag == "LitArea") specialLighting = false;
    	} else {
    		if (collision.gameObject.tag == "DarkArea") specialLighting = false;
    	}
    }
}