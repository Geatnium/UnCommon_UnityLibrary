using System;
using System.Reflection;
using System.Text;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Compilation;
using Assembly = System.Reflection.Assembly;
using System.IO;

namespace UnCommon
{

    /// <summary>
    /// プロジェクトビューの拡張スクリプト
    /// </summary>
    [InitializeOnLoad]
    static class ProjectBrowserExtensions
    {
        // コンストラクタ
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

        /// <summary>
        ///  C#スクリプトテンプレートの元ファイル名
        /// </summary>
        private const string CsScriptTemplateFileName = "CsTemplate.cs";

        /// <summary>
        /// C#スクリプトテンプレートの新規ファイル名
        /// </summary>
        private const string NewCsScriptFileName = "NewCsScript.cs";

        /// <summary>
        ///  MonoBehaviourスクリプトテンプレートの元ファイル名
        /// </summary>
        private const string MonoBehaviourScriptTemplateOriginalFileName = "MonoBehaviourScriptTemplate.cs";

        /// <summary>
        /// MonoBehaviourスクリプトテンプレートの新規ファイル名
        /// </summary>
        private const string NewMonoBehaviourScriptFileName = "NewMonoBehaviourScript.cs";

        /// <summary>
        ///  Componentスクリプトテンプレートの元ファイル名
        /// </summary>
        private const string ComponentScriptTemplateOriginalFileName = "ComponentTemplate.cs";

        /// <summary>
        /// Componentスクリプトテンプレートの新規ファイル名
        /// </summary>
        private const string NewComponentScriptFileName = "NewComponentScript.cs";

        /// <summary>
        ///  Componentのインターフェーステンプレートの元ファイル名
        /// </summary>
        private const string ComponentIFScriptTemplateOriginalFileName = "ComponentTemplateIF.cs";

        /// <summary>
        /// Componentのインターフェーステンプレートの新規ファイル名
        /// </summary>
        private const string NewComponentIFScriptFileName = "NewComponentScriptIF.cs";

        /// <summary>
        ///  ActorComponentスクリプトテンプレートの元ファイル名
        /// </summary>
        private const string ActorComponentScriptTemplateOriginalFileName = "ActorComponentTemplate.cs";

        /// <summary>
        /// ActorComponentスクリプトテンプレートの新規ファイル名
        /// </summary>
        private const string NewActorComponentScriptFileName = "NewActorComponentScript.cs";

        /// <summary>
        ///  ChildActorComponentスクリプトテンプレートの元ファイル名
        /// </summary>
        private const string ChildActorComponentScriptTemplateOriginalFileName = "ChildActorComponentTemplate.cs";

        /// <summary>
        /// ChildActorComponentスクリプトテンプレートの新規ファイル名
        /// </summary>
        private const string NewChildActorComponentScriptFileName = "NewChildActorComponentScript.cs";

        /// <summary>
        ///  WidgetComponentスクリプトテンプレートの元ファイル名
        /// </summary>
        private const string WidgetComponentScriptTemplateOriginalFileName = "WidgetComponentTemplate.cs";

        /// <summary>
        /// WidgetComponentスクリプトテンプレートの新規ファイル名
        /// </summary>
        private const string NewWidgetComponentScriptFileName = "NewWidgetComponentScript.cs";

        /// <summary>
        ///  Managerスクリプトテンプレートの元ファイル名
        /// </summary>
        private const string ManagerScriptTemplateOriginalFileName = "ManagerTemplate.cs";

        /// <summary>
        /// Managerスクリプトテンプレートの新規ファイル名
        /// </summary>
        private const string NewManagerScriptFileName = "NewManagerScript.cs";

        /// <summary>
        ///  Managerのインターフェーステンプレートの元ファイル名
        /// </summary>
        private const string ManagerIFScriptTemplateOriginalFileName = "ManagerTemplateIF.cs";

        /// <summary>
        /// Managerのインターフェーステンプレートの新規ファイル名
        /// </summary>
        private const string NewManagerIFScriptFileName = "NewManagerScriptIF.cs";

        /// <summary>
        /// テキストファイルの新規ファイル名
        /// </summary>
        private const string TextFileName = "NewText.txt";

