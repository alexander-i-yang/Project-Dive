using Core;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MyBox;
using UnityEditor;
using UnityEngine;

namespace Helpers {
    /// <summary>
    /// Static helper methods. Add whatever you want!
    /// </summary>
    public static class Helper
    {
        public static float ActualLerp(float a, float b, float t)
        {
            return a + (b - a) * t;
        }

        public static Vector3 ActualLerp(Vector3 a, Vector3 b, float t)
        {
            return new Vector3(ActualLerp(a.x, b.x, t), ActualLerp(a.y, b.y, t), ActualLerp(a.z, b.z, t));
        }

        public static void DrawArrow(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f,
            float arrowHeadAngle = 20.0f)
        {
            if (direction.Equals(Vector2.zero))
            {
                return;
            }

            Gizmos.color = color;
            Gizmos.DrawRay(pos, direction);

            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) *
                            new Vector3(0, 0, 1);
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) *
                           new Vector3(0, 0, 1);
            Gizmos.DrawRay(pos + direction / 2, right * arrowHeadLength);
            Gizmos.DrawRay(pos + direction / 2, left * arrowHeadLength);
        }

        public static String DictToString<Key, Val>(Dictionary<Key, Val> d)
        {
            return "{" + string.Join(",", d.Select(kv => kv.Key + "=" + kv.Value).ToArray()) + "}";
        }

        public static float RoundHalf(double value)
        {
            return (float)(Math.Round(value * 2) / 2);
        }

        public static Vector2 RoundVectorHalf(Vector2 v)
        {
            return new Vector2(RoundHalf(v.x), RoundHalf(v.y));
        }

        public static Vector2 RotateVector2(Vector2 v, float angleRad)
        {
            float sin = Mathf.Sin(angleRad);
            float cos = Mathf.Cos(angleRad);
            float _x = v.x * cos - v.y * sin;
            float _y = v.x * sin + v.y * cos;
            return new Vector2(_x, _y);
        }

        /// <summary>
        /// At a given position, casts a vector up and down the z axis to find Components of type <typeparamref name="T"/>
        /// </summary>
        /// <param name="pos">Position</param>
        /// <typeparam name="T">Component</typeparam>
        /// <returns>Component to find</returns>
        public static T OnComponent<T>(Vector3 pos) where T : MonoBehaviour
        {
            RaycastHit2D[] found = Physics2D.RaycastAll(
                pos,
                new Vector3(0, 0, 1),
                LayerMask.GetMask("Interactable")
            );
            T highestComponent = default(T);
            foreach (RaycastHit2D curCol in found)
            {
                T interact = curCol.transform.GetComponent<T>();
                if (interact != null)
                {
                    if (highestComponent == null ||
                        interact.transform.position.z < highestComponent.transform.position.z)
                    {
                        highestComponent = interact;
                    }
                }
            }

            return highestComponent;
        }

        public static List<T> OnComponents<T>(Vector3 pos) where T : MonoBehaviour
        {
            RaycastHit2D[] found = Physics2D.RaycastAll(
                pos,
                new Vector3(0, 0, 1),
                LayerMask.GetMask("Interactable")
            );
            List<T> components = new List<T>();
            foreach (RaycastHit2D curCol in found)
            {
                components.Add(curCol.transform.GetComponent<T>());
            }

            return components;
        }

        public static double Clamp(double low, double high, double a)
        {
            return Math.Min(Math.Max(a, low), high);
        }

        public static Color ColorLerp(Color a, Color b, float t)
        {
            Color c = Color.Lerp(a, b, t);
            c.a = ActualLerp(a.a, b.a, t);
            return c;
        }

        public static IEnumerator Fade(SpriteRenderer sr, float time, Color newColor, Action<int> done = null)
        {
            Color origColor = sr.color;
            for (float ft = 0; ft < time; ft += Game.Instance.IsPaused ? 0 : Time.deltaTime)
            {
                sr.color = ColorLerp(origColor, newColor, ft / time);
                yield return null;
            }

            if (done != null) done(0);
        }

        public static IEnumerator Shake(Transform t, float duration = 0.5f, float speed = 100f, float amount = 0.05f)
        {
            Vector3 origPos = t.localPosition;
            for (float ft = 0; ft < duration; ft += Game.Instance.IsPaused ? 0 : Time.deltaTime)
            {
                t.localPosition = origPos + new Vector3(Mathf.Sin(ft / duration * speed) * amount,
                    Mathf.Sin(ft * speed) * amount);
                yield return null;
            }

            t.localPosition = origPos;
        }

        #if UNITY_EDITOR
        public static void DrawText(Vector3 pos, string text, Color col = default)
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = col == default ? Color.white : col;
            Handles.Label(pos, text, style);
        }
        #endif

        public static IEnumerator Delay(float delayTime)
        {
            yield return new WaitForSeconds(delayTime / Game.Instance.TimeScale);
        }

        public static IEnumerator DelayAction(float delayTime, Action act) {
            //Wait for the specified delay time before continuing.
            yield return new WaitForSeconds(delayTime / Game.Instance.TimeScale);

            //Do the action after the delay time has finished.
            act();
        }

        public static Color TiledColorStringToColor(string color)
        {
            color = $"#{color.Substring(3, 6)}{color.Substring(1, 2)}";
            Color c;
            if (!ColorUtility.TryParseHtmlString(color, out c))
            {
                throw new ArgumentException($"Invalid hex code for Sprite Light: {color}");
            }

            return c;
        }

        public static Vector2 ComputeAverage(this Vector2[] points)
        {
            Vector2 avg = points.Aggregate((acc, cur) => acc + cur);
            return avg / points.Length;
        }

        public static Vector2[] ComputeNormalized(this Vector2[] points, Vector2 avg = default) {
            if (avg == default) avg = points.ComputeAverage();
            for (int i = 0; i < points.Length; ++i) {
                points[i] -= avg;
            }

            return points;
        }

        public static Vector3[] ToVector3(this Vector2[] p) {
            return Array.ConvertAll(p, item => (Vector3)item);
        }

        public static String ValuesToString(this Vector2[] p) {
            String ret = "";
            foreach (var v in p) {
                ret += $"<{v.x}, {v.y}> ";
            }
            return ret;
        }

        public static T[] Slice<T>(this T[] arr, int from, int len) {
            return new System.ArraySegment<T>(arr, 0, len).ToArray();
        }
        
        public static List<Transform> GetChildren(this Transform t) => t.GetChildsWhere(_ => true);
        
        
        /// <summary>
        /// Gets all children first, then iterates over those.
        /// </summary>
        /// <param name="t">Transform to get children from</param>
        /// <param name="f">Function to perform</param>
        public static void ForEachChild(this Transform t, Action<GameObject, int> f) {
            List<Transform> children = t.GetChildren();
            for (int i = 0; i < children.Count; ++i) {
                f(children[i].gameObject, i);
            }
        }
    }
}