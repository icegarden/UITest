using System.Threading.Tasks;
using UnityEngine;

namespace app
{
    public abstract class Mediator
    {
        private bool _awaked = false;
        protected GameObject _parent;
        protected LayerId _parentLayerId;
        protected string _skinPath;

        /// <summary>
        /// 设定模块皮肤路径，显示层级等基本信息
        /// </summary>
        public abstract void initSkinInfo();

        protected string _skinBundleBase;

        protected GameObject _view;

        public Mediator()
        {
            _skinBundleBase = "module";
        }

        public async Task loadSkin()
        {
            await ModuleManager.ins.initSkin(_skinBundleBase);
        }
        /// <summary>
        /// 根据皮肤信息初始化界面
        /// </summary>
        public virtual void initView()
        {
            var bundle = ModuleManager.ins.getBundleSkin(this._skinBundleBase);
            var obj = bundle.LoadAsset<GameObject>(_skinPath);
            _view = GameObject.Instantiate(obj);
        }
        public virtual void awake()
        {
            if (!_view)
            {
                return;
            }
            if (_awaked)
            {
                return;
            }
            _awaked = true;
            if (_parent != null)
            {
                LayerManager.ins.addChild(_view, _parent);
            }
            else
            {
                LayerManager.ins.addChild(_view, _parentLayerId);
            }
        }

        public virtual void sleep()
        {
            if (!_view)
            {
                return;
            }
            _awaked = false;
            LayerManager.ins.removeChild(_view);
        }
    }
}