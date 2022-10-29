using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
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

        private void convertJsonIngredients(ref DifficultyRecipe r) {
            r.jsonIngredients ??= new List<JsonElement>();
            r.ingredients ??= new List<RecipeIngredient>();

            //Converting from jsonIngredients to regular ingredients list because of incosntitent structure
            foreach (var ing in r.jsonIngredients) {
                if (ing.ValueKind == JsonValueKind.Array) {
                    r.ingredients.Add(new RecipeIngredient("item", ing[0].GetString() ?? "", ing[1].GetInt32()));
                } else {
                    RecipeIngredient? ingredient = ing.Deserialize<RecipeIngredient>();
                    if (ingredient != null)
                        r.ingredients.Add(ingredient);
                }
            }
        }

        public void loadDefaultRecipes() {

            //Loading the base recipes file
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


            //Cleaning up and removing the data:expand from the begining of the file
            Lua state = new Lua();

            state.DoString("jsonConv = require \"json\"");
            state.DoString("data = " + recipesText);
            state.DoString("len = #data");
            state.DoString("dataJson = jsonConv.encode(data)");
            string dataJson = (string)state["dataJson"];

            //Deserializing into objects
            List<Recipe>? loadedRecipes = JsonSerializer.Deserialize<List<Recipe>>(dataJson);

            if (loadedRecipes != null) {
                Logger.LogInfo("Number of loaded recipes: " + loadedRecipes.Count, "loadDefaultRecipes");


                //Cleaning up the recipes so all have a consitent structure
                foreach (Recipe r in loadedRecipes) {
                    r.resultCount ??= 1;

                    if (r.resultCount == 0) r.resultCount = 1;
                    
                    //If recipe only has ingredients list, add the same list to both normal and expensive recipe type
                    if (r.jsonIngredients != null) {
                        var tmpRecipe = new DifficultyRecipe();
                        tmpRecipe.jsonIngredients = r.jsonIngredients;
                        convertJsonIngredients(ref tmpRecipe);

                        //If singular result, add it to results list
                        if (r.result != null) {
                            tmpRecipe.results = new List<RecipeIngredient> {
                                new RecipeIngredient(r.result, (int)r.resultCount)
                            };
                        }

                        //If results exist outside of normal, add it to results
                        if (r.jsonResults != null) {
                            tmpRecipe.results ??= new List<RecipeIngredient>();

                            foreach (var ing in r.jsonResults) {
                                if (ing.ValueKind == JsonValueKind.Array) {
                                    tmpRecipe.results.Add(new RecipeIngredient("item", ing[0].GetString() ?? "", ing[1].GetInt32()));
                                } else {
                                    RecipeIngredient? ingredient = ing.Deserialize<RecipeIngredient>();
                                    if (ingredient != null) {
                                        tmpRecipe.results.Add(ingredient);
                                    }    
                                }
                            }
                        }

                        //Assing both normal and expensive type the same recipe object
                        r.normal = tmpRecipe;
                        r.expensive = tmpRecipe;

                        //Remove unecessary copy of ingredients
                        r.jsonIngredients.Clear();
                    }

                    //Check if result count exists in normal and expensive recipes and clean up the ingredients
                    if (r.normal != null) {
                        r.normal.resultCount ??= 1;
                        DifficultyRecipe normalRecipe = r.normal;
                        convertJsonIngredients(ref normalRecipe);
                        r.normal = normalRecipe;

                        if (r.normal.result != null) {
                            r.normal.results = new List<RecipeIngredient> {
                                new RecipeIngredient(r.normal.result, (int)r.resultCount)
                            };
                        }

                        if (r.normal.jsonResults != null) {
                            r.normal.results ??= new List<RecipeIngredient>();

                            foreach (var ing in r.normal.jsonResults) {
                                if (ing.ValueKind == JsonValueKind.Array) {
                                    r.normal.results.Add(new RecipeIngredient("item", ing[0].GetString() ?? "", ing[1].GetInt32()));
                                } else {
                                    RecipeIngredient? ingredient = ing.Deserialize<RecipeIngredient>();
                                    if (ingredient != null) {
                                        r.normal.results.Add(ingredient);
                                    }
                                }
                            }
                        }
                    }

                    if (r.expensive != null) {
                        r.expensive.resultCount ??= 1;
                        DifficultyRecipe expensiveRecipe = r.expensive;
                        convertJsonIngredients(ref expensiveRecipe);
                        r.expensive = expensiveRecipe;

                        if (r.expensive.result != null) {
                            r.expensive.results = new List<RecipeIngredient> {
                                new RecipeIngredient(r.expensive.result, (int)r.resultCount)
                            };
                        }

                        if (r.expensive.jsonResults != null) {
                            r.expensive.results ??= new List<RecipeIngredient>();

                            foreach (var ing in r.expensive.jsonResults) {
                                if (ing.ValueKind == JsonValueKind.Array) {
                                    r.expensive.results.Add(new RecipeIngredient("item", ing[0].GetString() ?? "", ing[1].GetInt32()));
                                } else {
                                    RecipeIngredient? ingredient = ing.Deserialize<RecipeIngredient>();
                                    if (ingredient != null) {
                                        r.expensive.results.Add(ingredient);
                                    }
                                }
                            }
                        }
                    }

                    if (Logger.VerboseLog) {
                        Logger.LogInfoVerbose("Recipe " + r.name + " restructured", "loadDefaultRecipes");
                    }
                }
            } else {
                Logger.LogError("No recipes were loaded", "loadDefaultRecipes");
            }
        }
    }
}
