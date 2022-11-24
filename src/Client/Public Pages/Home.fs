module Home

open Elmish
open Feliz
open Fable.Remoting.Client

open Shared

open Records
open Layout
open Router

open ContentHome
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

let update (msg: Msg) (model: Model) : Model * Cmd<Msg> = model, Cmd.none

let view (model: Model) (dispatch: Msg -> unit) links securityToken =

   let homeRecord =
      {
        Home = prop.className "current"
        Sluzby = prop.className "normal"
        Cenik = prop.className "normal"
        Nenajdete = prop.className "normal"
        Kontakt = prop.className "normal"
      }
      //nahradit asi bool hodnotou ofledne securityTokenFile  nebo oboji 
   match securityToken with
   | "securityToken" -> layout <| contentHome() <| homeRecord <| links //contentMaintenance()
   | _ -> layout <| contentHome() <| homeRecord <| links
   