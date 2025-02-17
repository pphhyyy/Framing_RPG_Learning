using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary; //将运行的数据文件转为 二进制文件以保存
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadManager : SingletonMonobehaviour <SaveLoadManager>
{
    public GameSave gameSave; //用来保存游戏数据

    // 这里在列表中装入的是接口而非具体的类，这样写，只要是继承了 ISaveable 的类都可以装入这个list，也就是可以装入不同类型的类
    //比如玩家数据的存储和场景数据存储的具体方式是不一样的，但只要两者都继承了ISaveable 接口，就可以在下面调用接口方法的时候根据各自类中实现的方法 分别进行 存储工作
    public List<ISaveable> iSaveableObjectList;

    //这个类是单例类， 而iSaveableObjectList 而是public ，所以任何类都可以调用这里的iSaveableObjectList ， 把自己装进去

    protected override void Awake()
    {
        base.Awake();
        iSaveableObjectList = new List<ISaveable>();
    }

    public void LoadDataFromFile()
    {
        BinaryFormatter bf = new BinaryFormatter();

        if(File.Exists(Application.persistentDataPath + "/wildHopeCreek.dat"))
        {
            gameSave = new GameSave();

            FileStream file = File.Open(Application.persistentDataPath + "/wildHopeCreek.dat", FileMode.Open);
            gameSave = (GameSave)bf.Deserialize(file); // 反序列化获得 gameSave

            // 遍历所有继承了 Isaveable 接口的 对象 , 并 应用他们 save data

            for (int i =  iSaveableObjectList.Count - 1; i > -1 ; i--)
            {
                if (gameSave.gameObjectData.ContainsKey(iSaveableObjectList[i].ISaveableUniqueID))
                {
                    iSaveableObjectList[i].ISaveableLoad(gameSave);
                }
                // 如果 IsaveableObject 的 unique ID 不在游戏的数据中 , 就摧毁他们
                else
                {
                    Component component = (Component)iSaveableObjectList[i];
                    Destroy(component.gameObject);
                }
            }

            file.Close();

        }

        UIManager.Instance.DisablePauseMenu();
    }

    public void SaveDataToFile()
    {
        gameSave = new GameSave();

        // 遍历所有继承了 ISaveable 接口的 物体,完成他们的数据保存

        foreach(ISaveable iSaveableObject in iSaveableObjectList)
        {
            gameSave.gameObjectData.Add(iSaveableObject.ISaveableUniqueID, iSaveableObject.ISaveableSave());
        }

        BinaryFormatter bf = new BinaryFormatter();

        FileStream file = File.Open(Application.persistentDataPath + "/wildHopeCreek.dat", FileMode.Create);
        bf.Serialize(file,gameSave);
        file.Close();

        UIManager.Instance.DisablePauseMenu();
    }

    public void StoreCurrentSceneData()
    {
        foreach (ISaveable isvaeableObject in iSaveableObjectList)
        {
            isvaeableObject.ISaveable_StoreScene(SceneManager.GetActiveScene().name);
        }
    }

    public void RestoreCurrentSceneData()
    {
        foreach (ISaveable isvaeableObject in iSaveableObjectList)
        {
            print("!!!" + SceneManager.GetActiveScene().name);
            isvaeableObject.ISaveable_RestoreScene(SceneManager.GetActiveScene().name);
        }
    }
}
