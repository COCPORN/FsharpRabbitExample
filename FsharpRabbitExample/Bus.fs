module Bus

    open System.Configuration
    open System.IO
    open System.Runtime.Serialization.Formatters.Binary

    open RabbitMQ.Client
    open RabbitMQ.Client.Events
    open Serialization

    // Turn "merchant create v1" into "merchant.create.v1"
    let private resolve<'T> () =
        typedefof<'T>.Name.Replace(' ', '.')        

    // Helpers
    let private declare (channel : IModel) queueName =
        channel.QueueDeclare(queueName, true, false, false, null)

    // Config
    let private host = "127.0.0.1"
    let private port = 5672
    let private userName = ""
    let private password = ""
    let private exchange = ""

    let private factory = ConnectionFactory(HostName = host) //, Port = port, UserName = userName, Password = password)    

   // Start push-based subscription to topic
    let subscribe<'T> callback = 
        let connection = factory.CreateConnection()
        let model = connection.CreateModel()
        
        let queueName = resolve<'T> ()
        declare model queueName |> ignore

        let consumer = EventingBasicConsumer(model)
        consumer.Received.Add((fun message -> 
            message.Body
            |> System.Text.Encoding.UTF8.GetString
            |> fromJson
            |> callback
        ))

        model.BasicConsume(queueName, true, consumer) |> ignore
        
        (fun () -> 
            model.Close()
            connection.Close()
        )
