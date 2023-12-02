namespace BodyCraftApi.Repositories

open BodyCraftApi.Data
open MySqlConnector

type FoodRepository() as this =
    inherit Repository()

    member private _.InternalGetById(id: int, connection: MySqlConnection) : option<Food> =
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
        this.GetByParamInUniqueConnection<int, Food>(id, this.InternalGetById)
