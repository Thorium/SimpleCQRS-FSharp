/// Event loop to store history of events
/// Second alternative using reactive extensions (Rx)
module EventStorage

// Execluded from solution.
// If you like Rx, you could replace the current EventStorage.fs with this

open Domain
open Events
open System
open EventBus

/// Infrastructure to save and restore data from some storage
type IRepository =
    abstract Save : AggregateRoot -> string
    abstract GetHistoryById<'t when 't :> AggregateRoot> : Guid -> ('t -> 't)

/// Container to capsulate events
type EventDescriptor(id:Guid, eventData:Event) = 
    member x.Id = id
    member x.EventData = eventData

/// Custom implementation of in-memory time async event storage. Using message passing.
type EventStorage() =

    let eventstorage = new System.Collections.Generic.ReplaySubject<EventDescriptor>()
    
    let SaveEvents id events = 
        let storeAndPublish evt =
            EventPublisher.OnNext evt
            EventDescriptor(id, evt) 
            |> eventstorage.OnNext

        events |> List.iter storeAndPublish

    let GetEventsForAggregate id =
        eventstorage
        |> Observable.filter(fun i -> i.Id=id)
        |> Observable.map (fun i -> i.EventData)     
           
    interface IRepository with
        member x.Save (item:AggregateRoot) = 
            SaveEvents item.Id item.GetUncommittedChanges
            "saved"

        member x.GetHistoryById id = 
            let currentEvents =
                let currentEventList = new System.Collections.Generic.List<Event>()
                GetEventsForAggregate id 
                |> Observable.subscribe(fun e -> currentEventList.Add(e))
                |> ignore
                currentEventList.ToArray() |> Array.toList

            LoadsFromHistory currentEvents

    member x.ShowItemHistory id = 
        GetEventsForAggregate id
        |> Observable.subscribe(Console.WriteLine)
        |> ignore
(*
// Tests for Interactive:         
let storage = new EventStorage()
let id = System.Guid.NewGuid()
(storage :> IRepository).Save (new InventoryItem(id, CreateType.New, "testi"))
storage.ShowItemHistory id
*)