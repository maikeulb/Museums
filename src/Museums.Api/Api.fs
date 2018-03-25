namespace MuseumsApi

module Api =
    open Suave.Web
    open MuseumsApi.Handlers
    open MuseumsApi.Db
    open Suave

    [<EntryPoint>]
    let main argv =

        Museums.seed () |> ignore
        Paintings.seed () |> ignore

        let museumWebPart = rootHandler "museums" {
            GetAll = Museums.getMuseums
            GetById = Museums.getMuseum
            Create = Museums.createMuseum
            Update = Museums.updateMuseum
            UpdateById = Museums.updateMuseumById
            Delete = Museums.deleteMuseum
            IsExists = Museums.isMuseumExists
        }

        let paintingWebPart = nestedHandler "museums" "paintings" {
            GetAll = Paintings.getPaintings
            GetById = Paintings.getPainting
            Create = Paintings.createPainting
            Update = Paintings.updatePainting
            UpdateById = Paintings.updatePaintingById
            Delete = Paintings.deletePainting
            IsExists = Paintings.isPaintingExists
        }

        let app = choose[museumWebPart; paintingWebPart]

        startWebServer defaultConfig app

        0
