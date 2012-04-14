/// Event broker for event based communication
/// Second alternative: Using reactive extensions (Rx)
module EventBus

// Add project references to these files:
//#r "System.CoreEx.dll"
//#r "System.Reactive.dll" 

open Events
open System

///Used just to notify others if anyone would be interested
let private eventBusSubject = new System.Collections.Generic.Subject<Event>()
let public EventPublisher = eventBusSubject :> IObservable<Event>

/// Used to subscribe to event changes
let public Subscribe (eventHandle: Events.Event -> unit) = EventBus.Subscribe(eventHandle)

