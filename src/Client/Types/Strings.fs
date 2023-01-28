namespace Strings.Client

module Strings = 

    open System

    let (|StringNonN|) s = 
        s 
        |> Option.ofObj 
        |> function 
            | Some value -> string value
            | None       -> String.Empty