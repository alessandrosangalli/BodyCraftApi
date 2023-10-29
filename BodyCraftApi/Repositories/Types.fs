namespace BodyCraftApi.Repositories

open FSharp.Data

module ProvidedTypes =
    open MySqlConnector

    [<Literal>]
    let private connectionString =
        "server=127.0.0.1;port=3306;database=BodyCraftApiDb;user=root;password="

    let connectionFactory = new MySqlConnection(connectionString)

type AppSettingsProvider = JsonProvider<"appsettings.json">
type DbSettings = AppSettingsProvider.DbConfiguration
