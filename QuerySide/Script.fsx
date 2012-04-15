// This file is a script that can be executed with the F# Interactive.  
// It can be used to explore and test the library project.
// By default script files are not be part of the project build.

// Learn more about F# at http://fsharp.net. See the 'F# Tutorial' project
// for more guidance on F# programming.

#r "System.Data.Entity.dll"
#r "FSharp.Data.TypeProviders.dll"

#load "..\CommandSide\Events.fs"
#load "..\CommandSide\Domain.fs"
#load "..\CommandSide\EventBus.fs"
#load "..\CommandSide\EventStorage.fs"

//#r "System.CoreEx.dll"
//#r "System.Reactive.dll" 
//#load "..\CommandSide\EventBusRx.fs"
//#load "..\CommandSide\EventStorageRx.fs"

#load "..\CommandSide\Commands.fs"
#load "..\CommandSide\CommandHandler.fs"

open Events
open EventBus
open System
open CommandHandler
open Commands

#load "ReadModel.fs"

open ReadModel

RegisterHandlersInventoryListView()
RegisterHandlersInvenotryItemDetailView()

let itemId1 = Guid.NewGuid()
CreateInventoryItem(itemId1, "MyItem") |> Handle

let storedItemName = InMemoryDatabase.InventoryItems.[0].Name
let storedDetailName = InMemoryDatabase.InventoryItemDetails.[itemId1].Name