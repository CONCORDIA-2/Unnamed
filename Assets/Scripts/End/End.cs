using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class End : MonoBehaviour {

	private LocalPlayerManager playerManager;
	private BoxCollider trigger;
	private GameObject localPlayer;
	private GameObject otherPlayer;

	public GameObject[] boats;

	public bool endTriggered = false;
	public bool playerOneHere = false;
	public bool playerTwoHere = false;

	public float boatSpeed = 0.8f;
	public float[] boatsSpeeds;

	void Start() {
		boats = new GameObject[10];
		boatsSpeeds = new float[10];
		for (int i = 0; i < boatsSpeeds.Length; i++)
		{
			boatsSpeeds[i] = Random.Range(boatSpeed + 0.3f, boatSpeed + 0.7f);
		}
		trigger =  GetComponents<BoxCollider>()[4];
		playerManager = GameObject.FindGameObjectWithTag("PlayerManager").GetComponent<LocalPlayerManager>();
		//localPlayer = playerManager.GetLocalPlayerObject();
		//otherPlayer = playerManager.GetOtherPlayerObject();
	}
	
	void Update() {

		if (playerOneHere && playerTwoHere && !endTriggered)
		{
			endTriggered = true;
			playerManager.GetLocalPlayerObject().GetComponent<Player_Movement>().enabled = false;
			playerManager.GetOtherPlayerObject().GetComponent<Player_Movement>().enabled = false;
			playerManager.GetLocalPlayerObject().transform.parent = transform;
			playerManager.GetOtherPlayerObject().transform.parent = transform;
		} 

		if (endTriggered)
		{
			transform.Translate(new Vector3(0, -boatSpeed, 0) * Time.deltaTime);
			for (int i = 0; i < boats.Length; i++)
			{
				boats[i].transform.Translate(new Vector3(0, -boatsSpeeds[i], 0) *Time.deltaTime);
			}
		}
	}

	void OnTriggerStay(Collider collider) {
		if (collider.gameObject == playerManager.GetLocalPlayerObject()) playerOneHere = true;
		if (collider.gameObject == playerManager.GetOtherPlayerObject()) playerTwoHere = true;
	}

	void OnTriggerExit(Collider collider) {
		if (collider.gameObject == playerManager.GetLocalPlayerObject()) playerOneHere = false;
		if (collider.gameObject == playerManager.GetOtherPlayerObject()) playerTwoHere = false;
	}

	IEnumerator Ending() {
		//wait seconds
		yield return new WaitForSeconds(10);

		//fade text appear

		//wait seconds
		yield return new WaitForSeconds(10);

		//fade to black

		// load menu
	}
}
