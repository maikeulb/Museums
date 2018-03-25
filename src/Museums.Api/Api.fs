namespace MuseumsApi

module Api =
    open Suave.Web
    open MuseumsApi.Handlers
    open MuseumsApi.Db
    open Suave

    [<EntryPoint>]
    let main argv =

        let museumWebPart = museumHandler "museums" {
            GetAll = MuseumsDb.getMuseums
            GetById = MuseumsDb.getMuseum
            Create = MuseumsDb.createMuseum
            Update = MuseumsDb.updateMuseum
            UpdateById = MuseumsDb.updateMuseumById
            Delete = MuseumsDb.deleteMuseum
            IsExists = MuseumsDb.isMuseumExists
        }

        let paintingWebPart = paintingHandler "paintings" {
            GetAll = PaintingsDb.getPaintings
            GetById = PaintingsDb.getPainting
            Create = PaintingsDb.createPainting
            Update = PaintingsDb.updatePainting
            UpdateById = PaintingsDb.updatePaintingById
            Delete = PaintingsDb.deletePainting
            IsExists = PaintingsDb.isPaintingExists
        }

        let app = choose[museumWebPart; paintingWebPart]

        startWebServer defaultConfig app

        0
