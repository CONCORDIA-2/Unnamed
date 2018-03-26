using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class End : MonoBehaviour {

	private LocalPlayerManager playerManager;
	private BoxCollider trigger;
	private GameObject localPlayer;
	private GameObject otherPlayer;

	public GameObject[] boats;

	public bool endTriggered = false;
	public bool playerOneHere = false;
	public bool playerTwoHere = false;

	public float boatSpeed = 1.5f;
	public float[] boatsSpeeds;

	void Start() {
		//boats = new GameObject[10];
		boatsSpeeds = new float[10];
		for (int i = 0; i < boatsSpeeds.Length; i++)
		{
			boatsSpeeds[i] = Random.Range(boatSpeed + 3.5f, boatSpeed + 5.5f);
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
			StartCoroutine("Ending");

			playerManager.GetLocalPlayerObject().GetComponent<Player_Movement>().enabled = false;
			playerManager.GetLocalPlayerObject().transform.parent = transform;

			playerManager.GetOtherPlayerObject().GetComponent<Player_Movement>().enabled = false;
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

		//fade text
		Text t = transform.Find("Canvas/Text").GetComponent<Text>();
		for (float i = 0; i < 1.2f; i+=0.01f) {
			yield return new WaitForSeconds(0.05f);
			t.color = new Color(1f, 1f, 1f, i);
		}

		//wait seconds
		yield return new WaitForSeconds(10);

		//fade black
		Image im = GameObject.Find("FinalFade").GetComponent<Image>();
		for (float i = 0; i < 1.2f; i+=0.01f) {
			yield return new WaitForSeconds(0.05f);
			im.color = new Color(0f, 0f, 0f, i);
		}

		//wait seconds
		yield return new WaitForSeconds(1);

		//load menu
		SceneManager.LoadScene("Scenes/Main Menu");
	}
}
