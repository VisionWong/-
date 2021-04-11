using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VFramework.UIManager
{
	public abstract class BasePanel : MonoBehaviour
	{
	    private CanvasGroup _canvasGroup = null;
	    protected CanvasGroup CanvasGroup
	    {
	        get
	        {
	            if (_canvasGroup == null)
	            {
	                _canvasGroup = GetComponent<CanvasGroup>();
	            }
	            return _canvasGroup;
	        }
	    }
	
	    public UIPanelType type;

        private List<BasePanel> childList = new List<BasePanel>();
	
	    /// <summary>
	    /// 界面初始化时
	    /// </summary>
	    public virtual void OnEnter()
        {
            foreach (var item in childList)
            {
                item.OnEnter();
            }
        }
	
	    /// <summary>
	    /// 界面暂停（冻结）时
	    /// </summary>
	    public virtual void OnPause()
        {
            foreach (var item in childList)
            {
                item.OnPause();
            }
        }
	
	    /// <summary>
	    /// 界面重启，解除暂停状态
	    /// </summary>
	    public virtual void OnResume()
        {
            foreach (var item in childList)
            {
                item.OnResume();
            }
        }
	
	    /// <summary>
	    /// 界面离开，实现隐藏或者销毁
	    /// </summary>
	    public virtual void OnExit()
        {
            foreach (var item in childList)
            {
                item.OnExit();
            }
        }
	
	    /// <summary>
	    /// 清除界面时，销毁自身
	    /// </summary>
	    public virtual void OnClear()
	    {
            foreach (var item in childList)
            {
                item.OnClear();
            }
            Destroy(gameObject);
	    }
	    
        protected void AddUIItem(UIPanelType type, bool isShow = true)
        {
            var item = UIManager.Instance.GetPanel(type);

            item.transform.SetParent(transform, false);
            childList.Add(item);

            if (isShow == false)
            {
                item.CanvasGroup.alpha = 0;
                item.CanvasGroup.interactable = false;
            } 
        }
	}
}

