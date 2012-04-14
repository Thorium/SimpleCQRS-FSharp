/// This is simle readonly-database, a bit like cache
/// In this sample this uses only in-memory as in real life would use F# Type Providers
module ReadModel
    
    open Events
    open EventBus
    open System
    open System.Collections.Generic
    
    //Type providers would use the compiler to create data transfer objects (dtos)

    type InventoryItemDetailsDto(id, name, currentCount) = 
        member x.Id = id
        member val Name = name with get, set
        member val Count = currentCount with get, set

    type InventoryItemListDto(id:Guid, name) = 
        member x.Id = id
        member val Name = name with get, set

    /// Database: Would be SQL, or No-SQL
    type InMemoryDatabase() = 
        let details = new Dictionary<Guid,InventoryItemDetailsDto>()
        member x.InventoryItems = new List<InventoryItemListDto>()
        member x.InventoryItemDetails = details
        member x.GetInventoryItemDetails id = details.[id]

    /// View: First screen
    type InventoryListView() = 
        let dbItems = InMemoryDatabase().InventoryItems
        member x.Handle =
            let knownEvents e =
                match e with
                | InventoryItemCreated(id, name) -> dbItems.Add(new InventoryItemListDto(id, name))
                | InventoryItemDeactivated(id) ->  dbItems.RemoveAll(fun stored -> stored.Id = id) |> ignore
                | InventoryItemRenamed(id, newName) -> 
                    let found = dbItems.Find(fun stored -> stored.Id = id)
                    found.Name <- newName
                | _ -> ignore()
            EventBus.Subscribe knownEvents

    /// View: Second screen
    type InvenotryItemDetailView() = 
        let dbDetailItem = InMemoryDatabase().InventoryItemDetails
        let getDetailsItem id =
            let found = dbDetailItem.TryGetValue id
            match fst found with
            | true -> snd found
            | false -> failwith("did not find the original inventory this shouldnt happen")
        member x.Handle =
            let knownEvents e =
                match e with
                | InventoryItemCreated(id, name) -> dbDetailItem.Add(id, new InventoryItemDetailsDto(id, name, 0));
                | InventoryItemDeactivated(id) -> dbDetailItem.Remove(id) |> ignore
                | InventoryItemRenamed(id, newName) -> 
                    let d = getDetailsItem id
                    d.Name <- newName
                | ItemsCheckedInToInventory(id, count) -> 
                    let d = getDetailsItem id
                    d.Count <- count
                | ItemsRemovedFromInventory(id, count) -> 
                    let d = getDetailsItem id
                    d.Count <- count
            EventBus.Subscribe knownEvents