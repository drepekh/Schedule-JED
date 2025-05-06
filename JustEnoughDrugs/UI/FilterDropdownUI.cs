using UnityEngine;
using UnityEngine.UI;
using MelonLoader;
using System;
using System.Collections.Generic;
using JustEnoughDrugs.Utils;
using UnityEngine.Events;

namespace JustEnoughDrugs.UI
{
    public class FilterDropdownUI
    {
        private Dropdown dropdown;
        private readonly List<string> filterOptions = new List<string>() { "Any", "Name", "Effects", "Ingredients" };

        public string CurrentFilter => dropdown != null ? filterOptions[dropdown.value] : "Any";

        public event Action<string> OnFilterChanged;
        private readonly string GOName = "SearchDropdown";

        public bool Initialize(Transform parent)
        {
            try
            {
                dropdown = SetupDropdown(parent);
                return dropdown != null;
            }
            catch (Exception e)
            {
                MelonLogger.Error($"Error initializing filter dropdown: {e}");
                return false;
            }
        }

        private Dropdown SetupDropdown(Transform parent)
        {
            var deliveryApp = GameObject.Find("DeliveryApp");
            if (deliveryApp == null)
            {
                throw new Exception("DeliveryApp not found.");
            }

            var originalDropdown = TransformExtensions.FindChildByPath(
                deliveryApp.transform,
                "Container/Scroll View/Viewport/Content/Dan's Hardware/Contents/Panel/Destination/Dropdown (Legacy)")
                .GetComponent<Dropdown>();

            if (originalDropdown == null)
            {
                throw new Exception("Original dropdown not found.");
            }

            var clonedDropdownGO = GameObject.Instantiate(originalDropdown.gameObject);
            if (clonedDropdownGO == null)
            {
                throw new Exception("Failed to clone dropdown.");
            }

            clonedDropdownGO.name = GOName;
            clonedDropdownGO.transform.SetParent(parent, false);

            var clonedDropdown = clonedDropdownGO.GetComponent<Dropdown>();
            var clonedDropdownRect = clonedDropdownGO.GetComponent<RectTransform>();
            clonedDropdownRect.sizeDelta = new Vector2(-1030, 60);
            clonedDropdownRect.anchoredPosition = new Vector2(100, 30);

            clonedDropdown.ClearOptions();
            foreach (var option in filterOptions)
            {
                clonedDropdown.options.Add(new Dropdown.OptionData(option));
            }

            clonedDropdown.onValueChanged.AddListener((UnityAction<int>)(index => OnDropdownValueChanged(index)));
            return clonedDropdown;
        }

        private void OnDropdownValueChanged(int index)
        {
            OnFilterChanged?.Invoke(filterOptions[index]);
            MelonLogger.Msg($"Filter changed to: {filterOptions[index]}");
        }
    }
}