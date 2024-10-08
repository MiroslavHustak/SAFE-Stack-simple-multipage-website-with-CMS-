//Code for routing taken from StackOverflow:
//https://stackoverflow.com/questions/54970180/how-can-i-do-a-simple-elmish-router
//Created by Maxime Mangel:
//https://stackoverflow.com/users/2911775/maxime-mangel

namespace MaximeRouter

[<RequireQualifiedAccess>]
module Router = 

    let private (</>) a b = a + "/" + string b

    type Route =
        | Home
        | Sluzby of int
        | Cenik of int
        | Nenajdete of int
        | Kontakt of int
        | Login of int
        | Logout       //not in use 
        | Maintenance  //not in use
        | CMSRozcestnik of int
        | CMSCenik of int
        | CMSKontakt of int
        | CMSLink of int

    let internal toHash (route : Route) =
        match route with
        | Home             -> "#home"
        | Sluzby id        -> "#sluzby" </> id
        | Cenik id         -> "#cenik" </> id
        | Nenajdete id     -> "#nenajdete" </> id
        | Kontakt id       -> "#kontakt" </> id
        | Login id         -> "#login" </> id
        | Logout           -> "#home"          //not in use 
        | Maintenance      -> "#maintenance"   //not in use
        | CMSRozcestnik id -> "#cmsRozcestnik" </> id
        | CMSCenik id      -> "#cmsCenik" </> id
        | CMSKontakt id    -> "#cmsKontakt" </> id
        | CMSLink id       -> "#cmsLink" </> id

    open Elmish.Navigation
    open Elmish.UrlParser

    let internal routeParser : Parser<Route -> Route, Route> =
        oneOf
            [   // Auth Routes
                map Route.Home (s "home")
                map (fun domainId -> Route.Cenik domainId) (s "cenik" </> i32)
                map (fun domainId -> Route.Sluzby domainId) (s "sluzby" </> i32)
                map (fun domainId -> Route.Nenajdete domainId) (s "nenajdete" </> i32)
                map (fun domainId -> Route.Kontakt domainId) (s "kontakt" </> i32)
                map (fun domainId -> Route.Login domainId) (s "login" </> i32)
                map Route.Logout (s "home")             //not in use 
                map Route.Maintenance (s "maintenance") //not in use
                map (fun domainId -> Route.CMSRozcestnik domainId) (s "cmsRozcestnik" </> i32)
                map (fun domainId -> Route.CMSCenik domainId) (s "cmsCenik" </> i32)
                map (fun domainId -> Route.CMSKontakt domainId) (s "cmsKontakt" </> i32)
                map (fun domainId -> Route.CMSLink domainId) (s "cmsLink" </> i32)

                // Default Route
                map Route.Home top
            ]