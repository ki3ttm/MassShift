﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightManager : MonoBehaviour {
	// 列挙型
	public enum Weight {    // 重さレベル
		flying = 0,
		light,
		heavy,
	}

	// 定数
	const Weight WeightDefLv = Weight.light;    // 標準重さレベル

	// 重さレベル
	[SerializeField]
	Weight weightLv = WeightDefLv;
	public Weight WeightLv { get { return weightLv; } set { weightLv = value; } }

	// 重さレベルによる上下動値
	[SerializeField]
	List<Vector3> weightFallVelList = new List<Vector3>();

	//	// 力を処理するコンポーネント
	//	ForceManager forceMng = null;

	// Use this for initialization
	//	void Start () {
	//		// 力コンポーネント
	//		forceMng = GetComponent<ForceManager>();
	//		if (forceMng == null) {
	//
	//
	//		}
	//	}

	// Update is called once per frame
	//	void Update() {}

	// pull元からpush先へ指定数の重さレベルを移し、移す事に成功したレベル数を返す
	public int PullWeight(WeightManager _from, int _num = 1) {
		int cnt = 0;
		// 指定数のレベルを移しきるか、値が変更できなくなるまでループ
		while ((_num > 0) && _from.SubWeightLevel(true) && AddWeightLevel(true)) {
			// 値を変更
			_from.SubWeightLevel(false);
			AddWeightLevel(false);

			// 変更回数を加算
			cnt++;

			// 残り変更回数を減算
			_num--;
		}
		// 変更回数を返す
		return cnt;
	}
	// 相手側のpull処理を行う事で間接的なpush処理を実装
	public int PushWeight(WeightManager _to, int _num = 1) {
		return _to.PullWeight(this, _num);
	}

	// 重さレベルを一段階変化させる
	// 成功したらtrueを返す
	// _checkOnlyがtrueなら成功するかどうかだけを返し、レベルを変更しない
	bool AddWeightLevel(bool _checkOnly) {
		string logMsg = name + ": " + weightLv + " -> ";
		switch (WeightLv) {
		case Weight.flying:
			if (!_checkOnly) {
				// 変更
				WeightLv = Weight.light;
			}
			return true;
		case Weight.light:
			if (!_checkOnly) {
				// 変更
				WeightLv = Weight.heavy;
			}
			return true;
		case Weight.heavy:
			// 変更不可
			return false;
		default:
			Debug.LogError("不明な重さレベルに変更されようとしました。\n" /*+ MessageLog.GetNameAndPos(gameObject)*/);
			return false;
		}
	}
	bool SubWeightLevel(bool _checkOnly) {
		switch (WeightLv) {
		case Weight.flying:
			// 変更不可
			return false;
		case Weight.light:
			if (!_checkOnly) {
				// 変更
				WeightLv = Weight.flying;
			}
			return true;
		case Weight.heavy:
			// 変更
			if (!_checkOnly) {
				WeightLv = Weight.light;
			}
			return true;
		default:
			Debug.LogError("不明な重さレベルに変更されようとしました。\n" /*+ MessageLog.GetNameAndPos(gameObject)*/);
			return false;
		}
	}

	// 現在の重さレベルでの上下動値を返す
	public Vector3 GetWeightFallVel() {
		// 上下動値のリストが不正ならエラー出力
		if (weightFallVelList.Count <= (int)WeightLv) {
			Debug.LogError(WeightLv + "(" + (int)WeightLv + ")の上下動値がリストに存在しません。\n" /*+ MessageLog.GetNameAndPos(gameObject)*/);
			return Vector3.zero;
		}

		// 上下動値を返す
		return weightFallVelList[(int)WeightLv];
	}

	// 
	public List<GameObject> GetCollisionObjectList() {
		List<GameObject> ret = new List<GameObject>();


		return ret;
	}
}
