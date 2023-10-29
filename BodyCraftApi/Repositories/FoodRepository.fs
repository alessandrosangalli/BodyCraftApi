namespace BodyCraftApi.Repositories

open BodyCraftApi.Data
open MySqlConnector
open System.Data
open ProvidedTypes

type FoodRepository() as this =
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
        let command = new MySqlCommand($"SELECT * FROM Foods WHERE id = {id};", connection)
        let reader = command.ExecuteReader()

        reader |> Food.asSeq |> Seq.tryHead

    member _.InternalAddFood(food: Food, connection: MySqlConnection) =
        let command =
            new MySqlCommand(
                $"INSERT INTO Foods (Name, ProteinPerGram, CarbPerGram, FatPerGram) VALUES ('{food.Name}', {food.ProteinPerGram}, {food.CarbPerGram}, {food.FatPerGram});",
                connection
            )

        command.ExecuteNonQuery() |> ignore

    member _.AddFood(food: Food) =
        this.PersistInUniqueConnection<Food>(food, this.InternalAddFood)

    member _.GetById(id: int) =
        this.GetByParamInUniqueConnection<int>(id, this.InternalGetById)
