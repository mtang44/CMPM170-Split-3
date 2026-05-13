// Place this file in any folder named "Editor" in your project
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(WaterDetailPlacer))]
public class WaterDetailPlacerEditor : Editor
{
	private List<Vector3> recentlyPlacedPositions = new List<Vector3>();
	private Vector3 lastBrushPosition = Vector3.zero;
	private bool hasBrushPosition = false;

	private void OnSceneGUI()
	{
		WaterDetailPlacer placer = (WaterDetailPlacer)target;
		Event e = Event.current;

		Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
		Plane waterPlane = new Plane(Vector3.up, Vector3.up * placer.waterHeight);

		if(!waterPlane.Raycast(ray, out float t))
			return;

		Vector3 hitPoint = ray.GetPoint(t);
		bool isErasing = e.shift;

		// Draw brush disc
		Handles.color = isErasing ? new Color(1f, 0.3f, 0.3f, 0.8f) : new Color(0.3f, 0.9f, 1f, 0.8f);
		Handles.DrawWireDisc(hitPoint, Vector3.up, placer.brushSize);
		Handles.color = isErasing ? new Color(1f, 0.3f, 0.3f, 0.05f) : new Color(0.3f, 0.9f, 1f, 0.05f);
		Handles.DrawSolidDisc(hitPoint, Vector3.up, placer.brushSize);

		// Mode label
		Handles.BeginGUI();
		GUIStyle labelStyle = new GUIStyle(EditorStyles.boldLabel);
		labelStyle.normal.textColor = isErasing ? new Color(1f, 0.4f, 0.4f) : new Color(0.3f, 0.9f, 1f);
		Vector2 screenPos = HandleUtility.WorldToGUIPoint(hitPoint + Vector3.up * 0.5f);
		GUI.Label(new Rect(screenPos.x + 10, screenPos.y - 10, 120, 20),
			isErasing ? "ERASE" : "PAINT", labelStyle);
		Handles.EndGUI();

		if(e.type == EventType.MouseDown || e.type == EventType.MouseDrag)
		{
			if(e.button == 0)
			{
				if(isErasing)
				{
					EraseAt(placer, hitPoint);
				}
				else
				{
					if(!hasBrushPosition || Vector3.Distance(hitPoint, lastBrushPosition) >= placer.density)
					{
						TryPlaceInBrush(placer, hitPoint);
						lastBrushPosition = hitPoint;
						hasBrushPosition = true;
					}
				}
				e.Use();
			}
		}

		if(e.type == EventType.MouseUp && e.button == 0)
		{
			hasBrushPosition = false;
			recentlyPlacedPositions.Clear();
			e.Use();
		}

		HandleUtility.Repaint();
	}

	private void TryPlaceInBrush(WaterDetailPlacer placer, Vector3 brushCenter)
	{
		GameObject prefabToUse = PickPrefab(placer);
		if(prefabToUse == null)
		{
			Debug.LogWarning("WaterDetailPlacer: No prefabs assigned.");
			return;
		}

		Vector2 randomCircle = Random.insideUnitCircle * placer.brushSize;
		Vector3 spawnPos = new Vector3(
			brushCenter.x + randomCircle.x,
			placer.waterHeight,
			brushCenter.z + randomCircle.y
		);

		foreach(Vector3 pos in recentlyPlacedPositions)
		{
			if(Vector3.Distance(spawnPos, pos) < placer.density)
				return;
		}

		foreach(Transform child in placer.transform)
		{
			if(Vector3.Distance(spawnPos, child.position) < placer.density)
				return;
		}

		GameObject placed = (GameObject)PrefabUtility.InstantiatePrefab(prefabToUse);
		placed.transform.position = spawnPos;
		placed.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

		float scaleVariation = Random.Range(-placer.scaleRandomRange, placer.scaleRandomRange);
		float finalScale = placer.baseScale * (1f + scaleVariation);
		placed.transform.localScale = Vector3.one * finalScale;

		placed.transform.SetParent(placer.transform);
		Undo.RegisterCreatedObjectUndo(placed, "Paint Water Detail");

		recentlyPlacedPositions.Add(spawnPos);
	}

