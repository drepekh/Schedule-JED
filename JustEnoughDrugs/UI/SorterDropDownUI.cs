using UnityEngine;
using UnityEngine.UI;
using MelonLoader;
using System;
using System.Collections.Generic;
using JustEnoughDrugs.Utils;
using UnityEngine.Events;

namespace JustEnoughDrugs.UI
{
    public class SorterDropDownUI
    {
        private Dropdown sorterDropdown;
        private Dropdown orderDropdown;
        private readonly List<string> sorterOptions = new() { "Newest", "Addictiveness", "Cost", "Price", "Profit" };
        private readonly List<string> sortOrderOptions = new() { "Asc", "Desc" };

        public string CurrentSorter => sorterDropdown != null ? sorterOptions[sorterDropdown.value] : "Newest";
        public string CurrentOrder => orderDropdown != null ? sortOrderOptions[orderDropdown.value] : "Asc";

        public event Action<string, string> OnSorterChanged;

        private readonly string sorterDropdownName = "SorterDropdown";
        private readonly string orderDropdownName = "SortOrderDropdown";

        public bool Initialize(Transform parent)
        {
            try
            {
                (sorterDropdown, orderDropdown) = SetupDropdowns(parent);
                return sorterDropdown != null && orderDropdown != null;
            }
            catch (Exception e)
            {
                MelonLogger.Error($"Error initializing sorter dropdowns: {e}");
                return false;
            }
        }

        private (Dropdown, Dropdown) SetupDropdowns(Transform parent)
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

            var sorterDropdownGO = GameObject.Instantiate(originalDropdown.gameObject);
            if (sorterDropdownGO == null)
            {
                throw new Exception("Failed to clone sorter dropdown.");
            }

            sorterDropdownGO.name = sorterDropdownName;
            sorterDropdownGO.transform.SetParent(parent, false);

            var sorterComponent = sorterDropdownGO.GetComponent<Dropdown>();
            var sorterRect = sorterDropdownGO.GetComponent<RectTransform>();
            sorterRect.sizeDelta = new Vector2(-1030, 60);
            sorterRect.anchoredPosition = new Vector2(-285, 30);

            sorterComponent.ClearOptions();
            foreach (var option in sorterOptions)
            {
                sorterComponent.options.Add(new Dropdown.OptionData(option));
            }

            var orderDropdownGO = GameObject.Instantiate(originalDropdown.gameObject);
            if (orderDropdownGO == null)
            {
                throw new Exception("Failed to clone order dropdown.");
            }

            orderDropdownGO.name = orderDropdownName;
            orderDropdownGO.transform.SetParent(parent, false);

            var orderComponent = orderDropdownGO.GetComponent<Dropdown>();
            var orderRect = orderDropdownGO.GetComponent<RectTransform>();
            orderRect.sizeDelta = new Vector2(-1030, 60);
            orderRect.anchoredPosition = new Vector2(-115, 30);
            orderRect.offsetMax = new Vector2(-700, 60);

            orderComponent.ClearOptions();
            foreach (var option in sortOrderOptions)
            {
                orderComponent.options.Add(new Dropdown.OptionData(option));
            }

            sorterComponent.onValueChanged.AddListener((UnityAction<int>)(index =>
            {
                OnDropdownValueChanged();
            }));

            orderComponent.onValueChanged.AddListener((UnityAction<int>)(index =>
            {
                OnDropdownValueChanged();
            }));

            return (sorterComponent, orderComponent);
        }

        private void OnDropdownValueChanged()
        {
            string currentSorter = CurrentSorter;
            string currentOrder = CurrentOrder;

            OnSorterChanged?.Invoke(currentSorter, currentOrder);
            MelonLogger.Msg($"Sort changed to: {currentSorter} ({currentOrder})");
        }
    }
}