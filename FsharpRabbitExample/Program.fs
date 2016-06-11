open System
open Messages

// Object used as queue message
type Order = { OrderId : int }

[<EntryPoint>]
let main argv = 

    // Set up subscription
    let cancelOrderReceivedQueue = Bus.subscribe<Messages.Message> (fun message -> 
        printfn "First listener: Id #%s" message.Id 
    )

    let cancelOrderReceivedQueue = Bus.subscribe<Messages.Message> (fun message -> 
        printfn "Second listener: Id #%s" message.Id 
    )

    // Keep looping until user quits
    let rec loop id =
        let char = Console.ReadKey()
        match char.Key with
        | ConsoleKey.Escape -> cancelOrderReceivedQueue()
        | _ ->
            printfn "Publishing on bus"
            Bus.publish (new Message(id.ToString(), "WAT"))
            loop (id + 1)
    
    printfn "Press a key to order, [ESC] to exit"
    loop 0

    0