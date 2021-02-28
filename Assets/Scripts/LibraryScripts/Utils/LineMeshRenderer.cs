using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class LineSegment
{
    public LineSegment(Vector3 startP, Vector3 endP)
	{
        start = startP;
        end = endP;
        segmentObject = null;
	}

    public LineSegment(Vector3 startP, Vector3 endP, GameObject obj)
    {
        start = startP;
        end = endP;
        segmentObject = obj;
    }

    public Vector3 start;
    public Vector3 end;
    public GameObject segmentObject;
}

public class LineMeshRenderer : MonoBehaviour
{
    public delegate void SegmentComplete();
    public event SegmentComplete SegmentDrawn;
    public delegate void LineComplete();
    public event LineComplete LineDrawn;
    public delegate void SegmentEraseComplete();
    public event SegmentEraseComplete SegmentErased;
    public delegate void LineEraseComplete();
    public event LineEraseComplete LineErased;

    public float LineDrawingSpeedUnitsPerSecond = 1.0f;
    public float PointReachedThreshold = 0.1f;
    public float LineThickness = 0.1f;
    public List<Vector3> Positions = new List<Vector3>();

    Vector3 currentSegmentDirection = Vector3.zero;
    List<LineSegment> lineSegments = new List<LineSegment>();
    Material lineObjectsMaterial;

    bool drawingLine = false;
    bool erasingLine = false;

    public string textureName = "White";
    public string TextureName
    {
        get
        {
            return textureName;
        }
        set
        {
            textureName = value;
            UpdateTexture();
        }
    }

    void UpdateTexture()
    {
        Material m = (Material)Resources.Load(TextureName, typeof(Material));
        if (m != null)
            lineObjectsMaterial = m;
    }

    void Awake()
    {
        UpdateTexture();
    }

    void AddSegment(Vector3 start, Vector3 end)
	{
        GameObject segmentObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        segmentObject.GetComponent<MeshRenderer>().material = lineObjectsMaterial;
        Destroy(segmentObject.GetComponent<BoxCollider>());
        lineSegments.Add(new LineSegment(start, end, segmentObject));
        UpdateLastSegmentObject();
    }

    void RemoveLineSegment(int index)
	{
        Assert.IsTrue(index < lineSegments.Count);
        Destroy(lineSegments[index].segmentObject);
        lineSegments.RemoveAt(index);
	}

    public void RenderLineImmediate()
    {
        for (int p = 0; p < Positions.Count - 1; ++p)
		{
            AddSegment(Positions[p], Positions[p + 1]);
        }
    }

    public void ErasePathImmediate()
    {
        foreach (LineSegment segment in lineSegments)
		{
            GameObject.Destroy(segment.segmentObject);
		}
        lineSegments.Clear();
    }

    public void BeginDrawingLine()
    {
        ErasePathImmediate();
        if (Positions.Count > 1)
        {
            AddSegment(Positions[0], Positions[0]);
            currentSegmentDirection = Positions[lineSegments.Count] - lineSegments[lineSegments.Count - 1].start;
            currentSegmentDirection.Normalize();
            drawingLine = true;
            StartCoroutine(LineDraw());
        }
    }

    public void BeginErasingLine()
    {
        if (lineSegments.Count > 0)
        {
            LineSegment segment = lineSegments[lineSegments.Count - 1];
            currentSegmentDirection = segment.start - segment.end;

            currentSegmentDirection.Normalize();
            erasingLine = true;
            StartCoroutine(LineErase());
        }
    }

    void UpdateLastSegmentObject()
    {
        LineSegment segment = lineSegments[lineSegments.Count - 1];
        if (segment.segmentObject == null)
            return;
        Vector3 forward = (segment.end - segment.start).normalized;
        Vector3 pos = segment.start + (segment.end - segment.start) * 0.5f;
        segment.segmentObject.transform.position = pos;
        segment.segmentObject.transform.LookAt(pos + forward);
        float localZScale = (segment.end - segment.start).magnitude;
        segment.segmentObject.transform.localScale = new Vector3(LineThickness, LineThickness, localZScale);
    }

    void FinishedDrawingSegment()
    {
        if (lineSegments.Count < Positions.Count - 1)
        {
            SegmentDrawn?.Invoke();
            Vector3 nextSegmentStart = lineSegments[lineSegments.Count - 1].end;
            AddSegment(nextSegmentStart, nextSegmentStart);
            currentSegmentDirection = Positions[lineSegments.Count] - lineSegments[lineSegments.Count - 1].start;
            currentSegmentDirection.Normalize();
        }
        else
        {
            LineDrawn?.Invoke();
            drawingLine = false;
        }
    }

    void FinishedErasingSegment()
    {
        if (lineSegments.Count > 0)
        {
            SegmentErased?.Invoke();
            LineSegment segment = lineSegments[lineSegments.Count - 1];
            currentSegmentDirection = segment.start - segment.end;
            currentSegmentDirection.Normalize();
        }
        else
        {
            LineErased?.Invoke();
            erasingLine = false;
        }
    }

	IEnumerator LineDraw()
	{
		while (drawingLine)
		{
            LineSegment segment = lineSegments[lineSegments.Count - 1];
            segment.end = segment.end + currentSegmentDirection * Time.deltaTime * LineDrawingSpeedUnitsPerSecond;
            float renderMagnitude = (segment.end - segment.start).magnitude;
            float targetMagnitude = (Positions[lineSegments.Count] - Positions[lineSegments.Count - 1]).magnitude;
            if (renderMagnitude > targetMagnitude || Mathf.Abs(targetMagnitude - renderMagnitude) < PointReachedThreshold)
            {
                segment.end = Positions[lineSegments.Count];
                UpdateLastSegmentObject();
                FinishedDrawingSegment();
            }
            UpdateLastSegmentObject();
            yield return null;
		}
		yield break;
	}

	IEnumerator LineErase()
	{
		while (erasingLine)
		{
            LineSegment segment = lineSegments[lineSegments.Count - 1];
            segment.end = segment.end + currentSegmentDirection * Time.deltaTime * LineDrawingSpeedUnitsPerSecond;
            float renderMagnitude = (segment.end - segment.start).magnitude;
            float targetMagnitude = 0.0f;
            Vector3 currentUnitVec = (segment.start - segment.end).normalized;
            if (Mathf.Approximately(Vector3.Dot(currentSegmentDirection, currentUnitVec), -1.0f) || Mathf.Abs(targetMagnitude - renderMagnitude) < PointReachedThreshold)
            {
                RemoveLineSegment(lineSegments.Count - 1);
                FinishedErasingSegment();
            }
            else
            {
                UpdateLastSegmentObject();
            }
            yield return null;
		}
		yield break;
	}
}