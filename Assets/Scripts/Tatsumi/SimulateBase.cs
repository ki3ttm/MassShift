using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulateBase : MonoBehaviour {
	// 種の特徴
	[SerializeField] float growthRate = 1.0f;       // 成長度
	[SerializeField] float lifeTimeRate = 10.0f;    // 長命度
	[SerializeField] float mutationRate = 0.1f;     // 変異度

	[SerializeField] WorldState worldStt = null;
	public WorldState WorldStt {
		get {
			if (worldStt == null) {
				worldStt = FindObjectOfType<WorldState>();
			}
			return worldStt;
		}
	}

	[SerializeField] float lifeTime = 0.0f;
	float LifeTime {
		get {
			return lifeTime;
		}
		set {
			lifeTime = value;
		}
	}

	// Use this for initialization
//	void Start () {}
	
	// Update is called once per frame
	void Update () {
		// 経年劣化
		LifeTime -= (1.0f / lifeTimeRate) * WorldStt.TimeScl * Time.deltaTime;
	}

	void Birth(GameObject[] parent = null) {
		LifeTime = 1.0f;
	}

	bool isLive() {
		return (lifeTime > 0.0f);
	}
}