	private void EraseAt(WaterDetailPlacer placer, Vector3 brushCenter)
	{
		List<Transform> toDelete = new List<Transform>();
		foreach(Transform child in placer.transform)
		{
			if(Vector3.Distance(child.position, brushCenter) <= placer.brushSize)
				toDelete.Add(child);
		}
		foreach(Transform child in toDelete)
			Undo.DestroyObjectImmediate(child.gameObject);
	}

	private GameObject PickPrefab(WaterDetailPlacer placer)
	{
		// Build a list of only assigned prefabs and their weights
		var options = new List<(GameObject prefab, float weight)>();
		if(placer.prefabA != null && placer.weightA > 0f) options.Add((placer.prefabA, placer.weightA));
		if(placer.prefabB != null && placer.weightB > 0f) options.Add((placer.prefabB, placer.weightB));
		if(placer.prefabC != null && placer.weightC > 0f) options.Add((placer.prefabC, placer.weightC));

		if(options.Count == 0) return null;

		float total = 0f;
		foreach(var o in options) total += o.weight;

		float roll = Random.Range(0f, total);
		float cumulative = 0f;
		foreach(var o in options)
		{
			cumulative += o.weight;
			if(roll <= cumulative) return o.prefab;
		}

		return options[options.Count - 1].prefab;
	}

	public override void OnInspectorGUI()
	{
		WaterDetailPlacer placer = (WaterDetailPlacer)target;
		serializedObject.Update();

		EditorGUILayout.LabelField("Water Settings", EditorStyles.boldLabel);
		placer.waterHeight = EditorGUILayout.FloatField("Water Height (Y)", placer.waterHeight);

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Prefabs & Weights", EditorStyles.boldLabel);
		EditorGUILayout.HelpBox("Weights are relative — e.g. 7 / 2 / 1 means 70% / 20% / 10%.", MessageType.None);

		float total = placer.weightA + placer.weightB + placer.weightC;

		DrawPrefabRow(ref placer.prefabA, ref placer.weightA, "Prefab A", total);
		DrawPrefabRow(ref placer.prefabB, ref placer.weightB, "Prefab B", total);
		DrawPrefabRow(ref placer.prefabC, ref placer.weightC, "Prefab C", total);

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Brush", EditorStyles.boldLabel);
		placer.brushSize = EditorGUILayout.Slider("Brush Size", placer.brushSize, 0.5f, 20f);
		placer.density = EditorGUILayout.Slider("Min Spacing", placer.density, 0.1f, 5f);

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Scale", EditorStyles.boldLabel);
		placer.baseScale = EditorGUILayout.FloatField("Base Scale", placer.baseScale);
		placer.scaleRandomRange = EditorGUILayout.Slider("Random Range (±%)", placer.scaleRandomRange, 0f, 1f);
		EditorGUILayout.HelpBox(
			$"Scale range: {placer.baseScale * (1f - placer.scaleRandomRange):F2} – {placer.baseScale * (1f + placer.scaleRandomRange):F2}",
			MessageType.None);

		EditorGUILayout.Space();
		EditorGUILayout.HelpBox("Select this GameObject and click/drag in the Scene view to paint.\nHold Shift to erase.", MessageType.Info);

		if(GUI.changed)
			EditorUtility.SetDirty(placer);

		serializedObject.ApplyModifiedProperties();
	}

	private void DrawPrefabRow(ref GameObject prefab, ref float weight, string label, float total)
	{
		EditorGUILayout.BeginHorizontal();
		prefab = (GameObject)EditorGUILayout.ObjectField(label, prefab, typeof(GameObject), false);
		weight = EditorGUILayout.FloatField(weight, GUILayout.Width(40));
		weight = Mathf.Max(0f, weight);

		string pct = total > 0f && prefab != null
			? $"{(weight / total * 100f):F0}%"
			: "--";
		EditorGUILayout.LabelField(pct, GUILayout.Width(35));
		EditorGUILayout.EndHorizontal();
	}
}