namespace BodyCraftApi.Data

open System.Data

type Food =
    { Id: option<int>
      Name: string
      ProteinPerGram: float32
      CarbPerGram: float32
      FatPerGram: float32 }

    static member fromDataReader(reader: IDataReader) =
        { Id = Some(reader.GetInt32 0)
          Name = reader.GetString 1
          ProteinPerGram = reader.GetFloat 2
          CarbPerGram = reader.GetFloat 3
          FatPerGram = reader.GetFloat 4 }

    static member asSeq(reader: IDataReader) =
        seq {
            while reader.Read() do
                yield Food.fromDataReader reader
        }

type FoodIdWithQuantity = { Id: int; QuantityInGrams: float }
