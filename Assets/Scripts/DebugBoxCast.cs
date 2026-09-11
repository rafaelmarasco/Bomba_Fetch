/*
 * DebugBoxCast.cs
 * Created by Arian - GameDevBox
 * YouTube Channel: https://www.youtube.com/@GameDevBox
 *
 * 🎮 Want more Unity tips, tools, and advanced systems?
 * 🧠 Learn from practical examples and well-explained logic.
 * 📦 Subscribe to GameDevBox for more game dev content!
 *
 * 🔍 A powerful utility for visualizing BoxCasts in the Unity editor.
 * 📦 This static class provides debug tools to draw BoxCasts in the Scene and Game views using Debug.DrawLine.
 * 🎯 Supports drawing both hit and non-hit states, including wireframes at cast start/end positions.
 * 🧰 Useful for understanding physics cast behavior, debugging collision issues, and refining detection logic.
 * 
 * Features:
 * - Draw full BoxCast paths with color-coding for hit/miss.
 * - Draw only the collision point box.
 * - Easily rotate boxes to match cast orientation.
 * - Efficient utility methods to calculate box corners and interpolate cast endpoints.
 *
 * 💡 Tip: Use this during development to visually inspect BoxCast direction, rotation, size, and hit responses.
 */

using UnityEngine;

public static class DebugBoxCast
{
    #region Configuration && Box Structure
    public static Color HitColor = Color.red;
    public static Color NoHitColor = Color.green;
    public struct Box
    {
        public Vector3 localFrontTopLeft { get; private set; }
        public Vector3 localFrontTopRight { get; private set; }
        public Vector3 localFrontBottomLeft { get; private set; }
        public Vector3 localFrontBottomRight { get; private set; }
        public Vector3 localBackTopLeft { get { return -localFrontBottomRight; } }
        public Vector3 localBackTopRight { get { return -localFrontBottomLeft; } }
        public Vector3 localBackBottomLeft { get { return -localFrontTopRight; } }
        public Vector3 localBackBottomRight { get { return -localFrontTopLeft; } }

        public Vector3 frontTopLeft { get { return localFrontTopLeft + origin; } }
        public Vector3 frontTopRight { get { return localFrontTopRight + origin; } }
        public Vector3 frontBottomLeft { get { return localFrontBottomLeft + origin; } }
        public Vector3 frontBottomRight { get { return localFrontBottomRight + origin; } }
        public Vector3 backTopLeft { get { return localBackTopLeft + origin; } }
        public Vector3 backTopRight { get { return localBackTopRight + origin; } }
        public Vector3 backBottomLeft { get { return localBackBottomLeft + origin; } }
        public Vector3 backBottomRight { get { return localBackBottomRight + origin; } }

        public Vector3 origin { get; private set; }

        public Box(Vector3 origin, Vector3 halfExtents, Quaternion orientation) : this(origin, halfExtents)
        {
            Rotate(orientation);
        }
        public Box(Vector3 origin, Vector3 halfExtents)
        {
            this.localFrontTopLeft = new Vector3(-halfExtents.x, halfExtents.y, -halfExtents.z);
            this.localFrontTopRight = new Vector3(halfExtents.x, halfExtents.y, -halfExtents.z);
            this.localFrontBottomLeft = new Vector3(-halfExtents.x, -halfExtents.y, -halfExtents.z);
            this.localFrontBottomRight = new Vector3(halfExtents.x, -halfExtents.y, -halfExtents.z);

            this.origin = origin;
        }


        public void Rotate(Quaternion orientation)
        {
            localFrontTopLeft = RotatePointAroundPivot(localFrontTopLeft, Vector3.zero, orientation);
            localFrontTopRight = RotatePointAroundPivot(localFrontTopRight, Vector3.zero, orientation);
            localFrontBottomLeft = RotatePointAroundPivot(localFrontBottomLeft, Vector3.zero, orientation);
            localFrontBottomRight = RotatePointAroundPivot(localFrontBottomRight, Vector3.zero, orientation);
        }
    }

    #endregion

    #region Public API

    //Draws just the box at where it is currently hitting.
    public static void DrawBoxCastOnHit(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Vector3 direction, float hitInfoDistance, Color color)
    {
        origin = CastCenterOnCollision(origin, direction, hitInfoDistance);
        SimpleDrawBox(origin, halfExtents, orientation, color);
    }

    //Draws the full box from start of cast to its end distance. Can also pass in hitInfoDistance instead of full distance
    public static void SimpleDrawBoxCast(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Vector3 direction, float distance, Color color)
    {
        direction.Normalize();
        Box bottomBox = new Box(origin, halfExtents, orientation);
        Box topBox = new Box(origin + (direction * distance), halfExtents, orientation);

        Debug.DrawLine(bottomBox.backBottomLeft, topBox.backBottomLeft, color);
        Debug.DrawLine(bottomBox.backBottomRight, topBox.backBottomRight, color);
        Debug.DrawLine(bottomBox.backTopLeft, topBox.backTopLeft, color);
        Debug.DrawLine(bottomBox.backTopRight, topBox.backTopRight, color);
        Debug.DrawLine(bottomBox.frontTopLeft, topBox.frontTopLeft, color);
        Debug.DrawLine(bottomBox.frontTopRight, topBox.frontTopRight, color);
        Debug.DrawLine(bottomBox.frontBottomLeft, topBox.frontBottomLeft, color);
        Debug.DrawLine(bottomBox.frontBottomRight, topBox.frontBottomRight, color);

        DrawBox(bottomBox, color);
        DrawBox(topBox, color);
    }

