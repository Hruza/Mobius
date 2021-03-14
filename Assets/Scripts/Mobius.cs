using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class Mobius : MonoBehaviour
{
    public float radius = 10;
    public float width = 2;

    public int widthResolution=10;
    public int lengthResolution=100;

    public static Mobius instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else Destroy(this);
    }

    static public void ChangeColor(Color toColor) {
        instance.StartCoroutine(instance.ColorChange(toColor));
    }

    public Material[] materials;

    IEnumerator ColorChange(Color toColor)
    {
        Color original = materials[0].color;
        foreach (Material material in materials)
        {
            material.SetColor("_Color2", toColor);
        }
        float t = -0.1f;
        while (t < 1.5)
        {
            foreach (Material material in materials)
            {
                material.SetFloat("transfer", t);
                t += Time.deltaTime / 2;
            }
            yield return new WaitForEndOfFrame();
        }
        foreach (Material material in materials)
        {
            material.color = toColor;
            material.SetFloat("transfer", -0.1f);
        }
    }

    public void GenerateMesh()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null) {
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));
        }

        MeshFilter meshFilter= GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
        }

        Mesh mesh = new Mesh();


        Vector3[] vertices = new Vector3[widthResolution*2*lengthResolution];
        Vector2[] uvs = new Vector2[widthResolution * 2 * lengthResolution];
        float fi = 0;
        float d = 0;
        for (int j = 0; j < 2*lengthResolution; j++)
        {
            for (int i = 0; i < widthResolution; i++)
            {
                fi = ((float)j / lengthResolution) * Mathf.PI;
                d = (((float)i / (widthResolution-1)) - 0.5f)*width;

                Vector3 normal = new Vector3(-(Mathf.Sin(fi)) * Mathf.Cos(2 * fi),
                            (Mathf.Cos(fi)),
                            -(Mathf.Sin(fi)) * Mathf.Sin(2 * fi)
                );

                vertices[(j*widthResolution)+i] = new Vector3((radius + (d * (Mathf.Cos(fi)))) * Mathf.Cos(2 * fi),
                                               d * (Mathf.Sin(fi)),
                                              (radius + (d * (Mathf.Cos(fi)))) * Mathf.Sin(2*fi)
                                              )+(normal*0.05f);
                uvs[(j * widthResolution) + i] = new Vector2((float)i / (widthResolution-1), (float)j / (2 * lengthResolution-1));

            }
        }

        mesh.vertices = vertices;

        int[] tris = new int[6*2*(widthResolution - 1) * (lengthResolution )];
        int index = 0;
        for (int i = 0; i <2* lengthResolution; i++)
        {
            for (int j = 0; j < widthResolution-1; j++)
            {
                index = 6 * (i*(widthResolution - 1) +j);
                try
                {
                    tris[index] = (i * widthResolution) + j;
                    tris[index + 2] = (i * widthResolution) + j + 1;
                    tris[index + 1] = (((i + 1)% (lengthResolution*2)) * widthResolution) + j;

                    tris[index + 3] = (i * widthResolution) + j+1;
                    tris[index + 5] = (((i + 1) % (lengthResolution*2)) * widthResolution) + j + 1;
                    tris[index + 4] = (((i + 1) % (lengthResolution*2)) * widthResolution) + j;
                }
                catch (System.IndexOutOfRangeException)
                {
                    Debug.LogError($"out of bounds: i: {i}, j: {j}, index:{index}");
                    throw;
                }
            }
        }

        mesh.triangles = tris;
        mesh.uv = uvs;

        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
    }
}
