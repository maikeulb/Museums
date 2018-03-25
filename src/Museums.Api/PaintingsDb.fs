namespace MuseumsApi.Db
open System
open System.Collections.Generic

module PaintingsDb =

    type Painting = {
        Id : int
        MuseumId : int
        Name : string
    }

    let paintingsStorage = new Dictionary<int, Painting>()

    let getPaintings rid =
        (* paintingsStorage.Values :> seq<Painting> *)
        if paintingsStorage.ContainsKey(rid) then
            let paintings =  paintingsStorage.Values :> seq<Painting>
            Some paintings
        else
            None

    let getPainting (_, id) =
        if paintingsStorage.ContainsKey(id) then
            Some paintingsStorage.[id]
        else
            None

    let createPainting rid painting =
        if paintingsStorage.ContainsKey(rid) then
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
