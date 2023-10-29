namespace BodyCraftApi.Controllers

open Microsoft.AspNetCore.Mvc
open BodyCraftApi.Repositories

[<ApiController>]
[<Route("[controller]")>]
type FoodController(foodRepository: FoodRepository) as this =
    inherit ControllerBase()

    [<HttpPost>]
    member _.Post([<FromBody>] foodDto) =
        task {
            foodRepository.AddFood(foodDto)
            return this.Ok()
        }

    [<Route("{id}")>]
    [<HttpGet>]
    member _.Get([<FromRoute>] id) =
        task {
            let result = foodRepository.GetById(id)

            let httpResult =
                if result.IsSome then
                    this.Ok(result) :> IActionResult
                else
                    this.NotFound() :> IActionResult

            return httpResult
        }
