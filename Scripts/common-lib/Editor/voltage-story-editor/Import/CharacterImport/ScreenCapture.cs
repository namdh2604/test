using UnityEngine;
using UnityEngine.UI;

using UnityEditor;
using System.IO;
using Voltage.Common.Utilities;
using Voltage.Witches.AssetManagement;

namespace Voltage.Story.Import.CharacterImport
{
	public class ScreenCapture
	{
	    private class Quad
	    {
	        public float x_min, x_max;
	        public float y_min, y_max;

	        public Quad(float x, float y)
	        {
	            x_min = x_max = x;
	            y_min = y_max = y;
	        }

			public override string ToString()
			{
				return string.Format("({0}, {1}, {2}, {3})", x_min, y_min, x_max, y_max);
			}
	    }

	    private Canvas _canvas;
		private Camera _camera;
	    private GameObject _anchor;
        private ICharacterBundleManager _manager;

	    private static readonly Vector2 RESOLUTION = new Vector2(2560, 1600);

	    public ScreenCapture(Canvas canvas, Camera camera, GameObject anchor)
	    {
	        _canvas = canvas;
			_camera = camera;
	        _anchor = anchor;
            _manager = new EditorCharacterBundleManager();
	    }

	    public void CreatePNG(string charName, string pose, string outfit, string expression, string destination)
	    {
			Camera prevCam = _canvas.worldCamera;
			_canvas.worldCamera = _camera;

			try
			{
		        GameObject go = new GameObject("pose", typeof(RectTransform));

		        CharacterPoses.DisplayPose(go, charName, pose, outfit, expression, _manager);
		        go.transform.SetParent(_anchor.transform, false);

		        _camera.Render();

		        Rect boundingRect = getBoundingRect(go.transform as RectTransform, _canvas, RESOLUTION);
		        Texture2D outputTexture = new Texture2D((int)boundingRect.width, (int)boundingRect.height, TextureFormat.RGB24, false);
		        RenderTexture.active = _camera.targetTexture;
		        outputTexture.ReadPixels(boundingRect, 0, 0);
		        RenderTexture.active = null;

		        byte[] bytes;
		        bytes = outputTexture.EncodeToPNG();

		        File.WriteAllBytes(destination, bytes);

		        GameObjectUtils.Destroy(go);
			}
			finally
			{
				_canvas.worldCamera = prevCam;
			}
	    }

	    /* 
	     * Finds the world-coordinate corners of a given game object
	     */
	    private Rect getBoundingRect(RectTransform rt, Canvas canvas, Vector2 requestedResolution)
	    {
	        Quad extents = new Quad(rt.position.x, rt.position.y);

	        extents = getExtents(rt, extents);

	        Vector3 minPoint = canvas.worldCamera.WorldToScreenPoint(new Vector3(extents.x_min, extents.y_min, 0));
	        Vector3 maxPoint = canvas.worldCamera.WorldToScreenPoint(new Vector3(extents.x_max, extents.y_max, 0));

	        Vector2 actualMin = new Vector2(minPoint.x * requestedResolution.x / canvas.worldCamera.pixelWidth, minPoint.y * requestedResolution.y / canvas.worldCamera.pixelHeight);
	        Vector2 actualMax = new Vector2(maxPoint.x * requestedResolution.x / canvas.worldCamera.pixelWidth, maxPoint.y * requestedResolution.y / canvas.worldCamera.pixelHeight);

	        return new Rect(actualMin.x, actualMin.y, actualMax.x - actualMin.x, actualMax.y - actualMin.y);
	    }

	    private Quad getExtents(RectTransform rt, Quad extents)
	    {
	        Vector3[] corners = new Vector3[4];
	        rt.GetWorldCorners(corners);

	        extents = getMinCorners(corners, extents);

	        foreach (var child in rt)
	        {
	            extents = getExtents(child as RectTransform, extents);
	        }

	        return extents;
	    }

	    private Quad getMinCorners(Vector3[] candidate, Quad extents)
	    {
	        if (candidate[0].x < extents.x_min)
	        {
	            extents.x_min = candidate[0].x;
	        }

	        if (candidate[2].x > extents.x_max)
	        {
	            extents.x_max = candidate[2].x;
	        }

	        if (candidate[0].y < extents.y_min)
	        {
	            extents.y_min = candidate[0].y;
	        }

	        if (candidate[1].y > extents.y_max)
	        {
	            extents.y_max = candidate[1].y;
	        }

	        return extents;
	    }
	}
}
