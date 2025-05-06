using UnityEngine;
using UnityEngine.UI;
using MelonLoader;
using System;
using UnityEngine.Events;

namespace JustEnoughDrugs.UI
{
    public class SearchBarUI
    {
        private InputField inputField;
        private bool wasFocusedLastFrame = false;

        public string SearchText => inputField?.text ?? string.Empty;

        public event Action<string> OnSearchChanged;
        public event Action<bool> OnFocusChanged;

        public bool Initialize(Transform parent)
        {
            try
            {
                inputField = CreateSearchBar(parent);
                return inputField != null;
            }
            catch (Exception e)
            {
                MelonLogger.Error($"Error initializing search bar: {e}");
                return false;
            }
        }

        public void Update()
        {
            if (inputField == null) return;

            bool isFocused = inputField.isFocused;

            if (isFocused != wasFocusedLastFrame)
            {
                OnFocusChanged?.Invoke(isFocused);

                if (isFocused)
                    MelonLogger.Msg("Input field focused.");
                else
                    MelonLogger.Msg("Input field lost focus.");
            }

            wasFocusedLastFrame = isFocused;
        }

        private InputField CreateSearchBar(Transform parent)
        {
            GameObject inputGO = new GameObject("DrugSearchInput");
            inputGO.transform.SetParent(parent, false);
            inputGO.AddComponent<CanvasRenderer>();

            var rect = inputGO.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(1, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(1, 1);
            rect.sizeDelta = new Vector2(415, 60);

            var bg = inputGO.AddComponent<Image>();
            bg.color = new Color(1f, 1f, 1f, 0.9f);

            var inputField = inputGO.AddComponent<InputField>();

            // Placeholder
            CreatePlaceholder(inputGO, inputField);

            // Text component
            CreateTextComponent(inputGO, inputField);

            // Clear button
            CreateClearButton(inputGO);

            inputField.onValueChanged.AddListener((UnityAction<String>)(value => OnSearchTextChanged(value)));

            return inputField;
        }

        private void CreatePlaceholder(GameObject parent, InputField inputField)
        {
            GameObject placeholderGO = new GameObject("Placeholder");
            placeholderGO.transform.SetParent(parent.transform, false);

            var placeholder = placeholderGO.AddComponent<Text>();
            placeholder.text = "Search";
            placeholder.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            placeholder.fontSize = 30;
            placeholder.color = Color.gray;
            placeholder.alignment = TextAnchor.MiddleLeft;

            var placeholderRect = placeholder.GetComponent<RectTransform>();
            placeholderRect.anchorMin = Vector2.zero;
            placeholderRect.anchorMax = Vector2.one;
            placeholderRect.offsetMin = new Vector2(10, 5);
            placeholderRect.offsetMax = new Vector2(-10, -5);

            inputField.placeholder = placeholder;
        }

        private void CreateTextComponent(GameObject parent, InputField inputField)
        {
            GameObject textGO = new GameObject("Text");
            textGO.transform.SetParent(parent.transform, false);

            var text = textGO.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = 22;
            text.color = Color.black;
            text.alignment = TextAnchor.MiddleLeft;

            var textRect = text.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(10, 5);
            textRect.offsetMax = new Vector2(-10, -5);

            inputField.textComponent = text;
            inputField.text = "";
            inputField.ForceLabelUpdate();
        }

        private void CreateClearButton(GameObject parent)
        {
            GameObject clearBtnGO = new GameObject("ClearButton");
            clearBtnGO.transform.SetParent(parent.transform, false);

            var buttonText = clearBtnGO.AddComponent<Text>();
            buttonText.text = "X";
            buttonText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            buttonText.fontSize = 30;
            buttonText.color = Color.red;
            buttonText.alignment = TextAnchor.MiddleCenter;

            var buttonRect = clearBtnGO.GetComponent<RectTransform>();
            buttonRect.sizeDelta = new Vector2(40, 40);
            buttonRect.anchorMin = new Vector2(1, 1);
            buttonRect.anchorMax = new Vector2(1, 1);
            buttonRect.anchoredPosition = new Vector2(-15, -30);

            var button = clearBtnGO.AddComponent<Button>();
            button.onClick.AddListener((UnityAction)(() => ClearSearchText()));
        }

        private void ClearSearchText()
        {
            inputField.text = "";
        }

        private void OnSearchTextChanged(string value)
        {
            OnSearchChanged?.Invoke(value);
        }
    }
}