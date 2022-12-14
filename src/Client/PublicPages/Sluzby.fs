namespace PublicPages

open Feliz
open Elmish
open Fable.Remoting.Client

open Records.Client
open HtmlFeliz.Layout
open HtmlFeliz.ContentSluzby

module Sluzby = 

    type Model =
        {      
            Id: int       
        }

    type Msg =
        | DummyMsg

    let init id : Model * Cmd<Msg> =
        let model = { Id = id }
        model, Cmd.none

    let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =  model, Cmd.none   

    let view (model: Model) (dispatch: Msg -> unit) links =

        let sluzbyRecord =
           {
               Home = prop.className "normal"
               Sluzby = prop.className "current"
               Cenik = prop.className "normal"
               Nenajdete = prop.className "normal"
               Kontakt = prop.className "normal"
           }

        layout <| contentSluzby() <| sluzbyRecord <| links
    
