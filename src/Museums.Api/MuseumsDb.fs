namespace MuseumsApi.Db
open System
open System.Collections.Generic

module MuseumsDb =

    type Museum = {
        Id : int
        Name : string
    }

    let museumsStorage = new Dictionary<int, Museum>()

    let getMuseums () =
        museumsStorage.Values :> seq<Museum>

    let getMuseum id =
        if museumsStorage.ContainsKey(id) then
            Some museumsStorage.[id]
        else
            None
    let createMuseum museum =
        let id = museumsStorage.Values.Count + 1
        let newMuseum = {museum with Id = id}
        museumsStorage.Add(id, newMuseum)
        newMuseum

    let updateMuseumById museumId museumToBeUpdated =
        if museumsStorage.ContainsKey(museumId) then
            let updatedMuseum = {museumToBeUpdated with Id = museumId}
            museumsStorage.[museumId] <- updatedMuseum
            Some updatedMuseum
        else
            None

    let updateMuseum museumToBeUpdated =
        updateMuseumById museumToBeUpdated.Id museumToBeUpdated

    let deleteMuseum museumId =
        museumsStorage.Remove(museumId) |> ignore

    let isMuseumExists  = museumsStorage.ContainsKey

