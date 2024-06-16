using Newtonsoft.Json;

namespace Chefster.Models;

public class GordonResponseModel
{
    public required List<GordonRecipeModel?> Response { get; set; }
    public required bool Success { get; set; }
}

public class GordonRecipeModel
{
    [JsonProperty("allIngredients")]
    public required List<string> AllIngredients { get; set; }

    [JsonProperty("notes")]
    public required string Notes { get; set; }

    [JsonProperty("recipes")]
    public required List<Recipe> Recipes { get; set; }

    public class Recipe
    {
        [JsonProperty("dishName")]
        public required string DishName { get; set; }

        [JsonProperty("ingredients")]
        public required List<string> Ingredients { get; set; }

        [JsonProperty("instructions")]
        public required List<string> Instructions { get; set; }

        [JsonProperty("prepareTime")]
        public required string PrepareTime { get; set; }

        [JsonProperty("servings")]
        public required int Servings { get; set; }
    }
}