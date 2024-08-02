namespace Logging

open System

open Newtonsoft.Json
open NReco.Logging.File
open Microsoft.Extensions.Logging

//**********************************

open Helpers.Server
open Server.Settings

module Logging =     

    // Function to format log entry as JSON array
    let private formatLogEntry (msg: LogMessage) =
        try
            let sb = System.Text.StringBuilder()
            let sw = new System.IO.StringWriter(sb) //prip. use
            let jsonWriter = new JsonTextWriter(sw) //prip. use

            try     
                jsonWriter.WriteValue(string DateTime.Now)
                //jsonWriter.WriteValue(string msg.LogLevel)
                jsonWriter.WriteStartArray()

                //jsonWriter.WriteValue(msg.LogName)
                //jsonWriter.WriteValue(msg.EventId.Id)

                jsonWriter.WriteValue(msg.Message)
                jsonWriter.WriteEndArray()

                Option.ofNullEmpty >> Result.fromOption <| sb             

            finally
                sw.Close()
                sw.Dispose()
                jsonWriter.Close()

        with ex -> Error <| string ex.Message
                                   
        |> function
            | Ok value  -> value  
            | Error err -> String.Empty //proste s tim nic nezrobime, kdyz to nebude fungovat...
                           

    //***************************Log files******************************       
    
    let private loggerFactory = 
        LoggerFactory.Create(
            fun builder ->                                        
                         builder.AddFile(
                             logFileName, 
                             fun fileLoggerOpts
                                 ->     
                                  //ostatni properties nefungovaly, TODO zjistit cemu  
                                  fileLoggerOpts.FileSizeLimitBytes <- 52428800
                                  fileLoggerOpts.MaxRollingFiles <- 10   
                                  fileLoggerOpts.FormatLogEntry <- formatLogEntry
                             ) 
                             |> ignore
        )
       
    let private logger = 
        loggerFactory.CreateLogger("SafeStackNutricniTerapie4")
                
    let internal logInfoMsg msg = 
        logger.LogInformation(msg)
        loggerFactory.Dispose()

