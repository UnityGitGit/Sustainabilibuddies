using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class FishingNet : MonoBehaviour {

	private Collider myColl;
	private Renderer myRenderer;

	public bool active;

	public List<GameObject> fishesInNet = new List<GameObject> ();

	[SerializeField]private Image fillRect;
	public int maxFishCount;
	public Text maxFishText;

	public FishNetState myState = FishNetState.netUp;

	public event DefaultDelegate onNetDown;
	public delegate void NetUpEvent(int fishCount);
	public event NetUpEvent onNetUp;

	void Start(){
		myColl = GetComponent<Collider> ();
		myRenderer = GetComponent<Renderer> ();
		fillRect.fillAmount = 0f;
		Deactivate ();
		maxFishText.text = maxFishCount.ToString();
	}

	void Update(){
		if (Input.GetKeyUp(KeyCode.Space)) {
			ToggleNet ();
		}
	}

	public void ToggleNet(){
		if(myState == FishNetState.netUp)
			ThrowOutNet ();
		else if(myState == FishNetState.netDown)
			StartCoroutine(PullUpNet ());		
	}

	void ThrowOutNet(){
		GameManager.instance.GameFeedback ("You cast out your nets...");
		myState = FishNetState.netDown;
		active = true;
		myColl.enabled = true;
		myRenderer.enabled = true;

		if (onNetDown != null) {
			onNetDown ();
		}
	}

	void Deactivate(){
		active = false;
		myColl.enabled = false;
		myRenderer.enabled = false;		
	}

	void OnTriggerEnter(Collider other){
		if (other.tag == "Fish") {
			TryPutFishInNet (other.gameObject);
		}
	}

	void TryPutFishInNet(GameObject fish){
		Debug.Log ("try put fish in net");
		if (myState != FishNetState.netDown)
			return;

		fishesInNet.Add (fish);
		fish.SetActive (false);
		fillRect.fillAmount = (float)fishesInNet.Count / maxFishCount;

		if (fishesInNet.Count > maxFishCount) {
			GameManager.instance.GameFeedback ("All of your fishes escape!", true);
			foreach (GameObject f in fishesInNet) {
				f.SetActive (true);
			}

			fillRect.fillAmount = 0f;
			fishesInNet.Clear ();
		}
	}

	IEnumerator PullUpNet(){
		GameManager.instance.GameFeedback ("You pull up your net...");
		myState = FishNetState.pullingNet;

		yield return new WaitForSeconds (4f);

		Deactivate ();
		fillRect.fillAmount = 0f;

		int fishCount = fishesInNet.Count;
		GameManager.instance.GameFeedback ("You find " + fishCount + " fishes in your net");
		for (int i = 0; i < fishCount; i++) {
			GameObject.Destroy (fishesInNet [i]);
		}

		fishesInNet.Clear ();	
		myState = FishNetState.netUp;

		if (onNetUp != null) {
			onNetUp (fishCount);
		}
	}

	public void UpgradeNetSize(int extraFishCount){
		maxFishCount += extraFishCount;
		maxFishText.text = maxFishCount.ToString();
	}
}

[System.Serializable]
public enum FishNetState{
	netDown,
	pullingNet,
	netUp
}