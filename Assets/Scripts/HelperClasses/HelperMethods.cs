using System;
using System.Collections.Generic;
using UnityEngine;

public static class HelperMethods  //������̬�� �� ���������ֱ�ӵ���
{

    public static bool GetComponentsAtCursorLocation<T>(out List<T> ComponentsAtPositionList, Vector3 positionToCheck)
    {
        bool found = false;
        List<T> componentList = new List<T>();

        Collider2D[] collider2DArray = Physics2D.OverlapPointAll(positionToCheck); // �� positionToCheck ���ص���һϵ�� collider2d  һ�� Collider2D ����z������˳������


        T tComponent = default(T);

        for (int i = 0; i < collider2DArray.Length; i++)
        {

            // ���������õ� Collider2D �ҵ����� Collider2D��gameobject Ȼ�� ��� T 
            tComponent = collider2DArray[i].gameObject.GetComponentInParent<T>();  // �Ӹ������� 
            if (tComponent != null)
            {
                found = true;
                componentList.Add(tComponent);
            }

            else
            {
                tComponent = collider2DArray[i].gameObject.GetComponentInChildren<T>(); // �������Ҳ������Ӷ��� 
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
    /// �ڵ�ǰ box ��Χ�� Ѱ�� ������ T ����Ķ��� ������ȫ��װ�� listComponentsAtBoxPosition
    /// </summary>
    /// <typeparam name="T"> �������� </typeparam>
    /// <param name="listComponentsAtBoxPosition"> out ����Ķ��� </param>
    /// <param name="point"> box ���� </param>
    /// <param name="size">box �ߴ� </param>
    /// <param name="angle"> box ƫ�ƽǶ� </param>
    /// <returns></returns>
    public static bool GetComponentsAtBoxLocation<T>(out List<T> listComponentsAtBoxPosition, Vector2 point, Vector2 size, float angle)
    {
        bool found = false;
        List<T> componentList = new List<T>();

        Collider2D[] collider2DArray = Physics2D.OverlapBoxAll(point, size, angle); // box ��Χ�� �ҵ� һ�� Collider2D 

        for (int i = 0; i < collider2DArray.Length; i++)
        {

            // ���������õ� Collider2D �ҵ����� Collider2D��gameobject Ȼ�� ��� T 
            T tComponent = collider2DArray[i].gameObject.GetComponentInParent<T>();  // �Ӹ������� 
            if (tComponent != null)
            {
                found = true;
                componentList.Add(tComponent);
            }

            else
            {
                tComponent = collider2DArray[i].gameObject.GetComponentInChildren<T>(); // �������Ҳ������Ӷ��� 
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


    //���֮ǰ�ķ��� Ч�ʸ��� �Ҳ��ᴥ���������� 
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
