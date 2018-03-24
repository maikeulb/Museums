namespace Museums.Api.Db

open FSharp.Data.Sql

module MuseumsDb =
    type Museums =
        {
            Id : int
          Name : string
        }

        type private Sql = SqlDataProvider< "Server=localhost;Database=SuaveMusicStore;Trusted_Connection=True;MultipleActiveResultSets=true;Integrated Security=SSPI;", DatabaseVendor=Common.DatabaseProviderTypes.MSSQLSERVER >

    type DbContext = Sql.dataContext

    type MuseumEntity = DbContext.``[dbo].[Museums]Entity``

    let private getContext() = Sql.GetDataContext()

    let firstOrNone s = s |> Seq.tryFind (fun _ -> true)

    let mapToMuseum (museumEntity : MuseumEntity) =
        {
            Id = albumEntity.Id
            Name = museumEntity.Name
        }

        let getMuseums () =
            getContext().``[dbo].[Museums]``
        |> Seq.map mapToMuseum

    let getMuseumEntityById (ctx : DbContext) id =
        query {
            for museum in ctx.``[dbo].[Museums]`` do
                where (museum.Id = id)
                select museum
        } |> firstOrNone

    let getMuseumById id =
        getMuseumEntityById (getContext()) id |> Option.map mapToMuseum

    let createMuseum museum =
        let ctx = getContext()
        let museum = ctx.``[dbo].[Museums]``.Create(museum.Id, museum.Name)
        ctx.SubmitUpdates()
        museum |> mapToMuseum

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

