using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace app
{
    public class LoginMediator : Mediator
    {
        public LoginMediator()
        {

        }

        public override void initSkinInfo()
        {
            _skinPath = "Login_Skin";
            _parentLayerId = LayerId.DIALOG;
        }
        public override void initView()
        {
            base.initView();
        }


        private async void updateView()
        {
            var rawImage = _view.getChild("RawImage");
            if (!rawImage)
            {
                return;
            }
            var loader = new WebItemLoader();
            await loader.loadImg("http://123.60.40.248:10000/laya/assets/loginassets.d/bg_begin.jpg");
            if (loader.result.code == WebResultCode.Success)
            {
                var tex = loader.result.data as Texture2D;
                var img = rawImage.GetComponent<RawImage>();
                img.texture = tex;
                img.SetNativeSize();
            }
        }
        public override void awake()
        {
            base.awake();
            _view.transform.position.Set(0, 0, 0);
            updateView();
        }

        public override void sleep()
        {
            base.sleep();
        }

    }
}