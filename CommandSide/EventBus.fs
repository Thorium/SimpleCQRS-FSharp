/// Event broker for event based communication
module EventBus

open Events

/// Used just to notify others if anyone would be interested
let public EventPublisher = 
    new Microsoft.FSharp.Control.Event<Event>()

/// Used to subscribe to event changes
let public Subscribe (eventHandle: Events.Event -> unit) = 
    EventPublisher.Publish |> Observable.add(eventHandle)
