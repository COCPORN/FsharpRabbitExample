module Messages

open FSharp.Data

type ``merchant create v1`` = JsonProvider<"SampleData/Messages/merchant.create.v1.json", RootName = "MerchantCreate">

let test = ``merchant create v1``.Parse("fds")

type Message (id : string, message : string) = 
    member val Id = id  with get, set
    member val Message = message with get, set
    