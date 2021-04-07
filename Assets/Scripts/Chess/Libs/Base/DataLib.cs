using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VFramework;

public class DataLib<T> : Singleton<DataLib<T>> where T : IData, new()
{
    protected Dictionary<int, T> _dataDict = new Dictionary<int, T>();

    public DataLib() { }

    public virtual void ParseInit(string resPath)
    {
        //首先读取Resources文件夹下的Json文档
        TextAsset ta = ResourceMgr.Instance.Load<TextAsset>(resPath);
        //解析Json文档，用这个库的好处就是能直接反序列化成对象列表，操作简便
        List<T> dataList = JsonConvert.DeserializeObject<List<T>>(ta.text);
        //存入字典保存
        foreach (var data in dataList)
        {
            _dataDict.Add(data.id, data);
        }
    }

    public T GetData(int id)
    {
        T data;
        _dataDict.TryGetValue(id, out data);
        if (data == null)
        {
            Debug.LogError(string.Format("数据库中没有id为{0}的数据条目", id));
        }
        return data;
    }
}
