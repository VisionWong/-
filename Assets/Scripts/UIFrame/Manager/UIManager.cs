using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace VFramework.UIManager
{
    public class UIManager : MonoSingleton<UIManager>
    {
        private Dictionary<UIPanelType, string> _panelPathDict; //缓存常用的UI界面资源路径
        private Dictionary<UIPanelType, BasePanel> _panelDict;  //缓存常用的UI界面实例
        private Stack<BasePanel> _panelStack;

        public Transform Canvas { get; set; }

        /// <summary>
        /// 从json文件初始化UI预制体到缓存中
        /// </summary>
        private void Awake()
        {
            _panelPathDict = new Dictionary<UIPanelType, string>();
            _panelDict = new Dictionary<UIPanelType, BasePanel>();
            _panelStack = new Stack<BasePanel>();

            // 读取json
            ParseUIPanelJson();

            //获取canvas
            GameObject canvasGO = GameObject.FindGameObjectWithTag(TagDefine.CANVAS);
            //让UIRoot不会销毁
            DontDestroyOnLoad(canvasGO.transform.parent);
            Canvas = canvasGO.transform;
        }

        /// <summary>
        /// 解析UI界面Json文档
        /// </summary>
        private void ParseUIPanelJson()
        {
            _panelPathDict = new Dictionary<UIPanelType, string>();
            //首先读取Resources文件夹下的Json文档
            TextAsset ta = ResourceMgr.Instance.Load<TextAsset>(PathDefine.CONFIG_UI_PANEL);
            //解析Json文档，用这个库的好处就是能直接反序列化成对象列表，操作简便
            List<UIPanelInfo> infoList = JsonConvert.DeserializeObject<List<UIPanelInfo>>(ta.text);
            //存入字典保存
            foreach (var info in infoList)
            {
                _panelPathDict.Add(info.panelType, info.resPath);
            }
        }

        /// <summary>
        /// 通过界面类型来获取一个界面对象
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private BasePanel GetPanel(UIPanelType type)
        {
            BasePanel panel;
            _panelDict.TryGetValue(type, out panel);
            if (panel == null)
            {
                //没有则加载panel
                string path = _panelPathDict[type];
                GameObject go = ResourceMgr.Instance.Load<GameObject>(path);
                //添加到画布下,不维持世界坐标
                go.transform.SetParent(Canvas, false);
                panel = go.GetComponent<BasePanel>();
            }
            return panel;
        }

        /// <summary>
        /// 将界面入栈，暂停之前所有界面
        /// </summary>
        /// <param name="type"></param>
        /// <param name="isCache">是否将其缓存</param>
        public void PushPanel(UIPanelType type, bool isCache = false)
        {
            //将栈顶界面暂停
            if (_panelStack.Count > 0)
            {
                BasePanel topPanel = _panelStack.Peek();
                topPanel.OnPause();
            }
            BasePanel panel = GetPanel(type);
            _panelStack.Push(panel);
            panel.OnEnter();

            //是否缓存
            if (isCache && _panelDict.ContainsKey(type) == false)
            {
                _panelDict.Add(type, panel);
            }
        }

        /// <summary>
        /// 将界面出栈，重启下层界面
        /// </summary>
        public void PopPanel()
        {
            if (_panelStack.Count <= 0) return;

            BasePanel topPanel = _panelStack.Pop();
            //若是缓存则隐藏，否则销毁
            if (_panelDict.ContainsKey(topPanel.type))
            {
                topPanel.OnExit();
            }
            else
            {
                topPanel.OnClear();
            }

            //恢复下一层
            if (_panelStack.Count > 0)
            {
                _panelStack.Peek().OnResume();
            }
        }

        /// <summary>
        /// 将当前存在的界面全部清空
        /// </summary>
        public void Clear()
        {
            while (_panelStack.Count > 0)
            {
                BasePanel panel = _panelStack.Pop();
                if (_panelDict.ContainsKey(panel.type))
                {
                    _panelDict.Remove(panel.type);
                }
                panel.OnClear();
            }

            //清理缓存
            foreach (var panel in _panelDict)
            {
                panel.Value.OnClear();
            }
            _panelDict.Clear();
        }
    }
}
