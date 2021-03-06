using UnityEngine;

//this script holds all of the important stats of the ship
//some of these can be upgraded, this is done using the ShipStat enum
public class ShipStats : MonoBehaviour {

	public float sailSpeed = 5f;
	public float rotationSpeed = 10f;
	public int netSize = 10;

	public void UpgradeStat(Upgrade upgrade){
		switch (upgrade.stat) {
		case ShipUpgradeable.sailSpeed:
			sailSpeed += upgrade.upgradeValue;
			break;
		case ShipUpgradeable.rotationSpeed:
			rotationSpeed += upgrade.upgradeValue;
			break;
		case ShipUpgradeable.netSize:
			netSize += (int)upgrade.upgradeValue;
			break;
		default:
			Debug.LogWarning ("unknown upgrade: " + upgrade.stat.ToString ());
			break;
		}
	}

	/* not needed?
	public float GetValue(ShipUpgradeable stat){
		switch (stat) {
		case ShipUpgradeable.sailSpeed:
			return sailSpeed;
		case ShipUpgradeable.rotationSpeed:
			return rotationSpeed;
		case ShipUpgradeable.netSize:
			return netSize;
		default:
			Debug.LogWarning ("unknown upgrade: " + stat.ToString ());
			return 0f;
		}
	}
	*/
}

public enum ShipUpgradeable{
	sailSpeed,
	netSize,
	rotationSpeed,
	permit
}
