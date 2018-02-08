using UnityEngine;
using System.Collections.Generic;

public class FishingNet : MonoBehaviour {

	private PlayerEconomics statsScript;
	private Collider myColl;
	private Renderer myRenderer;

	public bool active;

	public List<GameObject> fishesInNet = new List<GameObject> ();

	void Start(){
		statsScript = transform.parent.GetComponent<PlayerEconomics> ();
		myColl = GetComponent<Collider> ();
		myRenderer = GetComponent<Renderer> ();
	}

	void Update(){
		if (Input.GetKeyDown (KeyCode.Space)) {
			Activate ();
		} else if (Input.GetKeyUp (KeyCode.Space)) {
			Deactivate ();
		}
	}

	void Activate(){
		GameManager.instance.GameFeedback ("You cast out your nets...");
		active = true;
		myColl.enabled = true;
		myRenderer.enabled = true;
	}

	void Deactivate(){
		GameManager.instance.GameFeedback ("You pull up your net...");
		active = false;
		myColl.enabled = false;
		myRenderer.enabled = false;		

		statsScript.AddFish (fishesInNet.Count);
		for (int i = 0; i < fishesInNet.Count; i++) {
			GameObject.Destroy (fishesInNet [i]);
		}
		fishesInNet.Clear ();
	}

	void OnTriggerEnter(Collider other){
		if (other.tag == "Fish") {
			fishesInNet.Add (other.gameObject);
			other.gameObject.SetActive (false);
		}
	}
}