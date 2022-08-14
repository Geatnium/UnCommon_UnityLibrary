using System;
using System.Reflection;
using System.Text;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Compilation;
using Assembly = System.Reflection.Assembly;

namespace UnCommon
{

    /// <summary>
    /// プロジェクトビューの拡張スクリプト
    /// </summary>
    [InitializeOnLoad]
    static class ProjectBrowserExtensions
    {
        static ProjectBrowserExtensions()
        {
#if UNITY_EDITOR_OSX
            EditorApplication.projectWindowItemOnGUI += OnFirstTime;
            EditorApplication.projectChanged += OnChanged;
            EditorApplication.playModeStateChanged += OnPlayMode;
            CompilationPipeline.compilationFinished += OnCompilationFinished;
#endif
        }

        #region 定数

        private const string MonoBehaviourScriptTemplateOriginalFileName = "NewMonoBehaviourScriptTemplate.cs";
        private const string NewMonoBehaviourScriptFileName = "NewMonoBehaviourScript.cs";

        #endregion


        #region メニューに追加した処理（メイン処理）

        /// <summary>
        /// <br>新規MonoBehaviourスクリプトを作成</br>
        /// <br>NewBehaviourScriptTemplate.cs を複製したものであり、</br>
        /// <br>普通に [Create > C# Script] から作成すると文字化けするのを防ぐ。</br>
        /// </summary>
        [MenuItem("Assets/Create/Create Mono Behaviour Script", false, 0)]
        private static void CreateNewBehaviourScript()
        {
            // なぜか選択されているものが無ければ何もしない（無いことはないと思うけど）
            if (Selection.assetGUIDs == null) return;
            // 選択しているファイルの一つ目のGUIDからフォルダを取得
            string selectionFolderPath = GetParentFolderOfSelectionGUID();
            // スクリプトのテンプレート複製
            CopyAssetByAssetName(MonoBehaviourScriptTemplateOriginalFileName, selectionFolderPath, NewMonoBehaviourScriptFileName);
        }

        #endregion


        #region 処理もろもろ

        /// <summary>
        /// 指定の名前のファイルを探して複製する
        /// </summary>
        /// <param name="originalAssetName">複製したいファイル名</param>
        /// <param name="parentFolderPath">複製先のフォルダ</param>
        /// <param name="newAssetName">新しく生成されるファイル名</param>
        /// <returns></returns>
        private static bool CopyAssetByAssetName(string originalAssetName, string parentFolderPath, string newAssetName)
        {
            // 複製元のファイルを探す
            string originalFilePath = FindAssetPathWithContainsName(originalAssetName);
            if (!originalAssetName.IsNullOrEmpty())
            {
                string newCreateFilePath = GenerateCreateFilePath(parentFolderPath, newAssetName);
                // テンプレートをコピー
                if (AssetDatabase.CopyAsset(originalFilePath, newCreateFilePath))
                {
                    DebugLogger.Log($"新しいスクリプトを生成 : {newCreateFilePath}");
                    AssetDatabase.Refresh();
                    return true;
                }
                else
                {
                    DebugLogger.Log("スクリプトを生成できませんでした。");
                    return false;
                }
            }
            else
            {
                DebugLogger.Log($"{originalAssetName} が見つかりませんでした。");
                return false;
            }
        }

