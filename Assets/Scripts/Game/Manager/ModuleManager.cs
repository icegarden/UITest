using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace app
{
    public class ModuleManager
    {
        private static ModuleManager _ins;

        /// <summary>
        /// 模块类字典
        /// </summary>
        private Dictionary<ModuleId, Type> _clsDic;
        /// <summary>
        /// 实例字典
        /// </summary>
        private Dictionary<ModuleId, Mediator> _insDic;

        private Dictionary<string, AssetBundle> _bundleDic;
        public static ModuleManager ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new ModuleManager();
                }
                return _ins;
            }
        }
        public ModuleManager()
        {

        }

        public void init()
        {
            _clsDic = new Dictionary<ModuleId, Type>();
            _insDic = new Dictionary<ModuleId, Mediator>();
            _bundleDic = new Dictionary<string, AssetBundle>();

            registModule<LoginMediator>(ModuleId.Login);
            registModule<RoleMediator>(ModuleId.Role);
            registModule<BagMediator>(ModuleId.Bag);
        }

        public async Task initSkin(string key)
        {
            _bundleDic.TryGetValue(key, out var bundle);
            if (bundle == null)
            {
                var baseUrl = "http://127.0.0.1:3605/skin/";
                var url = baseUrl + key;
                var loader = new WebItemLoader();
                await loader.loadBundle(url);
                if (loader.result.code == WebResultCode.Success)
                {
                    var tmp = loader.result.data as AssetBundle;
                    _bundleDic[key] = tmp;
                }
            }
        }

        public AssetBundle getBundleSkin(string key)
        {
            _bundleDic.TryGetValue(key, out var bundle);
            return bundle;
        }

        private void registModule<T>(ModuleId id) where T : Mediator
        {
            _clsDic.Add(id, typeof(T));

        }

        public async void open(ModuleId id)
        {
            _insDic.TryGetValue(id, out var moduleIns);
            if (moduleIns == null)
            {
                var clsType = _clsDic[id];
                if (clsType != null)
                {
                    moduleIns = (Mediator)Activator.CreateInstance(clsType);
                    moduleIns.initSkinInfo();
                    //开始加载界面的转圈
                    await moduleIns.loadSkin();
                    //结束转圈
                    moduleIns.initView();
                    _insDic.Add(id, moduleIns);
                }

            }
            if (moduleIns != null)
            {
                moduleIns.awake();
            }
        }

        public void close(ModuleId id)
        {
            _insDic.TryGetValue(id, out var moduleIns);
            if (moduleIns != null)
            {
                moduleIns.sleep();
            }
        }

    }
}