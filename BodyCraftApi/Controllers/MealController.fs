namespace BodyCraftApi.Controllers

open Microsoft.AspNetCore.Mvc
open BodyCraftApi.Repositories
open System


type MealFoodRequest =
    { FoodId: int
      QuantityInGrams: float32 }

type MealRequest =
    { Time: string
      Foods: MealFoodRequest[] }

[<ApiController>]
[<Route("[controller]")>]
type MealController(mealRepository: MealRepository, foodRepository: FoodRepository) as this =
    inherit ControllerBase()

    member private _.MapMealFoodRequestToAddDto(mealFoodRequest: MealFoodRequest) : AddMealDtoFood =
        { FoodId = mealFoodRequest.FoodId
          QuantityInGrams = mealFoodRequest.QuantityInGrams }

    member private _.AddMeal(mealRequest: MealRequest) =
        mealRepository.AddMeal(
            { Time = mealRequest.Time |> TimeSpan.Parse
              Foods = mealRequest.Foods |> Array.map this.MapMealFoodRequestToAddDto }
        )

    member private _.FoodIdNotFound(foods: MealFoodRequest[]) : Option<int> =
        foods
        |> Seq.ofArray
        |> Seq.tryFind (fun food -> foodRepository.GetById(food.FoodId).IsNone)
        |> Option.map (fun food -> food.FoodId)

    [<HttpPost>]
    member _.Post([<FromBody>] mealRequest: MealRequest) =
        task {
            let httpResult =
                match this.FoodIdNotFound(mealRequest.Foods) with
                | Some value -> this.BadRequest($"Food {value} doesn't exist") :> IActionResult
                | None -> this.Ok(id) :> IActionResult

            match httpResult with
            | :? OkObjectResult -> this.AddMeal(mealRequest)
            | _ -> ()

            return httpResult
        }

    [<Route("{id}")>]
    [<HttpGet>]
    member _.Get([<FromRoute>] id) =
        task {
            let result = mealRepository.GetById(id)

            let httpResult =
                if result.IsSome then
                    this.Ok(result) :> IActionResult
                else
                    this.NotFound() :> IActionResult

            return httpResult
        }
