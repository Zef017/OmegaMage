using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum attackTypes {
	melee,
	elemental
}

public class EnemyDamager : MonoBehaviour {

	public int damage = 2;
	public float knockback = 0;
	public attackTypes attackType;
	public elements elementType;

	// Use this for initialization
	//void Start () {
		
	//}
	
	// Update is called once per frame
	//void Update () {
		
	//}
}
