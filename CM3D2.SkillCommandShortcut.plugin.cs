/*
  夜伽コマンド用ショートカットを付加するUnityInjector用プラグイン

    ・「1」～「9」キー等で夜伽コマンドの実行ができるようになります
    ・「Enter」キーでメッセージウィンドウの進行ができるようになります
    ・「Enter」キーでダイアログやメニュー画面の「OK」「Next」ボタンを押すことができるようになります
    ・「1」～「9」キーで選択肢やメイドの選択ができるようになります


▼ 前提

  ・Microsoft .NET Framework 3.5 がインストール済み
  ・UnityInjector が動作している
  ・C:\KISS\CM3D2_KAIZOU\ 下にゲーム一式がある


▼ コンパイル

(1) 今見ているこのファイルを、「CM3D2.SkillCommandShortCut.Plugin.cs」という名前で保存してください

(2) 「CM3D2.SkillCommandShortCut.Plugin.cs」を
    「C:\KISS\CM3D2_KAIZOU\UnityInjector\」フォルダーへコピーして、
    「C:\KISS\CM3D2_KAIZOU\UnityInjector\CM3D2.SkillCommandShortCut.Plugin.cs」というファイルが
    存在するようにしてください

(3) 保存した後、以下のコマンドを実行します

―――＜＜コマンドプロンプトでここから＞＞―――

cd /d C:\KISS\CM3D2_KAIZOU\UnityInjector
C:\Windows\Microsoft.NET\Framework\v3.5\csc /t:library /lib:..\CM3D2x64_Data\Managed /r:UnityInjector.dll /r:UnityEngine.dll /r:Assembly-CSharp.dll CM3D2.SkillCommandShortCut.Plugin.cs

―――＜＜ここまで実行＞＞―――

コンパイルに成功すると、同じフォルダーに「CM3D2.SkillCommandShortCut.Plugin.dll」というファイルが生成されます。


▼ 動作確認

  コンパイル後 C:\KISS\CM3D2_KAIZOU\CM3D2x64.exe を起動して、
  夜伽画面へ行き、コマンド選択肢の左に数字がついていることを確認してください。
  対応するキーを押すことで、コマンドが実行されます


▼ 履歴

    0.1.2   画面下部のメッセージウィンドウのテキストを「Enter」キーで行う機能を追加
            モーダルダイアログの「OK」ボタンを「Enter」キーで押す機能を追加
            昼/夜メニューの「Next」、昼/夜メニューの結果報告の「OK」、毎日収支報告の「OK」を「Enter」キーで押す機能を追加
            夜伽ステージ選択の「OK」、スキル選択の「Next」、スキル実行時の「Next」、完了画面の「Next」を「Enter」キーで押す機能を追加
            コミュニケーション、会話の選択肢を「1」キー等で押す機能を追加
            メイド選択画面のメイドアイコンを「1」キー等で押す機能を追加
            モーダルダイアログ、夜伽メイド選択の「Cancel」を「BackSpace」キーで押す機能を追加

    0.1.1   ショートカットが有効な場合のみ、キーのアイコンが表示されるように修正 (その１>>891)
            スキル変更ボタンを押した直後にコマンドを実行し続けると、ゲームが進行不能になるバグを修正 (その１>>978)

    0.1.0   最初の版

*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityInjector;
using UnityInjector.Attributes;

namespace CM3D2.SkillCommandShortCut.Plugin
{
    [PluginFilter("CM3D2x64"),
    PluginFilter("CM3D2x86"),
    PluginFilter("CM3D2VRx64"),
    PluginName("CM3D2 Skill Command Shortcut"),
    PluginVersion("0.1.2.0")]
    public class SkillCommandShortCut : PluginBase
    {
        GameObject uiObject;
        Dictionary<string, int> dicKeyNameToIndex;
        List<Label> labels;
        const float labelWidth = 18f;

        KeyCode kcMessageWindowClick = KeyCode.Return;
        KeyCode kcCancel = KeyCode.Backspace;

        void Awake()
        {
            GameObject.DontDestroyOnLoad(this);

            dicKeyNameToIndex = new Dictionary<string, int>();
            dicKeyNameToIndex.Add("1", 0);
            dicKeyNameToIndex.Add("2", 1);
            dicKeyNameToIndex.Add("3", 2);
            dicKeyNameToIndex.Add("4", 3);
            dicKeyNameToIndex.Add("5", 4);
            dicKeyNameToIndex.Add("6", 5);
            dicKeyNameToIndex.Add("7", 6);
            dicKeyNameToIndex.Add("8", 7);
            dicKeyNameToIndex.Add("9", 8);
            dicKeyNameToIndex.Add("a", 9);
            dicKeyNameToIndex.Add("b", 10);
            dicKeyNameToIndex.Add("c", 11);
            dicKeyNameToIndex.Add("d", 12);
            dicKeyNameToIndex.Add("e", 13);
            dicKeyNameToIndex.Add("f", 14);
            dicKeyNameToIndex.Add("g", 15);
            dicKeyNameToIndex.Add("h", 16);
            dicKeyNameToIndex.Add("i", 17);
            dicKeyNameToIndex.Add("j", 18);
            dicKeyNameToIndex.Add("k", 19);
        }

        public void OnLevelWasLoaded(int level)
        {
            uiObject = null;
            if (level == 14) // 夜伽中
            {
                uiObject = GameObject.Find("/UI Root/YotogiPlayPanel/CommandViewer/SkillViewer/MaskGroup/SkillGroup/CommandParent/CommandUnit");
            }
        }

        public void OnGUI()
        {
            if (labels == null)
            {
                return;
            }

            foreach (Label label in labels)
            {
                GUI.Label(label.rect, label.name.ToUpper(), label.style);
            }
        }

        public void Update()
        {
            try
            {
                Update2();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception : {0}", e);
            }
        }

        public void Update2()
        {
            labels = null;

            if (UpdateDialogClick())
            {
                return;
            }
            if (UpdateMessageWindowClick())
            {
                return;
            }
            if (UpdateNextButtonClick())
            {
                return;
            }
            if (UpdateCancelButtonClick())
            {
                return;
            }

            //	昼コミュニケーション選択
            //	夜伽メイド会話選択
            if (Application.loadedLevel == 15)
            {
                GameObject go = GameObject.Find("__GameMain__/SystemUI Root/MessageWindowPanel/SelectorViewer/Contents/SelectButtonParent");
                UpdateLabels(go, "selectButton");
                UIButton uiButton = UpdateKeys();
                if (uiButton != null)
                {
                    return;
                }
            }

            //	昼コミュニケーションメイド選択
            //	夜伽メイド選択画面
            if (Application.loadedLevel == 1)
            {
                GameObject go = GameObject.Find("UI Root/Parent/CharacterSelectPanel/Contents/MaidSkillUnitParent");
                UpdateMaidLabels(go, "MaidPlate(Clone)", "Button");
                UIButton uiButton = UpdateKeys();
                if (uiButton != null)
                {
                    return;
                }
            }

            //	夜伽ステージ選択
            if (Application.loadedLevel == 14)
            {
                GameObject go = GameObject.Find("UI Root/StageSelectPanel/StageSelectViewer/StageViewer/Contents/StageUnitParent");
                UpdateMaidLabels(go, "StageUnit(Clone)", "Parent");
                UIButton uiButton = UpdateKeys();
                if (uiButton != null)
                {
                    return;
                }
            }

            // 夜伽コマンド実行
            {
                UpdateLabels(uiObject, "cm:");
                UIButton uiButton = UpdateKeys();
                if (uiButton != null)
                {
                    return;
                }
            }
        }

        void UpdateLabels(GameObject ui, string prefix)
        {
            if (ui == null || !ui.activeInHierarchy || !GameMain.Instance.MainCamera.IsFadeStateNon())
            {
                return;
            }

            labels = new List<Label>();

            foreach (Transform t in ui.transform)
            {
                if (!t.name.StartsWith(prefix) || t.gameObject == null)
                {
                    continue;
                }

                int index = labels.Count;
                var kv = dicKeyNameToIndex.FirstOrDefault(e => e.Value == index);
                if (kv.Key == null)
                {
                    continue;
                }

                Rect rect = GetBoxColliderRect(t.gameObject);
                rect.x = rect.x - labelWidth;
                rect.width = labelWidth;

                labels.Add(new Label
                {
                    gameObject = t.gameObject,
                    index = index,
                    rect = rect,
                    name = kv.Key,
                    style = "box"
                });
            }
        }

        void UpdateMaidLabels(GameObject ui, string prefix, string childName)
        {
            if (ui == null || !ui.activeInHierarchy || !GameMain.Instance.MainCamera.IsFadeStateNon())
            {
                return;
            }

            labels = new List<Label>();

            foreach (Transform tt in ui.transform)
            {
                if (!tt.name.StartsWith(prefix) || tt.gameObject == null)
                {
                    continue;
                }

                Transform t = tt.Find(childName);
                if (t == null || t.gameObject == null)
                {
                    continue;
                }

                int index = labels.Count;
                var kv = dicKeyNameToIndex.FirstOrDefault(e => e.Value == index);
                if (kv.Key == null)
                {
                    continue;
                }

                Rect rect = GetBoxColliderRect(t.gameObject);
                string style = "box";
                string name = kv.Key;
                if (rect.width == 0 && rect.height == 0)
                {
                    style = "";
                    name = "";
                }
                else
                {
                    rect.x = rect.x - labelWidth;
                    rect.width = labelWidth;
                }

                labels.Add(new Label
                {
                    gameObject = t.gameObject,
                    index = index,
                    rect = rect,
                    name = name,
                    style = style
                });
            }
        }

        UIButton UpdateKeys()
        {
            var kv = dicKeyNameToIndex.FirstOrDefault(e => Input.GetKeyDown(e.Key));
            if (kv.Key == null)
            {
                return null;
            }

            if (labels == null)
            {
                return null;
            }

            int index = kv.Value;
            Label label = labels.FirstOrDefault(e => e.index == index);
            if (label == null)
            {
                return null;
            }

            UIButton uiButton = label.gameObject.GetComponent<UIButton>();
            if (uiButton == null || !uiButton.isEnabled)
            {
                return null;
            }
            uiButton.SendMessage("OnClick");
            return uiButton;
        }

        // ダイアログの「OK」「Cancel」をクリックする
        bool UpdateDialogClick()
        {
            SystemDialog systemDialog = GameMain.Instance.SysDlg;
            if (systemDialog.gameObject.activeSelf)
            {
                if (Input.GetKeyDown(kcMessageWindowClick))
                {
                    SystemDialogOnClickOk(systemDialog);
                    return true;
                }
                if (Input.GetKeyDown(kcCancel))
                {
                    SystemDialogOnClickCancel(systemDialog);
                    return true;
                }
            }
            return false;
        }

        // 画面下部のメッセージウィンドウをクリックする
        bool UpdateMessageWindowClick()
        {
            if (Input.GetKeyDown(kcMessageWindowClick))
            {
                MessageWindowMgr mwm = GameMain.Instance.ScriptMgr.adv_kag.MessageWindowMgr;
                if (mwm.IsClickAction() && mwm.on_click_event_ != null)
                {
                    mwm.on_click_event_();
                    return true;
                }
            }
            return false;
        }

        bool UpdateNextButtonClick()
        {
            if (!Input.GetKeyDown(kcMessageWindowClick))
            {
                return false;
            }

            // メイド選択
            if (Application.loadedLevel == 1)
            {
                // メイド選択画面での「OK」ボタン
                if (ClickGameObject("/UI Root/Parent/ButtonParent/OK"))
                {
                    return true;
                }
                return false;
            }

            // メニュー
            if (Application.loadedLevel == 3)
            {
                // 昼/夜メニューの「Next」ボタン
                if (ClickGameObject("/UI Root/DailyPanel/Next"))
                {
                    return true;
                }
                // 昼/夜結果報告画面の「OK」ボタン
                if (ClickGameObject("/UI Root/ResultWorkPanel/Ok"))
                {
                    return true;
                }
                // 収支報告画面の「OK」ボタン
                if (ClickGameObject("/UI Root/ResultIncomePanel/Ok"))
                {
                    return true;
                }
                return false;
            }

            // 夜伽
            if (Application.loadedLevel == 14)
            {
                // 夜伽ステージ選択時の「OK」ボタン
                if (ClickGameObject("/UI Root/StageSelectPanel/Ok"))
                {
                    return true;
                }

                // 夜伽スキル選択時の「Next」ボタン
                if (ClickGameObject("/UI Root/SkillSelectPanel/CommonPanel/UnderButtonGroup/Next"))
                {
                    return true;
                }

                // 夜伽スキル実行中の「Next」ボタン
                if (ClickGameObject("/UI Root/YotogiPlayPanel/CommonPanel/UnderButtonGroup/Next"))
                {
                    return true;
                }

                // 夜伽完了画面の「Next」ボタン
                if (ClickGameObject("/UI Root/ResultPanel/CommonPanel/UnderButtonGroup/Next"))
                {
                    return true;
                }
                return false;
            }

            return false;
        }

        bool UpdateCancelButtonClick()
        {
            if (!Input.GetKeyDown(kcCancel))
            {
                return false;
            }

            if (Application.loadedLevel == 1)
            {
                // 夜伽メイド選択画面の「Cancel」ボタン
                if (ClickGameObject("/UI Root/Parent/ButtonParent/Cancel"))
                {
                    return true;
                }
                return false;
            }

            return false;
        }

        static bool ClickGameObject(string name)
        {
            GameObject go = GameObject.Find(name);
            if (go != null)
            {
                UIButton uiButton = go.GetComponent<UIButton>();
                if (uiButton != null)
                {
                    Console.WriteLine("{0} . isEnabled = {1}", name, uiButton.isEnabled);
                    if (uiButton.isEnabled)
                    {
                        Console.WriteLine("{0} . OnClick", name);
                        uiButton.SendMessage("OnClick");
                        return true;
                    }
                }
            }
            else
            {
                Console.WriteLine("{0} is not found", name);
            }
            return false;
        }

        static Rect GetBoxColliderRect(GameObject gameObject)
        {
            BoxCollider boxCollider = gameObject.GetComponent<BoxCollider>();
            if (boxCollider == null)
            {
                return new Rect();
            }
            // http://answers.unity3d.com/questions/295408/
            Vector3 pt = boxCollider.transform.TransformPoint(boxCollider.center);
            pt = UICamera.currentCamera.WorldToScreenPoint(pt);
            float width = boxCollider.size.x;
            float height = boxCollider.size.y;
            return new Rect(
                pt.x - width / 2,
                Screen.height - pt.y - height / 2,
                width,
                height
            );
        }

        static void SystemDialogOnClickOk(SystemDialog systemDialog)
        {
            // class SystemDialog { private void OnClickOk(); }
            MethodInfo methodInfo = typeof(SystemDialog).GetMethod(
                "OnClickOk",
                BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new Type[] { },
                null
            );
            methodInfo.Invoke(systemDialog, new object[] { });
        }

        static void SystemDialogOnClickCancel(SystemDialog systemDialog)
        {
            // class SystemDialog { private void OnClickCancel(); }
            MethodInfo methodInfo = typeof(SystemDialog).GetMethod(
                "OnClickCancel",
                BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new Type[] { },
                null
            );
            methodInfo.Invoke(systemDialog, new object[] { });
        }
    }

    internal class Label
    {
        public GameObject gameObject;
        public int index;
        public Rect rect;
        public string name;
        public GUIStyle style;
    }
}