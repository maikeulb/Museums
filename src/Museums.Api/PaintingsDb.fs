namespace MuseumsApi.Db
open System
open System.Collections.Generic

module PaintingsDb =

    type Painting = {
        Id : int
        Name : string
    }

    let paintingsStorage = new Dictionary<int, Painting>()

    let getPaintings id =
        paintingsStorage.Values :> seq<Painting>

    let getPainting id =
        if paintingsStorage.ContainsKey(id) then
            Some paintingsStorage.[id]
        else
            None

    let createPainting painting =
        let id = paintingsStorage.Values.Count + 1
        let newPainting = {painting with Id = id}
        paintingsStorage.Add(id, newPainting)
        newPainting

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

