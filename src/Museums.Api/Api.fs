namespace MuseumsApi

module Api =
    open Suave.Web
    open MuseumsApi.Rest
    open MuseumsApi.Db
    open Suave

    [<EntryPoint>]
    let main argv =

        let museumWebPart = rest "museums" {
            GetAll = Db.getMuseums
            GetById = Db.getMuseum
            Create = Db.createMuseum
            Update = Db.updateMuseum
            UpdateById = Db.updateMuseum
            Delete = Db.deleteMuseum
            IsExists = Db.isMuseumExists
        }

        startWebServer defaultConfig app

        0
