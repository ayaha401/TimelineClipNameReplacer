using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;

namespace AyahaGraphicDevelopTools.TimelineClipNameReplacer
{
    /// <summary>
    /// TimelineClipをコピーしたときや、複製したときにm_Nameに(Clone)と接尾が増えてしまうので
    /// それを消すEditor拡張
    /// https://hackerslab.aktsk.jp/2025/12/17/103000
    /// </summary>
    public class ClipNameReplacer : AssetModificationProcessor
    {
        /// <inheritdoc cref="OnWillSaveAssets"/>
        private static string[] OnWillSaveAssets(string[] paths)
        {
            // 変更が必要なScriptableObjectを格納する
            var dirtyAssets = new HashSet<ScriptableObject>();
            
            foreach (var path in paths)
            {
                var timeline = AssetDatabase.LoadAssetAtPath<TimelineAsset>(path);
                if (timeline == null)
                {
                    continue;
                }

                foreach (var track in timeline.GetOutputTracks())
                {
                    foreach (var clip in track.GetClips())
                    {
                        if (clip.asset == null)
                        {
                            continue;
                        }
                        
                        var playableAsset = clip.asset as ScriptableObject;
                        if (playableAsset == null)
                        {
                            continue;
                        }
                        
                        if (string.IsNullOrEmpty(playableAsset.name))
                        {
                            continue;
                        }
                        
                        playableAsset.name = string.Empty;
                        dirtyAssets.Add(playableAsset);
                    }
                }
            }
            
            foreach (var asset in dirtyAssets)
            {
                EditorUtility.SetDirty(asset);
            }

            return paths;
        }
    }
}
