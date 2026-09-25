using UnityEngine;

public class FBMVisualiser : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private MeshRenderer backgroundPlane;
    [SerializeField] private int points = 100;
    [SerializeField] FBM1D fbmFunction = new FBM1D();

    [SerializeField] private float minX = 0f;
    [SerializeField] private float maxX = 100f;

    [SerializeField][Range(0, 10f)] private float scrollSpeed = 1f;

    private void Start()
    {
        lineRenderer.useWorldSpace = true;
    }

    private void Update()
    {
        DrawFBM(minX, maxX);
    }

    private void DrawFBM(float minx, float maxx)
    {
        lineRenderer.positionCount = points;
        for (int i = 0; i < points; i++)
        {
            lineRenderer.SetPosition(i, backgroundPlane.transform.position + new Vector3(
                (i - points / 2) / (float)(points - 1) * backgroundPlane.bounds.size.x,
                fbmFunction.EvalMin11(i / (float)(points - 1) * maxx + minx + Time.time * scrollSpeed) * (backgroundPlane.bounds.size.y / 2f),
                0f));
        }
    }
    // float val = i / (float)(points - 1) * maxx + minx + Time.time * scrollSpeed;
    // float x = (i - points / 2) / (float)(points - 1) * _width;
    // float y = fbmFunction.EvalMin11(val) * (_height / 2f);


}
