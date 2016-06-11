[<AutoOpen>]
module Bus

    open System.Configuration
    open System.IO
    open System.Runtime.Serialization.Formatters.Binary

    open RabbitMQ.Client
    open RabbitMQ.Client.Events
    open Serialization

    // Turn "merchant create v1" into "merchant.create.v1"
    let private resolve<'T> () =
        typedefof<'T>.Name.Replace("+", "")        

    // Helpers
    let private declare (channel : IModel) queueName =
        channel.ExchangeDeclare("bus", "fanout")
        //channel.QueueDeclare(queueName, true, false, false, null)

    // Config
    let private host = "127.0.0.1"
    let private port = 5672
    let private userName = ""
    let private password = ""
    let private exchange = "bus"

    let private factory = ConnectionFactory(HostName = host) //, Port = port, UserName = userName, Password = password)    

    let publish<'T> (message : 'T) = 
        use connection = factory.CreateConnection()
        use model = connection.CreateModel()

        let queueName = resolve<'T> ()
        
        let json = 
            message 
            |> toJson
            
        let serializedMessage = json |> System.Text.Encoding.UTF8.GetBytes
        printfn "Publishing: %s - %s - %s" exchange queueName json
        model.BasicPublish(exchange, queueName, null, serializedMessage)

   // Start push-based subscription to topic
    let subscribe<'T> callback = 
        let connection = factory.CreateConnection()
        let model = connection.CreateModel()        
        let queueName = resolve<'T> ()
        declare model queueName |> ignore

        printfn "Setup subscription: %s" queueName

        let consumer = EventingBasicConsumer(model)
        consumer.Received.Add((fun message -> 
            printfn "Got message: %A" message
            message.Body
            |> System.Text.Encoding.UTF8.GetString
            |> fromJson<'T>
            |> callback
        ))

        model.BasicConsume(queueName, true, consumer) |> ignore
        
        (fun () -> 
            model.Close()
            connection.Close()
        )
 