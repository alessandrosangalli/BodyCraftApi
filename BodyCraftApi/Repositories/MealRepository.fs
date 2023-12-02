namespace BodyCraftApi.Repositories

open BodyCraftApi.Data
open MySqlConnector
open System

type AddMealDtoFood =
    { FoodId: int
      QuantityInGrams: float32 }

type AddMealDto =
    { Time: TimeSpan
      Foods: AddMealDtoFood[] }

type MealRepository() as this =
    inherit Repository()

    member private _.MapFoodMealToFoodIdWithQuantity(foodMeal: FoodMeal) : FoodIdWithQuantity =
        { Id = foodMeal.FoodId
          QuantityInGrams = foodMeal.QuantityInGrams }

    member private _.InternalGetById(id: int, connection: MySqlConnection) : option<Meal> =
        let command = new MySqlCommand($"SELECT * FROM Meals WHERE id = {id};", connection)
        let reader = command.ExecuteReader()
        let meal = reader |> Meal.asSeq |> Seq.tryHead

        connection.Close()
        connection.Open()

        if (meal.IsNone) then
            None
        else
            let commandFoods =
                new MySqlCommand($"SELECT * FROM FoodsMeals WHERE MealId = {id};", connection)

            let foodIdWithQuantity =
                commandFoods.ExecuteReader()
                |> FoodMeal.asSeq
                |> Array.ofSeq
                |> Array.map this.MapFoodMealToFoodIdWithQuantity

            Some(
                { Id = Some(id)
                  FoodIdWithQuantity = foodIdWithQuantity
                  Time = meal.Value.Time }
            )

    member _.InternalAddMeal(meal: Meal, connection: MySqlConnection) =
        let mealTimeAsString = meal.Time.ToString()

        let command =
            new MySqlCommand($"INSERT INTO Meals (Time) VALUES ('{mealTimeAsString}');", connection)

        command.ExecuteNonQuery() |> ignore

        let lastMealId = this.GetLastInsertId(connection)

        connection.Close()
        connection.Open()

        for mealFood in meal.FoodIdWithQuantity do
            let command =
                new MySqlCommand(
                    $"INSERT INTO FoodsMeals (MealId, FoodId, QuantityInGrams) VALUES ({lastMealId}, {mealFood.Id}, {mealFood.QuantityInGrams});",
                    connection
                )

            command.ExecuteNonQuery() |> ignore

    member _.AddMeal(addMealDto: AddMealDto) =
        let meal: Meal =
            { Id = None
              FoodIdWithQuantity =
                addMealDto.Foods
                |> Array.map (fun food ->
                    { Id = food.FoodId
                      QuantityInGrams = food.QuantityInGrams })
              Time = addMealDto.Time }

        this.PersistInUniqueConnection<Meal>(meal, this.InternalAddMeal)

    member _.GetById(id: int) =
        this.GetByParamInUniqueConnection<int, Meal>(id, this.InternalGetById)
