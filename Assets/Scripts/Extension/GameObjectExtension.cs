using UnityEngine;
/// <summary>
/// 为GameObject添加getChildAt和getChild方法
/// </summary>
public static class GameObjectExtension
{
    public static GameObject getChild(this GameObject go, string name)
    {
        if (go.transform != null)
        {
            var len = go.transform.childCount;
            for (var i = 0; i < len; i++)
            {
                var child = go.getChildAt(i);
                if (child != null && child.name == name)
                {
                    return child;
                }
            }
        }
        return null;
    }
    public static GameObject getChildAt(this GameObject go, int index)
    {
        if (go.transform != null)
        {
            var child = go.transform.GetChild(index);
            if (child)
            {
                return child.gameObject;
            }
        }
        return null;
    }
}