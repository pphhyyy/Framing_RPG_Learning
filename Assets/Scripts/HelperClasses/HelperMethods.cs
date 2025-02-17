using System;
using System.Collections.Generic;
using UnityEngine;

public static class HelperMethods  //公共静态类 ， 其他类可以直接调用
{

    public static bool GetComponentsAtCursorLocation<T>(out List<T> ComponentsAtPositionList, Vector3 positionToCheck)
    {
        bool found = false;
        List<T> componentList = new List<T>();

        Collider2D[] collider2DArray = Physics2D.OverlapPointAll(positionToCheck); // 和 positionToCheck 点重叠的一系列 collider2d  一堆 Collider2D ，按z轴坐标顺序排序


        T tComponent = default(T);

        for (int i = 0; i < collider2DArray.Length; i++)
        {

            // 根据上面获得的 Collider2D 找到挂载 Collider2D的gameobject 然后 获得 T 
            tComponent = collider2DArray[i].gameObject.GetComponentInParent<T>();  // 从父对象找 
            if (tComponent != null)
            {
                found = true;
                componentList.Add(tComponent);
            }

            else
            {
                tComponent = collider2DArray[i].gameObject.GetComponentInChildren<T>(); // 父对象找不到找子对象 
                if (tComponent != null)
                {
                    found = true;
                    componentList.Add(tComponent);
                }
            }
        }

        ComponentsAtPositionList = componentList;

        return found;
    }


    /// <summary>
    /// 在当前 box 范围内 寻找 挂在了 T 组件的对象 并将其全部装入 listComponentsAtBoxPosition
    /// </summary>
    /// <typeparam name="T"> 泛型类型 </typeparam>
    /// <param name="listComponentsAtBoxPosition"> out 输出的对象 </param>
    /// <param name="point"> box 中心 </param>
    /// <param name="size">box 尺寸 </param>
    /// <param name="angle"> box 偏移角度 </param>
    /// <returns></returns>
    public static bool GetComponentsAtBoxLocation<T>(out List<T> listComponentsAtBoxPosition, Vector2 point, Vector2 size, float angle)
    {
        bool found = false;
        List<T> componentList = new List<T>();

        Collider2D[] collider2DArray = Physics2D.OverlapBoxAll(point, size, angle); // box 范围内 找到 一堆 Collider2D 

        for (int i = 0; i < collider2DArray.Length; i++)
        {

            // 根据上面获得的 Collider2D 找到挂载 Collider2D的gameobject 然后 获得 T 
            T tComponent = collider2DArray[i].gameObject.GetComponentInParent<T>();  // 从父对象找 
            if (tComponent != null)
            {
                found = true;
                componentList.Add(tComponent);
            }

            else
            {
                tComponent = collider2DArray[i].gameObject.GetComponentInChildren<T>(); // 父对象找不到找子对象 
                if (tComponent != null)
                {
                    found = true;
                    componentList.Add(tComponent);
                }
            }
        }

        listComponentsAtBoxPosition = componentList;

        return found;
    }


    //相比之前的方法 效率更高 且不会触发垃圾回收 
    public static T[] GetComponentsAtBoxLocationNonAlloc <T>(int numberOfCollidersToTest , Vector2 point, Vector2 size, float angle)
    {
        Collider2D[] collider2DArray = new Collider2D[numberOfCollidersToTest];

        Physics2D.OverlapBoxNonAlloc(point , size , angle , collider2DArray );

        T tComponent = default(T);
        T[] componentArray = new T[collider2DArray.Length];

        for(int i = collider2DArray.Length - 1; i >= 0; i--)
        {
            if (collider2DArray[i] != null)
            {
                tComponent = collider2DArray[i].gameObject.GetComponent<T>();

                if(tComponent != null)
                {
                    componentArray[i] = tComponent;
                }
            }
        }

        return componentArray;

    }
}
