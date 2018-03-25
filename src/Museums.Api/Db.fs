namespace MuseumsApi.Db
open System
open FSharp.Data.Sql

module Museums =

    type Museum = {
        Id : int
        Name : string
    }

    type Sql =
        SqlDataProvider<
            ConnectionString      = "Server=172.17.0.2;Database=nycmuseums;User Id=postgres;Password=P@ssw0rd!;",
            DatabaseVendor        = Common.DatabaseProviderTypes.POSTGRESQL,
            CaseSensitivityChange = Common.CaseSensitivityChange.ORIGINAL >

    type DbContext = Sql.dataContext

    let getContext() = Sql.GetDataContext()

    type MuseumEntity = DbContext.``public.museumsEntity``

    let firstOrNone s = s |> Seq.tryFind (fun _ -> true)

    let mapToMuseum (museumEntity : MuseumEntity) =
        {
            Id = museumEntity.Id
            Name = museumEntity.Name
        }

    let getMuseums () =
        getContext().Public.Museums
        |> Seq.map mapToMuseum

    let getMuseumEntityById (ctx : DbContext) id =
        query {
            for museum in ctx.Public.Museums do
                where (museum.Id = id)
                select museum
        } |> firstOrNone

    let getMuseum id =
        getMuseumEntityById (getContext()) id |> Option.map mapToMuseum

    let createMuseum museum =
        let ctx = getContext()
        let museumEntity = ctx.Public.Museums.Create()
        museumEntity.Name <- museum.Name
        ctx.SubmitUpdates()
        museumEntity |> mapToMuseum

    let updateMuseumById id museum =
        let ctx = getContext()
        let museumEntity = getMuseumEntityById ctx museum.Id
        match museumEntity with
        | None -> None
        | Some m ->
            m.Id <- museum.Id
            m.Name <- museum.Name
            ctx.SubmitUpdates()
            Some museum

    let updateMuseum museum =
        updateMuseumById museum.Id museum

    let deleteMuseum id =
        let ctx = getContext()
        let museumEntity = getMuseumEntityById ctx id
        match museumEntity with
        | None -> ()
        | Some a ->
            a.Delete()
            ctx.SubmitUpdates()

    let isMuseumExists id =
        let ctx = getContext()
        let museumEntity = getMuseumEntityById ctx id

        match museumEntity with
        | None -> false
        | Some _ -> true

module Paintings =

    type Painting = {
        Id : int
        MuseumId : int
        Title : string
        Artist : string
        Medium : string
    }

    type Sql =
        SqlDataProvider<
            ConnectionString      = "Server=172.17.0.2;Database=nycmuseums;User Id=postgres;Password=P@ssw0rd!;",
            DatabaseVendor        = Common.DatabaseProviderTypes.POSTGRESQL,
            CaseSensitivityChange = Common.CaseSensitivityChange.ORIGINAL >

    type DbContext = Sql.dataContext

    let getContext() = Sql.GetDataContext()

    type PaintingEntity = DbContext.``public.paintingsEntity``

    let firstOrNone s = s |> Seq.tryFind (fun _ -> true)

    let mapToPainting (paintingEntity : PaintingEntity) =
        {
            Id = paintingEntity.Id
            MuseumId = paintingEntity.MuseumId
            Title = paintingEntity.Title
            Artist = paintingEntity.Artist
            Medium = paintingEntity.Medium
        }

    let getPaintings () =
        getContext().Public.Paintings
        |> Seq.map mapToPainting

    let getPaintingEntitiesByMuseumId (ctx : DbContext) museumId =
        query {
            for painting in ctx.Public.Paintings do
                where (painting.MuseumId = museumId)
                select painting
        } |> Seq.toList

    let getPaintingEntityById (ctx : DbContext) id =
        query {
            for painting in ctx.Public.Paintings do
                where (painting.Id = id)
                select painting
        } |> firstOrNone

    let getPainting (_, id) =
        getPaintingEntityById (getContext()) id |> Option.map mapToPainting

    let getPaintingsByMuseumId museumId =
        let paintings =  getPaintingEntitiesByMuseumId (getContext()) museumId |> List.map mapToPainting
        if List.isEmpty paintings then
            None
        else
            Some paintings

    let createPainting museumId painting =
        let ctx = getContext()
        if Museums.isMuseumExists museumId then
            let paintingEntity = ctx.Public.Paintings.Create()
            paintingEntity.Title <- painting.Title
            paintingEntity.Artist <- painting.Artist
            paintingEntity.Medium <- painting.Medium
            paintingEntity.MuseumId <- museumId
            ctx.SubmitUpdates()
            let painting = paintingEntity |> mapToPainting
            Some painting
        else
            None

    let updatePaintingById id painting =
        let ctx = getContext()
        let paintingEntity = getPaintingEntityById ctx painting.Id
        match paintingEntity with
        | None -> None
        | Some p ->
            p.Id <- painting.Id
            p.MuseumId <- painting.MuseumId
            p.Title <- painting.Title
            p.Artist <- painting.Artist
            p.Medium <- painting.Medium
            ctx.SubmitUpdates()
            Some painting

    let updatePainting painting =
        updatePaintingById painting.Id painting

    let deletePainting id =
        let ctx = getContext()
        let paintingEntity = getPaintingEntityById ctx id
        match paintingEntity with
        | None -> ()
        | Some a ->
            a.Delete()
            ctx.SubmitUpdates()

    let isPaintingExists id =
        let ctx = getContext()
        let paintingEntity = getPaintingEntityById ctx id

        match paintingEntity with
        | None -> false
        | Some _ -> true
