namespace BodyCraftApi.Controllers

open Microsoft.AspNetCore.Mvc
open BodyCraftApi.Repositories

[<ApiController>]
[<Route("[controller]")>]
type FoodController(foodRepository: FoodRepository) =
    inherit ControllerBase()

    [<HttpGet>]
    member _.Get() =
        foodRepository.AddFood(
            { Name = "Test"
              QuantityInGrams = 1.1
              Protein = 1
              Carb = 2
              Fat = 3 }
        )
