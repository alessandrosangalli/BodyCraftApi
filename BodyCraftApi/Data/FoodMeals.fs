namespace BodyCraftApi.Data

open System.Data

type Quantity = { QuantityInGrams: float32 }

type FoodMeal =
    { FoodId: int
      MealId: int
      QuantityInGrams: float32 }

    static member fromDataReader(reader: IDataReader) : FoodMeal =
        { FoodId = reader.GetInt32 0
          MealId = reader.GetInt32 1
          QuantityInGrams = reader.GetFloat 2 }

    static member asSeq(reader: IDataReader) =
        seq {
            while reader.Read() do
                yield FoodMeal.fromDataReader reader
        }
