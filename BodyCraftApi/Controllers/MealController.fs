namespace BodyCraftApi.Controllers

open Microsoft.AspNetCore.Mvc
open BodyCraftApi.Repositories

[<ApiController>]
[<Route("[controller]")>]
type MealController(mealRepository: MealRepository) as this =
    inherit ControllerBase()

    [<HttpPost>]
    member _.Post([<FromBody>] mealDto) =
        task {
            mealRepository.AddMeal(mealDto)
            return this.Ok()
        }
