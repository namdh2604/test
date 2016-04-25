using System;
using UnityEngine;
using UnityEngine.UI;

namespace Voltage.Witches.Models.Avatar
{
    public class ImageBoundCalculator
    {
        public ImageBoundCalculator()
        {
        }

        // Given a rect transform, this will compute the bounds of this component and its children, relative to the parent
        // Returns a rect where the x & y indicate the center of the rectangle, and the width & height are as normal
        public Rect computeBounds(RectTransform rt)
        {
            Canvas canvas = rt.GetComponentInParent<Canvas>();
            RectTransform parentBounds = canvas.transform as RectTransform;
            Quad corners = getRawCorners(rt);

            corners = cropCornersToScreen(corners, new Vector2(parentBounds.rect.width, parentBounds.rect.height));

            float x = (corners.x_max + corners.x_min) / 2.0f;
            float y = (corners.y_max + corners.y_min) / 2.0f;
            Vector2 center = new Vector2(x, y);

            return new Rect(center.x, center.y, corners.x_max - corners.x_min, corners.y_max - corners.y_min);
        }

        // takes the given bounds (as returned by computeBounds) and converts them to scaled screen coordinates, matching the requestedResolution
        // The canvas is necessary, as it provides the scale information when converting between canvas and screen coordinates
        public Rect convertToScreenBounds(Canvas canvas, Rect canvasBounds, Vector2 requestedResolution)
        {
            Camera camera = canvas.worldCamera;

            GameObject dummyGO = new GameObject("Temp");
            RectTransform dummyRT = dummyGO.AddComponent<RectTransform>();
            dummyRT.anchoredPosition = new Vector2(canvasBounds.x, canvasBounds.y);
            dummyRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, canvasBounds.width);
            dummyRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, canvasBounds.height);
            dummyRT.SetParent(canvas.transform, false);

            Vector3[] corners = new Vector3[4];

            dummyRT.GetWorldCorners(corners);
            GameObject.Destroy(dummyGO);
            Vector3 minPoint = new Vector3(corners[0].x, corners[0].y, 0.0f);
            Vector3 maxPoint = new Vector3(corners[2].x, corners[2].y, 0.0f);

            minPoint = camera.WorldToScreenPoint(minPoint);
            maxPoint = camera.WorldToScreenPoint(maxPoint);

            if (minPoint.x < 0)
            {
                minPoint.x = 0;
            }
            if (minPoint.y < 0)
            {
                minPoint.y = 0;
            }

            if (maxPoint.x > camera.pixelWidth)
            {
                maxPoint.x = camera.pixelWidth;
            }
            if (maxPoint.y > camera.pixelHeight)
            {
                maxPoint.y = camera.pixelHeight;
            }

            Vector2 scaledMinPoint = new Vector2(minPoint.x * requestedResolution.x / camera.pixelWidth, minPoint.y * requestedResolution.y / camera.pixelHeight);
            Vector2 scaledMaxPoint = new Vector2(maxPoint.x * requestedResolution.x / camera.pixelWidth, maxPoint.y * requestedResolution.y / camera.pixelHeight);

