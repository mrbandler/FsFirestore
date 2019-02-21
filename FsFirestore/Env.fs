namespace FsFirestore

module Env =
    
    open System
    open Microsoft.FSharpLu.File
    open Chiron

    [<Literal>] 
    let private GCP_SYSTEM_VAR = "GOOGLE_APPLICATION_CREDENTIALS" 

    /// GCP Credentials.
    type Credentials = {
        //t: string
        projectId: string
        privateKeyId: string
        privateKey: string
        clientEmail: string
        clientId: string
        authUri: string
        tokenUri: string
        authProviderX509CertUrl: string
        clientX509CertUrl: string
    } with
        
        static member ToJson(x: Credentials) = json {
            do! Json.write "project_id" x.projectId
            do! Json.write "private_key_id" x.privateKeyId
            do! Json.write "private_key" x.privateKey
            do! Json.write "client_email" x.clientEmail
            do! Json.write "client_id" x.clientId
            do! Json.write "auth_uri" x.authUri
            do! Json.write "token_uri" x.tokenUri
            do! Json.write "auth_provider_x509_cert_url" x.authProviderX509CertUrl
            do! Json.write "client_x509_cert_url" x.clientX509CertUrl
        }

        static member FromJson(_: Credentials) = json {
            let! projectId = Json.read "project_id"
            let! privateKeyId = Json.read "private_key_id"
            let! privateKey = Json.read "private_key"
            let! clientEmail = Json.read "client_email"
            let! clientId = Json.read "client_id"
            let! authUri = Json.read "auth_uri"
            let! tokenUri = Json.read "token_uri"
            let! authProviderX509CertUrl = Json.read "auth_provider_x509_cert_url"
            let! clientX509CertUrl = Json.read "client_x509_cert_url"

            return {
                projectId = projectId
                privateKeyId = privateKeyId
                privateKey = privateKey
                clientEmail = clientEmail
                clientId = clientId
                authUri = authUri
                tokenUri = tokenUri
                authProviderX509CertUrl = authProviderX509CertUrl
                clientX509CertUrl = clientX509CertUrl
            }
        }

    /// Deserializes the GCP credentials found by the given path.
    let private deserializeGCPCredentials path = 
        (atomaticReadAllText path) |> Json.parse |> Json.deserialize

    /// Sets the GCP environment for the current process.
    /// If the setup is successfull the firebase project ID will be returned.
    let internal setupGCPEnvironment path =
        match getExistingFile path with
        | Some path -> 
            Environment.SetEnvironmentVariable(GCP_SYSTEM_VAR, path)
            Some((deserializeGCPCredentials path).projectId)
        | None -> None
        