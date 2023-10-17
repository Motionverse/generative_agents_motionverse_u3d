using UnityEngine;
using System.Collections;

public class LayerManager : MonoBehaviour {

	static public int redLayer = 31;
	static public int blueLayer = 30;
	static public int pinkLayer = 29;
	static public int greenLayer = 28;

	static public LayerMask GetEnemyLayer(Team team){

		LayerMask mask = 0;

		if (team != Team.Red) mask = mask | (1 << redLayer);
		if (team != Team.Blue) mask = mask | (1 << blueLayer);
		if (team != Team.Pink) mask = mask | (1 << pinkLayer);
		if (team != Team.Green) mask = mask | (1 << greenLayer);

		return mask;
	}
}
