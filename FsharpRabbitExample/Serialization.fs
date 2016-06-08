module Serialization

    open Microsoft.FSharp.Reflection
    open System.IO
    open System.Reflection
    open System.Runtime.Serialization
    open System.Runtime.Serialization.Formatters.Binary
    open System.Runtime.Serialization.Json
    open System.Text
    open System.Xml
    open System.Xml.Serialization

    let toString = System.Text.Encoding.ASCII.GetString
    let toBytes (x : string) = System.Text.Encoding.ASCII.GetBytes x

    let toJson<'a> (x : 'a) = 
        let jsonSerializer = new DataContractJsonSerializer(typedefof<'a>)

        use stream = new MemoryStream()
        jsonSerializer.WriteObject(stream, x)
        toString <| stream.ToArray()

    let fromJson<'a> (json : string) =
        let jsonSerializer = new DataContractJsonSerializer(typedefof<'a>)

        use stream = new MemoryStream(toBytes json)
        jsonSerializer.ReadObject(stream) :?> 'a