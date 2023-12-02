namespace BodyCraftApi.Data

open System.Data

type ProteinPerGram = ProteinPerGram of float32
type CarbPerGram = CarbPerGram of float32
type FatPerGram = FatPerGram of float32

type Food =
    { Id: option<int>
      Name: string
      ProteinPerGram: ProteinPerGram
      CarbPerGram: CarbPerGram
      FatPerGram: FatPerGram }

    static member fromDataReader(reader: IDataReader) =
        { Id = Some(reader.GetInt32 0)
          Name = reader.GetString 1
          ProteinPerGram = ProteinPerGram(reader.GetFloat 2)
          CarbPerGram = CarbPerGram(reader.GetFloat 3)
          FatPerGram = FatPerGram(reader.GetFloat 4) }

    static member asSeq(reader: IDataReader) =
        seq {
            while reader.Read() do
                yield Food.fromDataReader reader
        }

type FoodIdWithQuantity = { Id: int; QuantityInGrams: float32 }
