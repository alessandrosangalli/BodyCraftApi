namespace BodyCraftApi.Repositories

open MySqlConnector
open System.Data
open ProvidedTypes

type Repository() =
    member this.OpenIfConnectionIsClosed(connection: MySqlConnection) =
        if connection.State = ConnectionState.Closed then
            connection.Open()

    member this.CloseIfConnectionIsOpen(connection: MySqlConnection) =
        if connection.State = ConnectionState.Open then
            connection.Close()

    member this.GetByParamInUniqueConnection<'ParamType, 'OutputType>
        (
            param: 'ParamType,
            f: 'ParamType * MySqlConnection -> option<'OutputType>
        ) =
        let connection = connectionFactory
        this.OpenIfConnectionIsClosed(connection)

        let result = f (param, connection)

        this.CloseIfConnectionIsOpen(connection)

        result

    member this.PersistInUniqueConnection<'PersistType>(persistType: 'PersistType, f) =
        let connection = connectionFactory
        this.OpenIfConnectionIsClosed(connection)

        f (persistType, connection)

        this.CloseIfConnectionIsOpen(connection)

    member this.GetLastInsertId(connection: MySqlConnection) : int64 =
        let command = new MySqlCommand("SELECT last_insert_id();", connection)
        let reader = command.ExecuteReader()
        reader.Read() |> ignore
        reader.GetInt64 0