        #endregion


        #region メニューに追加した処理（メイン処理）

        /// <summary>
        /// <br>新規MonoBehaviourスクリプトを作成</br>
        /// <br>CsTemplate.cs を複製したものであり、</br>
        /// <br>普通に [Create > C# Script] から作成すると文字化けするのを防ぐ。</br>
        /// </summary>
        [MenuItem("Assets/Create/UnCommon/Create Empty C# Script", false, 0)]
        private static void CreateEmptyCsScript()
        {
            // なぜか選択されているものが無ければ何もしない（無いことはないと思うけど）
            if (Selection.assetGUIDs == null) return;
            // 選択しているファイルの一つ目のGUIDからフォルダを取得
            string selectedFolderPath = GetParentFolderOfSelectionGUID();
            // スクリプトのテンプレート複製
            CopyAssetByAssetName(CsScriptTemplateFileName, selectedFolderPath, NewCsScriptFileName);
        }

        /// <summary>
        /// <br>新規MonoBehaviourスクリプトを作成</br>
        /// <br>NewBehaviourScriptTemplate.cs を複製したものであり、</br>
        /// <br>普通に [Create > C# Script] から作成すると文字化けするのを防ぐ。</br>
        /// </summary>
        [MenuItem("Assets/Create/UnCommon/Create Mono Behaviour Script", false, 0)]
        private static void CreateNewBehaviourScript()
        {
            // なぜか選択されているものが無ければ何もしない（無いことはないと思うけど）
            if (Selection.assetGUIDs == null) return;
            // 選択しているファイルの一つ目のGUIDからフォルダを取得
            string selectedFolderPath = GetParentFolderOfSelectionGUID();
            // スクリプトのテンプレート複製
            CopyAssetByAssetName(MonoBehaviourScriptTemplateOriginalFileName, selectedFolderPath, NewMonoBehaviourScriptFileName);
        }

        /// <summary>
        /// <br>新規Componentスクリプトを作成</br>
        /// <br>ComponentTemplate.cs を複製したもの</br>
        /// </summary>
        [MenuItem("Assets/Create/UnCommon/Create Component Script", false, 0)]
        private static void CreateNewComponentScript()
        {
            // なぜか選択されているものが無ければ何もしない（無いことはないと思うけど）
            if (Selection.assetGUIDs == null) return;
            // 選択しているファイルの一つ目のGUIDからフォルダを取得
            string selectedFolderPath = GetParentFolderOfSelectionGUID();
            // スクリプトのテンプレート複製
            CopyAssetByAssetName(ComponentScriptTemplateOriginalFileName, selectedFolderPath, NewComponentScriptFileName);
        }

        /// <summary>
        /// <br>新規Componentのインターフェースを作成</br>
        /// <br>ComponentTemplateIF.cs を複製したもの</br>
        /// </summary>
        [MenuItem("Assets/Create/UnCommon/Create Component Interface Script", false, 0)]
        private static void CreateNewComponentIFScript()
        {
            // なぜか選択されているものが無ければ何もしない（無いことはないと思うけど）
            if (Selection.assetGUIDs == null) return;
            // 選択しているファイルの一つ目のGUIDからフォルダを取得
            string selectedFolderPath = GetParentFolderOfSelectionGUID();
            // スクリプトのテンプレート複製
            CopyAssetByAssetName(ComponentIFScriptTemplateOriginalFileName, selectedFolderPath, NewComponentIFScriptFileName);
        }

        /// <summary>
        /// <br>新規ActorComponentスクリプトを作成</br>
        /// <br>ActorComponentTemplate.cs を複製したもの</br>
        /// </summary>
        [MenuItem("Assets/Create/UnCommon/Create Actor Component Script", false, 0)]
        private static void CreateNewActorComponentScript()
        {
            // なぜか選択されているものが無ければ何もしない（無いことはないと思うけど）
            if (Selection.assetGUIDs == null) return;
            // 選択しているファイルの一つ目のGUIDからフォルダを取得
            string selectedFolderPath = GetParentFolderOfSelectionGUID();
            // スクリプトのテンプレート複製
            CopyAssetByAssetName(ActorComponentScriptTemplateOriginalFileName, selectedFolderPath, NewActorComponentScriptFileName);
        }

