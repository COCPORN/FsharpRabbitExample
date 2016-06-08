module Messages

open FSharp.Data

type ``merchant create v1`` = JsonProvider<"SampleData/Messages/merchant.create.v1.json", RootName = "MerchantCreate">

let test = ``merchant create v1``.Parse("fds")
