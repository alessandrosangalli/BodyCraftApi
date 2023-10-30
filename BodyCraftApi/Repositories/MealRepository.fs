namespace BodyCraftApi.Repositories

open BodyCraftApi.Data
open MySqlConnector
open System.Data
open ProvidedTypes
open System

type AddMealDtoFood = { FoodId: int; QuantityInGrams: float }

type AddMealDto =
    { Time: TimeSpan
      Foods: AddMealDtoFood[] }

type MealRepository() as this =
    member private _.OpenIfConnectionIsClosed(connection: MySqlConnection) =
        if connection.State = ConnectionState.Closed then
            connection.Open()

    member private _.CloseIfConnectionIsOpen(connection: MySqlConnection) =
        if connection.State = ConnectionState.Open then
            connection.Close()

    member private _.GetByParamInUniqueConnection<'ParamType>(param: 'ParamType, f) =
        let connection = connectionFactory
        this.OpenIfConnectionIsClosed(connection)

        let result = f (param, connection)

        this.CloseIfConnectionIsOpen(connection)

        result

    member private _.PersistInUniqueConnection<'PersistType>(persistType: 'PersistType, f) =
        let connection = connectionFactory
        this.OpenIfConnectionIsClosed(connection)

        f (persistType, connection)

        this.CloseIfConnectionIsOpen(connection)

    member private _.InternalGetById(id: int, connection: MySqlConnection) =
        let command = new MySqlCommand($"SELECT * FROM Meals WHERE id = {id};", connection)
        let reader = command.ExecuteReader()

        reader |> Meal.asSeq |> Seq.tryHead

    member _.InternalAddMeal(meal: Meal, connection: MySqlConnection) =
        let mealTimeAsString = meal.Time.ToString("hh\\:mm\\:ss")

        let command =
            new MySqlCommand($"INSERT INTO Meals (Time) VALUES ('{mealTimeAsString}');", connection)

        command.ExecuteNonQuery() |> ignore

        let lastMealId = this.GetLastInsertId(connection)

        connection.Close()
        connection.Open()

        for mealFood in meal.FoodWithQuantity do
            let command =
                new MySqlCommand(
                    $"INSERT INTO FoodsMeals (MealId, FoodId, QuantityInGrams) VALUES ({lastMealId}, {mealFood.Id}, {mealFood.QuantityInGrams});",
                    connection
                )

            command.ExecuteNonQuery() |> ignore

    member private _.GetLastInsertId(connection: MySqlConnection) : int64 =
        let command = new MySqlCommand("SELECT last_insert_id();", connection)
        let reader = command.ExecuteReader()
        reader.Read() |> ignore
        reader.GetInt64 0

    member _.AddMeal(addMealDto: AddMealDto) =
        let meal: Meal =
            { Id = None
              FoodWithQuantity =
                addMealDto.Foods
                |> Array.map (fun food ->
                    { Id = food.FoodId
                      QuantityInGrams = food.QuantityInGrams })
              Time = addMealDto.Time }

        this.PersistInUniqueConnection<Meal>(meal, this.InternalAddMeal)

    member _.GetById(id: int) =
        this.GetByParamInUniqueConnection<int>(id, this.InternalGetById)
