using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace app
{
    /// <summary>
    /// 层级管理
    /// 单例
    /// </summary>
    public class LayerManager
    {
        private GameObject _root;
        private GameObject _canvas;

        private static LayerManager _ins;
        private Dictionary<LayerId, GameObject> containerDic;
        public static LayerManager ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new LayerManager();
                }
                return _ins;
            }
        }

        public LayerManager()
        {
            containerDic = new Dictionary<LayerId, GameObject>();
        }
        public void init(GameObject root)
        {
            _root = root;
            createCanvas();
            initLayer();
        }

        private void initLayer()
        {
            createLayer(LayerId.MAP_CONTAINER, "MAP_CONTAINER", _canvas);
            createLayer(LayerId.MAP_BACKGROUD, "MAP_BACKGROUD", containerDic[LayerId.MAP_CONTAINER]);
            createLayer(LayerId.MAP_ROLE, "MAP_ROLE", containerDic[LayerId.MAP_CONTAINER]);
            createLayer(LayerId.UI_CONTAINER, "UI_CONTAINER", _canvas);
            createLayer(LayerId.DIALOG, "DIALOG", containerDic[LayerId.UI_CONTAINER]);
            createLayer(LayerId.ALERT, "ALERT", containerDic[LayerId.UI_CONTAINER]);
            createLayer(LayerId.MESSAGE, "MESSAGE", containerDic[LayerId.UI_CONTAINER]);
        }

        private void createLayer(LayerId id, string name, GameObject parent)
        {
            var go = new GameObject(name);
            go.transform.position.Set(0, 0, 0);
            addChild(go, parent);
            containerDic[id] = go;
        }

        public void addChild(GameObject child, GameObject parent)
        {
            child.transform.SetParent(parent.transform);
        }

        public void addChild(GameObject child, LayerId layerId)
        {
            var parent = containerDic[layerId];
            if (parent != null)
            {

                addChild(child, parent);
            }
        }

        public void removeChild(GameObject child)
        {
            child.transform.SetParent(null);
        }

        private void createCanvas()
        {
            var go = _canvas = new GameObject("Canvas");
            go.transform.position.Set(0f, 0f, 0f);

            var canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = go.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);

            go.AddComponent<GraphicRaycaster>();

            addChild(go, _root);
        }
    }
}