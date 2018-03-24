using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrollTexture : MonoBehaviour {

	// public variables
	public float m_speedScrollX = 0.5f;
	public float m_speedScrollY = 0.5f;
	public bool m_activate = true;

	// private variables

	// ------------------------------------
	// Use this for initialization
	// ------------------------------------
	void Start () {
		
	}

	// ------------------------------------
	// Update is called once per frame
	// ------------------------------------
	void Update () {
		if (m_activate) {
			float offsetX = Time.time * m_speedScrollX;
			float offsetY = Time.time * m_speedScrollY;

			GetComponent<Renderer> ().material.mainTextureOffset = new Vector2 (offsetX, offsetY);
		}
	}

	// ------------------------------------
	// Methods
	// ------------------------------------

}
