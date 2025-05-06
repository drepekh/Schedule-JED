using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using MelonLoader;
using JustEnoughDrugs.Models;
#if IL2CPP
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne;
using Il2CppScheduleOne.Product;
#else
using ScheduleOne.PlayerScripts;
using ScheduleOne.DevUtilities;
using ScheduleOne;
using ScheduleOne.Product;
using ScheduleOne.Property;
#endif

namespace JustEnoughDrugs.UI
{
    public class UIManager
    {
        private SearchBarUI searchBar;
        private FilterDropdownUI filterDropdown;
        private SorterDropDownUI sorterDropdown;
        private DrugListUI drugList;
        private RecipeUI recipeUI;
        private List<InputAction> disabledActions = new List<InputAction>();

        public bool Initialize()
        {
            var productManagerApp = GameObject.Find("ProductManagerApp");
            if (productManagerApp == null || !productManagerApp.activeInHierarchy)
                return false;

            var topbar = Utils.TransformExtensions.FindChildByPath(productManagerApp.transform, "Container/Topbar");
            if (topbar == null)
                return false;

            searchBar = new SearchBarUI();
            filterDropdown = new FilterDropdownUI();
            sorterDropdown = new SorterDropDownUI();
            drugList = new DrugListUI();
            recipeUI = new RecipeUI();

            bool searchBarInitialized = searchBar.Initialize(topbar);
            bool dropdownInitialized = filterDropdown.Initialize(topbar);
            bool sorterInitialized = false;
            if (ModConfig.EnableDrugSorting)
            {
                sorterInitialized = sorterDropdown.Initialize(topbar);
            }
            bool drugListInitialized = drugList.Initialize(productManagerApp.transform);

            // Connect components
            if (searchBarInitialized && dropdownInitialized && drugListInitialized && (sorterInitialized || !ModConfig.EnableDrugSorting))
            {
                searchBar.OnSearchChanged += HandleSearchChanged;
                filterDropdown.OnFilterChanged += HandleFilterChanged;
                searchBar.OnFocusChanged += HandleSearchFocus;
                if (ModConfig.EnableDrugSorting)
                {
                    sorterDropdown.OnSorterChanged += HandleSorterChanged;
                }
            }

            return searchBarInitialized && dropdownInitialized && drugListInitialized && (sorterInitialized || !ModConfig.EnableDrugSorting);
        }

        public void Update()
        {
            searchBar?.Update();
        }

        private void HandleSearchChanged(string searchText)
        {
            drugList.UpdateDrugDisplay(searchText, filterDropdown.CurrentFilter);
        }

        private void HandleFilterChanged(string filter)
        {
            drugList.UpdateDrugDisplay(searchBar.SearchText, filter);
        }

        private void HandleSorterChanged(string sorterType, string sortOrder)
        {
            drugList.ReorderDrugs(sorterType, sortOrder);
        }

        private void HandleSearchFocus(bool focused)
        {
            if (focused)
                DisableShortcuts();
            else
                RestoreShortcuts();
        }

        public GameObject AddTextElement(string name, string content, Transform parent, int siblingIndex, Color color, int fontSize)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);

            var text = go.AddComponent<Text>();
            text.text = content;
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = fontSize;
            text.color = color;
            text.alignment = TextAnchor.UpperLeft;

            var rect = text.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(300, 30);
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;

            if (siblingIndex >= 0)
                go.transform.SetSiblingIndex(siblingIndex);
            else
                go.transform.SetAsLastSibling();

            return go;
        }

        private void DisableShortcuts()
        {
            var input = Singleton<GameInput>.Instance;
            var inputAsset = input.PlayerInput;

#if IL2CPP
            foreach (var actionMap in inputAsset.actions.actionMaps)
            {
                foreach (var action in actionMap.actions)
                {
                    string bindingString = "";
                    foreach (var binding in action.bindings)
                    {
                        bindingString += ",";
                        bindingString += binding.path;
                    }

                    if (bindingString.Contains("/keyboard/tab") ||
                        bindingString.Contains("/keyboard/escape") ||
                        bindingString.Contains("/mouse/rightButton"))
                    {
                        continue;
                    }

                    if (action.enabled)
                    {
                        action.Disable();
                        disabledActions.Add(action);
                    }
                }
            }
#else
            foreach (var action in inputAsset.actions)
            {
                string bindingString = string.Join(",", action.bindings.Select(b => b.path));

                if (bindingString.Contains("/keyboard/tab") ||
                    bindingString.Contains("/keyboard/escape") ||
                    bindingString.Contains("/mouse/rightButton"))
                {
                    continue;
                }

                if (action.enabled)
                {
                    action.Disable();
                    disabledActions.Add(action);
                }
            }
#endif
        }

        private void RestoreShortcuts()
        {
            foreach (var action in disabledActions)
            {
                action.Enable();
            }
            disabledActions.Clear();
        }

        public void DetailClicked(ProductEntry product)
        {
            MelonLogger.Msg($"{product.Definition.Name} clicked");
            if (product?.Definition == null) return;

            var definition = product.Definition;
            var viewport = GameObject.Find("ProductManagerApp/Container/Details/Scroll View/Viewport/Content");

            if (viewport == null) return;

            string[] toRemove = { "CostDisplay", "FullRecipeTitle", "FullRecipeList", "FullRecipeValue" };
            foreach (var name in toRemove)
            {
                var child = viewport.transform.Find(name);
                if (child != null)
                    GameObject.Destroy(child.gameObject);
            }

            if (MainMod.ProductCosts.TryGetValue(definition, out var cost))
            {
                var costGo = AddTextElement("CostDisplay", "ingredient costs:", viewport.transform, 3, Color.white, 16);
                var costVal = AddTextElement("CostValue", $"${cost}", costGo.transform, 3, Color.white, 14);
                var rectTransform = costVal.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(78, 0);
            }

            if (ModConfig.ShowFullRecipe)
            {

                AddTextElement("FullRecipeTitle", "Full recipe(s) :", viewport.transform, -1, Color.white, 16);
                if (MainMod.ExtendedRecipes.TryGetValue(definition, out var recipes) && recipes.Count > 0)
                {

                    recipeUI.BuildFullRecipe(viewport.transform, recipes, definition);
                }
            }
            var beforeSpace = viewport.transform.Find("Space");
            if (beforeSpace != null)
            {
                beforeSpace.SetAsLastSibling();
            }
        }

    }
}