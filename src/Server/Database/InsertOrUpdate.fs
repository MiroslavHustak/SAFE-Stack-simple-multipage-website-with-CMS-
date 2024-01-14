namespace Database

open System
open System.Data.SqlClient
open FsToolkit.ErrorHandling

open ErrorTypes.Server
open Auxiliaries.Server

open Queries.SqlQueries
open DtoSend.Server.DtoSend
open Auxiliaries.Server.CEBuilders

module InsertOrUpdate = 

    //**************** Sql query strings *****************
    //See the file SQL Queries.fs
       
    //**************** Sql queries - inner functions  *****************
    let internal insertOrUpdate getConnection closeConnection (sendCenikValues : CenikValuesDtoSend) =

        try
            //failwith "Simulated exception SqlInsertOrUpdate"
            let connection = getConnection()

            try
                let idInt = sendCenikValues.Id //idInt = Primary Key for new/old/fixed value state
                let valState = sendCenikValues.ValueState
                let idString = string idInt        
   
                //**************** Parameters for command.Parameters.AddWithValue("@val", some value) *****************
                let newParamList =
                    [
                        ("@valState", valState); ("@val01", sendCenikValues.V001); ("@val02", sendCenikValues.V002);
                        ("@val03", sendCenikValues.V003); ("@val04", sendCenikValues.V004); ("@val05", sendCenikValues.V005);
                        ("@val06", sendCenikValues.V006); ("@val07", sendCenikValues.V007); ("@val08", sendCenikValues.V008); ("@val09", sendCenikValues.V009)
                    ]       

                //**************** SqlCommands *****************
                use cmdExists = new SqlCommand(queryExists idString, connection) //non-nullable, ex caught with tryWith                                   
                use cmdInsert = new SqlCommand(queryInsert, connection) //non-nullable, ex caught with tryWith 
                use cmdUpdate = new SqlCommand(queryUpdate idString, connection)//non-nullable, ex caught with tryWith 
                                
                //**************** Add values to parameters and execute commands with business logic *****************
                //Objects handled with extra care due to potential type-related concerns (you can call it paranoia :-)). 
                match cmdExists.ExecuteScalar() |> Option.ofNull with
                | Some _ -> 
                        newParamList |> List.iter (fun item -> cmdUpdate.Parameters.AddWithValue(item) |> ignore) 
                        let rowsAffected = cmdUpdate.ExecuteNonQuery() //non-nullable, ex caught with tryWith 
                        match rowsAffected with
                        | 0 -> Error InsertOrUpdateError 
                        | _ -> Ok ()                      
                | None   -> 
                        cmdInsert.Parameters.AddWithValue("@valId", idInt) |> ignore
                        newParamList |> List.iter (fun item -> cmdInsert.Parameters.AddWithValue(item) |> ignore)
                        let rowsAffected = cmdInsert.ExecuteNonQuery() //non-nullable, ex caught with tryWith 
                        match rowsAffected with
                        | 0 -> Error InsertOrUpdateError 
                        | _ -> Ok ()    
            finally               
                closeConnection connection //just in case :-) 
        with
        | _ -> Error InsertOrUpdateError 

