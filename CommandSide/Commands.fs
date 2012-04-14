// The verbs (actions) of the system (in imperative mood / present tense)
module Commands
 
    open Events
    open Domain
    open System
    
    type Command = 
    | CreateInventoryItem       of Guid * string
    | DeactivateInventoryItem   of Guid 
    | RenameInventoryItem       of Guid * string
    | CheckInItemsToInventory   of Guid * int
    | RemoveItemsFromInventory  of Guid * int