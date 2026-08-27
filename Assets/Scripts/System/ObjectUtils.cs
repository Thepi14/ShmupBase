using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace ObjectUtils
{
    public static class GameObjectGeneral
    {
        /// <summary>
        /// Acha um GameObject por hierarquia nos transforms como se fosse um arquivo em pastas.
        /// </summary>
        /// <param name="start">Gameobject de referência.</param>
        /// <param name="path">Caminho do GameObject separados por '\' ou '/'.</param>
        /// <returns>O GameObject no caminho inserido.</returns>
        public static GameObject GetGameObject(this GameObject start, string path)
        {
            if (start == null)
                throw new System.ArgumentNullException("GameObject of name \'" + start.name + "\' is null");
            //lista de nomes
            List<string> names = new List<string>();
            //lista de caracteres do path
            char[] chars = path.ToCharArray();
            //nome temporário em chars
            List<char> temp = new List<char>();
            //iterar sobre os chars
            for (int i = 0; i < path.Length; i++)
            {
                //se for uma separação
                if (chars[i] == '\\' || chars[i] == '/')
                {
                    //adiciona um nome
                    names.Add(new string(temp.ToArray()));
                    //renova a temp
                    temp = new List<char>();
                    //continua
                    continue;
                }
                //adiciona novo char se não for uma separação
                temp.Add(chars[i]);
            }
            //última iteração adicionada como último destino do caminho.
            names.Add(new string(temp.ToArray()));
            //go se torna referência inicial
            GameObject go = start;
            //iterar sobre cada nome
            foreach (string name in names)
            {
                //se o filho de go for nulo
                if (go.transform.Find(name) == null)
                    Debug.LogWarning(name + " was not been found or it doesn't exist on the path: " + path);
                //se não for nulo, go é igual ao gameobject achado como filho do seu transform
                else
                    go = go.transform.Find(name).gameObject;
            }
            if (go == null)
                Debug.LogWarning("GameObject " + go.name + " has not been found on the end of the path: " + path);
            //retornar go
            return go;
        }

        /// <summary>
        /// Acha um GameObject por hierarquia nos transforms como se fosse um arquivo em pastas.
        /// </summary>
        /// <param name="start">Gameobject de referência.</param>
        /// <param name="path">Caminho do GameObject separados por '\' ou '/'.</param>
        /// <returns>O GameObject no caminho inserido.</returns>
        public static GameObject GetGameObject(this Component start, string path) => GetGameObject(start.gameObject, path);

        /// <summary>
        /// Acha o componente designado de um GameObject por hierarquia nos transforms como se fosse um arquivo em pastas.
        /// </summary>
        /// <typeparam name="T">O componente.</typeparam>
        /// <param name="start">Gameobject de referência.</param>
        /// <param name="path">Caminho do GameObject separados por '\' ou '/'.</param>
        /// <returns>O componente do GameObject no caminho inserido.</returns>
        public static T GetGameObjectComponent<T>(this GameObject start, string path)
        {
            var go = GetGameObject(start, path);
            if (go == null)
                Debug.LogWarning("GameObject has not been found on the path.");
            if (go.GetComponent<T>() == null)
                Debug.LogWarning("Component has not been found on " + go.name);
            //retornar go
            return go.GetComponent<T>();
        }

        /// <summary>
        /// Acha o componente designado de um GameObject por hierarquia nos transforms como se fosse um arquivo em pastas.
        /// </summary>
        /// <typeparam name="T">O componente.</typeparam>
        /// <param name="start">Gameobject de referência.</param>
        /// <param name="path">Caminho do GameObject separados por '\' ou '/'.</param>
        /// <returns>O componente do GameObject no caminho inserido.</returns>
        public static T GetGameObjectComponent<T>(this Component start, string path) => GetGameObjectComponent<T>(start.gameObject, path);

        /// <summary>
        /// Acha um componente em um filho de um GameObject parente.
        /// </summary>
        /// <typeparam name="T">O componente.</typeparam>
        /// <param name="parent">O GameObject parente.</param>
        /// <param name="childName">Nome do filho.</param>
        /// <returns>O componente do GameObject filho do parent escolhido com o nome inserido.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static T FindComponentInChild<T>(this GameObject parent, string childName)
        {
            if (parent == null)
                throw new System.ArgumentNullException("Parent of name \'" + parent.name + "\' is null");
            if (parent.transform.Find(childName) == null)
                Debug.LogWarning("Child of name \'" + childName + "\' was not found on the \'" + parent.name + "\' GameObject");
            if (parent.transform.Find(childName).GetComponent<T>() == null)
                Debug.LogWarning("Component was not been found in child of name \'" + childName + "\' of parent \'" + parent.name + "\'");
            return parent.transform.Find(childName).GetComponent<T>();
        }
        /// <summary>
        /// Acha um componente em um filho de um GameObject parente.
        /// </summary>
        /// <typeparam name="T">O componente.</typeparam>
        /// <param name="parent">O GameObject parente.</param>
        /// <param name="childName">Nome do filho.</param>
        /// <returns>O componente do GameObject filho do parent escolhido com o nome inserido.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static T FindComponentInChild<T>(this Component parent, string childName) => FindComponentInChild<T>(parent.gameObject, childName);

        public static GameObject[] GetGameObjectChildren(this GameObject gameObject)
        {
            var list = new List<GameObject>();
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                list.Add(gameObject.transform.GetChild(i).gameObject);
            }
            return list.ToArray();
        }

        public static GameObject[] GetGameObjectChildren(this Component gameObject) => GetGameObjectChildren(gameObject.gameObject);

        public static void DestroyGameObjecyChildren(this GameObject gameObject)
        {
            foreach (GameObject gameObjectChild in GetGameObjectChildren(gameObject))
            {
                UnityEngine.Object.Destroy(gameObject);
            }
        }

        public static void DestroyGameObjectChildren(this Component gameObject) => DestroyGameObjecyChildren(gameObject.gameObject);

        public static List<GameObject> FindObjects(Vector3 point, LayerMask layerMask, Func<GameObject, bool> condition, FindObjectsSortMode findObjectsSortMode = FindObjectsSortMode.None)
        {
            GameObject[] sceneObjects = UnityEngine.Object.FindObjectsByType<GameObject>(findObjectsSortMode);
            List<GameObject> result = new List<GameObject>();

            for (int i = 0; i < sceneObjects.Length; i++)
            {
                if (sceneObjects[i].layer == layerMask.value && condition(sceneObjects[i]))
                {
                    result.Add(sceneObjects[i]);
                }
            }
            return result;
        }

        public static List<GameObject> FindNearObjects(Vector3 point, LayerMask layerMask, float distance, FindObjectsSortMode findObjectsSortMode = FindObjectsSortMode.None)
        {
            GameObject[] sceneObjects = UnityEngine.Object.FindObjectsByType<GameObject>(findObjectsSortMode);
            List<GameObject> result = new List<GameObject>();

            for (int i = 0; i < sceneObjects.Length; i++)
            {
                if (sceneObjects[i].layer == layerMask.value)
                {
                    if (Vector3.Distance(sceneObjects[i].transform.position, point) < distance)
                    {
                        result.Add(sceneObjects[i]);
                    }
                }
            }
            return result;
        }

        public static List<GameObject> FindNearObjects(Vector3 point, LayerMask layerMask, float distance, Func<GameObject, bool> condition, FindObjectsSortMode findObjectsSortMode = FindObjectsSortMode.None)
        {
            GameObject[] sceneObjects = UnityEngine.Object.FindObjectsByType<GameObject>(findObjectsSortMode);
            List<GameObject> result = new List<GameObject>();

            for (int i = 0; i < sceneObjects.Length; i++)
            {
                if (sceneObjects[i].layer == layerMask.value && condition(sceneObjects[i]))
                {
                    if (Vector3.Distance(sceneObjects[i].transform.position, point) < distance)
                    {
                        result.Add(sceneObjects[i]);
                    }
                }
            }
            return result;
        }

        public static void SetFakeParent(this Transform child, Transform parent, Vector3 positionOffset, Quaternion rotationOffset)
        {
            child.position = parent.position;
            child.Translate(positionOffset, parent);

            child.rotation = parent.rotation * rotationOffset;
        }

        public static bool MaskContainsLayer(this LayerMask layerMask, string layerName) => layerMask == (layerMask | 1 << LayerMask.NameToLayer(layerName));

        public static bool MaskContainsLayer(this LayerMask layerMask, int layer) => layerMask == (layerMask | 1 << layer);

        public static bool MaskContainsLayer(this LayerMask layerMask, LayerMask layer) => layerMask == (layerMask | 1 << layer);
    }
    public static class UIGeneral
    {
        /// <returns>Retorna a posição do mouse com o fator de escala do canvas.</returns>
        [Obsolete]
        public static Vector2 MousePositionScaled(Canvas canvas)
        {
            return Mouse.current.position.ReadValue() * canvas.scaleFactor;
        }

        /// <returns>Retorna a posição do mouse.</returns>
        [Obsolete]
        public static Vector2 MousePosition()
        {
            return Mouse.current.position.ReadValue();
        }

        private const int UILayer = 5;

        /// <summary>
        /// Returns 'true' if touched or hovering on Unity UI element.
        /// </summary>
        /// <returns></returns>
        public static bool IsPointerOverUIElement() => IsPointerOverUIElement(GetEventSystemRaycastResults());

        /// <summary>
        /// Returns 'true' if touched or hovering on Unity UI element.
        /// </summary>
        /// <param name="eventSystemRaysastResults"></param>
        /// <returns></returns>
        private static bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
        {
            for (int index = 0; index < eventSystemRaysastResults.Count; index++)
            {
                RaycastResult curRaysastResult = eventSystemRaysastResults[index];
                if (curRaysastResult.gameObject.layer == UILayer)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Gets all event system raycast results of current mouse or touch position.
        /// </summary>
        /// <returns></returns>
        public static List<RaycastResult> GetEventSystemRaycastResults()
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;
            List<RaycastResult> raysastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raysastResults);
            return raysastResults;
        }
    }

    public static class LinqEx
    {
        public static void Add<T>(this IList<T> list, params T[] items)
        {
            foreach (T item in list)
            {
                list.Add(item);
            }
        }

        public static IList<T> Shuffle<T>(this IList<T> list)
        {
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            int n = list.Count;
            while (n > 1)
            {
                byte[] box = new byte[1];
                do provider.GetBytes(box);
                while (!(box[0] < n * (Byte.MaxValue / n)));
                int k = (box[0] % n);
                n--;
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

            return list;
        }

        public static IList<T> Shuffle<T>(this IList<T> list, System.Random random)
        {
            int n = list.Count;
            while (n > 1)
            {
                byte[] box = new byte[1];
                do random.NextBytes(box);
                while (!(box[0] < n * (Byte.MaxValue / n)));
                int k = (box[0] % n);
                n--;
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

            return list;
        }

        public static T GetRandom<T>(this IList<T> list, System.Random random)
        {
            if (list == null)
                throw new ArgumentNullException("list", "The provided list is null");
            if (list.Count == 0)
                throw new ArgumentException("Cant get a random item in a list that is empty!");

            return list[random.Next(0, list.Count - 1)];
        }

        public static T GetRandom<T>(this IList<T> list, Unity.Mathematics.Random random)
        {
            if (list == null)
                throw new ArgumentNullException("list", "The provided list is null");
            if (list.Count == 0)
                throw new ArgumentException("Cant get a random item in a list that is empty!");

            return list[random.NextInt(0, list.Count - 1)];
        }

        public static T MaxBy<T>(this IList<T> list, Func<T, int> predicate)
        {
            var biggest = int.MinValue;
            T value = list[0];

            foreach (T item in list)
            {
                var current = predicate(item);
                if (current > biggest)
                {
                    biggest = current;
                    value = item;
                }
            }

            return value;
        }

        public static T MaxBy<T>(this IList<T> list, Func<T, float> predicate)
        {
            var biggest = float.MinValue;
            T value = list[0];

            foreach (T item in list)
            {
                var current = predicate(item);
                if (current > biggest)
                {
                    biggest = current;
                    value = item;
                }
            }

            return value;
        }

        public static T MinBy<T>(this IList<T> list, Func<T, int> predicate)
        {
            var tinniest = int.MaxValue;
            T value = list[0];

            foreach (T item in list)
            {
                var current = predicate(item);
                if (current < tinniest)
                {
                    tinniest = current;
                    value = item;
                }
            }

            return value;
        }

        public static T MinBy<T>(this IList<T> list, Func<T, float> predicate)
        {
            var tinniest = float.MaxValue;
            T value = list[0];

            foreach (T item in list)
            {
                var current = predicate(item);
                if (current < tinniest)
                {
                    tinniest = current;
                    value = item;
                }
            }

            return value;
        }
    }

    public static class MathEx
    {
        /// <summary>
        /// Powers the value 'f' to two.
        /// </summary>
        public static float Pow2(float f)
        {
            return Mathf.Pow(f, 2);
        }

        /// <summary>
        /// Clamps value from 0 to 1.
        /// </summary>
        [Obsolete ("Use Mathf.Clamp01 instead.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static float Clamp(float f) => f < 0 ? 0 : f > 1 ? 1 : f;

        public static Vector2 RadianToVector2(float radian)
        {
            return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
        }
        public static Vector2 DegreeToVector2(float degree)
        {
            return RadianToVector2(degree * Mathf.Deg2Rad);
        }

        public static Vector2 AngleVectors(Vector2 a, Vector2 b)
        {
            return RadianToVector2(AngleRadian(a, b));
        }

        public static float AngleRadian(Vector2 a, Vector2 b)
        {
            return Mathf.Atan2(a.y - b.y, a.x - b.x);
        }
        public static float AngleDegrees(Vector2 a, Vector2 b)
        {
            return AngleRadian(a, b) * Mathf.Rad2Deg;
        }

        public static Vector3 Abs(this Vector3 v)
        {
            return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
        }

        public static Vector3 Abs(this Vector2 v)
        {
            return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y));
        }

        public static float Absolute(this Vector3 v)
        {
            var a = v.Abs();
            return a.x + a.y + a.z;
        }

        public static float Absolute(this Vector2 v)
        {
            var a = v.Abs();
            return a.x + a.y;
        }

        public static float AbsoluteDivided(this Vector3 v)
        {
            var a = v.Abs();
            return (a.x + a.y + a.z) / 3f;
        }

        public static float AbsoluteDivided(this Vector2 v)
        {
            var a = v.Abs();
            return (a.x + a.y + a.z) * .5f;
        }

        public static Vector2 LerpDelta(Vector2 a, Vector2 b, float t)
        {
            return Vector2.Lerp(a, b, t * Time.deltaTime);
        }
        public static Vector2 LerpFixedDelta(Vector2 a, Vector2 b, float t)
        {
            return Vector2.Lerp(a, b, t * Time.fixedDeltaTime);
        }

        public static Vector3 LerpDelta(Vector3 a, Vector3 b, float t)
        {
            return Vector3.Lerp(a, b, t * Time.deltaTime);
        }
        public static Vector3 LerpFixedDelta(Vector3 a, Vector3 b, float t)
        {
            return Vector3.Lerp(a, b, t * Time.fixedDeltaTime);
        }

        public static Vector3 Multiply(this Vector3 a, Vector3 b)
        {
            return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        public static Vector3 Multiply(this Vector3 a, float x, float y, float z)
        {
            return new Vector3(a.x * x, a.y * y, a.z * z);
        }

        public static Vector3 Divide(this Vector3 a, Vector3 b)
        {
            return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
        }

        public static Vector3 Divide(this Vector3 a, float x, float y, float z)
        {
            return new Vector3(a.x / x, a.y / y, a.z / z);
        }

        public static float SqrMagnitude(this Vector3 a, Vector3 b)
        {
            float xD = a.x - b.x, yD = a.y - b.y, zD = a.z - b.z;
            return  xD * xD + yD * yD + zD * zD;
        }

        public static Vector3 EulerAnglesToDirection(Vector3 eulerAnglesDegrees, bool invertCos = true)
        {
            Vector3 eulerAngles = eulerAnglesDegrees * Mathf.Deg2Rad;

            float sinYaw = Mathf.Sin(eulerAngles.y);
            float cosYaw = Mathf.Cos(eulerAngles.y);

            float sinPitch = Mathf.Sin(eulerAngles.x);
            float cosPitch = Mathf.Cos(eulerAngles.x);
            if (invertCos)
                cosPitch *= -1.0f;

            Vector3 rotatedDirection = new Vector3(
                sinYaw * cosPitch,
                sinPitch,
                cosYaw * cosPitch
            );

            return rotatedDirection;
        }
        public static Vector3 EulerAnglesToDirectionB(Vector3 eulerAnglesDegrees, bool invertCos = true)
        {
            Vector3 eulerAngles = (eulerAnglesDegrees + new Vector3(180f, 0f)) * Mathf.Deg2Rad;

            float sinYaw = Mathf.Sin(eulerAngles.y);
            float cosYaw = Mathf.Cos(eulerAngles.y);

            float sinPitch = Mathf.Sin(eulerAngles.x);
            float cosPitch = Mathf.Cos(eulerAngles.x);
            if (invertCos)
                cosPitch *= -1.0f;

            Vector3 rotatedDirection = new Vector3(
                sinYaw * cosPitch,
                sinPitch,
                cosYaw * cosPitch
            );

            return rotatedDirection;
        }

        public static bool Deflect(Ray ray, out Ray deflected, out RaycastHit hit)
        {
            if (Physics.Raycast(ray, out hit))
            {
                Vector3 normal = hit.normal;
                Vector3 deflect = Vector3.Reflect(ray.direction, normal);

                deflected = new Ray(hit.point, deflect);
                return true;
            }

            deflected = new Ray(Vector3.zero, Vector3.zero);
            return false;
        }

        public static Vector3 NearestPointOnLine(Vector3 linePnt, Vector3 lineDir, Vector3 pnt)
        {
            lineDir.Normalize();
            var v = pnt - linePnt;
            var d = Vector3.Dot(v, lineDir);
            return linePnt + lineDir * d;
        }

        public static Vector3 NearestPointOnFiniteLine(Vector3 start, Vector3 end, Vector3 pnt)
        {
            var line = (end - start);
            var len = line.magnitude;
            line.Normalize();

            var v = pnt - start;
            var d = Vector3.Dot(v, line);
            d = Mathf.Clamp(d, 0f, len);
            return start + line * d;
        }

        public static bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2, float factor = 0.0001f)
        {
            Vector3 lineVec3 = linePoint2 - linePoint1;
            Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
            Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);

            float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

            //is coplanar, and not parallel
            if (Mathf.Abs(planarFactor) < factor
                    && crossVec1and2.sqrMagnitude > factor)
            {
                float s = Vector3.Dot(crossVec3and2, crossVec1and2)
                        / crossVec1and2.sqrMagnitude;
                intersection = linePoint1 + (lineVec1 * s);
                return true;
            }
            else
            {
                intersection = Vector3.zero;
                return false;
            }
        }

        /// <summary>
        /// Short for "Greatest Common Factor".
        /// </summary>
        /// <returns>Greatest common factor of a and b.</returns>
        public static int GCF(int a, int b)
        {
            while (b != 0)
            {
                int temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        /// <summary>
        /// Short for "Last Common Multiple".
        /// </summary>
        /// <returns>Last common multiple of a and b.</returns>
        public static int LCM(int a, int b)
        {
            return (a / GCF(a, b)) * b;
        }

        #region PARAMETRIC FUNCTIONS

        public static float CurveLength(Func<float, Vector2> curve, int precision, float start = 0f, float end = 1f)
        {
            float add = (end - start) / (float)precision;
            float length = 0f;

            Vector2 currentPosition = curve(start);

            for (float i = start + add; i <= end; i += add)
            {
                var newPosition = curve(i);
                length += Vector2.Distance(currentPosition, newPosition);
                currentPosition = newPosition;
            }

            return length;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="angle"></param>
        /// <param name="maxValueX"></param>
        /// <param name="maxValueY"></param>
        /// <returns></returns>
        public static Vector2 Cruciform(float a, float b, float angle, float maxValueX = 100f, float maxValueY = 100f)
        {
            float x = a / Mathf.Cos(angle), y = b / Mathf.Sin(angle);
            return new(Mathf.Abs(x) > maxValueX ? maxValueX : x, Mathf.Abs(y) > maxValueY ? maxValueY : y);
        }

        /// <summary>
        /// Rotates by angle a circle with radius r outside a circle with radius R.
        /// </summary>
        /// <param name="R">Radius of the main circle.</param>
        /// <param name="r">Radius of the inner circle.</param>
        /// <param name="angle">Angle to be taken into the function.</param>
        /// <returns>The calculated 2D position from this function.</returns>
        public static Vector2 Epicycloid(float R, float r, float angle)
        {
            float plusRr = R + r;
            float theta = angle * 2f * Mathf.PI;
            float div = (plusRr / r) * theta;

            return new(
                plusRr * Mathf.Cos(theta) - r * Mathf.Cos(div),
                plusRr * Mathf.Sin(theta) - r * Mathf.Sin(div));
        }
        /// <summary>
        /// Rotates by angle a circle with radius r outside a circle with radius R.
        /// <para>This version takes degrees as values.</para>
        /// </summary>
        /// <param name="R">Radius of the main circle.</param>
        /// <param name="r">Radius of the inner circle.</param>
        /// <param name="angle">Angle to be taken into the function.</param>
        /// <returns>The calculated 2D position from this function.</returns>
        public static Vector2 EpicycloidDeg(float R, float r, float angle) => Epicycloid(R, r, angle * Mathf.Deg2Rad);

        /// <summary>
        /// Rotates by angle a circle with radius r inside a circle with radius R.
        /// </summary>
        /// <param name="R">Radius of the main circle.</param>
        /// <param name="r">Radius of the inner circle.</param>
        /// <param name="angle">Angle to be taken into the function.</param>
        /// <returns>The calculated 2D position from this function.</returns>
        public static Vector2 Hypocycloid(float R, float r, float angle)
        {
            float minusRr = R - r;
            float theta = angle * 2f * Mathf.PI * (LCM((int)r, (int)R) / R);
            float div = (minusRr / r) * theta;

            return new(
                minusRr * Mathf.Cos(theta) + r * Mathf.Cos(div),
                minusRr * Mathf.Sin(theta) - r * Mathf.Sin(div));
        }
        /// <summary>
        /// Rotates by angle a circle with radius r inside a circle with radius R.
        /// <para>This version takes degrees as values.</para>
        /// </summary>
        /// <param name="R">Radius of the main circle.</param>
        /// <param name="r">Radius of the inner circle.</param>
        /// <param name="angle">Angle to be taken into the function.</param>
        /// <returns>The calculated 2D position from this function.</returns>
        public static Vector2 HypocycloidDeg(float R, float r, float angle) => Hypocycloid(R, r, angle * Mathf.Deg2Rad);

        /// <summary>
        /// Rotates by angle a circle with radius r with offset d from its center outside a circle with radius R.
        /// </summary>
        /// <param name="R">Radius of the main circle.</param>
        /// <param name="r">Radius of the inner circle.</param>
        /// <param name="d">Offset from the center of the inner circle.</param>
        /// <param name="angle">Angle to be taken into the function.</param>
        /// <returns>The calculated 2D position from this function.</returns>
        public static Vector2 Hypertrochoid(float R, float r, float d, float angle)
        {
            float plusRr = R + r;
            float theta = angle * 2f * Mathf.PI;
            float div = (plusRr / r) * theta;

            return new(
                plusRr * Mathf.Cos(theta) - d * Mathf.Cos(div),
                plusRr * Mathf.Sin(theta) - d * Mathf.Sin(div));
        }
        /// <summary>
        /// Rotates by angle a circle with radius r with offset d from its center outside a circle with radius R.
        /// <para>This version takes degrees as values.</para>
        /// </summary>
        /// <param name="R">Radius of the main circle.</param>
        /// <param name="r">Radius of the inner circle.</param>
        /// <param name="d">Offset from the center of the inner circle.</param>
        /// <param name="angle">Angle to be taken into the function.</param>
        /// <returns>The calculated 2D position from this function.</returns>
        public static Vector2 HypertrochoidDeg(float R, float r, float d, float angle) => Hypertrochoid(R, r, d, angle * Mathf.Deg2Rad);

        /// <summary>
        /// Rotates by angle a circle with radius r with offset d from its center inside a circle with radius R.
        /// </summary>
        /// <param name="R">Radius of the main circle.</param>
        /// <param name="r">Radius of the inner circle.</param>
        /// <param name="d">Offset from the center of the inner circle.</param>
        /// <param name="angle">Angle to be taken into the function.</param>
        /// <returns>The calculated 2D position from this function.</returns>
        public static Vector2 Hypotrochoid(float R, float r, float d, float angle)
        {
            float minusRr = R - r;
            float theta = angle * 2f * Mathf.PI * (LCM((int)r, (int)R) / R);
            float div = (minusRr / r) * theta;

            return new(
                minusRr * Mathf.Cos(theta) + d * Mathf.Cos(div),
                minusRr * Mathf.Sin(theta) - d * Mathf.Sin(div));
        }
        /// <summary>
        /// Rotates by angle a circle with radius r with offset d from its center inside a circle with radius R.
        /// <para>This version takes degrees as values.</para>
        /// </summary>
        /// <param name="R">Radius of the main circle.</param>
        /// <param name="r">Radius of the inner circle.</param>
        /// <param name="d">Offset from the center of the inner circle.</param>
        /// <param name="angle">Angle to be taken into the function.</param>
        /// <returns>The calculated 2D position from this function.</returns>
        public static Vector2 HypotrochoidDeg(float R, float r, float d, float angle) => Hypotrochoid(R, r, d, angle * Mathf.Deg2Rad);

        /// <summary>
        /// Makes A infinity symbol.
        /// </summary>
        /// <param name="c">Half the distance of its focis, its turned into the half width 'a' which is equal to c * Sqrt(2).</param>
        /// <param name="angle">Angle to be taken into the function.</param>
        /// <returns>The calculated 2D position from this function.</returns>
        public static Vector2 Lemniscate(float c, float angle)
        {
            float sqr2 = Mathf.Sqrt(2f);
            float a = c * sqr2;
            float cosAngle = Mathf.Cos(angle);
            float sinAngle = Mathf.Sin(angle);
            float powSinAngle = Pow2(sinAngle);
            return new(
                (a * cosAngle) / (1f + powSinAngle),
                (a * sinAngle * cosAngle) / (1f + powSinAngle));
        }
        /// <summary>
        /// Makes A infinity symbol.
        /// <para>This version takes degrees as values.</para>
        /// </summary>
        /// <param name="c">Half the distance of its focis, its turned into the half width 'a' which is equal to c * Sqrt(2).</param>
        /// <param name="angle">Angle to be taken into the function.</param>
        /// <returns>The calculated 2D position from this function.</returns>
        public static Vector2 LemniscateDeg(float c, float angle) => Lemniscate(c, angle * Mathf.Deg2Rad);

        public static float CosDeg(float degrees) => Mathf.Cos(degrees * Mathf.Deg2Rad);
        public static float SinDeg(float degrees) => Mathf.Sin(degrees * Mathf.Deg2Rad);

        public static Vector2 CosSinPos(Vector2 positionRef, float angle, float radius) => new Vector2(positionRef.x + (radius * Mathf.Cos(angle)), positionRef.y + (radius * Mathf.Sin(angle)));
        public static Vector2 CosSinPos(Vector2 positionRef, float angle, float radiusX, float radiusY) => new Vector2(positionRef.x + (radiusX * Mathf.Cos(angle)), positionRef.y + (radiusY * Mathf.Sin(angle)));

        public static Vector2 CosSinPos(float angle, float radius) => CosSinPos(Vector2.zero, angle, radius);
        public static Vector2 CosSinPos(float angle, float radiusX, float radiusY) => CosSinPos(Vector2.zero, angle, radiusX, radiusY);

        public static Vector2 CosSinDegPos(Vector2 positionRef, float angle, float radius) => CosSinPos(positionRef, angle * Mathf.Deg2Rad, radius);
        public static Vector2 CosSinDegPos(Vector2 positionRef, float angle, float radiusX, float radiusY) => CosSinPos(positionRef, angle * Mathf.Deg2Rad, radiusX, radiusY);

        public static Vector2 CosSinDegPos(float angle, float radius) => CosSinDegPos(Vector2.zero, angle, radius);
        public static Vector2 CosSinDegPos(float angle, float radiusX, float radiusY) => CosSinDegPos(Vector2.zero, angle, radiusX, radiusY);

        #endregion

        public static Vector3 SetZeroY(Vector3 pos) => new Vector3(pos.x, 0, pos.z);

        #region Bits

        public static bool GetBit(this byte bitArray, byte index)
        {
            return ((bitArray >> index) & 1) != 0;
        }

        public static byte SetBit(this byte bitArray, byte index, bool value)
        {
            return bitArray ^= (byte)((-(value ? 1 : 0) ^ bitArray) & (1 << index));
        }

        #endregion
    }

    public static class MonoBehaviourGeneral
    {
        public static T DeclareSingleton<T>(T @object, T instance) where T : UnityEngine.Object
        {
            if (instance != null)
            {
                UnityEngine.Object.Destroy(@object);
                return instance;
            }

            return @object;
        }

        public static T DeclareSingletonDontDestroyOnLoad<T>(T monoBehaviour, T instance) where T : MonoBehaviour
        {
            if (instance != null)
            {
                UnityEngine.Object.Destroy(monoBehaviour);
                return instance as T;
            }

            MonoBehaviour.DontDestroyOnLoad(monoBehaviour);
            return monoBehaviour as T;
        }
    }

    public static class CoroutineGeneral
    {
        [Obsolete("O sistema de \'pausa\' da unity é duvidoso ent isso aqui está sem cabimento.")]
        public static IEnumerator WaitFixedFrames(int frames)
        {
            for (int i = 0; i < frames; i++)
            {
                yield return new WaitForFixedUpdate();
            }
        }
    }
}
