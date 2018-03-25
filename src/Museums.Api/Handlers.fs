namespace MuseumsApi.Handlers
open Newtonsoft.Json
open Newtonsoft.Json.Serialization
open Suave
open Suave.Operators
open Suave.Http
open Suave.Successful


[<AutoOpen>]
module Rest =
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

    type RootResource<'a> = {
        GetAll : unit -> 'a seq
        GetById : int -> 'a option
        IsExists : int -> bool
        Create : 'a -> 'a
        Update : 'a -> 'a option
        UpdateById : int -> 'a -> 'a option
        Delete : int -> unit
    }

    let rootHandler resourceName resource =

        let resourcePath = "/api/" + resourceName

        let resourceIdPath = new PrintfFormat<(int -> string),unit,string,string,int>(resourcePath + "/%d")

        let badRequest = BAD_REQUEST "Resource not found"

        let handleResource requestError = function
            | Some r -> r |> JSON
            | _ -> requestError

        let getAll= warbler (fun _ -> resource.GetAll () |> JSON)

        let getResourceById =
            resource.GetById >> handleResource (NOT_FOUND "Resource not found")

        let updateResourceById id =
            request (getResourceFromReq >> (resource.UpdateById id) >> handleResource badRequest)

        let deleteResourceById id =
            resource.Delete id
            NO_CONTENT

        let isResourceExists id =
            if resource.IsExists id then OK "" else NOT_FOUND ""

        choose [
            path resourcePath >=> choose [
                GET >=> getAll
                POST >=> request (getResourceFromReq >> resource.Create >> JSON)
                PUT >=> request (getResourceFromReq >> resource.Update >> handleResource badRequest)
            ]
            DELETE >=> pathScan resourceIdPath deleteResourceById
            GET >=> pathScan resourceIdPath getResourceById
            PUT >=> pathScan resourceIdPath updateResourceById
            HEAD >=> pathScan resourceIdPath isResourceExists
        ]

    type NestedResource<'a> = {
        GetAll : int -> 'a seq option
        GetById : (int*int) -> 'a option
        IsExists : int -> bool
        Create : int -> 'a -> 'a option
        Update : 'a -> 'a option
        UpdateById : int -> 'a -> 'a option
        Delete : int -> unit
    }

    let nestedHandler resourceRoot resourceName resource =

        let resourceBase = "/api/" + resourceRoot

        let resourcePath = new PrintfFormat<(int -> string),unit,string,string,int>(resourceBase + "/%d/" + resourceName)

        let resourceIdPath = new PrintfFormat<(int -> string),unit,string,string,(int * int)>(resourceBase + "%d/" + resourceName + "/%d")

        let badRequest = BAD_REQUEST "Resource not found"

        let handleResource requestError = function
            | Some r -> r |> JSON
            | _ -> requestError

        let getAll =
            resource.GetAll >> handleResource (NOT_FOUND "Resource not found")

        let createResource rid =
            request (getResourceFromReq >> (resource.Create rid) >> handleResource badRequest)

        let getResourceById =
            resource.GetById >> handleResource (NOT_FOUND "Resource not found")

        let updateResourceById (_, id) =
            request (getResourceFromReq >> (resource.UpdateById id) >> handleResource badRequest)

        let deleteResourceById (_, id) =
            resource.Delete id
            NO_CONTENT

        let isResourceExists (_, id) =
            if resource.IsExists id then OK "" else NOT_FOUND ""

        choose [
            GET >=> pathScan resourcePath getAll
            POST >=> pathScan resourcePath createResource
            GET >=> pathScan resourceIdPath getResourceById
            DELETE >=> pathScan resourceIdPath deleteResourceById
            PUT >=> pathScan resourceIdPath updateResourceById
            HEAD >=> pathScan resourceIdPath isResourceExists
        ]