            return new Rect(scaledMinPoint.x, scaledMinPoint.y, scaledMaxPoint.x - scaledMinPoint.x, scaledMaxPoint.y - scaledMinPoint.y);
        }

        // Because image dimensions must be integers, this normalizes the float coordinates to align with pixel boundaries
        // Expects a rect with screen boundaries, such as that provided by convertToScreenBounds. The x and y should indicate the top left corner of the area
        public Rect alignScreenCoordsToPixels(Rect coords)
        {
            int width = (int)Math.Ceiling(coords.width);
            int height = (int)Math.Ceiling(coords.height);

            int x = (int)Math.Floor(coords.x);
            int y = (int)Math.Floor(coords.y);

            if (x < 0) { x = 0; }
            if (y < 0) { y = 0; }

            return new Rect(x, y, width, height);
        }

        // Takes screen coordinates and converts them to canvas ones.
        // The input screen coordinates should have the x & y indicate the top left corner.
        // The canvas is necessary for the scale information when converting between coordinates
        public Rect convertScreenCoordsToCanvasCoords(Rect screenCoords, Canvas canvas, Vector2 requestedResolution)
        {
            Camera camera = canvas.worldCamera;
            // scale the screen coordinates back to the canvas resolution
            Vector2 scaledMinPoint = new Vector2(screenCoords.x * camera.pixelWidth / requestedResolution.x, screenCoords.y * camera.pixelHeight / requestedResolution.y);
            Vector2 scaledMaxPoint = new Vector2((screenCoords.x + screenCoords.width) * camera.pixelWidth / requestedResolution.x, (screenCoords.y + screenCoords.height) * camera.pixelHeight / requestedResolution.y);

            Vector3 worldMin = camera.ScreenToWorldPoint(scaledMinPoint);
            Vector3 worldMax = camera.ScreenToWorldPoint(scaledMaxPoint);

            float width = worldMax.x - worldMin.x;
            float height = worldMax.y - worldMin.y;

            Vector3 worldCenter = new Vector3(worldMin.x + width / 2.0f, worldMin.y + height / 2.0f, worldMin.z);

            Vector3 canvasScale = canvas.transform.localScale;
            Vector3 scaledCenter = new Vector3(worldCenter.x / canvasScale.x, worldCenter.y / canvasScale.y, worldCenter.z / canvasScale.z);
            float scaledWidth = width / canvasScale.x;
            float scaledHeight = height / canvasScale.y;

            return new Rect(scaledCenter.x, scaledCenter.y, scaledWidth, scaledHeight);
        }

        private Quad getRawCorners(RectTransform rt)
        {
            Quad bounds = new Quad(rt.localPosition.x, rt.localPosition.y);

            computeCorners(rt, bounds, Vector2.zero, Vector2.one);

            return bounds;
        }

        private Quad cropCornersToScreen(Quad corners, Vector2 resolution)
        {
            Quad croppedCorners = new Quad(corners);

            if (croppedCorners.x_min < -resolution.x / 2.0f)
            {
                croppedCorners.x_min = -resolution.x / 2.0f;
            }

            if (croppedCorners.x_max > resolution.x / 2.0f)
            {
                croppedCorners.x_max = resolution.x / 2.0f;
            }

            if (croppedCorners.y_min < -resolution.y / 2.0f)
            {
                croppedCorners.y_min = -resolution.y / 2.0f;
            }

            if (croppedCorners.y_max > resolution.y / 2.0f)
            {
                croppedCorners.y_max = resolution.y / 2.0f;
            }

            return croppedCorners;
        }

        private void computeCorners(RectTransform rt, Quad existingBounds, Vector2 parentOffset, Vector2 parentScale)
        {
            Vector3[] corners = new Vector3[4];
            rt.GetLocalCorners(corners);

            Vector2 scale = new Vector2(parentScale.x * rt.localScale.x, parentScale.y * rt.localScale.y);

            Vector2 offsets = new Vector2(parentOffset.x + rt.localPosition.x * parentScale.x, parentOffset.y + rt.localPosition.y * parentScale.y);

            for (int i = 0; i < 4; ++i)
            {
                corners[i] = new Vector3(corners[i].x * scale.x + offsets.x, corners[i].y * scale.y + offsets.y, corners[i].z);
            }

            for (int i = 0; i < 4; ++i)
            {
                if (corners[i].x < existingBounds.x_min)
                {
                    existingBounds.x_min = corners[i].x;
                }

                if (corners[i].x > existingBounds.x_max)
                {
                    existingBounds.x_max = corners[i].x;
                }

                if (corners[i].y < existingBounds.y_min)
                {
                    existingBounds.y_min = corners[i].y;
                }

                if (corners[i].y > existingBounds.y_max)
                {
                    existingBounds.y_max = corners[i].y;
                }
            }

            foreach (Transform child in rt)
            {
                if (child.gameObject.activeSelf)
                {
                    computeCorners(child as RectTransform, existingBounds, offsets, scale);
                }
            }
        }
    }

    public class Quad
    {
        public float x_min, x_max;
        public float y_min, y_max;

        public Quad(Quad existing)
        {
            x_min = existing.x_min;
            x_max = existing.x_max;
            y_min = existing.y_min;
            y_max = existing.y_max;
        }

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
}
