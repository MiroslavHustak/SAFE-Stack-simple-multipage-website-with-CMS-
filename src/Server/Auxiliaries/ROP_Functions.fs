module ROP_Functions

open System
open System.IO

open DiscriminatedUnions

open SharedTypes

let tryWith f1 f2 s = 
    try
        try          
           f1 s |> Success
        finally
           f2 s 
    with
    | ex -> Failure (sprintf"%s: %s" s ex.Message)

let deconstructor1 =  
    function
    | Success x  -> String.Empty                                                   
    | Failure ex -> ex 

let deconstructor2 a =  
    function
    | Success x  -> x, String.Empty                                                    
    | Failure ex -> a, ex

let optionToFailwith str = 
    function
    | Some value -> value
    | None       -> failwith str  
                    
                                      
                    