        /// <summary>
        /// <br>新規ChildActorComponentスクリプトを作成</br>
        /// <br>ChildActorComponentTemplate.cs を複製したもの</br>
        /// </summary>
        [MenuItem("Assets/Create/UnCommon/Create Child Actor Component Script", false, 0)]
        private static void CreateNewChildActorComponentScript()
        {
            // なぜか選択されているものが無ければ何もしない（無いことはないと思うけど）
            if (Selection.assetGUIDs == null) return;
            // 選択しているファイルの一つ目のGUIDからフォルダを取得
            string selectedFolderPath = GetParentFolderOfSelectionGUID();
            // スクリプトのテンプレート複製
            CopyAssetByAssetName(ChildActorComponentScriptTemplateOriginalFileName, selectedFolderPath, NewChildActorComponentScriptFileName);
        }

        /// <summary>
        /// <br>新規WidgetComponentスクリプトを作成</br>
        /// <br>WidgetComponentTemplate.cs を複製したもの</br>
        /// </summary>
        [MenuItem("Assets/Create/UnCommon/Create Widget Component Script", false, 0)]
        private static void CreateNewWidgetComponentScript()
        {
            // なぜか選択されているものが無ければ何もしない（無いことはないと思うけど）
            if (Selection.assetGUIDs == null) return;
            // 選択しているファイルの一つ目のGUIDからフォルダを取得
            string selectedFolderPath = GetParentFolderOfSelectionGUID();
            // スクリプトのテンプレート複製
            CopyAssetByAssetName(WidgetComponentScriptTemplateOriginalFileName, selectedFolderPath, NewWidgetComponentScriptFileName);
        }

        /// <summary>
        /// <br>新規Managerスクリプトを作成</br>
        /// <br>ManagerTemplate.cs を複製したもの</br>
        /// </summary>
        [MenuItem("Assets/Create/UnCommon/Create Manager Script", false, 0)]
        private static void CreateNewManagerScript()
        {
            // なぜか選択されているものが無ければ何もしない（無いことはないと思うけど）
            if (Selection.assetGUIDs == null) return;
            // 選択しているファイルの一つ目のGUIDからフォルダを取得
            string selectedFolderPath = GetParentFolderOfSelectionGUID();
            // スクリプトのテンプレート複製
            CopyAssetByAssetName(ManagerScriptTemplateOriginalFileName, selectedFolderPath, NewManagerScriptFileName);
        }

        /// <summary>
        /// <br>新規Managerのインターフェースを作成</br>
        /// <br>ManagerTemplate.cs を複製したもの</br>
        /// </summary>
        [MenuItem("Assets/Create/UnCommon/Create Manager Interface Script", false, 0)]
        private static void CreateNewManagerIFScript()
        {
            // なぜか選択されているものが無ければ何もしない（無いことはないと思うけど）
            if (Selection.assetGUIDs == null) return;
            // 選択しているファイルの一つ目のGUIDからフォルダを取得
            string selectedFolderPath = GetParentFolderOfSelectionGUID();
            // スクリプトのテンプレート複製
            CopyAssetByAssetName(ManagerIFScriptTemplateOriginalFileName, selectedFolderPath, NewManagerIFScriptFileName);
        }

        /// <summary>
        /// 新規テキストファイルを作成する
        /// </summary>
        [MenuItem("Assets/Create/UnCommon/TextFile", false, 0)]
        private static void CreateTextFile()
        {
            // なぜか選択されているものが無ければ何もしない（無いことはないと思うけど）
            if (Selection.assetGUIDs == null) return;
            // 選択した親フォルダを取得
            string selectedFolderPath = GetParentFolderOfSelectionGUID();
            // 新規ファイル名の生成
            string newCreatePath = GenerateCreateFilePath(selectedFolderPath, TextFileName);
            // 空のテキストを書き込んでおく
            File.WriteAllText(newCreatePath, "", Encoding.UTF8);
            // 更新
            AssetDatabase.Refresh();
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