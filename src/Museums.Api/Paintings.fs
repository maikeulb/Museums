namespace MuseumsApi.Handlers
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
        GetById : (int*int) -> 'a option
        IsExists : int -> bool
        Create : 'a -> 'a
        Update : 'a -> 'a option
        UpdateById : int -> 'a -> 'a option
        Delete : int -> unit
    }

    let paintingHandler resourceName resource =

        let resourceBase = "/museums"

        let resourcePath = new PrintfFormat<(int -> string),unit,string,string,int>(resourceBase + "/%d/" + resourceName)

        let resourceIdPath = new PrintfFormat<(int -> string),unit,string,string,(int * int)>(resourceBase + "/%d/" + resourceName + "/%d")

        let badRequest = BAD_REQUEST "Resource not found"

        let handleResource requestError = function
            | Some r -> r |> JSON
            | _ -> requestError

        (* let getAll id = *)
        (*     resource.GetAll id >> handleResource (NOT_FOUND "Resource not found") *)

        let getResourceById =
            resource.GetById >> handleResource (NOT_FOUND "Resource not found")

        let updateResourceById (_, id) =
            request (getResourceFromReq >> (resource.UpdateById id) >> handleResource badRequest)

        let deleteResourceById urlparams =
            let _, id = urlparams
            resource.Delete id
            NO_CONTENT

        let isResourceExists urlparams =
            let _, id = urlparams
            if resource.IsExists id then OK "" else NOT_FOUND ""

        choose [
            GET >=> pathScan resourceIdPath getResourceById
            DELETE >=> pathScan resourceIdPath  deleteResourceById
            PUT >=> pathScan resourceIdPath updateResourceById
            HEAD >=> pathScan resourceIdPath isResourceExists
        ]