    public static void SimpleDrawBox(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Color color)
    {
        DrawBox(new Box(origin, halfExtents, orientation), color);
    }


    // Main draw method with separate hit/non-hit colors
    public static void AdvancedDrawBoxCast(Vector3 origin, Vector3 size, Quaternion rotation, Vector3 direction,
                          float maxDistance, bool hasHit = false, float hitDistance = 0,
                          Color? hitColor = null, Color? noHitColor = null)
    {
        Color drawColor = hasHit ? (hitColor ?? HitColor) : (noHitColor ?? NoHitColor);
        Color lineColor = hasHit ? (hitColor ?? HitColor) : (noHitColor ?? NoHitColor);

        // Calculate end position
        Vector3 endPosition = hasHit ?
            origin + direction.normalized * hitDistance :
            origin + direction.normalized * maxDistance;

        // Draw boxes (start box always in no-hit color, end box in hit color if applicable)
        DrawBox(origin, size, rotation, noHitColor ?? NoHitColor);
        DrawBox(endPosition, size, rotation, drawColor);

        // Draw connecting lines
        Vector3[] startCorners = GetBoxCorners(origin, size, rotation);
        Vector3[] endCorners = GetBoxCorners(endPosition, size, rotation);

        for (int i = 0; i < 8; i++)
        {
            Debug.DrawLine(startCorners[i], endCorners[i], lineColor);
        }
    }

    #endregion

    #region Helper Methods

    //This should work for all cast types
    private static Vector3 CastCenterOnCollision(Vector3 origin, Vector3 direction, float hitInfoDistance)
    {
        return origin + (direction.normalized * hitInfoDistance);
    }
    //Rotate BoxCasting
    private static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation)
    {
        Vector3 direction = point - pivot;
        return pivot + rotation * direction;
    }

    // Draw a single wireframe box
    private static void DrawBox(Box box, Color color)
    {
        Debug.DrawLine(box.frontTopLeft, box.frontTopRight, color);
        Debug.DrawLine(box.frontTopRight, box.frontBottomRight, color);
        Debug.DrawLine(box.frontBottomRight, box.frontBottomLeft, color);
        Debug.DrawLine(box.frontBottomLeft, box.frontTopLeft, color);

        Debug.DrawLine(box.backTopLeft, box.backTopRight, color);
        Debug.DrawLine(box.backTopRight, box.backBottomRight, color);
        Debug.DrawLine(box.backBottomRight, box.backBottomLeft, color);
        Debug.DrawLine(box.backBottomLeft, box.backTopLeft, color);

        Debug.DrawLine(box.frontTopLeft, box.backTopLeft, color);
        Debug.DrawLine(box.frontTopRight, box.backTopRight, color);
        Debug.DrawLine(box.frontBottomRight, box.backBottomRight, color);
        Debug.DrawLine(box.frontBottomLeft, box.backBottomLeft, color);
    }

    // Draw a single wireframe box
    private static void DrawBox(Vector3 center, Vector3 size, Quaternion rotation, Color color)
    {
        Vector3[] corners = GetBoxCorners(center, size, rotation);

        // Bottom square
        Debug.DrawLine(corners[0], corners[1], color);
        Debug.DrawLine(corners[1], corners[3], color);
        Debug.DrawLine(corners[3], corners[2], color);
        Debug.DrawLine(corners[2], corners[0], color);

        // Top square
        Debug.DrawLine(corners[4], corners[5], color);
        Debug.DrawLine(corners[5], corners[7], color);
        Debug.DrawLine(corners[7], corners[6], color);
        Debug.DrawLine(corners[6], corners[4], color);

        // Vertical lines
        Debug.DrawLine(corners[0], corners[4], color);
        Debug.DrawLine(corners[1], corners[5], color);
        Debug.DrawLine(corners[2], corners[6], color);
        Debug.DrawLine(corners[3], corners[7], color);
    }

    // Get all 8 corners of a box
    private static Vector3[] GetBoxCorners(Vector3 center, Vector3 size, Quaternion rotation)
    {
        Vector3[] corners = new Vector3[8];
        Vector3 extents = size / 2f;

        for (int i = 0; i < 8; i++)
        {
            float x = ((i & 1) == 0) ? -extents.x : extents.x;
            float y = ((i & 2) == 0) ? -extents.y : extents.y;
            float z = ((i & 4) == 0) ? -extents.z : extents.z;
            corners[i] = center + rotation * new Vector3(x, y, z);
        }

        return corners;
    }

    #endregion
}
