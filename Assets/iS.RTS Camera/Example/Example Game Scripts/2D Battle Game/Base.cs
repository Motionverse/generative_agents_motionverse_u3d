using UnityEngine;
using System.Collections;

public class Base : MonoBehaviour {

	public Commander commander;
	public Transform spawnPoint;
	public Tank tank;
	public float spawnTimeUpdateTime;
	public ISRange spawnTimeUp;

	float currentSpawnTimeUp;

	void Start () {
		StartCoroutine(UpdateTimeUp());
		StartCoroutine (Spawn ());
	}

	IEnumerator UpdateTimeUp(){
		while(true){

			currentSpawnTimeUp = ISMath.Random (spawnTimeUp);

			yield return new WaitForSeconds (spawnTimeUpdateTime);
		}
	}

	IEnumerator Spawn(){
		while(true){

			GameObject obj = Instantiate (tank.gameObject, spawnPoint.position, spawnPoint.rotation) as GameObject;
			if(commander) commander.ownedTanks.Add (obj.GetComponent<Tank> ());

			yield return new WaitForSeconds (currentSpawnTimeUp);
		}
	}
}
