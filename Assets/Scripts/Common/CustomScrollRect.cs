using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient.Common
{
    public class CustomScrollRect : ScrollRect
    {
        public new void SetContentAnchoredPosition(Vector2 position) => base.SetContentAnchoredPosition(position);  //把这个方法封装成public方法来调用
    }
}
