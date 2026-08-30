using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using EditorTools;
using UnityEngine;

[Serializable]
public class Grid<T>
{
    public event EventHandler<OnGridValueChangedEventArgs> onGridValueChanged;
    private Func<Grid<T>, int, int, T> createGridObjectFunc;

    public class OnGridValueChangedEventArgs : EventArgs
    {
        public int x;
        public int y;
    }

    [ShowOnly]
    [SerializeField]
    private int width;
    [ShowOnly]
    [SerializeField]
    private int height;

#if UNITY_EDITOR
    private T[,] _gridArray;

    [SerializeField]
    private List<T> gridList;

    private T[,] gridArray
    {
        get
        {
            return _gridArray;
        }
        set
        {
            _gridArray = value;
            gridList = GridToList();
        }
    }
#else
    private T[,] gridArray;
#endif

    public Grid()
    {
        gridArray = new T[width, height];
    }

    public Grid(int width, int height)
    {
        this.width = width;
        this.height = height;

        gridArray = new T[width, height];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="CreateGridObject"></param>
    public Grid(int width, int height, Func<Grid<T>, int, int, T> CreateGridObject)
    {
        this.width = width;
        this.height = height;

        gridArray = new T[width, height];
        createGridObjectFunc = CreateGridObject;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                gridArray[x, y] = CreateGridObject(this, x, y);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public T[,] GetArray() => gridArray;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="position"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void GetXY(Vector2 position, out int x, out int y)
    {
        x = (int)position.x;
        y = (int)position.y;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="value"></param>
    public void SetGridObject(int x, int y, T value)
    {
        if (IsInsideArray(x, y, gridArray))
            gridArray[x, y] = value;
        if (onGridValueChanged != null)
            onGridValueChanged(this, new OnGridValueChangedEventArgs { x = x, y = y });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="position"></param>
    /// <param name="value"></param>
    public void SetGridObject(Vector2 position, T value)
    {
        if (IsInsideArray((int)position.x, (int)position.y, gridArray))
            gridArray[(int)position.x, (int)position.y] = value;
        if (onGridValueChanged != null) onGridValueChanged(this, new OnGridValueChangedEventArgs { x = (int)position.x, y = (int)position.y });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void TriggerGridObjectChanged(int x, int y)
    {
        if (onGridValueChanged != null) onGridValueChanged(this, new OnGridValueChangedEventArgs { x = x, y = y });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public T Get(int x, int y)
    {
        if (IsInsideArray(x, y, gridArray))
            return gridArray[x, y];
        else
            return default;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public T GetGridObject(Vector2 position)
    {
        GetXY(position, out int x, out int y);
        return Get(x, y);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public int GetWidth() => width;

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public int GetHeight() => height;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="array"></param>
    public void ArrayToGrid(T[,] array) => gridArray = array;

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public List<T> GridToList()
    {
        if (gridArray == null)
            throw new Exception("Grid Array is null.");

        var list = new List<T>();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                list.Add(Get(x, y));
            }
        }

        return list;
    }

    /// <summary>
    /// Cria uma nova grade a partir de uma lista com uma função com retorno.
    /// </summary>
    /// <param name="list"></param>
    /// <param name="func">int 1 = x, int 2 = y, int 3 = list index.</param>
    /// <param name="width"></param>
    /// <exception cref="SystemException"></exception>
    public void ListToGrid(List<T> list, Func<int, int, int, T> func, int width)
    {
        if (list.Count % width != 0)
            throw new SystemException(string.Format("List with size {0} is not divisible by {1}", list.Count, width));

        this.width = width;
        height = list.Count / width;
        T[,] newArray = new T[this.width, height];
        int index = 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < this.width; x++)
            {
                newArray[x, y] = list.ToArray()[index];
                index++;
            }
        }

        gridArray = newArray;
        index = 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < this.width; x++)
            {
                SetGridObject(x, y, func(x, y, index));
                index++;
            }
        }
    }

    /// <summary>
    /// Cria uma nova grade a partir de uma lista com uma função de ação.
    /// </summary>
    /// <param name="list"></param>
    /// <param name="action">int 1 = x, int 2 = y, int 3 = list index.</param>
    /// <param name="width"></param>
    /// <exception cref="SystemException"></exception>
    public void ListToGrid(List<T> list, Action<int, int, int> action, int? width = null)
    {
        var tWidth = width ?? this.width;
        if (list.Count % width != 0)
            throw new SystemException(string.Format("List with size {0} is not divisible by {1}", list.Count, tWidth));

        this.width = tWidth;
        height = list.Count / tWidth;
        T[,] newArray = new T[this.width, height];
        int index = 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < this.width; x++)
            {
                newArray[x, y] = list.ToArray()[index];
                index++;
            }
        }

        gridArray = newArray;
        index = 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < this.width; x++)
            {
                action(x, y, index);
                index++;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="list"></param>
    /// <param name="width"></param>
    /// <exception cref="SystemException"></exception>
    public void ListToGrid(List<T> list, int? width = null)
    {
        var tWidth = width ?? this.width;
        if (list.Count % width != 0)
            throw new SystemException(string.Format("List with size {0} is not divisible by {1}", list.Count, tWidth));

        this.width = tWidth;
        height = list.Count / tWidth;
        T[,] newArray = new T[this.width, height];
        int index = 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < this.width; x++)
            {
                newArray[x, y] = list[index];
                index++;
            }
        }

