namespace DbAccess

(*

open System

open Dapper
open FSharp.Control
open Dapper.FSharp.MSSQL
open System.Data.SqlClient

open SharedTypes
open Queries.SqlQueries
open DiscriminatedUnions.Server

open Auxiliaries.Server.DapperHelper
open Auxiliaries.Server.ROP_Functions
open Auxiliaries.Connections.Connection

module Dapper = 

    //Dapper.Sharp and Dapper tested here for learning purposes only

    //Dapper.FSharp
    let private table = table'<DapperGetCenikValues> "CENIK"

    //**************** Sql queries - inner functions  *****************
    let insertOrUpdate (getCenikValues: GetCenikValues) =

        let dapperGetCenikValues = dapperGetCenikValues { getCenikValues with Msgs = Messages.Default }

        let insertOrUpdateNow x =

            //failwith "Simulated exception SqlInsertOrUpdate" 

            use connection = new SqlConnection(connStringSomee) 
            connection.Open()  

            let idInt = getCenikValues.Id //idInt = Primary Key for new/old/fixed value state
               
            //**************** SqlCommands *****************    
            //plain Dapper
            let cmdExistsDapper = connection.ExecuteScalar(queryExists(string idInt))

            //Dapper.FSharp
            let cmdInsert() = 
                insert
                    {
                        into table
                        value dapperGetCenikValues
                    } |> connection.InsertAsync

            //Dapper.FSharp        
            let cmdUpdate() = 
                update
                    {
                        for p in table do
                            set dapperGetCenikValues
                            where (p.Id = dapperGetCenikValues.Id)
                    } |> connection.UpdateAsync   

            //**************** Execute commands with business logic *****************
            match cmdExistsDapper |> Option.ofObj with
            | Some _ -> cmdUpdate().GetAwaiter().GetResult() |> ignore                      
            | None   -> cmdInsert().GetAwaiter().GetResult() |> ignore

        let exnSql = (insertOrUpdateNow, (fun x -> ()), "ErrorSql1") |||> tryWith |> deconstructor1

        match getCenikValues.Msgs.Msg1 = "First run" with
        | true when exnSql <> String.Empty  -> sprintf"%s %s" "Byly dosazeny defaultní nebo předchozí hodnoty, neb došlo k této chybě:" exnSql
        | true when exnSql = String.Empty   -> String.Empty 
        | false when exnSql <> String.Empty -> sprintf"%s %s" "Zadané hodnoty nebyly nebo nebudou uloženy, neb došlo k této chybě:" exnSql
        | false when exnSql = String.Empty  -> String.Empty
        | _                                 -> exnSql                                          
    
    let selectValues idInt =

        let myErrMsg = [| String.Empty; String.Empty |]
        let myErrType = [| Default; Default |]

        let selectValuesNow x = 

            //failwith "Simulated exception SqlSelectValues"

            use connection = new SqlConnection(connStringSomee) //Tohle uzavre ve vhodne dobe a navic je mozne toto mit v try with bloku. Vzpomen si, co to delalo pri samostatnem connection.Close() 
            connection.Open()

            //**************** SqlCommands ***************** 
            //plain Dapper
            let cmdExistsDapper = connection.ExecuteScalar(queryExists(string idInt))

             //Dapper.FSharp
            let cmdSelect() =       
                task
                    {
                        let! values =
                            select
                                {
                                     for p in table do                            
                                         where (p.Id = idInt)
                                } |> connection.SelectAsync<DapperGetCenikValues> //Dapper.FSharp creators rendered the resulting type as a task
                        
                        match values |> Option.ofObj with
                        | Some values -> return Seq.head values
                        | None        -> return DapperGetCenikValues.Default  
                    }

            //**************** Execute commands with business logic *****************
            match cmdExistsDapper |> Option.ofObj with 
            | Some _ -> cmdSelect().GetAwaiter().GetResult() //Gets to be done here, but in cases where not, transfer the task up the code
            | None   -> let exnSql1 = insertOrUpdate GetCenikValues.Default
                        Array.set myErrMsg 0 exnSql1 //Error message can be caught only here, not further down the code
                        match myErrMsg |> Array.head with
                        | "" -> Array.set myErrType 0 NotFullDb 
                        | _  -> Array.set myErrType 0 OtherProblems  
                        cmdSelect().GetAwaiter().GetResult() //Gets to be done here, but in cases where not, transfer the task up the code

        let (getValues, exnSql2) = (selectValuesNow, (fun x -> ()), "ErrorSql2") |||> tryWith |> deconstructor2 DapperGetCenikValues.Default

        let exnSql =
            match myErrType |> Array.head with
            | NotFullDb -> //Not fully filled db, but InsetUpdate ended with success => ignoring exceptions on condition there are no problems with data reading
                           match myErrType |> Array.last with
                           | OtherProblems -> sprintf "%s %s %s" exnSql2 (myErrMsg |> Array.head) (myErrMsg |> Array.last)
                           | _             -> String.Empty   
            | _         -> sprintf "%s %s %s" exnSql2 (myErrMsg |> Array.head) (myErrMsg |> Array.last)
   
        getCenikValues getValues, exnSql 

        (*
          Result and RunSynchronously block the thread pool
          CPU is idle waiting for them to return
          *************************************************
          Using task expressions is preferred when interoperating extensively with .NET libraries that create or consume .NET tasks.
        *)

*)