// --------------------------------------------------------------------------------------------------------------------
/// <copyright file="ObjectUtils.cs">
///   Created by Pi14.
/// </copyright>
// --------------------------------------------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
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
                    throw new System.Exception(name + " was not been found or it doesn't exist on the path: " + path);
                //go é igual ao gameobject achado como filho do seu transform
                go = go.transform.Find(name).gameObject;
            }
            if (go == null)
                throw new System.Exception("GameObject " + go.name + " has not been found on the end of the path: " + path);
            //retornar go
            return go;
        }
        /// <summary>
        /// Acha um GameObject por hierarquia nos transforms como se fosse um arquivo em pastas.
        /// </summary>
        /// <param name="start">Gameobject de referência.</param>
        /// <param name="path">Caminho do GameObject separados por '\' ou '/'.</param>
        /// <returns>O GameObject no caminho inserido.</returns>
        public static GameObject GetGameObject(this Component start, string path)
        {
            return GetGameObject(start.gameObject, path);
        }
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
                throw new System.Exception("GameObject has not been found on the path.");
            if (go.GetComponent<T>() == null)
                throw new System.Exception("Component has not been found on " + go.name);
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
        public static T GetGameObjectComponent<T>(this Component start, string path)
        {
            return GetGameObjectComponent<T>(start.gameObject, path);
        }
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
                throw new System.ArgumentNullException("Child of name \'" + childName + "\' was not found on the \'" + parent.name + "\' GameObject");
            if (parent.transform.Find(childName).GetComponent<T>() == null)
                throw new System.ArgumentNullException("Component was not been found in child of name \'" + childName + "\' of parent \'" + parent.name + "\'");
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
        public static T FindComponentInChild<T>(this Component parent, string childName)
        {
            return FindComponentInChild<T>(parent.gameObject, childName);
        }
        public static GameObject[] GetGameObjectChildren(this GameObject gameObject)
        {
            var list = new List<GameObject>();
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                list.Add(gameObject.transform.GetChild(i).gameObject);
            }
            return list.ToArray();
        }
        public static GameObject[] GetGameObjectChildren(this Component gameObject)
        {
            return GetGameObjectChildren(gameObject.gameObject);
        }

        public static List<GameObject> FindObjects(Vector3 point, LayerMask layerMask, Func<GameObject, bool> condition)
        {
            GameObject[] sceneObjects = UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
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

        public static List<GameObject> FindNearObjects(Vector3 point, LayerMask layerMask, float distance)
        {
            GameObject[] sceneObjects = UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
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

        public static List<GameObject> FindNearObjects(Vector3 point, LayerMask layerMask, float distance, Func<GameObject, bool> condition)
        {
            GameObject[] sceneObjects = UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
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

        public static bool CheckLayerNameInMask(this LayerMask layerMask, string layerName)
        {
            return layerMask == (layerMask | 1 << LayerMask.NameToLayer(layerName));
        }
    }
    public static class UIGeneral
    {
        /// <returns>Retorna a posição do mouse com o fator de escala do canvas.</returns>
        public static Vector2 MousePositionScaled()
        {
            return Mouse.current.position.ReadValue() * FindCanvas().scaleFactor;
        }
        /// <returns>Retorna a posição do mouse.</returns>
        public static Vector2 MousePosition()
        {
            return Mouse.current.position.ReadValue();
        }
        /// <returns>Retorna o canvas pelo nome "Canvas" (deve ter somente 1 Canvas).</returns>
        public static Canvas FindCanvas()
        {
            return GameObject.Find("Canvas").GetComponent<Canvas>();
        }
        /// <returns>Retorna o canvas pelo nome "DontDestroyOnLoadCanvas" (deve ter somente 1 DontDestroyOnLoadCanvas).</returns>
        public static Canvas FindDontDestroyOnLoadCanvas()
        {
            return GameObject.Find("DontDestroyOnLoadCanvas").GetComponent<Canvas>();
        }
        /// <summary>
        /// Fator de escala do canvas.
        /// </summary>
        public static float CanvasScaleFactor => FindCanvas().scaleFactor;

        private const int UILayer = 5;
        //Returns 'true' if we touched or hovering on Unity UI element.
        public static bool IsPointerOverUIElement() => IsPointerOverUIElement(GetEventSystemRaycastResults());
        //Returns 'true' if we touched or hovering on Unity UI element.
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
        //Gets all event system raycast results of current mouse or touch position.
        public static List<RaycastResult> GetEventSystemRaycastResults()
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;
            List<RaycastResult> raysastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raysastResults);
            return raysastResults;
        }
    }
    public static class MathEx
    {
        public static float Pow2(float f)
        {
            return Mathf.Pow(f, 2);
        }

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

        public static T GetRandom<T>(this IList<T> list)
        {
            if (list == null)
                throw new ArgumentNullException("list", "The provided list is null");
            if (list.Count == 0)
                throw new ArgumentException("Cant get a random item in a list that is empty!");

            return list[0];
        }

        /// <summary>
        /// Clamps value from 0 to 1.
        /// </summary>
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
            return RadianToVector2(Mathf.Atan2(a.y - b.y, a.x - b.x));
        }
        public static Vector2 AngleVectors(Vector3 a, Vector3 b)
        {
            return RadianToVector2(Mathf.Atan2(a.z - b.z, a.x - b.x));
        }
        public static float AngleRadian(Vector3 a, Vector3 b)
        {
            return Mathf.Atan2(a.z - b.z, a.x - b.x);
        }

        public static Vector3 Abs(this Vector3 v)
        {
            return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
        }

        public static float Absolute(this Vector3 v)
        {
            var a = v.Abs();
            return a.x + a.y + a.z;
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

        public static Vector3 SetZeroY(Vector3 pos) => new Vector3(pos.x, 0, pos.z);

        public static bool MaskContainsLayer(this LayerMask mask, int layer) => (mask & (1 << layer)) != 0;
    }

    public static class MonoBehaviourGeneral
    {
        public static T DeclareSingleton<T>(MonoBehaviour monoBehaviour, MonoBehaviour instance) where T : MonoBehaviour
        {
            if (instance != null)
            {
                UnityEngine.Object.Destroy(monoBehaviour);
                return instance as T;
            }

            return monoBehaviour as T;
        }

        public static T DeclareSingletonDontDestroyOnLoad<T>(MonoBehaviour monoBehaviour, MonoBehaviour instance) where T : MonoBehaviour
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
}