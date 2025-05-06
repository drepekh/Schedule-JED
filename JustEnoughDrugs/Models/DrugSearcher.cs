using System.Collections.Generic;
using System.Linq;
using MelonLoader;
#if IL2CPP
using Il2CppScheduleOne.Product;
using Il2CppScheduleOne.Properties;
using Il2CppScheduleOne.StationFramework;
#else
using ScheduleOne.Product;
#endif

namespace JustEnoughDrugs.Models
{
    public class DrugSearcher
    {
        public enum SearchFilter { Any, Ingredients, Effects, Name }

        public bool ShouldShowDrug(ProductEntry productEntry, string searchText, string filterKey)
        {
            if (string.IsNullOrEmpty(searchText)) return true;

            bool matchesEffect = false;
            bool matchesName = false;
            bool matchesIngredient = false;

            if (filterKey == SearchFilter.Effects.ToString() || filterKey == SearchFilter.Any.ToString())
            {
                matchesEffect = MatchesEffects(productEntry, searchText);
            }

            if (filterKey == SearchFilter.Ingredients.ToString() || filterKey == SearchFilter.Any.ToString())
            {
                matchesIngredient = MatchesIngredients(productEntry, searchText);
            }

            if (filterKey == SearchFilter.Name.ToString() || filterKey == SearchFilter.Any.ToString())
            {
                matchesName = MatchesName(productEntry, searchText);
            }

            return matchesName || matchesEffect || matchesIngredient;
        }
        private bool MatchesName(ProductEntry entry, string searchText)
        {
            string drugName = entry.Definition.ToString().ToLowerInvariant();
            return MatchesSearch(new List<string> { drugName }, searchText);
        }


        private bool MatchesEffects(ProductEntry entry, string searchText)
        {
            var effectList = new List<string>();
            entry.Definition.Properties.ForEach((Action<Property>)(p => effectList.Add(p.ToString().ToLowerInvariant())));
            return MatchesSearch(effectList, searchText);
        }

        private bool MatchesIngredients(ProductEntry entry, string searchText)
        {
            var ingredientList = new List<string>();
            entry.Definition.Recipes.ForEach((Action<StationRecipe>)(r => r.Ingredients.ForEach((Action<StationRecipe.IngredientQuantity>)(i =>
                ingredientList.Add(i.Item.ToString().ToLowerInvariant())))));

            return MatchesSearch(ingredientList, searchText);
        }
        private bool MatchesSearch(List<string> targetList, string searchText)
        {
            var AndChar = ",";
            var OrChar = "|";
            searchText = searchText.ToLowerInvariant();

            if (searchText.Contains(AndChar) && searchText.Split(AndChar.ToCharArray())[1] != "")
            {
                var terms = searchText.Split(AndChar.ToCharArray());
                return terms.All(term =>
                {
                    return targetList.Any(target => target.Contains(term.Trim()));

                });
            }
            else if (searchText.Contains(OrChar) && searchText.Split(OrChar.ToCharArray())[1] != "")
            {

                var terms = searchText.Split(OrChar.ToCharArray());

                return terms.Any(term =>
                 {
                     return targetList.Any(target => target.Contains(term.Trim()));

                 });
            }
            else
            {
                return targetList.Any(target => target.Contains(searchText.Replace(OrChar, "").Replace(AndChar, "").Trim()));

            }
        }
    }
}