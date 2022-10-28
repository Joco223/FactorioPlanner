using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioPlanner {

    internal class RecipeIngredient {

        public string Type { get; set; }
        public string Name { get; set; }

        public int Amount { get; set; }

        public RecipeIngredient() {
            Type = "";
            Name = "";
            Amount = 0;
        }

        public RecipeIngredient(string name, int amount) {
            Type = "";
            Name = name;
            Amount = amount;
        }

        public RecipeIngredient(string type, string name, int amount) {
            Type = type;
            Name = name;
            Amount = amount;
        }
    };

    internal class Recipe {
        public string Name { get; set; }
        public string Result { get; set; }

        public int ResultCount { get; set; }

        public List<RecipeIngredient> Ingredients { get; set; }
        public List<RecipeIngredient> ExpensiveIngredients { get; set; }

        public Recipe() {
            Name = "";
            Result = "";
            ResultCount = 0;
            Ingredients = new List<RecipeIngredient>();
            ExpensiveIngredients = new List<RecipeIngredient>();
        }

        public Recipe(string name, string result, int resultCount, List<RecipeIngredient> ingredients) {
            Name = name;
            Result = result;
            ResultCount = resultCount;
            Ingredients = ingredients;
            ExpensiveIngredients = new List<RecipeIngredient>();
        }

        public Recipe(string name, string result, int resultCount, List<RecipeIngredient> ingredients, List<RecipeIngredient> expnesiveIngredients) {
            Name = name;
            Result = result;
            ResultCount = resultCount;
            Ingredients = ingredients;
            ExpensiveIngredients = expnesiveIngredients;
        }
    }
}
