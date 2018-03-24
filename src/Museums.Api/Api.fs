namespace MuseumsApi

module Api =
        open Suave.Web
    open MuseumsApi.Rest
    open MuseumsApi.Db
    open Suave

    [<EntryPoint>]
    let main argv =

            startWebServer defaultConfig app

        0
