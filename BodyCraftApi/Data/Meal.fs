namespace BodyCraftApi.Data

open System
open System.Data

type Quantity = { QuantityInGrams: float }

type Meal =
    { Id: option<int>
      FoodWithQuantity: FoodIdWithQuantity[]
      Time: TimeSpan }

    static member fromDataReader(reader: IDataReader) : Meal =
        { Id = Some(reader.GetInt32 0)
          FoodWithQuantity = Array.empty
          Time = reader.GetString 1 |> TimeSpan.Parse }

    static member asSeq(reader: IDataReader) =
        seq {
            while reader.Read() do
                yield Food.fromDataReader reader
        }
