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

    let getMuseumEntitiesByName (ctx : DbContext) name =
        query {
            for museum in ctx.Public.Museums do
                where (museum.Name = name)
                select museum
        } |> Seq.toList

    let getMuseumEntityById (ctx : DbContext) id =
        query {
            for museum in ctx.Public.Museums do
                where (museum.Id = id)
                select museum
        } |> firstOrNone

    let getMuseum id =
        getMuseumEntityById (getContext()) id |> Option.map mapToMuseum

    let getMuseumsByName name =
        getMuseumEntitiesByName (getContext()) name |> List.map mapToMuseum

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
        | Some a ->
            a.Id <- museum.Id
            a.Name <- museum.Name
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
        Name : string
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
            Name = paintingEntity.Name
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

    (* let getPaintings rid = *)
    (*     (1* paintingsStorage.Values :> seq<Painting> *1) *)
    (*     (1* if paintingsStorage.ContainsKey(rid) then *1) *)
    (*         let paintings =  paintingsStorage.Values :> seq<Painting> *)
    (*         Some paintings *)
    (*     (1* else *1) *)
    (*         (1* None *1) *)

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
        Some paintings

    let createPainting museumId painting =
        let ctx = getContext()
        let paintingEntity = ctx.Public.Paintings.Create()
        paintingEntity.Name <- painting.Name
        paintingEntity.MuseumId <- museumId
        ctx.SubmitUpdates()
        let paintings = paintingEntity |> mapToPainting
        Some paintings


    let updatePaintingById id painting =
        let ctx = getContext()
        let paintingEntity = getPaintingEntityById ctx painting.Id
        match paintingEntity with
        | None -> None
        | Some a ->
            a.Id <- painting.Id
            a.Name <- painting.Name
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




(* module Paintings = *)

    (* type Painting = { *)
    (*     Id : int *)
    (*     Name : string *)
    (* } *)

    (* let paintingsStorage = new Dictionary<int, Painting>() *)

    (* let getPaintings rid = *)
    (*     (1* paintingsStorage.Values :> seq<Painting> *1) *)
    (*     (1* if paintingsStorage.ContainsKey(rid) then *1) *)
    (*         let paintings =  paintingsStorage.Values :> seq<Painting> *)
    (*         Some paintings *)
    (*     (1* else *1) *)
    (*         (1* None *1) *)

    (* let getPainting (_, id) = *)
    (*     if paintingsStorage.ContainsKey(id) then *)
    (*         Some paintingsStorage.[id] *)
    (*     else *)
    (*         None *)

    (* let createPainting rid painting = *)
    (*     (1* if paintingsStorage.ContainsKey(rid) then *1) *)
    (*         let id = paintingsStorage.Values.Count + 1 *)
    (*         let newPainting = {painting with Id = id} *)
    (*         paintingsStorage.Add(id, newPainting) *)
    (*         Some newPainting *)
    (*     (1* else *1) *)
    (*         (1* None *1) *)

    (* let updatePaintingById paintingId paintingToBeUpdated = *)
    (*     if paintingsStorage.ContainsKey(paintingId) then *)
    (*         let updatedPainting = {paintingToBeUpdated with Id = paintingId} *)
    (*         paintingsStorage.[paintingId] <- updatedPainting *)
    (*         Some updatedPainting *)
    (*     else *)
    (*         None *)

    (* let updatePainting paintingToBeUpdated = *)
    (*     updatePaintingById paintingToBeUpdated.Id paintingToBeUpdated *)

    (* let deletePainting paintingId = *)
    (*     paintingsStorage.Remove(paintingId) |> ignore *)

(*     let isPaintingExists = paintingsStorage.ContainsKey *)

(*     let seed () = *)
(*         createPainting {Id=1; Name="lourve"} *)
(*         createPainting {Id=2; Name="yeah"} *)
(*         createPainting {Id=3; Name="is that"} *)
