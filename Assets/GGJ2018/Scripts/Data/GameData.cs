﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData {

	public int version;
	public List<LevelData> levels = new List<LevelData>();

	public static void Save(GameData gameData) {
		string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, "game_data.txt");
		GameData oldData = JsonUtility.FromJson<GameData>(System.IO.File.ReadAllText(filePath));
		gameData.version = oldData.version + 1;

		string json = JsonUtility.ToJson(gameData);
		Debug.Log("Save Json: " + json);
		System.IO.File.WriteAllText(filePath, json);
	}
	public static IEnumerator Load(string downUrl, System.Action<GameData> response) {
		WWW download;
		// Fix Url無效
		if (GetWWW(downUrl, out download)) {
			yield return download;
			// Fix download失敗
			if (download.error == null) {
				// Fix download的不是json
				try {
					GameData data = JsonUtility.FromJson<GameData>(download.text);
					// Fix download的json不對
					if (data.version > 0 && data.levels.Count != 0) {
						response(data);
						yield break;
					};
				}
				catch { }
			}
		}

		// 後備
		string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, "game_data.txt");
		string result;
		if (filePath.Contains("://") || filePath.Contains(":///")) {
			WWW www = new WWW(filePath);
			yield return www;
			result = www.text;
		}
		else
			result = System.IO.File.ReadAllText(filePath);

		response(JsonUtility.FromJson<GameData>(result));
	}
	public static bool GetWWW(string downUrl, out WWW www) {
		try {
			www = new WWW(downUrl);
			return true;
		}
		catch {
			www = null;
			return false;
		}
	}
}

[System.Serializable]
public class LevelData {
	public List<StageData> stages = new List<StageData>();
}

[System.Serializable]
public class StageData {
	public string name;
	public List<StageObjectData> objects = new List<StageObjectData>();
}

[System.Serializable]
public class StageObjectData {
	public string name;
	public Vector2Int position;
	public Direction direction;
}