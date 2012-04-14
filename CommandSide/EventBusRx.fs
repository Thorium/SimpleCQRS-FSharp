/// Event broker for event based communication
/// Second alternative: Using reactive extensions (Rx)
module EventBus

// Execluded from solution.
// If you like Rx, you could replace the current EventBus.fs with this

// Add project references to these files:
//#r "System.CoreEx.dll"
//#r "System.Reactive.dll" 

open Events
open System

///Used just to notify others if anyone would be interested
let EventPublisher = new System.Collections.Generic.Subject<Event>()

/// Used to subscribe to event changes
let public Subscribe (eventHandle: Events.Event -> unit) = EventPublisher.Subscribe(eventHandle)

