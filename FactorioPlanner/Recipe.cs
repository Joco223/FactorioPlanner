using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FactorioPlanner {

    internal class RecipeIngredient {

        public string? type { get; set; }
        public string? name { get; set; }

        public int? amount { get; set; }

        public RecipeIngredient() {
            type = "";
            name = "";
            amount = 0;
        }

        public RecipeIngredient(string name, int amount) {
            type = "";
            this.name = name;
            this.amount = amount;
        }

        public RecipeIngredient(string type, string name, int amount) {
            this.type = type;
            this.name = name;
            this.amount = amount;
        }
    };

    internal class DifficultyRecipe {
        public string? result { get; set; }


        [JsonPropertyName("results")]
        public List<JsonElement>? jsonResults { get; set; }
        
        [JsonIgnore]
        public List<RecipeIngredient>? results { get; set; }

        public int? resultCount { get; set; }

        [JsonIgnore]
        public List<RecipeIngredient>? ingredients { get; set; }

        [JsonPropertyName("ingredients")]
        public List<JsonElement>? jsonIngredients { get; set; }
    };

    internal class Recipe {
        public string name { get; set; }
        public string? result { get; set; }

        [JsonPropertyName("results")]
        public List<JsonElement>? jsonResults { get; set; }

        public int? resultCount { get; set; }

        [JsonPropertyName("ingredients")]
        public List<JsonElement>? jsonIngredients { get; set; }
        public DifficultyRecipe? normal { get; set; }
        public DifficultyRecipe? expensive { get; set; }

        public Recipe() {
            name = "";
            result = "";
            resultCount = 0;
        }
    }
}
