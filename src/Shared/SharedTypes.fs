namespace SharedTypes

type LoginInfo = { Username: string; Password: string }

type GetCenikValues =
    {
        V001: string; V002: string; V003: string;
        V004: string; V005: string; V006: string;
        V007: string; V008: string; V009: string
    }
    static member Default = //fixed values
        {
           V001 = "300"; V002 = "300"; V003 = "2 200";
           V004 = "250"; V005 = "230"; V006 = "400";
           V007 = "600"; V008 = "450"; V009 = "450"
        }        

type GetKontaktValues =
    {
        V001: string; V002: string; V003: string;
        V004: string; V005: string; V006: string;
        V007: string
    }

type Kontakt =
    {
        Id: int
        KontaktValuesV001: string 
        KontaktValuesV002: string 
        KontaktValuesV003: string 
        KontaktValuesV004: string 
        KontaktValuesV005: string 
        KontaktValuesV006: string 
        KontaktValuesV007: string    
    }

type GetLinkAndLinkNameValues =
    {
        V001: string; V002: string; V003: string;
        V004: string; V005: string; V006: string;
        V001n: string; V002n: string; V003n: string;
        V004n: string; V005n: string; V006n: string
    }

type IGetApi =
    {
        login : LoginInfo -> Async<SharedApi.LoginResult> 
        getCenikValues: GetCenikValues -> Async<GetCenikValues> //GetCenikValues ponechano quli jednotnosti, jinak staci unit -> Async<GetCenikValues> **
        sendOldCenikValues: GetCenikValues -> Async<GetCenikValues>
        sendDeserialisedCenikValues: GetCenikValues -> Async<GetCenikValues>
        getKontaktValues: GetKontaktValues -> Async<GetKontaktValues> //GetKontaktValues ponechano quli jednotnosti, jinak staci unit -> Async<GetKontaktValues> **
        sendOldKontaktValues: GetKontaktValues -> Async<GetKontaktValues>
        sendDeserialisedKontaktValues: GetKontaktValues -> Async<GetKontaktValues>
        getLinkAndLinkNameValues: GetLinkAndLinkNameValues -> Async<GetLinkAndLinkNameValues> //GetCenikValues ponechano quli jednotnosti, jinak staci unit -> Async<GetLinkAndLinkNameValues> **
        sendOldLinkAndLinkNameValues: GetLinkAndLinkNameValues -> Async<GetLinkAndLinkNameValues>
        sendDeserialisedLinkAndLinkNameValues: GetLinkAndLinkNameValues -> Async<GetLinkAndLinkNameValues>
    }
    //** pokud bych dal unit, mel bych misto plno prazdnych hodnot v prislusnych fs souborech zase plno kodu na server a shared


