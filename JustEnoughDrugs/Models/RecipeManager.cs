using System.Collections.Generic;
#if IL2CPP
using Il2CppScheduleOne.Product;
#else
using ScheduleOne.Product;
using ScheduleOne.Property;
#endif

namespace JustEnoughDrugs.Models
{
    public static class RecipeManager
    {
        public static void ProcessNewRecipe(ProductDefinition product)
        {
            if (product == null)
                return;

            var ingredients = DeepSearchRecipe(product);

            float totalCost = CalculateTotalCost(ingredients);

            MainMod.ProductCosts[product] = totalCost;
            MainMod.ExtendedRecipes[product] = ingredients;
        }

        private static float CalculateTotalCost(List<PropertyItemDefinition> ingredients)
        {
            float totalCost = 0f;
            foreach (var ingredient in ingredients)
            {
                if (ingredient is not ProductDefinition)
                {
                    totalCost += ingredient.BasePurchasePrice;
                }
            }
            return totalCost;
        }

        public static List<PropertyItemDefinition> DeepSearchRecipe(ProductDefinition product)
        {
            var result = new List<PropertyItemDefinition>();
            DeepSearchRecursive(product, result, new List<ProductDefinition>());
            return result;
        }

        private static void DeepSearchRecursive(ProductDefinition product, List<PropertyItemDefinition> result, List<ProductDefinition> visited)
        {
            if (product == null || visited.Contains(product))
                return;

            visited.Add(product);

            if (product.Recipes.Count == 0)
            {
                result.Insert(0, product);
                return;
            }

            foreach (var ingredient in product.Recipes[0].Ingredients)
            {
                if (ingredient?.Item is ProductDefinition subProduct)
                {
                    DeepSearchRecursive(subProduct, result, visited);
                }
                else if (ingredient?.Item is PropertyItemDefinition rawIngredient)
                {
                    result.Add(rawIngredient);
                }
            }
        }


    }
}