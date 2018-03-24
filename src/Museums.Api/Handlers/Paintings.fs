namespace Museums.Api.Handlers
open Newtonsoft.Json
open Newtonsoft.Json.Serialization
open Suave
open Suave.Operators
open Suave.Http
open Suave.Successful


[<AutoOpen>]
module Paintings =
    open Suave.RequestErrors
    open Suave.Filters

    let JSON v =
        let jsonSerializerSettings = new JsonSerializerSettings()
        jsonSerializerSettings.ContractResolver <- new CamelCasePropertyNamesContractResolver()

        JsonConvert.SerializeObject(v, jsonSerializerSettings)
        |> OK
        >=> Writers.setMimeType "application/json; charset=utf-8"

    let fromJson<'a> json =
        JsonConvert.DeserializeObject(json, typeof<'a>) :?> 'a

    let getResourceFromReq<'a> (req : HttpRequest) =
        let getString rawForm = System.Text.Encoding.UTF8.GetString(rawForm)
        req.rawForm |> getString |> fromJson<'a>

    type RestResource<'a> = {
        GetAll : unit -> 'a seq
        GetById : int -> 'a option
        IsExists : int -> bool
        Create : 'a -> 'a
        Update : 'a -> 'a option
        UpdateById : int -> 'a -> 'a option
        Delete : int -> unit
    }

    let paintingHandler resourceName resource =

        let resourcePath = new PrintfFormat<(int -> string),unit,string,string,int>("/museums" + "/%d" + resourceName)

        let resourceIdPath = new PrintfFormat<(int -> int -> string),unit,string,string,(int * int)>("/museums" + "/%d" + resourceName + "/%d")

        let badRequest = BAD_REQUEST "Resource not found"

        let handleResource requestError = function
            | Some r -> r |> JSON
            | _ -> requestError

        let getAll (id _)=
            resource.GetByAll id >> handleResource (NOT_FOUND "Resource not found")

        let getResourceById (_ id)=
            resource.GetById >> handleResource (NOT_FOUND "Resource not found")

        let updateResourceById (_ id) =
            request (getResourceFromReq >> (resource.UpdateById id) >> handleResource badRequest)

        let deleteResourceById (_ id) =
            resource.Delete id
            NO_CONTENT

        let isResourceExists id (_ id)=
            if resource.IsExists id then OK "" else NOT_FOUND ""

        choose [
            path resourcePath  >=> choose [
                GET >=> getAll
                POST >=> request (getResourceFromReq >> resource.Create >> JSON)
                PUT >=> request (getResourceFromReq >> resource.Update >> handleResource badRequest)
            ]
            DELETE >=> pathScan resourceIdPath  deleteResourceById
            GET >=> pathScan resourceIdPath getResourceById
            PUT >=> pathScan resourceIdPath updateResourceById
            HEAD >=> pathScan resourceIdPath isResourceExists
        ]
