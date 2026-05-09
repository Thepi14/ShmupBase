using UnityEngine;
using System.Collections;
using System;

public class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = UnityEngine.JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }
    public static MapData<T> MapFromJson<T>(string json)
    {
        MapData<T> wrapper = UnityEngine.JsonUtility.FromJson<MapData<T>>(json);
        return wrapper;
    }
    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return UnityEngine.JsonUtility.ToJson(wrapper);
    }
    public static string MapToJson<T>(MapData<T> map)
    {
        return UnityEngine.JsonUtility.ToJson(map);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }

}
[Serializable]
public class MapData<T>
{
    public int width;
    public int height;
    public int cyclesToWin;
    public int roundTime;
    public int startX;
    public int startY;

    public T[] Tiles;

    public MapData(int width, int height, int cyclesToWin, int roundTime, int startX, int startY, T[] tiles)
    {
        this.width = width;
        this.height = height;
        this.cyclesToWin = cyclesToWin;
        this.roundTime = roundTime;
        this.startX = startX;
        this.startY = startY;
        Tiles = tiles;
    }
}