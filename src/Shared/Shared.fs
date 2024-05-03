namespace Shared

open System
open SharedTypes

module Route =

    let builder typeName methodName = sprintf "/api/%s/%s" typeName methodName

module SharedMessageDefaultValues =

    let messageDefault = //fixed values
        {
            Msg1 = String.Empty; Msg2 = String.Empty;
            Msg3 = String.Empty; Msg4 = String.Empty;
            Msg5 = String.Empty; Msg6 = String.Empty
        }

//"create" -> client-side transformation layer

module SharedLoginValues =

    let create username password = 
        {
            Username = username   //client -> server
            Password = password   //client -> server
        }
  
module SharedCenikValues =    

    let cenikValuesDomainDefault = //fixed values
        {
            Id = 1; ValueState = "fixed";
            V001 = "500"; V002 = "300"; V003 = "2500";
            V004 = "250"; V005 = String.Empty; V006 = String.Empty;
            V007 = String.Empty; V008 = "450"; V009 = "450";
            Msgs = SharedMessageDefaultValues.messageDefault
        }
          
    let create id valState v001 v002 v003 v004 v005 v006 v007 v008 v009 =
        {
            Id = id; ValueState = valState;
            V001 = v001; V002 = v002; V003 = v003;
            V004 = v004; V005 = v005; V006 = v006;
            V007 = v007; V008 = v008; V009 = v009;
            Msgs = SharedMessageDefaultValues.messageDefault
        }
   
module SharedDeserialisedCenikValues =
   
    let create (cenikInputValues: CenikValuesDomain) =
        {
            Id = cenikInputValues.Id; ValueState = cenikInputValues.ValueState;
            V001 = cenikInputValues.V001; V002 = cenikInputValues.V002; V003 = cenikInputValues.V003;
            V004 = cenikInputValues.V004; V005 = cenikInputValues.V005; V006 = cenikInputValues.V006;
            V007 = cenikInputValues.V007; V008 = cenikInputValues.V008; V009 = cenikInputValues.V009;
            Msgs = SharedMessageDefaultValues.messageDefault
        }

module SharedKontaktValues =

    let kontaktValuesDomainDefault = 
        {
            V001 = "Mgr. Hana NOVÁKOVÁ"; V002 = "Nutriční teraupetka"; V003 = "Pohoří 247";
            V004 = "725 26 Ostrava-Krásné Pole"; V005 = "Tel.: 739 421 710"; V006 = "E-mail: nutricniterapie@centrum.cz";
            V007 = (char)32 |> string; Msgs = SharedMessageDefaultValues.messageDefault
        }
            
    let create v001 v002 v003 v004 v005 v006 v007 =
        {
            V001 = v001; V002 = v002; V003 = v003
            V004 = v004; V005 = v005;
            V006 = v006; V007 = v007;  Msgs = SharedMessageDefaultValues.messageDefault
        }

module SharedDeserialisedKontaktValues =    
   
    let create (kontaktInputValues: KontaktValuesDomain) =
        {
            V001 = kontaktInputValues.V001; V002 = kontaktInputValues.V002; V003 = kontaktInputValues.V003;
            V004 = kontaktInputValues.V004; V005 = kontaktInputValues.V005;
            V006 = kontaktInputValues.V006; V007 = kontaktInputValues.V007; Msgs = SharedMessageDefaultValues.messageDefault          
        }

module SharedLinkAndLinkNameValues =

    let linkAndLinkNameValuesDomainDefault = 
        {
            V001 = "https://blog.kaloricketabulky.cz/2013/08/nutricni-terapeut-vs-vyzivovy-poradce-kdo-nam-muze-radit-s-vyzivou/";
            V002 = "http://www.aktivityprozdravi.cz/zdravotni-problemy/civilizacni-psychologicke-a-jine-nemoci/civilizacni-choroby-a-nas-zivotni-styl";
            V003 = "https://www.novinky.cz/zena/zdravi/403392-obezita-je-problem-ktery-lide-casto-prehlizeji.html";
            V004 = String.Empty; V005 = "https://www.morevsrdcievropy.cz";
            V006 = "https://www.facebook.com/nutricniterapie/";
            V001n = "Kdo nám může radit s výživou?"; V002n = "Civilizační choroby"; V003n = "Problém obezity"; V004n = String.Empty;
            V005n = "Moře v srdci Evropy"; V006n = "Facebook"; Msgs = SharedMessageDefaultValues.messageDefault  
        }
    
    let create v001 v002 v003 v004 v005 v006 v001n v002n v003n v004n v005n v006n =
        {
            V001 = v001; V002 = v002; V003 = v003;
            V004 = v004; V005 = v005; V006 = v006;
            V001n = v001n; V002n = v002n; V003n = v003n;
            V004n = v004n; V005n = v005n; V006n = v006n;
            Msgs = SharedMessageDefaultValues.messageDefault    
        }

module SharedDeserialisedLinkAndLinkNameValues =
   
    let create (linkAndLinkNameInputValues: LinkAndLinkNameValuesDomain) =
        {
            V001 = linkAndLinkNameInputValues.V001; V002 = linkAndLinkNameInputValues.V002; V003 = linkAndLinkNameInputValues.V003;
            V004 = linkAndLinkNameInputValues.V004; V005 = linkAndLinkNameInputValues.V005; V006 = linkAndLinkNameInputValues.V006;
            V001n = linkAndLinkNameInputValues.V001n; V002n = linkAndLinkNameInputValues.V002n; V003n = linkAndLinkNameInputValues.V003n;
            V004n = linkAndLinkNameInputValues.V004n; V005n = linkAndLinkNameInputValues.V005n; V006n = linkAndLinkNameInputValues.V006n;
            Msgs = SharedMessageDefaultValues.messageDefault 
        }



