using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary; //�����е������ļ�תΪ �������ļ��Ա���
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadManager : SingletonMonobehaviour <SaveLoadManager>
{
    public GameSave gameSave; //����������Ϸ����

    // �������б���װ����ǽӿڶ��Ǿ�����࣬����д��ֻҪ�Ǽ̳��� ISaveable ���඼����װ�����list��Ҳ���ǿ���װ�벻ͬ���͵���
    //����������ݵĴ洢�ͳ������ݴ洢�ľ��巽ʽ�ǲ�һ���ģ���ֻҪ���߶��̳���ISaveable �ӿڣ��Ϳ�����������ýӿڷ�����ʱ����ݸ�������ʵ�ֵķ��� �ֱ���� �洢����
    public List<ISaveable> iSaveableObjectList;

    //������ǵ����࣬ ��iSaveableObjectList ����public �������κ��඼���Ե��������iSaveableObjectList �� ���Լ�װ��ȥ

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
            gameSave = (GameSave)bf.Deserialize(file); // �����л���� gameSave

            // �������м̳��� Isaveable �ӿڵ� ���� , �� Ӧ������ save data

            for (int i =  iSaveableObjectList.Count - 1; i > -1 ; i--)
            {
                if (gameSave.gameObjectData.ContainsKey(iSaveableObjectList[i].ISaveableUniqueID))
                {
                    iSaveableObjectList[i].ISaveableLoad(gameSave);
                }
                // ��� IsaveableObject �� unique ID ������Ϸ�������� , �ʹݻ�����
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

        // �������м̳��� ISaveable �ӿڵ� ����,������ǵ����ݱ���

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
