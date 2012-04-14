/// Event loop to store history of events
module EventStorage

open EventBus
open Domain
open Events
open System

/// Infrastructure to save and restore data from some storage
type IRepository =
    abstract Save : AggregateRoot -> string
    abstract GetHistoryById<'t when 't :> AggregateRoot> : Guid -> ('t -> 't)

/// Infrastructure to generic event storage system
type EventStoreMethod =
| SaveEvents of Guid * Event list
| GetEventsForAggregate of Guid * AsyncReplyChannel<Event list>
| Quit

/// Container to capsulate events
type EventDescriptor(id:Guid, eventData:Event) = 
    member x.Id = id
    member x.EventData = eventData

/// Custom implementation of in-memory time async event storage. Using message passing.
type EventStorage() =
    let eventstorage = MailboxProcessor.Start(fun ev ->
        let rec msgPassing history =
            async { let! k = ev.Receive()
                    match k with
                    | Quit -> return ()
                    | SaveEvents(id, events) ->

                        let storeAndPublish evt =
                            EventPublisher.Trigger evt
                            EventDescriptor(id, evt) 

                        let descriptors = events |> List.map (storeAndPublish)

                        return! msgPassing(descriptors @ history)
                    | GetEventsForAggregate(id, reply) ->

                        let evts = history 
                                   |> List.filter(fun i -> i.Id=id)
                                   |> List.map (fun i -> i.EventData)
                        reply.Reply(evts)
                        return! msgPassing(history)
            }
        msgPassing [])

    interface IRepository with
        member x.Save (item:AggregateRoot) = 
            eventstorage.Post(SaveEvents(item.Id, item.GetUncommittedChanges))
            "saved"

        member x.GetHistoryById id = 
            eventstorage.PostAndReply(fun rep -> GetEventsForAggregate(id,rep))
            |> List.rev
            |> LoadsFromHistory

    member x.Quit = 
        eventstorage.Post(Quit)
        do Console.WriteLine("Storage exited.")

    member x.ShowItemHistory id = 
        eventstorage.PostAndReply(fun rep -> GetEventsForAggregate(id,rep))
        |> List.rev
        |> List.iter Console.WriteLine

(*
// Tests for Interactive:         
let storage = new EventStorage()
let id = System.Guid.NewGuid()
(storage :> IRepository).Save (new InventoryItem(id, CreateType.New, "testi"))
storage.ShowItemHistory id
storage.Quit
*)