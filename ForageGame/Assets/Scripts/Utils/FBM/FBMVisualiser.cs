using UnityEngine;

public class FBMVisualiser : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private MeshRenderer backgroundPlane;
    [SerializeField] private int points = 100;
    [SerializeField] FBM1D fbmFunction = new FBM1D();

    [SerializeField] private float minX = 0f;
    [SerializeField] private float maxX = 100f;

    [SerializeField] [Range(0, 10f)] private float scrollSpeed = 1f;

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
        float width = backgroundPlane.bounds.size.x;
        float height = backgroundPlane.bounds.size.y;


        lineRenderer.positionCount = points;
        for (int i = 0; i < points; i++)
        {
            float val = i / (float)(points - 1) * maxx + minx + Time.time * scrollSpeed;
            float x = (i - points / 2) / (float)(points - 1) * width;
            float y = fbmFunction.EvalMin11(val) * (height / 2f);
            lineRenderer.SetPosition(i, backgroundPlane.transform.position + new Vector3(x, y, 0f));
        }
    }


}
