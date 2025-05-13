
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace Sonic853.Udon.Japan.Weather.Board
{
    public class UrlLoaderCore : UdonSharpBehaviour
    {
        public VRCUrl url;
        public VRCUrl altUrl;
        public bool useAlt = false;
        public bool loadOnStart = true;
        public bool isLoading = false;
        // [NonSerialized] public bool isLoaded = false;
        public UdonBehaviour udonSendFunction;
        public string sendCustomEvent = "SendFunction";
        public string setVariableName = "value";
        public RawImage rawImage;
        public Image image;
        public int retryCount = 3;
        protected int _retryCount = 0;
        public bool cacheContent = false;
        public VRCUrl[] cacheUrls;
        bool useUpdateDownload = false;
        protected bool UseUpdateDownload
        {
            get => useUpdateDownload;
            set
            {
                enabled = useUpdateDownload = value;
            }
        }
        void Update()
        {
            if (UseUpdateDownload)
            {
                LoadUrl();
                UseUpdateDownload = false;
            }
        }
        public virtual void LoadUrl() => LoadUrl(false);
        public virtual void LoadUrl(bool reload = false)
        {
            Debug.LogError("Don't use this class directly, use UrlsImageLoader or UrlsStringLoader");
        }
        public void SetImage(Texture2D texture)
        {
            if (rawImage != null) rawImage.texture = texture;
            if (image != null) image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        }
        public void SendFunction(UdonBehaviour udonBehaviour, string customEvent = "", string variableName = "", object value = null)
        {
            if (udonBehaviour == null) return;
            if (!string.IsNullOrWhiteSpace(variableName))
                udonBehaviour.SetProgramVariable(variableName, value);
            if (!string.IsNullOrWhiteSpace(customEvent))
                udonBehaviour.SendCustomEvent(customEvent);
        }
        public void SendFunction() => LoadUrl();
        public void SendFunctions() => LoadUrl();
    }
}
