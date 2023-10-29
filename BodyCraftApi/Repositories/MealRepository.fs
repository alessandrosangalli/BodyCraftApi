namespace BodyCraftApi.Repositories

open BodyCraftApi.Data
open MySqlConnector


// type MealRepository() =

//     member _.AddMeal(meal: MealToInsert) =
//         connection.Open()

//         let command =
//             new MySqlCommand(
//                 $"INSERT INTO Meals (Food) VALUES ('{}', {Meal.ProteinPerGram}, {Meal.CarbPerGram}, {Meal.FatPerGram});",
//                 connection
//             )

//         command.ExecuteNonQuery() |> ignore
//         connection.Close()
