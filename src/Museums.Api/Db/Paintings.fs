namespace Museums.Api.Db
open System.Collections.Generic


type Paintings = {
    Id : int
    Name : string
}

module PaintingsDb =

    let paintingsStorage = new Dictionary<int, Painting>()
    let getPaintings () =
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

