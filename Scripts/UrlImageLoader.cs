
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Image;
using VRC.SDKBase;
using VRC.Udon;

namespace Sonic853.Udon.Japan.Weather.Board
{
    public class UrlImageLoader : UrlLoaderCore
    {
        public Texture2D content;
        VRCImageDownloader _imageDownloader;
        public Texture2D[] cacheContents;
        void Start()
        {
            _imageDownloader = new VRCImageDownloader();
            if (loadOnStart)
                UseUpdateDownload = true;
        }
        public override void LoadUrl(bool reload = false)
        {
            var _url = useAlt ? altUrl : url;
            if (!reload && cacheContent && IndexOf(cacheUrls, _url, out var index) != -1)
            {
                useAlt = false;
                var _cacheContent = cacheContents[index];
                SendFunction(udonSendFunction, sendCustomEvent, setVariableName, _cacheContent);
                SetImage(_cacheContent);
            }
            else
            {
                if (isLoading) return;
                isLoading = true;
                var gameobj = (UdonBehaviour)GetComponent(typeof(UdonBehaviour));
                if (gameobj == null || _imageDownloader == null)
                {
                    isLoading = false;
                    UseUpdateDownload = true;
                    return;
                }
                _imageDownloader.DownloadImage(_url, null, gameobj, null);
            }
        }
        public override void OnImageLoadSuccess(IVRCImageDownload result)
        {
            isLoading = false;
            _retryCount = 0;
            var _url = useAlt ? altUrl : url;
            if (cacheContent)
            {
                IndexOf(cacheUrls, _url, out var urli);
                if (urli == -1)
                {
                    Add(ref cacheUrls, _url);
                    Add(ref cacheContents, result.Result);
                }
                else
                {
                    cacheContents[urli] = result.Result;
                }
            }
            content = result.Result;
            useAlt = false;
            SendFunction(udonSendFunction, sendCustomEvent, setVariableName, content);
            SetImage(content);
        }
        public override void OnImageLoadError(IVRCImageDownload result)
        {
            isLoading = false;
            if (_retryCount < retryCount)
            {
                _retryCount++;
                Debug.LogWarning($"UdonLab.UrlLoader.UrlImageLoader: {result.Error} Could not load {result.Url} : {result.ErrorMessage} retrying {_retryCount}/{retryCount}");
                LoadUrl();
                return;
            }
            if (
                !useAlt
                && altUrl != null
                && !string.IsNullOrEmpty(altUrl.ToString())
                && altUrl.ToString() != url.ToString()
            )
            {
                Debug.LogWarning($"UdonLab.UrlLoader.UrlImageLoader: {result.Error} Could not load {result.Url} : {result.ErrorMessage} trying alt url");
                useAlt = true;
                _retryCount = 0;
                LoadUrl();
                return;
            }
            Debug.LogError($"UdonLab.UrlLoader.UrlImageLoader: {result.Error} Could not load {result.Url} : {result.ErrorMessage} ");
            useAlt = false;
            _retryCount = 0;
        }
        public static T[] Add<T>(T[] _array, T _value, bool duplicates = true)
        {
            if (!duplicates)
            {
                if (IndexOf(_array, _value) != -1)
                    return _array;
            }
            var length = _array.Length;
            var _newArray = new T[length + 1];
            Array.Copy(_array, _newArray, length);
            // _newArray.SetValue(_value, length);
            _newArray[length] = _value;
            return _newArray;
        }
        public static T[] Add<T>(ref T[] _array, T _value, bool duplicates = true) => _array = Add(_array, _value, duplicates);
        public static int IndexOf<T>(T[] _array, T _value) => Array.IndexOf(_array, _value);
        public static int IndexOf<T>(T[] _array, T _value, out int index) => index = Array.IndexOf(_array, _value);
    }
}
