namespace LoginPages

open System

open Feliz
open Elmish
open Fable.Remoting.Client

open Shared
open SharedTypes

open HtmlFeliz.ContentLogin
open HtmlFeliz.ContentCMSRozcestnik

    module Login =

        //ExternalMsg by Maxime Mangel
        //https://medium.com/@MangelMaxime/my-tips-for-working-with-elmish-ab8d193d52fd
        type ExternalMsg =
            | NoOp
            | SignedIn of SharedApi.LoginResult

        type ApplicationUser =  
            | FirstTimeRunAnonymous
            | Anonymous
            | LoggedIn of SharedApi.User

        type Model =
            {
                User: ApplicationUser 
                InputUsr: string
                InputPsw: string
                Id: int
            }
        
        [<RequireQualifiedAccess>]
        type Page =       
            | CMSRozcestnik of CMSPages.CMSRozcestnik.Model 

        type Msg =
            | SetUsrInput of string
            | SetPswInput of string
            | SendUsrPswToServer
            | GetLoginResults of SharedApi.LoginResult
            | LoginCompleted of SharedApi.LoginResult
            | Logout
            | CMSRozcestnikMsg of CMSPages.CMSRozcestnik.Msg
            | CMSRozcestnikModel of CMSPages.CMSRozcestnik.Model 

        let private getLoginApi =
            Remoting.createApi ()
            |> Remoting.withRouteBuilder Route.builder
            |> Remoting.buildProxy<IGetApi>

        let init id : Model * Cmd<Msg> =

            let model = {
                            User = FirstTimeRunAnonymous
                            InputUsr = String.Empty
                            InputPsw = String.Empty
                            Id = id
                        }
            model, Cmd.none

        let update (msg: Msg) (model: Model): Model * Cmd<Msg> * ExternalMsg =

            match msg with
            | SetUsrInput value  -> { model with InputUsr = value }, Cmd.none, NoOp
            | SetPswInput value  -> { model with InputPsw = value }, Cmd.none, NoOp

            | SendUsrPswToServer ->
                let buttonClickEvent = SharedLoginValues.create model.InputUsr model.InputPsw
                let cmd = Cmd.OfAsync.perform getLoginApi.login buttonClickEvent GetLoginResults 
                model, cmd, NoOp

            | GetLoginResults value -> 
                let model =           
                    match value with
                    | SharedApi.UsernameOrPasswordIncorrect -> { model with User = ApplicationUser.Anonymous } //potrebne pro na konci modulu uvedeny kod
                    | SharedApi.LoggedIn user               -> { model with User = ApplicationUser.LoggedIn user } //potrebne pro na konci modulu uvedeny kod    
                model, Cmd.ofMsg (LoginCompleted value), NoOp

            | LoginCompleted session -> model, Cmd.none, SignedIn session
            | Logout -> model, Cmd.none, NoOp
            | CMSRozcestnikMsg _ -> model, Cmd.none, NoOp
            | CMSRozcestnikModel _ -> model, Cmd.none, NoOp

        let view (model: Model) (dispatch: Msg -> unit) =

            let errorMsg1 = "Bu?? u??ivatelsk?? jm??no anebo heslo je neplatn??."
            let errorMsg2 = "Pros??m zadej ??daje znovu."

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
                    prop.placeholder "U??ivatelsk?? jm??no"
                    prop.value model.InputUsr
                    // prop.onChange (fun (ev: string) -> SetUsrInput ev |> dispatch)
                    prop.onChange (SetUsrInput >> dispatch)
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
                     // prop.onChange (fun (ev: string) -> SetPswInput ev |> dispatch)
                     prop.onChange (SetPswInput >> dispatch)  
                     prop.style
                         [
                           style.width(200)
                           style.fontFamily "sans-serif"
                         ]
                ]   
         
          //************************************************************************

            let fnError() =
                contentLogin
                <| submitInput                        
                <| inputElementUsr 
                <| inputElementPsw  
                <| (errorMsg1, errorMsg2)
                <| false //related to hiding
                <| (dispatch: Msg -> unit)

            let fnFirstRun() =
                contentLogin
                <| submitInput                        
                <| inputElementUsr 
                <| inputElementPsw                    
                <| (String.Empty, String.Empty)
                <| true //related to hiding
                <| (dispatch: Msg -> unit)

            match model.User with      
            | Anonymous             -> fnError()
            | FirstTimeRunAnonymous -> fnFirstRun()
            | LoggedIn user         -> CMSPages.CMSRozcestnik.view CMSRozcestnikModel user (CMSRozcestnikMsg >> dispatch) //it is not strictly necessary for the model and user to be here, but I left them here to keep things tidy

                   
    //Redundant code - for learning purposes only!!!
    //Code by Maxime Mangel
    //https://medium.com/@MangelMaxime/my-tips-for-working-with-elmish-ab8d193d52fd
    module Parent =

        open SharedTypes

        type Model =
            { Session : SharedApi.LoginResult option
              Login : Login.Model  }        

        type Msg =
            | LoginMsg of Login.Msg

        let private update (msg : Msg) (model : Model) =
            match msg with
            | LoginMsg loginMsg ->        
                let (loginModel, loginCmd, loginExtraMsg) = Login.update loginMsg model.Login

                // Here we can match over loginExtraMsg to do something
                let newModel =
                    match loginExtraMsg with
                    | Login.ExternalMsg.NoOp -> model
                    | Login.ExternalMsg.SignedIn session -> { model with Session = Some session }

                { newModel with Login = loginModel }, Cmd.map LoginMsg loginCmd