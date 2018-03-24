namespace Museums.Api

module Api =
    open Suave.Web
    open MuseumsApi.Rest
    open MuseumsApi.Db
    open Suave

    [<EntryPoint>]
    let main argv =

        let museumWebPart = rest "museums" {
            GetAll = Db.getMuseums
            GetById = Db.getMuseumById
            Create = Db.createMuseum
            Update = Db.updateMuseum
            UpdateById = Db.updateMuseumById
            Delete = Db.deleteMuseum
            IsExists = Db.isMuseumExists
        }

        let app = museumsWebPart

        startWebServer defaultConfig app

        0
