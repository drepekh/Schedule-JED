using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if IL2CPP
using Il2CppScheduleOne.Product;
using Il2CppScheduleOne.UI.Tooltips;
using Il2CppInterop.Runtime;
#else
using ScheduleOne.Property;
using ScheduleOne.Product;
using ScheduleOne.UI.Tooltips;
#endif

namespace JustEnoughDrugs.UI
{
    public class RecipeUI
    {
        private const int ITEMS_PER_ROW = 5;
        private const float INGREDIENT_SIZE = 50f;
        private const float SEPARATOR_SIZE = 24f;
        private const float ROW_SPACING = 5f;
        private const float ITEM_SPACING = 10f;

        public void BuildFullRecipe(Transform parent, List<PropertyItemDefinition> recipe, PropertyItemDefinition definition)
        {
            GameObject root = CreateRecipeContainer(parent);

            for (int i = 0; i < recipe.Count; i++)
            {
                GameObject currentLine = GetOrCreateRow(root.transform, i);

                AddIngredientToLine(currentLine.transform, recipe[i]);

                bool isLast = i == recipe.Count - 1;
                bool isEndOfLine = (i + 1) % ITEMS_PER_ROW == 0;

                if (!isEndOfLine || isLast)
                {
                    AddSeparator(currentLine.transform, isLast);
                }

                if (isLast)
                {
                    AddResultIcon(currentLine.transform, definition);
                }
            }
        }

        private GameObject CreateRecipeContainer(Transform parent)
        {
            var root = new GameObject("FullRecipeValue", TypeOf<RectTransform>(), TypeOf<VerticalLayoutGroup>());
            root.transform.SetParent(parent, false);

            var verticalLayout = root.GetComponent<VerticalLayoutGroup>();
            verticalLayout.spacing = ROW_SPACING;
            verticalLayout.childAlignment = TextAnchor.MiddleLeft;
            verticalLayout.childForceExpandHeight = false;
            verticalLayout.childForceExpandWidth = false;

            return root;
        }

        private GameObject GetOrCreateRow(Transform parent, int itemIndex)
        {
            int rowIndex = itemIndex / ITEMS_PER_ROW;
            string rowName = $"Line_{rowIndex}";

            Transform existingRow = parent.Find(rowName);
            if (existingRow != null)
                return existingRow.gameObject;

            var row = new GameObject(rowName, TypeOf<RectTransform>(), TypeOf<HorizontalLayoutGroup>());
            row.transform.SetParent(parent, false);

            var layout = row.GetComponent<HorizontalLayoutGroup>();
            layout.spacing = ITEM_SPACING;
            layout.childAlignment = TextAnchor.MiddleLeft;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = false;

            return row;
        }

        private void AddIngredientToLine(Transform lineParent, PropertyItemDefinition ingredient)
        {
            var ingGO = new GameObject(ingredient.name, TypeOf<Image>());
            ingGO.transform.SetParent(lineParent, false);
            var tooltip = ingGO.AddComponent<Tooltip>();
            tooltip.text = ingredient.Name;
            var img = ingGO.GetComponent<Image>();
            img.sprite = ingredient.Icon;
            img.preserveAspect = true;
            img.rectTransform.sizeDelta = new Vector2(INGREDIENT_SIZE, INGREDIENT_SIZE);
        }

        private void AddSeparator(Transform lineParent, bool isLastIngredient)
        {
            var separatorName = isLastIngredient ? "Arrow" : "Plus";
            var recipeContainer = GameObject.Find("RecipesContainer");

            if (recipeContainer == null) return;


            var firstRecipe = recipeContainer.transform.GetChild(0);
            var originalSeparator = firstRecipe.Find(separatorName);

            if (originalSeparator == null) return;


            var separator = GameObject.Instantiate(originalSeparator);
            separator.transform.SetParent(lineParent, false);

            var rect = separator.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.sizeDelta = new Vector2(SEPARATOR_SIZE, SEPARATOR_SIZE);
                rect.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            }
        }

        private void AddResultIcon(Transform lineParent, PropertyItemDefinition definition)
        {
            var resultGO = new GameObject(definition.name + "_Result", TypeOf<Image>());
            resultGO.transform.SetParent(lineParent, false);

            var resultImg = resultGO.GetComponent<Image>();
            var tooltip = resultGO.AddComponent<Tooltip>();
            tooltip.text = definition.Name;
            resultImg.sprite = definition.Icon;
            resultImg.preserveAspect = true;
            resultImg.rectTransform.sizeDelta = new Vector2(INGREDIENT_SIZE, INGREDIENT_SIZE);
        }

#if IL2CPP
        private static Il2CppSystem.Type TypeOf<T>()
        {
            return Il2CppType.Of<T>();
        }
#else
        private static Type TypeOf<T>()
        {
            return typeof(T);
        }
#endif
    }
}