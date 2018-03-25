using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPlayer : MonoBehaviour {

	public bool isRaven = true;

	void Start() {
		isRaven = GameObject.FindGameObjectWithTag("PlayerManager").GetComponent<LocalPlayerManager>().IsRaven();
	}

	void OnCollisionStay(Collision collision) {
		if (collision.gameObject.tag == "block rabbit" && isRaven) {
			Debug.Log("collision ignored - raven");
			Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), GetComponents<CapsuleCollider>()[0]);
			Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), GetComponents<CapsuleCollider>()[1]);
		}

		if (collision.gameObject.tag == "block raven" && !isRaven) {
			Debug.Log("collision ignored - rabbit");
			Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), GetComponents<CapsuleCollider>()[0]);
			Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), GetComponents<CapsuleCollider>()[1]);
		}
	}
}