using UnityEngine;

public class WaterDetailPlacer : MonoBehaviour
{
	[Header("Water Settings")]
	public float waterHeight = 0f;

	[Header("Prefabs & Weights")]
	public GameObject prefabA;
	[Min(0f)] public float weightA = 7f;

	public GameObject prefabB;
	[Min(0f)] public float weightB = 2f;

	public GameObject prefabC;
	[Min(0f)] public float weightC = 1f;

	[Header("Brush Settings")]
	[Range(0.5f, 20f)]
	public float brushSize = 3f;
	[Range(0.1f, 5f)]
	public float density = 1f;

	[Header("Scale Settings")]
	public float baseScale = 1f;
	[Range(0f, 1f)]
	public float scaleRandomRange = 0.2f;
}