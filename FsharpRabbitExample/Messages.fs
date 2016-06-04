module Messages

open FSharp.Data

type merchantCreateV1 = JsonProvider<"SampleData/Messages/merchant.create.v1.json", RootName = "MerchantCreate">

let test = merchantCreateV1.Parse("fds")
