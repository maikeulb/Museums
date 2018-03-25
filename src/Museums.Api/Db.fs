namespace MuseumsApi.Db
open System
open System.Collections.Generic

module Museums =

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

    let seed () =
        createMuseum {Id=1; Name="Museum of Modern Art"} |> ignore
        createMuseum {Id=2; Name="Whiteny Museum of American Art"} |> ignore
        createMuseum {Id=3; Name="The Metropolitcan Museum of Art"} |> ignore


module Paintings =

    type Painting = {
        Id : int
        Title : string
        Artist : string
        Medium : string
        MuseumId : int
    }

    let paintingsStorage = new Dictionary<int, Painting>()

    let getPaintings museumId =
        let paintings = paintingsStorage.Values |> Seq.filter (fun x-> x.MuseumId = museumId)
        if Seq.isEmpty paintings then
            None
        else
            Some paintings

    let getPainting (_, id) =
        if paintingsStorage.ContainsKey(id) then
            Some paintingsStorage.[id]
        else
            None

    let createPainting museumId painting =
        if Museums.isMuseumExists museumId then
            let id = paintingsStorage.Values.Count + 1
            let newPainting = {painting with Id = id}
            paintingsStorage.Add(id, newPainting)
            Some newPainting
        else
            None

    let updatePaintingById paintingId paintingToBeUpdated =
        if paintingsStorage.ContainsKey(paintingId) then
            let updatedPainting = {paintingToBeUpdated with Id = paintingId}
            paintingsStorage.[paintingId] <- updatedPainting
            Some updatedPainting
        else
            None

    let updatePainting paintingToBeUpdated =
        updatePaintingById paintingToBeUpdated.Id paintingToBeUpdated

    let deletePainting paintingId =
        paintingsStorage.Remove(paintingId) |> ignore

    let isPaintingExists = paintingsStorage.ContainsKey

    let seed () =
        createPainting 1 {Id = 1; Title = "The Starry Night"; Artist="Vincent van Gogh"; Medium="Oil on canvas"; MuseumId=1} |> ignore
        createPainting 1 {Id = 2; Title = "Les Demoiselles dAvignon"; Artist="Pablo Picasso"; Medium="Oil on canvas"; MuseumId=1} |> ignore
        createPainting 2 {Id = 3; Title = "Bain a la Genouillere"; Artist="Claude Monet"; Medium="Oil on canvas"; MuseumId=2} |> ignore
        createPainting 2 {Id = 4; Title = "Young Sailor II"; Artist="Henri Matisse"; Medium="Oil on canvas"; MuseumId=2} |> ignore
        createPainting 3 {Id = 5; Title = "Early Sunday Morning"; Artist="Edward Hopper"; Medium="Oil on canvas"; MuseumId=3} |> ignore
        createPainting 3 {Id = 6; Title = "Willem and Bicyle"; Artist="Willem de Kooning"; Medium="Oil on canvas"; MuseumId=3} |> ignore
