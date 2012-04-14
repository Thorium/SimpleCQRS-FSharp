// This file is a script that can be executed with the F# Interactive.  
// It can be used to explore and test the library project.
// By default script files are not be part of the project build.

// Learn more about F# at http://fsharp.net. See the 'F# Tutorial' project
// for more guidance on F# programming.

#load "Events.fs"
#load "Domain.fs"
#load "EventBus.fs"
#load "EventStorage.fs"

//#r "System.CoreEx.dll"
//#r "System.Reactive.dll" 
//#load "EventBusRx.fs"
//#load "EventStorageRx.fs"

#load "Commands.fs"
#load "CommandHandler.fs"

open Domain
open Events
open EventBus
open EventStorage
open Commands
open System
open System.Collections.Generic
open CommandHandler

// Some testing:
let itemId1 = Guid.NewGuid()
CreateInventoryItem(itemId1, "MyItem") |> CommandHandler
CheckInItemsToInventory(itemId1, 10) |> CommandHandler
CheckInItemsToInventory(itemId1, 20) |> CommandHandler
//CheckInItemsToInventory(itemId1, -20) |> CommandHandler
RemoveItemsFromInventory(itemId1, 5) |> CommandHandler
RenameInventoryItem(itemId1, "NewItemName") |> CommandHandler
DeactivateInventoryItem(itemId1) |> CommandHandler

(Storage :?> EventStorage).ShowItemHistory itemId1
