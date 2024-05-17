using System.Collections;
using System.Threading.Tasks;
using FairyGUI;
using UnityEngine;
using UnityEngine.Networking;
namespace app
{
    public class WebItemLoader
    {
        public static string LOAD_COMPLETE = "LOAD_COMPLETE";
        private EventDispatcher _ed;
        public WebResult result;
        private LoadHelper _loadHelper;

        public WebItemLoader()
        {
            _ed = new EventDispatcher();

        }

        public void load(string url, WebItemType type)
        {
            var help = new GameObject("WebItemLoader");
            GameObject.DontDestroyOnLoad(help);
            // help.hideFlags = HideFlags.HideInHierarchy;
            // help.SetActive(true);
            _loadHelper = help.AddComponent<LoadHelper>();
            switch (type)
            {
                case WebItemType.AssetsBundle:
                    _loadHelper.loadAssetsBundle(url, (WebResult _result) =>
                    {
                        result = _result;
                        dispatch(LOAD_COMPLETE);
                    });
                    break;
                case WebItemType.Image:
                    _loadHelper.loadImage(url, (WebResult _result) =>
                        {
                            result = _result;
                            dispatch(LOAD_COMPLETE);
                        });
                    break;
            }
        }

        public Task<int> loadBundle(string url)
        {
            var tsc = new TaskCompletionSource<int>();
            var task = tsc.Task;
            once(LOAD_COMPLETE, () =>
            {
                tsc.SetResult(1);
            });
            load(url, WebItemType.AssetsBundle);
            return task;
        }

        public Task<int> loadImg(string url)
        {
            var tsc = new TaskCompletionSource<int>();
            var task = tsc.Task;
            once(LOAD_COMPLETE, () =>
            {
                tsc.SetResult(1);
            });
            load(url, WebItemType.Image);
            return task;
        }

        public void once(string strType, EventCallback0 callback)
        {

            EventCallback0 tmp = null;
            tmp = () =>
            {
                callback();
                _ed.RemoveEventListener(strType, tmp);
            };
            _ed.AddEventListener(strType, tmp);

        }

        public void on(string strType, EventCallback0 callback)
        {
            _ed.AddEventListener(strType, callback);
        }


        public void off(string strType, EventCallback0 callback)
        {
            _ed.RemoveEventListener(strType, callback);
        }


        public bool dispatch(string strType)
        {
            return _ed.DispatchEvent(strType);
        }


    }

    class LoadHelper : MonoBehaviour
    {
        public delegate void CallBack(WebResult result);

        private CallBack _callBack;
        public void loadAssetsBundle(string url, CallBack callback)
        {
            _callBack = callback;
            StartCoroutine(loadBundle(url));
        }

        public void loadImage(string url, CallBack callback)
        {
            _callBack = callback;
            StartCoroutine(loadImage(url));
        }


        private IEnumerator loadImage(string url)
        {
            var uwr = UnityWebRequestTexture.GetTexture(url);
            yield return uwr.SendWebRequest();
            var result = new WebResult();
            result.url = url;
            if (uwr.result == UnityWebRequest.Result.Success)
            {
                result.code = WebResultCode.Success;
                Texture2D texture = DownloadHandlerTexture.GetContent(uwr);
                result.data = texture;
            }
            else
            {
                result.data = uwr.error;
                if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
                {
                    result.code = WebResultCode.NetError;
                }
                else
                {
                    result.code = WebResultCode.Fail;
                }
            }
            _callBack(result);
            _callBack = null;
            GameObject.Destroy(this.gameObject);
        }

        private IEnumerator loadBundle(string url)
        {
            var uwr = UnityWebRequestAssetBundle.GetAssetBundle(url);
            yield return uwr.SendWebRequest();
            var result = new WebResult();
            result.url = url;
            if (uwr.result == UnityWebRequest.Result.Success)
            {
                result.code = WebResultCode.Success;
                AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(uwr);
                result.data = bundle;
            }
            else
            {
                result.data = uwr.error;
                if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
                {
                    result.code = WebResultCode.NetError;
                }
                else
                {
                    result.code = WebResultCode.Fail;
                }
            }
            _callBack(result);
            _callBack = null;
            GameObject.Destroy(this.gameObject);
        }
    }
}