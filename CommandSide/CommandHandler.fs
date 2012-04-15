/// Controller: Takes in the commands
module CommandHandler

    open Domain
    open Events
    open EventStorage
    open Commands

    open System
    open System.Collections.Generic

    let Storage = new EventStorage() :> IRepository

    let AsyncHandle (msg:Command) = 
        async {
            let action = 
                let fetchitem id = new InventoryItem(id) |> Storage.GetHistoryById id
                match msg with
                | CreateInventoryItem(id, name) ->
            
                    let item = new InventoryItem(id, New, name)
                    Storage.Save item

                | DeactivateInventoryItem(id) -> 
                    
                    let itm = fetchitem id
                    itm.Deactivate
                    itm |> Storage.Save
                
                | RemoveItemsFromInventory(id, count) -> 

                    let itm = fetchitem id
                    itm.Remove count
                    itm |> Storage.Save
                
                | CheckInItemsToInventory(id, count) -> 

                    let itm = fetchitem id
                    itm.CheckIn count
                    itm |> Storage.Save
                
                | RenameInventoryItem(id, newName) ->  

                    let itm = fetchitem id
                    itm.ChangeName newName
                    itm |> Storage.Save
            
            action |> ignore
        }
        
    let Handle (msg:Command) =  AsyncHandle msg |> Async.RunSynchronously

    //Examples in Scripts.fsx