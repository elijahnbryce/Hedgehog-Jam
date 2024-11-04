using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PixelLineRenderer : MonoBehaviour
{
    private LineRenderer lineRenderer;
    [SerializeField] private float endSegmentSize = 0.1f; // Size of the non-stretching end segments
    [SerializeField] private Material lineMaterial;
    [SerializeField] private float lineWidth = 0.1f;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.material = lineMaterial;
        lineRenderer.widthMultiplier = lineWidth;

        // Set up for 4 points (start cap, start stretch, end stretch, end cap)
        lineRenderer.positionCount = 4;

        // Enable texture mode for clean pixel art
        lineRenderer.textureMode = LineTextureMode.Stretch;
    }

    public void UpdateLine(Vector3 startPoint, Vector3 endPoint)
    {
        Vector3 direction = (endPoint - startPoint).normalized;
        float totalLength = Vector3.Distance(startPoint, endPoint);

        // Calculate positions for the four points
        Vector3 startCap = startPoint;
        Vector3 startStretch = startPoint + direction * endSegmentSize;
        Vector3 endStretch = endPoint - direction * endSegmentSize;
        Vector3 endCap = endPoint;

        // Set the positions
        lineRenderer.SetPosition(0, startCap);
        lineRenderer.SetPosition(1, startStretch);
        lineRenderer.SetPosition(2, endStretch);
        lineRenderer.SetPosition(3, endCap);

        // Adjust UV coordinates to maintain end textures
        lineRenderer.material.mainTextureScale = new Vector2(totalLength / lineWidth, 1);
    }

    // Helper method to set texture coordinates for fixed ends
    private void SetTextureScale(float totalLength)
    {
        // Calculate the texture scale based on the total length and end segment size
        float middleSection = totalLength - (endSegmentSize * 2);
        float textureScale = middleSection / lineWidth;

        // Set texture tiling
        lineRenderer.material.mainTextureScale = new Vector2(textureScale, 1);
    }
}