namespace Museums.Api.Db
open FSharp.Data.Sql


module Paintings =
    type Paintings =
        {
            Id : int
            Title : string
            Artist : string
            Medium : string
        }

    type private Sql = SqlDataProvider< "Server=localhost;Database=SuaveMusicStore;Trusted_Connection=True;MultipleActiveResultSets=true;Integrated Security=SSPI;", DatabaseVendor=Common.DatabaseProviderTypes.MSSQLSERVER >

    type DbContext = Sql.dataContext

    type PaintingEntity = DbContext.``[dbo].[Paintings]Entity``

    let private getContext() = Sql.GetDataContext()

    let firstOrNone s = s |> Seq.tryFind (fun _ -> true)

    let mapToPainting (paintingEntity : PaintingEntity) =
        {
            Id = paintingEntity.Id
            Title = paintingEntity.Name
            Artist = paintingEntity.Name
            Medium = paintingEntity.Name
        }

        let getPaintings () =
            getContext().``[dbo].[Paintings]``
        |> Seq.map mapToPainting

    let getPaintingEntityById (ctx : DbContext) id =
        query {
            for painting in ctx.``[dbo].[Paintings]`` do
                where (painting.Id = id)
                select painting
        } |> firstOrNone

    let getPaintingById id =
        getPaintingEntityById (getContext()) id |> Option.map mapToPainting

    let createPainting painting =
        let ctx = getContext()
        let painting = ctx.``[dbo].[Paintings]``.Create(painting.Id, painting.Name)
        ctx.SubmitUpdates()
        painting |> mapToPainting

    let updatePaintingById id painting =
        let ctx = getContext()
        let paintingEntity = getPaintingEntityById ctx painting.Id
        match paintingEntity with
        | None -> None
        | Some a ->
            a.Id <- painting.Id
            a.Title <- painting.Title
            a.Artist <- painting.Artist
            a.Medium <- painting.Medium
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
