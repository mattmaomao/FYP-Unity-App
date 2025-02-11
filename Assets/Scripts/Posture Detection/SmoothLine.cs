using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class SmoothLine : MonoBehaviour
{
    public Gradient lineGradient;

    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void drawLine(List<Vector3> points)
    {
        lineRenderer.positionCount = points.Count * 10; // Increase the resolution for a smoother curve

        // Set the color gradient for the line
        lineRenderer.colorGradient = lineGradient;

        for (int i = 0; i < points.Count - 1; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                float t = j / 9.0f;
                Vector2 newPos = CatmullRomSplineInterpolation(points[WrapIndex(i - 1, points.Count)],
                                                                points[i],
                                                                points[WrapIndex(i + 1, points.Count)],
                                                                points[WrapIndex(i + 2, points.Count)],
                                                                t);

                lineRenderer.SetPosition(i * 10 + j, newPos);
            }
        }
    }

    // Catmull-Rom spline interpolation
    private Vector2 CatmullRomSplineInterpolation(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;

        float b0 = 0.5f * (-t3 + 2.0f * t2 - t);
        float b1 = 0.5f * (3.0f * t3 - 5.0f * t2 + 2.0f);
        float b2 = 0.5f * (-3.0f * t3 + 4.0f * t2 + t);
        float b3 = 0.5f * (t3 - t2);

        return b0 * p0 + b1 * p1 + b2 * p2 + b3 * p3;
    }

    private int WrapIndex(int index, int max)
    {
        if (index < 0)
        {
            return max + index;
        }
        if (index >= max)
        {
            return index - max;
        }
        return index;
    }
}