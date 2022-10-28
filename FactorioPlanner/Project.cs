using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Documents;
using NLua;

namespace FactorioPlanner {
    internal class Project {
        public string Path { get; set; }
        public string Name { get; set; }

        public bool NewProject { get; set; }

        public List<Recipe> Recipes { get; set; }

        public Project() {
            Path = "";
            Name = "Untitled";
            NewProject = true;
            Recipes = new List<Recipe>();
        }

        public Project(string name) {
            Path = "";
            Name = name;
            NewProject = true;
            Recipes = new List<Recipe>();
        }

        public Project(string path, string name) {
            Path = path;
            Name = name;
            NewProject = false;
            Recipes = new List<Recipe>();
        }

        private void transferData(Project other) {
            Name = other.Name;
            Path = other.Path;
            NewProject = other.NewProject;
        }

        public void loadProject(string path) {
            string jsonFile = File.ReadAllText(path);
            Project? tmp = JsonSerializer.Deserialize<Project>(jsonFile);

            if (tmp == null) {
                MessageBox.Show("Unable to open project at: " + path);
                return;
            } else {
                transferData(tmp);
            }
        }

        public void saveProject(string path) {
            string jsonFile = JsonSerializer.Serialize(this);
            File.WriteAllText(path, jsonFile);
        }

        public void loadDefaultRecipes() {
            string recipesPath = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Factorio\\data\\base\\prototypes\\recipe.lua";
            var recipesFileLines = File.ReadAllLines(recipesPath);
            string recipesText = "";
            bool skipFirst = true;

            foreach (var line in recipesFileLines) {
                if (skipFirst) {
                    skipFirst = false;
                } else {
                    if (line != ")") {
                        recipesText += line + "\n";
                    }   
                }
            }

            Lua state = new Lua();

            state.DoString("data = " + recipesText);
            state.DoString("len = #data");

            int recipesCount = (int)(double)state["len"];

            for (int i = 1; i < recipesCount; i++) {           
                state.DoString("name = data[" + i + "][\"name\"]");      

                string recipeName = (string)state["name"];

                state.DoString("normal = data[" + i + "][\"normal\"]");
                LuaTable normal = (LuaTable)state["normal"];

                if (normal == null) {
                    List<RecipeIngredient> recipeIngredients = new List<RecipeIngredient>();

                    state.DoString("ingredientsCount = #data[" + i + "][\"ingredients\"]");
                    int ingredientsCount = (int)(double)state["ingredientsCount"];

                    state.DoString("result = data[" + i + "][\"result\"]");
                    string recipeResult = (string)state["result"];

                    state.DoString("resultCount = data[" + i + "][\"result_count\"]");
                    state.DoString("if resultCount == nil then\n   resultCount = 1\nend");
                    int resultCount = (int)(double)state["resultCount"];

                    for (int j = 1; j < ingredientsCount; j++) {
                        state.DoString("ingredientName = data[" + i + "][\"ingredients\"][" + j + "][1]");
                        state.DoString("ingredientCount = data[" + i + "][\"ingredients\"][" + j + "][2]");
                        state.DoString("ingredientType = data[" + i + "][\"ingredients\"][" + j + "][\"type\"]");

                        string ingredientName = (string)state["ingredientName"];
                        string ingredientType = (string)state["ingredientType"];
                        if (ingredientType != "fluid" && ingredientType != "item") {
                            int ingredientCount = (int)(double)state["ingredientCount"];
                            recipeIngredients.Add(new RecipeIngredient(ingredientName, ingredientCount));
                        }
                    }
                    Recipes.Add(new Recipe(recipeName, recipeResult, resultCount, recipeIngredients));
                } else {
                    List<RecipeIngredient> recipeIngredients = new List<RecipeIngredient>();
                    List<RecipeIngredient> recipeExpensiveIngredients = new List<RecipeIngredient>();

                    state.DoString("ingredientsCount = #data[" + i + "][\"normal\"][\"ingredients\"]");
                    int ingredientsCount = (int)(double)state["ingredientsCount"];

                    state.DoString("result = data[" + i + "][\"normal\"][\"result\"]");
                    string recipeResult = (string)state["result"];

                    state.DoString("resultCount = data[" + i + "][\"normal\"][\"result_count\"]");
                    state.DoString("if resultCount == nil then\n   resultCount = 1\nend");
                    int resultCount = (int)(double)state["resultCount"];

                    for (int j = 1; j < ingredientsCount; j++) {
                        state.DoString("ingredientName = data[" + i + "][\"normal\"][\"ingredients\"][" + j + "][1]");
                        state.DoString("ingredientCount = data[" + i + "][\"normal\"][\"ingredients\"][" + j + "][2]");
                        state.DoString("ingredientType = data[" + i + "][\"normal\"][\"ingredients\"][" + j + "][\"type\"]");

                        string ingredientName = (string)state["ingredientName"];
                        string ingredientType = (string)state["ingredientType"];
                        if (ingredientType != "fluid" && ingredientType != "item") {
                            int ingredientCount = (int)(double)state["ingredientCount"];
                            recipeIngredients.Add(new RecipeIngredient(ingredientName, ingredientCount));
                        }
                    }

                    state.DoString("ingredientsCount = #data[" + i + "][\"expensive\"][\"ingredients\"]");
                    ingredientsCount = (int)(double)state["ingredientsCount"];

                    state.DoString("result = data[" + i + "][\"expensive\"][\"result\"]");
                    recipeResult = (string)state["result"];

                    state.DoString("resultCount = data[" + i + "][\"expensive\"][\"result_count\"]");
                    state.DoString("if resultCount == nil then\n   resultCount = 1\nend");
                    resultCount = (int)(double)state["resultCount"];

                    for (int j = 1; j < ingredientsCount; j++) {
                        state.DoString("ingredientName = data[" + i + "][\"expensive\"][\"ingredients\"][" + j + "][1]");
                        state.DoString("ingredientCount = data[" + i + "][\"expensive\"][\"ingredients\"][" + j + "][2]");
                        state.DoString("ingredientType = data[" + i + "][\"expensive\"][\"ingredients\"][" + j + "][\"type\"]");

                        string ingredientName = (string)state["ingredientName"];
                        string ingredientType = (string)state["ingredientType"];
                        if (ingredientType != "fluid" && ingredientType != "item") {
                            int ingredientCount = (int)(double)state["ingredientCount"];
                            recipeIngredients.Add(new RecipeIngredient(ingredientName, ingredientCount));
                        }
                    }

                    Recipes.Add(new Recipe(recipeName, recipeResult, resultCount, recipeIngredients, recipeExpensiveIngredients));
                }
            }

            Trace.WriteLine(Recipes.Count());
        }
    }
}
