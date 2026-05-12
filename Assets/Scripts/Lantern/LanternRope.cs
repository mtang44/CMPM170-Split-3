using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RopeRenderer : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;

    public int segments = 15;
    public float sagAmount = 0.5f;

    private LineRenderer line;

    void Start()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = segments;
    }
    //splits rope into segments, then calculates position at middle then sets the position of each segment in the line renderer to make curve (logic reused from other project)
    void LateUpdate()
    {
        if (startPoint == null || endPoint == null)
            return;

        for (int i = 0; i < segments; i++)
        {
            float t = i / (float)(segments - 1);

            // Straight line between points
            Vector3 pos = Vector3.Lerp(startPoint.position, endPoint.position, t);

            // sag, t is 0 at start, 1 at end, so it peaks in the middle
            float sag = Mathf.Sin(t * Mathf.PI) * sagAmount;

            pos += Vector3.down * sag;

            line.SetPosition(i, pos);
        }
    }
}