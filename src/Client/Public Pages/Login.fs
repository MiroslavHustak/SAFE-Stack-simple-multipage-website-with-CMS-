module Login

open System

open Feliz
open Elmish
open Fable.Remoting.Client

open Shared
open SharedTypes

open ContentLogin
open ContentCMSRozcestnik

type ApplicationUser =  //zatim v Login nevyuzito
    | FirstTimeRunAnonymous
    | Anonymous
    | LoggedIn of SharedApi.User

type Model =
    {
      User: ApplicationUser 
      GetSecurityTokenFile: bool //zatim v Login nevyuzito
      DeleteSecurityTokenFile: unit //zatim v Login nevyuzito
      InputUsr: string
      InputPsw: string
      Id: int
    }

type Msg =
    | SetUsrInput of string
    | SetPswInput of string
    | SendUsrPswToServer
    | GetLoginResults of SharedApi.LoginResult
    | AskServerForSecurityTokenFile
    | AskServerForDeletingSecurityTokenFile
    | GetSecurityTokenFileMsg of bool //zatim v Login nevyuzito
    | DeleteSecurityTokenFileMsg of unit //zatim v Login nevyuzito


let private api() = Remoting.createApi ()
                    |> Remoting.withRouteBuilder Route.builder
                    |> Remoting.buildProxy<IGetApi>

let private getSecurityTokenFileApi = api()
let private deleteSecurityTokenFileApi = api()
let private getLoginApi = api()

let init id : Model * Cmd<Msg> =
    let model = {
                    User = FirstTimeRunAnonymous
                    GetSecurityTokenFile = false
                    DeleteSecurityTokenFile = ()                           
                    InputUsr = String.Empty
                    InputPsw = String.Empty
                    Id = id
                }
    model, Cmd.none

let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =

    match msg with
    | SetUsrInput value -> { model with InputUsr = value }, Cmd.none
    | SetPswInput value -> { model with InputPsw = value }, Cmd.none
    | SendUsrPswToServer ->
        let buttonClickEvent = SharedLoginValues.create model.InputUsr model.InputPsw
        let cmd = Cmd.OfAsync.perform getLoginApi.login buttonClickEvent GetLoginResults 
        model, cmd   
    | GetLoginResults value -> 
        let result =           
            match value with
            | SharedApi.UsernameOrPasswordIncorrect    -> { model with User = ApplicationUser.Anonymous}
            | SharedApi.LoggedIn user                  -> { model with User = ApplicationUser.LoggedIn user}     
        result, Cmd.ofMsg AskServerForSecurityTokenFile
        
    | AskServerForSecurityTokenFile ->
        let sendEvent = GetSecurityTokenFile.create () 
        let cmd = Cmd.OfAsync.perform getSecurityTokenFileApi.getSecurityTokenFile sendEvent GetSecurityTokenFileMsg
        model, cmd

    | AskServerForDeletingSecurityTokenFile ->
        let sendEvent = DeleteSecurityTokenFile.create () 
        let cmd = Cmd.OfAsync.perform deleteSecurityTokenFileApi.deleteSecurityTokenFile sendEvent DeleteSecurityTokenFileMsg
        model, cmd

    | GetSecurityTokenFileMsg value -> { model with GetSecurityTokenFile = value }, Cmd.none     

    | DeleteSecurityTokenFileMsg _ -> model, Cmd.none // value je unit, takze staci placement   

let view (model: Model) (dispatch: Msg -> unit) =

    let errorMsg1 = "Buď uživatelské jméno anebo heslo je neplatné."
    let errorMsg2 = "Prosím zadej údaje znovu."

    let proponClick =
        prop.onClick (fun e -> e.preventDefault()
                               dispatch SendUsrPswToServer
                     )

    let submitInput =
        Html.input [
            prop.type' "submit"
            prop.value "Odeslat"
            prop.id "Button1"                                                                           
            proponClick
            prop.style
                [
                  style.width(200)
                  style.fontWeight.bold
                  style.fontSize(16) 
                  style.color.blue
                  style.fontFamily "sans-serif"
                ]
        ]                      

    let inputElementUsr =
        Html.input [          
            prop.type' "text"
            prop.id "userNameID"
            prop.name "userName"            
            prop.placeholder "Uživatelské jméno"
            prop.value model.InputUsr
            prop.onChange (fun (ev: string) -> SetUsrInput ev |> dispatch)
            prop.style
                [
                  style.width(200)
                  style.fontFamily "sans-serif"
                ]
            prop.autoFocus true
        ]
        
    let inputElementPsw =
        Html.input [            
             prop.type' "password"
             prop.id "passWID"
             prop.name "passW"
             prop.placeholder "Heslo"
             prop.value model.InputPsw
             prop.onChange (fun (ev: string) -> SetPswInput ev |> dispatch)  
             prop.style
                 [
                   style.width(200)
                   style.fontFamily "sans-serif"
                 ]
        ]   

    //prvni rozcestnik    
    let returnButtonDiv askServerForDeletingSecurityTokenFile =
        Html.div [
            Html.form [
                prop.action (RouterM.toHash (RouterM.Home))
                prop.children [
                    Html.input [
                        prop.type' "submit"
                        prop.value "Logout a návrat na webové stránky" //prvni rozcestnik
                        prop.id "Button2"
                        prop.onClick (fun _ -> dispatch askServerForDeletingSecurityTokenFile)
                        prop.style
                            [
                                style.width(300)
                                style.height(30)
                                style.fontWeight.bold
                                style.fontSize(16) 
                                style.color.blue
                                style.fontFamily "sans-serif"
                            ]
                    ]
                ]                   
            ]       
        ]              
  
  //************************************************************************

    let fnError() =
        contentLogin
        <| submitInput                        
        <| inputElementUsr 
        <| inputElementPsw  
        <| (errorMsg1, errorMsg2)
        <| false //zneviditelneni
        <| (dispatch: Msg -> unit)

    let fnFirstRun() =
        contentLogin
        <| submitInput                        
        <| inputElementUsr 
        <| inputElementPsw                    
        <| (String.Empty, String.Empty)
        <| true //zneviditelneni
        <| (dispatch: Msg -> unit)

    match model.User with      
    | Anonymous             -> fnError()
    | FirstTimeRunAnonymous -> fnFirstRun()
    | LoggedIn user         -> contentCMSRozcestnik (returnButtonDiv AskServerForDeletingSecurityTokenFile) //parametr se presune do logout
                                   
