using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PileWeight : MonoBehaviour {
	// 四辺に存在する当たり判定
	[SerializeField]
	GameObject[] fourSideCol = new GameObject[4];

	// Use this for initialization
	//	void Start () {}

	// Update is called once per frame
	//	void Update () {}

	public List<GameObject> GetPileBoxList(Vector3 _vec) {
		List<GameObject> ret = new List<GameObject>();
		AddPileBoxList(ret, _vec);
		return ret;
	}

	void AddPileBoxList(List<GameObject> _boxList, Vector3 _vec) {
		List<GameObject> forward = new List<GameObject>();  // 対象コライダー
		List<GameObject> back = new List<GameObject>();     // 除外コライダー

		// 判定用マスク
		int mask = LayerMask.GetMask(new string[] { "Player", "Box" });

		//Debug.Log("AddChainBoxList");
		// 四辺コライダーを指定方向側と反対方向側に振り分け
		DotFourSideCollider(_vec, forward, back);

		// 指定方向側の四辺コライダーに接触している対象オブジェクトのコライダーをリスト化	
		List<Collider> hitColList = new List<Collider>();
		for (int idx = 0; idx < forward.Count; idx++) {
			hitColList.AddRange(Physics.OverlapBox(forward[idx].transform.position, forward[idx].transform.localScale * 0.5f, forward[idx].transform.rotation, mask));
		}

		//		// 対象以外のオブジェクトを除外
		//		for (int colIdx = hitColList.Count - 1; colIdx >= 0; colIdx--) {
		//			if (hitColList[colIdx].tag != "WeightObject") {
		//				hitColList.RemoveAt(colIdx);
		//			}
		//		}

		// 対象オブジェクトのコライダーのリストをオブジェクトのリストに変換
		List<GameObject> hitObjList = new List<GameObject>();
		while (hitColList.Count > 0) {
			hitObjList.Add(hitColList[0].gameObject);
			hitColList.RemoveAt(0);
		}

		// test
		/*
		testStr = "A List.Count:" + hitObjList.Count + " " + name + "\n";
		for (int cnt = 0; cnt < hitObjList.Count; cnt++) {
			testStr += hitObjList[cnt].name + "\n";
		}
		Debug.Log(testStr);
		//*/

		// 重複を排除
		RemoveDuplicateGameObject(hitObjList);

		// 自身を排除
		hitObjList.Remove(gameObject);

		// test
		/*
		testStr = "B List.Count:" + hitObjList.Count + " " + name + "\n";
		for (int cnt = 0; cnt < hitObjList.Count; cnt++) {
			testStr += hitObjList[cnt].name + "\n";
		}
		Debug.Log(testStr);
		//*/

		// 指定方向の反対側の四辺コライダーに接触している対象オブジェクトのコライダーをリスト化	
		List<Collider> outColList = new List<Collider>();
		for (int idx = 0; idx < back.Count; idx++) {
			outColList.AddRange(Physics.OverlapBox(back[idx].transform.position, back[idx].transform.localScale * 0.5f, back[idx].transform.rotation, mask));
		}

		//		// 対象以外のオブジェクトを除外
		//		for (int colIdx = outColList.Count - 1; colIdx >= 0; colIdx--) {
		//			if (outColList[colIdx].tag != "WeightObject") {
		//				outColList.RemoveAt(colIdx);
		//			}
		//		}

		// 除外オブジェクトのコライダーのリストをオブジェクトのリストに変換
		List<GameObject> outObjList = new List<GameObject>();
		while (outColList.Count > 0) {
			outObjList.Add(outColList[0].gameObject);
			outColList.RemoveAt(0);
		}

		// test
		/*
		testStr = "C outList.Count:" + outObjList.Count + " " + name + "\n";
		for (int cnt = 0; cnt < outObjList.Count; cnt++) {
			testStr += outObjList[cnt].name + "\n";
		}
		Debug.Log(testStr);
		//*/

		// 重複を排除
		RemoveDuplicateGameObject(outObjList);

		// 自身を排除
		outObjList.Remove(gameObject);

		// test
		/*
		testStr = "D outList.Count:" + outObjList.Count + " " + name + "\n";
		for (int cnt = 0; cnt < outObjList.Count; cnt++) {
			testStr += outObjList[cnt].name + "\n";
		}
		Debug.Log(testStr);
		//*/

		// 指定方向から遠いコライダ－に接触している対象オブジェクトをリストから排除
		for (int outObjIdx = 0; outObjIdx < outObjList.Count; outObjIdx++) {
			hitObjList.Remove(outObjList[outObjIdx]);
		}

		// test
		/*
		testStr = "E List.Count:" + hitObjList.Count + " " + name + "\n";
		for (int cnt = 0; cnt < hitObjList.Count; cnt++) {
			testStr += hitObjList[cnt].name + "\n";
		}
		Debug.Log(testStr);
		//*/

		// 既存リストに存在する排除対象オブジェクトをリストから除外
		for (int boxListIdx = 0; boxListIdx < _boxList.Count; boxListIdx++) {
			hitObjList.Remove(_boxList[boxListIdx]);
		}

		// test
		/*
		testStr = "F List.Count:" + hitObjList.Count + " " + name + "\n";
		for (int cnt = 0; cnt < hitObjList.Count; cnt++) {
			testStr += hitObjList[cnt].name + "\n";
		}
		Debug.Log(testStr);
		//*/

		// リスト内の対象オブジェクトを既存リストと統合
		_boxList.AddRange(hitObjList);

		// リストの重複を排除
		RemoveDuplicateGameObject(_boxList);

		// 新たな対象オブジェクトそれぞれで再帰呼び出し
		for (int hitObjIdx = 0; hitObjIdx < hitObjList.Count; hitObjIdx++) {
			PileWeight otherBox = hitObjList[hitObjIdx].GetComponent<PileWeight>();
			if (otherBox != null) {
				// 再帰呼び出し
				otherBox.AddPileBoxList(_boxList, _vec);
			}
		}

		//		// test
		//		topCol = forward;
		//		underCol = back;
	}

	// 四辺コライダーが指定方向に存在するか判定して振り分ける
	void DotFourSideCollider(Vector3 _vec, List<GameObject> _forward, List<GameObject> _back) {
		// 四辺コライダーが設定されていない場合
		if (fourSideCol.Length == 0) {
			Debug.LogError("四辺コライダー配列の要素が存在していません。\n" /*+ MessageLog.GetNameAndPos(gameObject)*/);
			return;
		}

		// 正規化
		_vec = _vec.normalized;

		// リストの初期化
		_forward.Clear();
		_back.Clear();

		// 全ての四辺コライダーについて指定の方向に存在するか判定して振り分ける
		for (int idx = 0; idx < fourSideCol.Length; idx++) {
			Vector3 vec = fourSideCol[idx].transform.position - transform.position;
			if (Vector3.Dot(vec, _vec) > 0.0f) {
				_forward.Add(fourSideCol[idx]);
			} else {
				_back.Add(fourSideCol[idx]);
			}
		}
	}

	//	// 四辺のコライダーを指定の方向に近い順に並び替え
	//	void SortFourSideCollider(Vector3 _vec) {
	//		// コライダーが配列に正常に設定されていない場合
	//		if (fourSideCol.Length != 4) {
	//			Debug.LogError("四辺コライダー配列の要素数が" + fourSideCol.Length + "です。\n" /*+ MessageLog.GetNameAndPos(gameObject)*/);
	//			return;
	//		}
	//		// 四辺コライダーが設定されていない場合
	//		for (int idx = 0; idx < fourSideCol.Length; idx++) {
	//			if (fourSideCol[idx] == null) {
	//				Debug.LogError("四辺コライダー配列[" + idx + "]が設定されていません。\n" /*+ MessageLog.GetNameAndPos(gameObject)*/);
	//			}
	//		}
	//
	//		_vec = _vec.normalized;
	//
	//		// 最も近いコライダーと最も遠いコライダーを求める
	//		float near = Mathf.Infinity, far = Mathf.NegativeInfinity;
	//		int nearIdx = -1, farIdx = -1;
	//		for (int fourSideIdx = 0; fourSideIdx < fourSideCol.Length; fourSideIdx++) {
	//			float dis = Vector3.Distance(fourSideCol[fourSideIdx].transform.position, transform.position + _vec);
	//			// 最も近いコライダーとの距離とインデックス
	//			if (dis < near) {
	//				near = dis;
	//				nearIdx = fourSideIdx;
	//			}
	//			// 最も遠いコライダーとの距離とインデックス
	//			if (dis > far) {
	//				far = dis;
	//				farIdx = fourSideIdx;
	//			}
	//		}
	//
	//		// 残り二つのコライダーの近い方と遠い方を求める
	//		int semiNearIdx = -1, semiFarIdx = -1;
	//		near = Mathf.Infinity;
	//		far = Mathf.NegativeInfinity;
	//		for (int fourSideIdx = 0; fourSideIdx < fourSideCol.Length; fourSideIdx++) {
	//			// 最も近いコライダーか最も遠いコライダーであれば処理しない
	//			if ((fourSideIdx == nearIdx) || (fourSideIdx == farIdx)) continue;
	//
	//			float dis = Vector3.Distance(fourSideCol[fourSideIdx].transform.position, transform.position + _vec);
	//			// 二番目に近いコライダーとの距離とインデックス
	//			if (dis < near) {
	//				near = dis;
	//				semiNearIdx = fourSideIdx;
	//			}
	//			// 三番目に近いコライダーとの距離とインデックス
	//			if (dis > far) {
	//				far = dis;
	//				semiFarIdx = fourSideIdx;
	//			}
	//		}
	//
	//		// 順番通りに並び替えた配列を作成
	//		BoxCollider[] sortCol = new BoxCollider[4];
	//		sortCol[0] = fourSideCol[nearIdx];
	//		sortCol[1] = fourSideCol[semiNearIdx];
	//		sortCol[2] = fourSideCol[semiFarIdx];
	//		sortCol[3] = fourSideCol[farIdx];
	//
	//		// 既存の配列を並び替え後の配列に変更
	//		fourSideCol = sortCol;
	//	}

	// GameObjectのリストから重複している要素を排除し、排除した数を返す
	int RemoveDuplicateGameObject(List<GameObject> _list) {
		int cnt = 0;
		// 対象リストから重複を排除
		for (int targetIdx = 0; targetIdx < _list.Count; targetIdx++) {
			// 以降の同様の要素を探す
			while (_list.LastIndexOf(_list[targetIdx]) > targetIdx) {
				// 重複している要素を排除
				_list.RemoveAt(_list.LastIndexOf(_list[targetIdx]));
				cnt++;
			}
		}
		return cnt;
	}
}
