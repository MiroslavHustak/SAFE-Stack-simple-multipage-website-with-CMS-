namespace Database

open System
open System.Data.SqlClient
open FsToolkit.ErrorHandling

open Logging.Logging
open ErrorTypes.Server

open Queries.SqlQueries
open DtoToStorage.Server.DtoToStorage

module InsertOrUpdate = 

    //**************** Sql query strings *****************
    //See the file SQL Queries.fs
       
    //**************** Sql queries - inner functions  *****************
    //let internal insertOrUpdate (createConnection: unit -> SqlConnection) (sendCenikValues : CenikValuesDtoToStorage) =
    let internal insertOrUpdate (connection: SqlConnection) (sendCenikValues : CenikValuesDtoToStorage) = 
                            
        try
            //failwith "Simulated exception SqlInsertOrUpdate"

            let isolationLevel = System.Data.IsolationLevel.Serializable //Transaction locking behaviour
                            
            //let connection: SqlConnection = createConnection()
            let transaction: SqlTransaction = connection.BeginTransaction(isolationLevel) //Transaction to be implemented for all commands linked to the connection

            try
                let idInt = sendCenikValues.Id //idInt = Primary Key for new/old/fixed value state
                let valState = sendCenikValues.ValueState
   
                //**************** Parameters for command.Parameters.AddWithValue("@val", some value) *****************
                let newParamList =
                    [
                        ("@valState", valState)
                        ("@val01", sendCenikValues.V001)
                        ("@val02", sendCenikValues.V002)
                        ("@val03", sendCenikValues.V003)
                        ("@val04", sendCenikValues.V004)
                        ("@val05", sendCenikValues.V005)
                        ("@val06", sendCenikValues.V006)
                        ("@val07", sendCenikValues.V007)
                        ("@val08", sendCenikValues.V008)
                        ("@val09", sendCenikValues.V009)
                    ]       

                //**************** SqlCommands *****************
                use cmdExists = new SqlCommand(queryExists, connection, transaction) //non-nullable, ex caught with tryWith
                
                cmdExists.Parameters.AddWithValue("@Id", idInt) |> ignore

                match cmdExists.ExecuteScalar() |> Option.ofNull with
                | Some _ ->
                          use cmdUpdate = new SqlCommand(queryUpdate, connection, transaction)//non-nullable, ex caught with tryWith
                          cmdUpdate.Parameters.AddWithValue("@Id", idInt) |> ignore
                                                                                    
                          newParamList |> List.iter (fun item -> cmdUpdate.Parameters.AddWithValue(item) |> ignore)

                          cmdUpdate.ExecuteNonQuery() > 0 //rowsAffected, non-nullable, ex caught with tryWith
                          |> function
                              | false ->
                                       transaction.Rollback()
                                       logInfoMsg <| sprintf "Error019 %s" String.Empty
                                       Error InsertOrUpdateError 
                              | true  ->
                                       Ok <| transaction.Commit()
                                            
                | None   ->
                          use cmdInsert = new SqlCommand(queryInsert, connection, transaction) //non-nullable, ex caught with tryWith                                        
                          cmdInsert.Parameters.AddWithValue("@valId", idInt) |> ignore

                          newParamList |> List.iter (fun item -> cmdInsert.Parameters.AddWithValue(item) |> ignore)

                          cmdInsert.ExecuteNonQuery() > 0 //rowsAffected, non-nullable, ex caught with tryWith
                          |> function
                              | false ->
                                       transaction.Rollback()
                                       logInfoMsg <| sprintf "Error020 %s" String.Empty
                                       Error InsertOrUpdateError 
                              | true  ->
                                       Ok <| transaction.Commit() 
                                
            finally
                transaction.Dispose()
        with
        | ex ->
              logInfoMsg <| sprintf "Error021 %s" (string ex.Message)
              Error InsertOrUpdateError