        gridArray = newArray;
    }

    public void ResizeGrid(int width, int height)
    {
        T[,] newArray = new T[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x >= this.width || y >= this.height)
                {
                    newArray[x, y] = createGridObjectFunc(this, x, y);
                    continue;
                }
                newArray[x, y] = gridArray[x, y];
            }
        }
        this.width = width;
        this.height = height;
        gridArray = newArray;
    }

    /// <summary>
    /// Verifica se a posição fornecida está dentro dos limites do array 2D fornecido.
    /// </summary>
    /// <typeparam name="S">O tipo do array, pra suportar todos os tipos e classes, não é necessário colocar o tipo.</typeparam>
    /// <param name="x">Localização X.</param>
    /// <param name="y">Localização Y.</param>
    /// <param name="a">O array que será usado de medida.</param>
    private static bool IsInsideArray<S>(int x, int y, S[,] a) => x >= 0 && y >= 0 && x < a.GetLength(0) && y < a.GetLength(1);

    /// <summary>
    /// Verifica se a posição fornecida está dentro dos limites da grid.
    /// </summary>
    /// <param name="x">Localização X.</param>
    /// <param name="y">Localização Y.</param>
    public bool IsInsideGrid(int x, int y) => x >= 0 && y >= 0 && x < width && y < height;

    public T this [int i]
    {
        get
        {
            return GridToList()[i];
        }
        set
        {
            var list = GridToList();
            list[i] = value;
            ListToGrid(list, width);
        }
    }

    public T this [int x, int y]
    {
        get => gridArray[x, y];
        set => gridArray[x, y] = value;
    }

    public static implicit operator T[,](Grid<T> grid)
    {
        return grid;
    }

    public static implicit operator Grid<T>(T[,] array)
    {
        return new Grid<T>(array.GetLength(0), array.GetLength(1), (grid, x, y) => { return array[x, y]; });
    }

    private StringBuilder stringBuilder = new();

    public override string ToString()
    {
        stringBuilder.Clear();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                stringBuilder.Append("x: ").Append(x).Append(", y: ").Append(y).Append(", ").Append(Get(x, y).ToString()).Append(";");
            }
            stringBuilder.AppendLine();
        }
        stringBuilder.Remove(stringBuilder.Length - 4, 3);

        return stringBuilder.ToString();
    }
}