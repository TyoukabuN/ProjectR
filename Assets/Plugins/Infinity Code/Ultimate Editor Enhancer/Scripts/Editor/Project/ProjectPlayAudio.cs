/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using InfinityCode.UltimateEditorEnhancer.UnityTypes;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.UltimateEditorEnhancer.ProjectTools
{
    [InitializeOnLoad]
    public static class ProjectPlayAudio
    {
        private static AudioClip clip;

        static ProjectPlayAudio()
        {
            ProjectItemDrawer.Register("PLAY_AUDIO", DrawButton, 10);
        }

        private static void DrawButton(ProjectItem item)
        {
            if (!Prefs.projectPlayAudio) return;
            if (!(item.asset is AudioClip)) return;
            
            bool isPlaying = clip == item.asset;
            
            if (isPlaying)
            {
                if (!AudioUtilsRef.IsClipPlaying(clip))
                {
                    isPlaying = false;
                    clip = null;
                }
            }
            
            if (!item.hovered && !isPlaying) return;
            
            Rect r = item.rect;
            r.xMin = r.xMax - 18;
            r.height = 16;

            item.rect.xMax -= 18;

            Texture icon;
            string tooltip;
            if (isPlaying)
            {
                icon = EditorIconContents.preAudioPlayOn.image;
                tooltip = "Stop Audio";
            }
            else
            {
                icon = EditorIconContents.playButtonOn.image;
                tooltip = "Play Audio";
            }

            ButtonEvent be = GUILayoutUtils.Button(r, TempContent.Get(icon, tooltip), GUIStyle.none);
            if (be == ButtonEvent.click)
            {
                Event e = Event.current;
                if (e.button == 0)
                {
                    if (isPlaying)
                    {
                        AudioUtilsRef.StopAllClips();
                        clip = null;
                    }
                    else
                    {
                        AudioUtilsRef.StopAllClips();
                        clip = item.asset as AudioClip;
                        AudioUtilsRef.PlayClip(clip);
                    }
                }
            }
        }
    }
}