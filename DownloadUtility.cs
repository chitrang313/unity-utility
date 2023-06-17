using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Security.Policy;
using System.Net;

namespace ViitorCloud.Soreal {
    public class DownloadUtility {
        public static IEnumerator DownloadAsset(string url,
            string path,
            Action<string> OnDownloadComplete = null,
            Action<string> OnDownloadFail = null,
            Action<float> progress = null) {

            string ext = GetFileNameWithoutSpace(url);
            string filePath = Path.Combine(path, ext);
            if (File.Exists(filePath)) {
                OnDownloadComplete?.Invoke(filePath);
                yield break;
            }

            Uri uri = new Uri(url);
            UnityWebRequest WebRequest = UnityWebRequest.Get(uri.AbsoluteUri);
            WebRequest.SendWebRequest();

            while (WebRequest.downloadProgress != 1.0f) {
                yield return new WaitForEndOfFrame();
                progress?.Invoke(WebRequest.downloadProgress);
            }
            yield return new WaitUntil(() => WebRequest.downloadProgress == 1.0f);
            if (WebRequest.error != null) {
                OnDownloadFail?.Invoke(WebRequest.error);
                yield break;
            } else {
                string savePath = Path.Combine(path, GetFileNameWithoutSpace(url));
                File.WriteAllBytes(savePath, WebRequest.downloadHandler.data);
                OnDownloadComplete?.Invoke(savePath);
            }
        }

        public static async Task DownloadAssetAsync(string url,
            string path,
            Action<string> OnDownloadComplete = null,
            Action<string> OnDownloadFail = null,
            Action<float> progress = null) {
           
            string ext = GetFileNameWithoutSpace(url);
            string filePath = Path.Combine(path, ext);
            if (File.Exists(filePath)) {
                OnDownloadComplete?.Invoke(filePath);
                return;
            }

            using (UnityWebRequest request = UnityWebRequest.Get(url)) {
                UnityWebRequestAsyncOperation asyncOp = request.SendWebRequest();

                while (!asyncOp.isDone) {
                    progress?.Invoke(asyncOp.progress);
                    await Task.Delay(100);
                }

                if (request.result != UnityWebRequest.Result.Success) {
                    OnDownloadFail?.Invoke(request.error);
                } else {
                    string savePath = Path.Combine(path, GetFileNameWithoutSpace(url));
                    System.IO.File.WriteAllBytes(savePath, request.downloadHandler.data);
                    OnDownloadComplete?.Invoke(savePath);
                }
            }
        }
        private static string GetFileNameWithoutSpace(string url) {
            return Path.GetFileName(url).Replace("%20", " ");
        }
    }//DownloadUtility class end
}
