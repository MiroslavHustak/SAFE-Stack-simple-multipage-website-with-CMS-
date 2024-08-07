module Server.Tests

open System
open System.IO

open Expecto
open FsToolkit.ErrorHandling

open Server
open Shared

open ErrorTypes.Server
open Helpers.Server.Security2

let private pathToUberHashTxt = 
    try
        sprintf "%s%s%s" AppDomain.CurrentDomain.BaseDirectory "Resources" @"\uberHash.txt" //CopyAlways      
    with
    | ex -> failwith (sprintf "Závažná chyba na serveru !!! %s" ex.Message)  

let private server =
    testList "Server"
        [
            //just testing a test :-), no real benefit out of this test
            testCase "testingExpectoServer" <| fun _ ->

                let expected = 5
                Expect.isLessThanOrEqual expected (2+3) "2+3 = 5"//test description

            //real test 
            testCase "uberHashServer" <| fun _ ->

                let expected =

                    let uberHashError uberHash credential seqFn =

                        try
                            match uberHash with
                            | Ok uberHash ->
                                           match verify (uberHash |> seqFn) credential with 
                                           | true  -> LegitimateTrue
                                           | false -> LegitimateFalse
                            | Error _     ->
                                           ExceptionError
                        with
                        | _ -> ExceptionError  
                          
                    let uberHash =
                        try 
                            match File.Exists(Path.GetFullPath(pathToUberHashTxt)) with
                            | false -> Error String.Empty                                
                            | true  -> Ok (File.ReadAllLines(pathToUberHashTxt) |> Seq.ofArray)     
                        with
                        | ex -> Error (string ex.Message)                        

                    uberHash

                Expect.isOk expected "secret credential"  
                Expect.equal (expected.OkValue |> Seq.last) "....." "secret credential" 
        ]

let private all =
    testList "All"
        [
            Shared.Tests.shared
            server
        ]

[<EntryPoint>]
let internal main _ = runTestsWithCLIArgs [] [||] all