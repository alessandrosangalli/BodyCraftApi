namespace BodyCraftApi.Data

open System
open System.Data

type Meal =
    { Id: option<int>
      FoodIdWithQuantity: FoodIdWithQuantity[]
      Time: TimeSpan }

    static member fromDataReader(reader: IDataReader) : Meal =
        { Id = Some(reader.GetInt32 0)
          FoodIdWithQuantity = Array.empty
          Time = reader.GetValue 1 |> unbox<TimeSpan> }

    static member asSeq(reader: IDataReader) =
        seq {
            while reader.Read() do
                yield Meal.fromDataReader reader
        }