        /// <summary>
        /// 指定の文字列が含まれているアセットを探し、そのパスを返す
        /// </summary>
        /// <param name="fileName">探したいアセットの含まれる文字列</param>
        /// <returns></returns>
        private static string FindAssetPathWithContainsName(string name)
        {
            AssetDatabase.Refresh();
            // 全アセットのパスを取得し、探しているものがあったらそれを返す
            string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();
            for (int i = 0; i < allAssetPaths.Length; i++)
            {
                if (allAssetPaths[i].Contains(name))
                {
                    return allAssetPaths[i];
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// <br>現在選択しているファイルのGUIDから親のフォルダのパスを取得する</br>
        /// <br>（そもそもフォルダだったらそのフォルダのパスを返す）</br> 
        /// </summary>
        /// <returns></returns>
        private static string GetParentFolderOfSelectionGUID()
        {
            // 選択しているファイルの一つ目のGUIDからフォルダを取得
            string guid = Selection.assetGUIDs[0];
            string selectionPath = AssetDatabase.GUIDToAssetPath(guid);
            // ファイルを選択していた時用に親のフォルダを取得
            return GetParentFolderPath(selectionPath);
        }

        /// <summary>
        /// 指定のパスがフォルダじゃない場合は親のフォルダパスを返す
        /// </summary>
        /// <param name="path">変換したいパス</param>
        /// <returns></returns>
        private static string GetParentFolderPath(string path)
        {
            // フォルダーだったらそのまま
            if (AssetDatabase.IsValidFolder(path))
            {

                return path;
            }
            else
            {
#if UNITY_EDITOR_OSX
                // バックスラッシュで分割して、最後のふたつ（\xxx.yy）をカット
                string[] splits = path.Split("/");
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < splits.Length - 1; i++)
                {
                    builder.Append(splits[i]).Append("/");
                }
                builder.Remove(builder.Length - 1, 1);
                return builder.ToString();
#else
                // バックスラッシュで分割して、最後のふたつ（\xxx.yy）をカット
                string[] splits = path.Split("\\");
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < splits.Length - 2; i++)
                {
                    builder.Append(splits[i]);
                    DebugLogger.Log(builder.ToString());
                }
                return builder.ToString();
#endif

            }
        }

        /// <summary>
        /// 指定のフォルダ内に新しいファイルを生成する際のパスを作成する
        /// </summary>
        /// <param name="parentFolderPath">生成したいフォルダ</param>
        /// <param name="newFileName">新しく生成するファイル名</param>
        /// <returns></returns>
        private static string GenerateCreateFilePath(string parentFolderPath, string newFileName)
        {
            // フォルダ + \新しいファイル名 で文字列結合
            StringBuilder builder = new();
            builder.Append(parentFolderPath).Append("\\").Append(newFileName);
            string temp = builder.ToString();
            // 同じファイルがもしあった場合はユニークなパスを返す
            return AssetDatabase.GenerateUniqueAssetPath(temp);
        }

        #endregion


        #region Macでのフォルダー上詰め処理

#if UNITY_EDITOR_OSX

        private const string TempFolderName = "UnCommonEditorTemp";
        private const string TempFolderCreatePathFilter = "UnCommonEditor";

        private static void OnFirstTime(string guid, Rect selectionrect)
        {
            EditorApplication.projectWindowItemOnGUI -= OnFirstTime;
            Refresh();
            EditorRefresh();
        }

        private static void OnChanged()
        {
            Refresh();
        }

        private static void OnPlayMode(PlayModeStateChange obj)
        {
            Refresh();
            EditorRefresh();
        }

        private static void OnCompilationFinished(object obj)
        {
            Refresh();
            EditorRefresh();
        }

        private static void Refresh()
        {
            EditorApplication.delayCall += () =>
            {
                Assembly assembly = Assembly.GetAssembly(typeof(Editor));
                Type projectBrowser = assembly.GetType("UnityEditor.ProjectBrowser");
                FieldInfo field = projectBrowser.GetField("s_ProjectBrowsers", BindingFlags.Static | BindingFlags.NonPublic);
                if (field == null)
                    return;
                IEnumerable list = (IEnumerable)field.GetValue(projectBrowser);
                foreach (var pb in list)
                    SetFolderFirstForProjectWindow(pb);
            };
        }

        private static void EditorRefresh()
        {
            EditorApplication.delayCall += () =>
            {
                string tempFolderPath = FindAssetPathWithContainsName(TempFolderName);
                if (tempFolderPath.IsNullOrEmpty())
                {
                    string editorPath = FindAssetPathWithContainsName(TempFolderCreatePathFilter);
                    string parentPath = GetParentFolderPath(editorPath);
                    AssetDatabase.CreateFolder(parentPath, TempFolderName);
                }
                else
                {
                    AssetDatabase.DeleteAsset(tempFolderPath);
                }
            };
        }

        private static void SetFolderFirstForProjectWindow(object pb)
        {
            System.Collections.Generic.IEnumerable<FieldInfo> members = pb.GetType().GetRuntimeFields();
            foreach (FieldInfo member in members)
            {
                switch (member.Name)
                {
                    // One column
                    case "m_AssetTree":
                        SetOneColumnFolderFirst(pb, member);
                        break;
                    // Two column
                    case "m_ListArea":
                        SetTwoColumnFolderFirst(pb, member);
                        break;
                }
            }
        }

        private static void SetTwoColumnFolderFirst(object pb, FieldInfo listAreaField)
        {
            if (listAreaField == null)
                return;
            object listArea = listAreaField.GetValue(pb);
            if (listArea == null) return;
            PropertyInfo folderFirst = listArea.GetType().GetProperties().Single(x => x.Name == "foldersFirst");
            folderFirst.SetValue(listArea, true);
        }

        private static void SetOneColumnFolderFirst(object pb, FieldInfo assetTreeField)
        {
            if (assetTreeField == null)
                return;
            object assetTree = assetTreeField.GetValue(pb);
            if (assetTree == null) return;
            PropertyInfo data = assetTree.GetType().GetRuntimeProperties().Single(x => x.Name == "data");
            // AssetsTreeViewDataSource
            object dataSource = data.GetValue(assetTree);
            PropertyInfo folderFirst = dataSource.GetType().GetProperties().Single(x => x.Name == "foldersFirst");
            folderFirst.SetValue(dataSource, true);
        }

#endif

        #endregion
    }

}