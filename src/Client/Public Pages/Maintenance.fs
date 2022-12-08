module Maintenance

open Feliz
open Elmish
open Fable.Remoting.Client

open ContentMaintenance

type Model =
    {        
      Dummy: unit      
    }

type Msg =
    | DummyMsgText   

let init () : Model * Cmd<Msg> =
    let model = { Dummy = () } 
    model, Cmd.none

let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =  model, Cmd.none

let view (model: Model) (dispatch: Msg -> unit) = contentMaintenance()


    