using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class LightConeMesh : MonoBehaviour
{
    public float coneAngle  = 15f;
    public float coneLength = 500f;
    public int   segments   = 32;

    void Start()
    {
        GetComponent<MeshFilter>().mesh = BuildConeMesh();

        Shader shader = Shader.Find("Legacy Shaders/Particles/Additive");
        if (shader == null) shader = Shader.Find("Particles/Additive");
        if (shader == null) shader = Shader.Find("Unlit/Color");

        Material mat = new Material(shader);
        mat.color = new Color(1f, 1f, 0.5f, 1f);
        GetComponent<MeshRenderer>().material = mat;

        Debug.Log("Shader used: " + mat.shader.name);
        Debug.Log("Cone built with " + segments + " segments, length " + coneLength);
    }

    Mesh BuildConeMesh()
    {
        float radius = Mathf.Tan(coneAngle * Mathf.Deg2Rad) * coneLength;

        Vector3[] vertices  = new Vector3[segments + 2];
        Color[]   colors    = new Color[segments + 2];
        int[]     triangles = new int[segments * 3];

        // Tip — brightest point
        vertices[0] = Vector3.zero;
        colors[0]   = new Color(1f, 1f, 0.7f, 0.6f);

        for (int i = 0; i <= segments; i++)
        {
            float angle = (float)i / segments * Mathf.PI * 2f;
            vertices[i + 1] = new Vector3(
                Mathf.Cos(angle) * radius,
                Mathf.Sin(angle) * radius,
                coneLength
            );
            // Base ring — fully transparent
            colors[i + 1] = new Color(1f, 1f, 0.7f, 0f);
        }

        for (int i = 0; i < segments; i++)
        {
            triangles[i * 3]     = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        Mesh mesh      = new Mesh();
        mesh.name      = "LightCone";
        mesh.vertices  = vertices;
        mesh.colors    = colors;       
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        return mesh;
    }
}