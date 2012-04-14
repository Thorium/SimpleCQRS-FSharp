/// Model: The domain model classes
/// In this pattern, domain model includes the business logic and their validation rules
/// and the model is not exact model of every single object and property
module Domain

    open Events
    open System

    type CreateType = LoadHistory | New

    /// This base class has id and a list of events related to current domain object.
    [<AbstractClass>]
    type AggregateRoot(id) =
        let mutable changes = []
        member x.Id = id
        member x.GetUncommittedChanges = changes
        member x.MarkChangesAsCommitted = changes <- []
        abstract member Apply : Event -> unit

        member x.ApplyChange (evt:Event) = 
            x.Apply evt
            changes <- evt :: changes

    /// Load from event history
    let LoadsFromHistory<'t when 't :> AggregateRoot> (history:Event list) (item:'t) = 
        history |> List.map item.Apply |> ignore
        item
           
    /// Sample domain object 
    type InventoryItem(id:Guid, creation:CreateType, name:string) =
        inherit AggregateRoot(id)

        let constructorEvent = 
            if(creation = CreateType.New) then base.ApplyChange(InventoryItemCreated(id, name))
        do constructorEvent |> ignore

        ///Serialization constructor
        internal new(id:Guid) = InventoryItem(id, CreateType.LoadHistory, String.Empty)

        member val Activated = true with get, set

        override x.Apply (evt:Event) =
            match evt with
            | InventoryItemCreated(i, n) ->  x.Activated <- true
            | InventoryItemDeactivated(_) -> x.Activated <- false
            | _ -> ignore()

        member x.ChangeName (name:string) = 
            if String.IsNullOrEmpty(name) then failwith("must give a name")
            InventoryItemRenamed(x.Id, name) |> x.ApplyChange

        member x.Remove count = 
            if count <= 0 then failwith("cant remove negative count from inventory")
            ItemsRemovedFromInventory(x.Id, count) |> x.ApplyChange

        member x.CheckIn count = 
            if count <= 0 then failwith("must have a count greater than 0 to add to inventory")
            ItemsCheckedInToInventory(x.Id, count) |> x.ApplyChange

        member x.Deactivate = 
            if x.Activated = false then failwith("already deactivated")
            InventoryItemDeactivated(x.Id) |> x.ApplyChange