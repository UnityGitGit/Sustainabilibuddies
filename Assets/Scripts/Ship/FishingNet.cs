using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class FishingNet : MonoBehaviour {

	private Collider myColl;
	private Renderer myRenderer;
	private ShipStats shipStats;

	public List<GameObject> fishesInNet = new List<GameObject> ();

	[SerializeField]private Image fillRect;

	public FishNetState myState = FishNetState.netUp;

	public event DefaultDelegate onNetDown;
	public delegate void NetUpEvent(int fishCount);
	public event NetUpEvent onNetUp;

	public bool canHandleNet;

	private Counter catchFishCounter;
	public float maxCatchFishInterval = 0.5f;
	private bool canCatchFish = true;

	//shipStats implementation:
	private int maxFishCount{
		get{
			return shipStats.netSize;
		}
	}

	void Start(){
		shipStats = transform.root.GetComponent<ShipStats> ();
		myColl = GetComponent<Collider> ();
		myRenderer = GetComponent<Renderer> ();
		fillRect.fillAmount = 0f;
		Deactivate ();

		//set up the system that is needed to prevent the net from getting full in a very short time
		catchFishCounter = new Counter (maxCatchFishInterval);
		catchFishCounter.onCount += EnableCatchingFish;

		//make sure we can only use the net if the game manager allows us to:
		GameManager.instance.onBoatControlsDisabled += BoatControlsDisabled;
		GameManager.instance.onBoatControlsEnabled += BoatControlsEnabled;
	}

	void Update(){
		if (!canHandleNet)
			return;

		if (Input.GetKeyUp(KeyCode.Space) || ShipInputManager.instance.doubleTapped) {
			ToggleNet ();
		}
	}

	public void ToggleNet(){
		if(myState == FishNetState.netUp)
			ThrowOutNet ();
		else if(myState == FishNetState.netDown || myState == FishNetState.netBroken)
			PullUpNet ();		
	}

	void ThrowOutNet(){
		GameManager.instance.GameFeedback ("You cast out your nets...");
		myState = FishNetState.netDown;
		myColl.enabled = true;
		myRenderer.enabled = true;

		if (onNetDown != null) {
			onNetDown ();
		}
	}

	void Deactivate(){
		myColl.enabled = false;
		myRenderer.enabled = false;		
	}

	void OnTriggerEnter(Collider other){
		if (other.tag == "Fish" && canCatchFish) {
			TryPutFishInNet (other.gameObject);
		}
	}

	void TryPutFishInNet(GameObject fishObject){
		if (myState != FishNetState.netDown)
			return;
		
		if (fishesInNet.Count == maxFishCount) {
			myState = FishNetState.netBroken;
			GameManager.instance.GameFeedback ("All of your fishes escape!", true);
			foreach (GameObject f in fishesInNet) {
				FishModel fish = f.GetComponent<FishModel> ();
				fish.Escape ();
			}

			fillRect.fillAmount = 0f;
			fishesInNet.Clear ();
		} else {
			fishesInNet.Add (fishObject);
			FishModel fish = fishObject.GetComponent<FishModel> ();
			fish.CatchMe (transform);
		}

		fillRect.fillAmount = (float)fishesInNet.Count / maxFishCount;

		OnFishCaught ();
	}

	void PullUpNet(){
		//GameManager.instance.GameFeedback ("You pull up your net...");
		//myState = FishNetState.pullingNet;

		//yield return new WaitForSeconds (4f);

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

	//make sure we can't catch 10 fishes in 0.001 second:
	private void OnFishCaught(){
		canCatchFish = false;
		catchFishCounter.StartCounter ();
	}

	private void EnableCatchingFish(){
		canCatchFish = true;
	}

	private void BoatControlsDisabled(){
		canHandleNet = false;
	}

	private void BoatControlsEnabled(){
		canHandleNet = true;
	}
}

[System.Serializable]
public enum FishNetState{
	netDown,
	//pullingNet,
	netUp,
	netBroken
}