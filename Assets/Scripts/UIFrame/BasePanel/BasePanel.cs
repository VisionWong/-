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
	
	    /// <summary>
	    /// 界面初始化时
	    /// </summary>
	    public virtual void OnEnter() { }
	
	    /// <summary>
	    /// 界面暂停（冻结）时
	    /// </summary>
	    public virtual void OnPause() { }
	
	    /// <summary>
	    /// 界面重启，解除暂停状态
	    /// </summary>
	    public virtual void OnResume() { }
	
	    /// <summary>
	    /// 界面离开，实现隐藏或者销毁
	    /// </summary>
	    public virtual void OnExit() { }
	
	    /// <summary>
	    /// 清除界面时，销毁自身
	    /// </summary>
	    public virtual void OnClear()
	    {
	        Destroy(gameObject);
	    }
	
	}
}

